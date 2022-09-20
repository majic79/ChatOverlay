using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace MaJiCSoft.ChatOverlay
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.runtime.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<WritableOptionsInfo<T>>();
            services.AddTransient<IWritableOptionsInfo<T>>(x =>
            {
                var woi = x.GetRequiredService<WritableOptionsInfo<T>>();
                woi.File = file;
                woi.Section = section.Key;
                return woi;
            });
            services.AddTransient<IWritableOptions<T>, WritableOptions<T>>();
            //services.AddTransient<IWritableOptions<T>>(provider =>
            //{
            //    var environment = provider.GetService<IHostEnvironment>();
            //    var options = provider.GetService<IOptionsMonitor<T>>();
            //    return new WritableOptions<T>(environment, options, section.Key, file);
            //});
            return services;
        }

        public static IServiceCollection AddFactoryType<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>();
            services.AddSingleton<Func<TService>>(x => () => x.GetRequiredService<TService>());
            services.AddSingleton<ITypedFactory<TService>, TypedFactory<TService>>();
            return services;
        }
        public static IServiceCollection AddFactoryType<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddSingleton<TService>();
            services.AddSingleton<Func<TService>>(x => () => x.GetRequiredService<TService>());
            services.AddSingleton<ITypedFactory<TService>, TypedFactory<TService>>();
            return services;
        }
    }
}
