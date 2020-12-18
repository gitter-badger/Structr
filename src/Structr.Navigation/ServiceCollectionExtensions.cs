using Microsoft.Extensions.DependencyInjection.Extensions;
using Structr.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNavigation(this IServiceCollection services, Action<NavigationConfigurator> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var configurator = new NavigationConfigurator();
            configure.Invoke(configurator);

            services.TryAddSingleton(configurator);
            services.TryAddSingleton<INavigationBuilder, NavigationBuilder>();
            // TODO: 

            return services;
        }
    }
}
