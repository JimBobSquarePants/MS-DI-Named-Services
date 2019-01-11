using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MSDI.NamedServiceExtensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a service/implementation relationship to the collectior for the given target type. 
        /// </summary>
        /// <typeparam name="TService">The type of service to add.</typeparam>
        /// <typeparam name="TImplementation">The implementation type for the service.</typeparam>
        /// <typeparam name="TTarget">The target type that this implementation should be used for.</typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="serviceLifetime">The lifetime of the service.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddServiceFor<TService, TImplementation, TTarget>(
            this IServiceCollection serviceCollection,
            ServiceLifetime serviceLifetime,
            ServiceLifetime targetLifetime)
            where TImplementation : class
            where TTarget : class
        {
            ServiceDescriptor descriptor = serviceCollection.LastOrDefault(x => x.ServiceType == typeof(NamedServiceFactory<TService>));
            var factory = descriptor?.ImplementationInstance as NamedServiceFactory<TService>;
            if (factory is null)
            {
                factory = new NamedServiceFactory<TService>();
                serviceCollection.AddSingleton(factory);
            }

            string name = typeof(TImplementation).FullName + ":" + typeof(TTarget).FullName;
            factory.Register<TImplementation>(name);
            Type targetType = typeof(TTarget);

            // We don't want to register using the service descriptor since that would mean multiple TService types
            // would be registered causing resolution problems for non-named registrations.
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton<TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped<TImplementation>();
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient<TImplementation>();
                    break;
            }

            // Now register our target type. This uses our factory to determine the type to tell the provider to resolve for our specified type
            // and resolves all other types as normal via the provider.
            ConstructorInfo constructorInfo = targetType.GetConstructors().Last(x => x.GetParameters().Any(y => y.ParameterType == typeof(TService)));
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            serviceCollection.Add(new ServiceDescriptor(typeof(TTarget), p =>
            {
                object[] args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    // Our named type? 
                    Type parameterType = parameters[i].ParameterType;
                    if (parameterType == typeof(TService))
                    {
                        args[i] = p.GetNamedService<TService>(name);
                        continue;
                    }

                    args[i] = p.GetService(parameterType);
                }

                return Activator.CreateInstance(typeof(TTarget), args);
            }, targetLifetime));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a service/implementation relationship to the collection, keyed to the given name. 
        /// </summary>
        /// <typeparam name="TService">The type of service to add.</typeparam>
        /// <typeparam name="TImplementation">The implementation type for the service.</typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="name">The name to register the service against.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddNamedService<TService, TImplementation>(this IServiceCollection serviceCollection, string name, ServiceLifetime lifetime)
            where TImplementation : class
        {
            ServiceDescriptor descriptor = serviceCollection.LastOrDefault(x => x.ServiceType == typeof(NamedServiceFactory<TService>));
            var factory = descriptor?.ImplementationInstance as NamedServiceFactory<TService>;
            if (factory is null)
            {
                factory = new NamedServiceFactory<TService>();
                serviceCollection.AddSingleton(factory);
            }

            factory.Register<TImplementation>(name);

            // We don't want to register using the service descriptor since that would mean multiple TService types
            // would be registered causing resolution problems for non-named registrations.
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton<TImplementation>();
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped<TImplementation>();
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient<TImplementation>();
                    break;
            }

            return serviceCollection;
        }

        /// <summary>
        /// Returns an instance of the service type matching the given name.
        /// </summary>
        /// <typeparam name="TService">The type of service to return.</typeparam>
        /// <param name="provider">The service provider for retrieving service objects.</param>
        /// <param name="name">The name the service type is registered as.</param>
        /// <returns>The <see cref="TService"/>.</returns>
        public static TService GetNamedService<TService>(this IServiceProvider provider, string name)
        {
            NamedServiceFactory<TService> factory = provider.GetServices<NamedServiceFactory<TService>>().LastOrDefault();
            if (factory is null)
            {
                throw new InvalidOperationException($"No service for type {typeof(TService)} named '{name}' has been registered.");
            }

            return factory.Resolve(provider, name);
        }
    }
}
