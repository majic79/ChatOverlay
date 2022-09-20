using MaJiCSoft.ChatOverlay.OAuth;
using MaJiCSoft.ChatOverlay.Options;
using MaJiCSoft.ChatOverlay.OAuth.Options;
using MaJiCSoft.NotifyIcon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Text;
using System.Web;
using MaJiCSoft.ChatOverlay.Events;

namespace MaJiCSoft.ChatOverlay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public static IServiceProvider ServiceProvider { get; private set; }
        private ILogger logger;
        private uint windowMessage;
        private Mutex mutex;
        private NamedPipeManager pipeManager;
        private CancellationTokenSource cts;
        // private NotifyIcon notify;

        public App()
        {
            // AppDomain.CurrentDomain.BaseDirectory ??
            var appPath = Process.GetCurrentProcess().MainModule.FileName;
            cts = new CancellationTokenSource();
            host = CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

            ServiceProvider = host.Services;
            logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((context, builder) => {
                        builder.AddJsonFile("appsettings.runtime.json", optional: true, reloadOnChange: true);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        RegisterWindows(context.Configuration, services);
                        RegisterViews(context.Configuration, services);
                        RegisterServices(context.Configuration, services);
                        RegisterOptions(context.Configuration, services);
                    })
                    .ConfigureLogging((logging) => {
                        logging.SetMinimumLevel(LogLevel.Debug);
                        logging.AddConsole();
                        logging.AddDebug();
                    });

        private static void RegisterWindows(IConfiguration config, IServiceCollection services)
        {
            services
                .AddSingleton<MainWindow>();
        }
        private static void RegisterViews(IConfiguration config, IServiceCollection services)
        {
            ViewModelLocator.ConfigureViewModels(config, services);
        }

        private static void RegisterServices(IConfiguration config, IServiceCollection services)
        {
            services
                .AddSingleton<OAuthState>()
                .AddTransient<OAuthResponseValidator>()
                .AddSingleton<NamedPipeManager>()
                .AddSingleton<Api.ApiInteractor>()
                .AddSingleton<EventAggregator>()
                .AddTransient<IEventAggregator>((s) => s.GetRequiredService<EventAggregator>())
                ;
        }

        private static void RegisterOptions(IConfiguration config, IServiceCollection services)
        {
            services
                .Configure<AppOptions>(config.GetSection(nameof(AppOptions)))
                .Configure<TwitchClientOptions>(config.GetSection(nameof(TwitchClientOptions)))
                .Configure<OAuthServiceOptions>(config.GetSection(nameof(OAuthServiceOptions)))
                .ConfigureWritable<OAuthAccessOptions>(config.GetSection(nameof(OAuthAccessOptions)), "appsettings.runtime.json")
            ;
        }

        private bool SingleInstance(NamedPipeManager pipeManager)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            bool mutexCreated;
            string mutexName = string.Format(CultureInfo.InvariantCulture, pipeManager.NamedPipeName, assembly.GetType().GUID, assembly.GetName().Name);

            mutex = new Mutex(true, mutexName, out mutexCreated);
            windowMessage = WinApi.User32.RegisterWindowMessage(mutexName);

            if (!mutexCreated)
            {
                var args = Environment.GetCommandLineArgs();

                SendOAuthResponse(args[1]);

                mutex = null;

                Current.Shutdown();
                return false;
            }
            return true;
        }

        private void SendOAuthResponse(string response)
        {
            var client = pipeManager.GetClientStream(5000);
            client.WriteStream(Encoding.UTF8.GetBytes(response)); // Send first argument into the pipe
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync();

            pipeManager = host.Services.GetRequiredService<NamedPipeManager>();
            if (SingleInstance(pipeManager))
            {

                var mainWindow = host.Services.GetRequiredService<MainWindow>();
                MainWindow = mainWindow;
                mainWindow.Show();

                base.OnStartup(e);
            }

            pipeManager.StartServer(cts.Token);
            pipeManager.ReceiveString += Handle_PipeReceive;

            if(e.Args.Length > 1)
            {
                SendOAuthResponse(e.Args[1]);
            }
            //notify = new NotifyIcon(HandleMouseEvent, null, null);

            //var assembly = Assembly.GetExecutingAssembly();

            //var icon = Icon.ExtractAssociatedIcon(assembly.Location);
            //notify.Icon = icon;
            //notify.ToolTip = assembly.GetName().Name;
            //notify.Visible = true;
        }
        private void Handle_PipeReceive(object sender, string data)
        {
            logger.LogInformation($"PipeReceive: {data}");

            var validator = host.Services.GetRequiredService<OAuthResponseValidator>();
            var oauthState = host.Services.GetRequiredService<OAuthState>();
            var access_token = oauthState.ValidateOAuthResponse(data);
            oauthState.AccessToken = access_token;

            //Dispatcher.Invoke(() =>
            //{

            //    if (WindowState == WindowState.Minimized)
            //        WindowState = WindowState.Normal;

            //    this.Activate();
            //});
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            logger.LogInformation("Application Exiting...");
            cts.Cancel();
            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(10));
            }
            base.OnExit(e);
        }

        private void HandleMouseEvent(object sender, MouseEvent me)
        {
            logger.LogDebug($"MouseEvent: {me}");
        }

    }
}
