using MaJiCSoft.ChatOverlay.Api;
using MaJiCSoft.ChatOverlay.OAuth;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MaJiCSoft.ChatOverlay.ViewModels
{
    public class UserInfoViewModel : ViewModelBase
    {
        private readonly ILogger logger;
        private readonly OAuthState state;
        private readonly ApiInteractor api;

        private UserInfo userInfo;
        public UserInfo UserInfo { get => userInfo; set => SetProperty(ref userInfo, value); }

        private DelegateCommand getUserInfo;
        public ICommand GetUserInfo
        {
            get => getUserInfo;
        }

        public UserInfoViewModel(ILogger<UserInfoViewModel> logger, OAuthState state, ApiInteractor api)
        {
            this.logger = logger;
            this.state = state;
            this.api = api;
        }

        private void OnGetUserInfo(object parameter)
        {
            UserInfo = state.GetUserInfo();
        }
    }
}
