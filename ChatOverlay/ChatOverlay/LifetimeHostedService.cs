using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaJiCSoft.ChatOverlay
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
    internal class LifetimeHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IHostApplicationLifetime applicationLifetime;
        private NamedPipeManager pipeManager;

        public LifetimeHostedService(ILogger<LifetimeHostedService> logger, IHostApplicationLifetime applicationLifetime, NamedPipeManager pipeManager)
        {
            this.logger = logger;
            this.applicationLifetime = applicationLifetime;
            this.pipeManager = pipeManager;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);

            RegisterCustomURIScheme("mjchat", Assembly.GetExecutingAssembly().GetName().Name);

            return Task.CompletedTask;
        }

        public void RegisterCustomURIScheme(string protocolName, string appPath)
        {
            //  HKCR:
            //      [<ProtocolName>]
            //          (Default) = "URL:<ProtocolName>"
            //          URL Protocol = ""
            //          [DefaultIcon] = "<ApplicationPath>"
            //          [shell]
            //              [open]
            //                  [command]
            //                      (Default) = "<ApplicationPath> "%1""
            var key = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Classes").CreateSubKey(protocolName);
            key.SetValue(null, $"URL:{protocolName}", RegistryValueKind.String);
            key.SetValue("URL Protocol", "", RegistryValueKind.String);

            var defIconKey = key.CreateSubKey("DefaultIcon");
            defIconKey.SetValue(null, appPath, RegistryValueKind.String);

            var shellKey = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
            shellKey.SetValue(null, $"{appPath} \"%1\"");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}
