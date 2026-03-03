using System;
using Godot.DependencyInjection.Sample;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Godot.DependencyInjection
{
    internal partial class DependencyInjection
    {
        // Optional
        static partial void CreateHostBuilder(ref IHostBuilder builder)
        {
            
        }

        // Optional
        static partial void ConfigureHostConfiguration(IConfigurationBuilder configurationBuilder)
        {
            //....
        }
        
        static partial void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<MessageBusNode>()
                .AddHostedService(p  => p.GetRequiredService<MessageBusNode>());
        }
    }
};