using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MSDI.NamedServiceExtensions;

namespace MSDI.NamedServiceExtensionsDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("I Can Haz Named and Targeted Services!");
            Console.WriteLine();

            var services = new ServiceCollection();

            // Register everything.
            services.AddTransient(typeof(DateTime), _ => DateTime.Now);
            services.AddTransient(typeof(TimeSpan), _ => TimeSpan.MaxValue);

            // Add a default IServiceImplementation
            services.AddTransient<IService, ServiceDefault>();

            // Add our named targets. These are resolved using the service locator in the target ctrs.
            services.AddNamedService<IService, ServiceA>(nameof(ServiceA), ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceB>(nameof(ServiceB), ServiceLifetime.Transient);
            services.AddTransient<ConsumerA>();
            services.AddTransient<ConsumerB>();

            // Add a target keyed to a specific target type. 
            // This adds both types at the same time automatically injecting the service implementation
            // along standard injected dependencies. See ConsumerC ctr.
            services.AddServiceFor<IService, ServiceC, ConsumerC>(ServiceLifetime.Transient, ServiceLifetime.Transient);

            // Add a sevice with named dependencies tied to parameter names.
            services.AddNamedService<IService, ServiceD>(nameof(ServiceD), ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceE>(nameof(ServiceE), ServiceLifetime.Transient);

            var args2 = new NamedDependency[]{
                // {registeredType, registeredName, parameterName}
                new NamedDependency(typeof(IService), nameof(ServiceD), "serviceX"),
                new NamedDependency(typeof(IService), nameof(ServiceE), "serviceY")
            };

            services.AddServiceWithNamedDependencies<IConsumerXY, ConsumerD>(ServiceLifetime.Transient, args2);

            // Add a service with one named dependency and one default. 
            // The other depency should resolve as our default service.
            var args3 = new NamedDependency[]{
                // {registeredType, registeredName, parameterName}
                new NamedDependency(typeof(IService), nameof(ServiceE), "serviceY")
            };

            services.AddServiceWithNamedDependencies<IConsumerXY, ConsumerE>(ServiceLifetime.Transient, args3);

            // Resolve our two types. They should now output different injected types.
            Console.WriteLine("Resolved types:");
            Console.WriteLine();
            ServiceProvider provider = services.BuildServiceProvider();
            ConsumerA a = provider.GetService<ConsumerA>();
            ConsumerB b = provider.GetService<ConsumerB>();
            ConsumerC c = provider.GetService<ConsumerC>();

            IEnumerable<IConsumerXY> multiple = provider.GetServices<IConsumerXY>();

            Console.ReadLine();
        }
    }
}
