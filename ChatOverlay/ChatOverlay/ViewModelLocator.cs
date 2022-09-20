using Microsoft.Extensions.DependencyInjection;
using MaJiCSoft.ChatOverlay.ViewModels;
using Microsoft.Extensions.Configuration;

namespace MaJiCSoft.ChatOverlay
{
    public class ViewModelLocator
    {
        public static void ConfigureViewModels(IConfiguration config, IServiceCollection services)
        {
            var oauthSectionName = nameof(OAuth.Options.OAuthServiceOptions);
            var configSection = config.GetSection(oauthSectionName);

            services
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<StatusViewModel>()
                .AddSingleton<UserInfoViewModel>()
                ;
        }
        public MainWindowViewModel MainViewModel
        {
            get { return App.ServiceProvider.GetRequiredService<MainWindowViewModel>(); }
        }
        public StatusViewModel StatusViewModel
        {
            get { return App.ServiceProvider.GetRequiredService<StatusViewModel>(); }
        }
        public UserInfoViewModel UserInfoViewModel
        {
            get { return App.ServiceProvider.GetRequiredService<UserInfoViewModel>(); }
        }
    }
}
