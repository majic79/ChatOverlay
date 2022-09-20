using MaJiCSoft.ChatOverlay.Api;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaJiCSoft.ChatOverlay.Views
{
    /// <summary>
    /// Interaction logic for UserInfo.xaml
    /// </summary>
    public partial class UserInfoView : UserControl
    {
        private readonly ILogger logger;

        public UserInfoView(ILogger<UserInfoView> logger, ApiInteractor api)
        {
            this.logger = logger;

            InitializeComponent();
        }
    }
}
