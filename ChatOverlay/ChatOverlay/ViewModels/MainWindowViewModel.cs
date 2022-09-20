using MaJiCSoft.ChatOverlay.Events;
using MaJiCSoft.ChatOverlay.OAuth;
using MaJiCSoft.ChatOverlay.OAuth.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace MaJiCSoft.ChatOverlay.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly DelegateCommand authoriseClient;
        public ICommand AuthoriseClient { get => authoriseClient; }

        private readonly DelegateCommand setAccessToken;
        public ICommand SetAccessToken { get => setAccessToken; }

        private readonly DelegateCommand viewLoaded;
        public ICommand ViewLoaded { get => viewLoaded; }

        private readonly DelegateCommand connectChat;
        public ICommand ConnectChat { get => connectChat; }

        public ObservableCollection<TabItem> tabs;
        public ObservableCollection<TabItem> Tabs { get => tabs; }

        private TabItem tabItem;
        public TabItem SelectedTab
        {
            get => tabItem;
            set
            {
                SetProperty(ref tabItem, value, nameof(SelectedTab));
            }
        }

        private readonly ILogger logger;
        private readonly IOptions<OAuthServiceOptions> oauthServiceOptions;
        private readonly IWritableOptions<OAuthAccessOptions> oauthAccessOptions;
        private readonly OAuthResponseValidator validator;
        private readonly IEventAggregator ea;
        public OAuthState OAuth { get; }

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger, IOptions<OAuthServiceOptions> oauthServiceOptions, IWritableOptions<OAuthAccessOptions> oauthAccessOptions, IEventAggregator ea, OAuthState oauthState, OAuthResponseValidator validator)
        {
            this.logger = logger;
            this.ea = ea;
            this.oauthServiceOptions = oauthServiceOptions;
            this.oauthAccessOptions = oauthAccessOptions;
            this.OAuth = oauthState;
            this.validator = validator;

            this.tabs = new ObservableCollection<TabItem>();

            this.ea.GetEvent<StringMessage>().Subscribe((m) => { OAuth.AccessToken = oauthAccessOptions.Value.AccessToken; }, ThreadOption.UIThread, true, (m) => string.Compare(m.Message, "StatusLoaded") == 0);
            OAuth.PropertyChanged += OAuth_PropertyChanged;

            authoriseClient = new DelegateCommand(OnStartAuth, (x) => { return OAuth.IsNotSigned; });
            setAccessToken = new DelegateCommand(OnSetAccessToken, (x) => true );
            connectChat = new DelegateCommand(OnConnectChat, (x) => { return OAuth.IsSigned; });
            viewLoaded = new DelegateCommand(OnViewLoaded, (x) => true);
        }

        public void OnConnectChat(object sender)
        {
            var chatTab = new TabItem { ItemName = "Chat", TabType = TabItemType.Chat };
            tabs.Add(chatTab);
            SelectedTab = chatTab;
        }

        public void OnViewLoaded(object sender)
        {
            var statusTab = new TabItem { ItemName = "Status", TabType = TabItemType.Status };
            tabs.Add(statusTab);
            SelectedTab = statusTab;
        }

        public void OnSetAccessToken(object sender)
        {
            OAuth.AccessToken = oauthAccessOptions.Value.AccessToken;
        }

        private void OAuth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(OAuth.AccessToken))
            {
                ea.GetEvent<StatusMessage>().Publish(new StatusMessage { Status = $"Access Token updated." });
                ValidateToken(OAuth.AccessToken);
            }
        }

        private async void ValidateToken(string access_token)
        {
            oauthAccessOptions.Update((opt) => { opt.AccessToken = access_token; });

            if (string.IsNullOrEmpty(access_token))
            {
                return;
            }

            ea.GetEvent<StatusMessage>().Publish(new StatusMessage { Status = $"Validating token..." });
            var validation = await validator.ValidateToken(access_token);
            if (validation != null)
            {
                ea.GetEvent<StatusMessage>().Publish(new StatusMessage { Status = $"Token Validated (ExpiresIn = {validation.ExpiresIn}s)" });
                OAuth.AccessToken = access_token;
                OAuth.ExpiresOn = DateTime.Now.AddSeconds(validation.ExpiresIn);
            }
            else
            {
                ea.GetEvent<StatusMessage>().Publish(new StatusMessage { Status = $"Token Invalidated" });
                OAuth.AccessToken = string.Empty;
                OAuth.ExpiresOn = DateTime.Now.AddSeconds(-1);
            }
        }

        private void OnStartAuth(object parameter)
        {
            ea.GetEvent<StatusMessage>().Publish(new StatusMessage { Status = $"Starting OAuth Access request process..." });

            OAuth.Token = null;
            var scopes = new string[] { "chat:read", "chat:edit", "openid" };

            OAuth.Request = OAuthRequest.BuildAuthorizeRequest(oauthServiceOptions.Value, validator, true, false, scopes);

            var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = OAuth.Request.AuthorizationRequestUri;
            p.StartInfo.Verb = "Open";
            p.Start();
        }
    }
}
