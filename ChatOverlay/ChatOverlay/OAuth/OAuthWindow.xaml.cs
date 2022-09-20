using System.Threading;
using System.Windows;
using System.Windows.Threading;

using System.Windows.Controls;
using System.Diagnostics;
using MaJiCSoft.ChatOverlay.OAuth.Options;
using Microsoft.Extensions.Options;

namespace MaJiCSoft.ChatOverlay.OAuth
{
    /// <summary>
    /// Interaction logic for OAuth.xaml
    /// </summary>
    public partial class OAuthWindow : Window
    {
        private readonly IOptions<OAuthServiceOptions> oauthServiceOptions;
        private readonly IWritableOptions<OAuthAccessOptions> oauthAuthorization;
        private readonly OAuthState state;
        private readonly OAuthResponseValidator validator;

        public OAuthWindow(IWritableOptions<OAuthServiceOptions> oauthServiceOptions, IWritableOptions<OAuthAccessOptions> oauthAuthorization, OAuthState state, OAuthResponseValidator validator)
        {
            this.oauthServiceOptions = oauthServiceOptions;
            this.oauthAuthorization = oauthAuthorization;
            this.state = state;
            this.validator = validator;
            InitializeComponent();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
