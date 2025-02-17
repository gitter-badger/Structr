using Microsoft.Extensions.DependencyInjection.Extensions;
using Structr.Notices;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNotices(this IServiceCollection services, params Assembly[] assembliesToScan)
            => AddNotices(services, null, assembliesToScan);

        public static IServiceCollection AddNotices(this IServiceCollection services,
            Action<NoticeServiceOptions> configureOptions,
            params Assembly[] assembliesToScan)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new NoticeServiceOptions();

            configureOptions?.Invoke(options);

            services.TryAdd(new ServiceDescriptor(typeof(INoticePublisher), options.PublisherType, options.PublisherServiceLifetime));

            services.AddClasses(assembliesToScan);

            return services;
        }

        private static IServiceCollection AddClasses(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            if (assembliesToScan != null && assembliesToScan.Length > 0)
            {
                var allTypes = assembliesToScan
                    .Where(a => !a.IsDynamic && a != typeof(INoticePublisher).Assembly)
                    .Distinct()
                    .SelectMany(a => a.DefinedTypes)
                    .ToArray();

                var openTypes = new[]
                {
                    typeof(INoticeHandler<>)
                };

                foreach (var typeInfo in openTypes.SelectMany(openType => allTypes
                    .Where(t => t.IsClass
                        && !t.IsGenericType
                        && !t.IsAbstract
                        && t.AsType().ImplementsGenericInterface(openType))))
                {
                    var implementationType = typeInfo.AsType();

                    foreach (var interfaceType in implementationType.GetInterfaces()
                        .Where(i => openTypes.Any(openType => i.ImplementsGenericInterface(openType))))
                    {
                        services.AddTransient(interfaceType, implementationType);
                    }
                }
            }

            return services;
        }

        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
            => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}
