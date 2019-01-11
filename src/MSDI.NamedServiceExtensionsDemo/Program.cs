using System;
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

            // Add our named targets. These are resolved using the service locator.
            services.AddNamedService<IService, ServiceA>(nameof(ServiceA), ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceB>(nameof(ServiceB), ServiceLifetime.Transient);
            services.AddTransient<ConsumerA>();
            services.AddTransient<ConsumerB>();

            // Add a target keyed to a specific target type. 
            // This adds both types at the same time.
            services.AddServiceFor<IService, ServiceC, ConsumerC>(ServiceLifetime.Transient, ServiceLifetime.Transient);

            // Add a sevice with named dependencies tied to parameter names.
            services.AddNamedService<IService, ServiceD>(nameof(ServiceD), ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceE>(nameof(ServiceE), ServiceLifetime.Transient);
            var args2 = new Tuple<Type, string, string>[]{
                // {registeredType, registeredName, parameterName}
                Tuple.Create(typeof(IService), nameof(ServiceD), "serviceX"),
                Tuple.Create(typeof(IService), nameof(ServiceE), "serviceY")
            };
            services.AddServiceWithNamedDependencies<ConsumerD, ConsumerD>(ServiceLifetime.Transient, args2);

            // Resolve our two types. They should now output different injected types.
            Console.WriteLine("Resolved types:");
            Console.WriteLine();
            ServiceProvider provider = services.BuildServiceProvider();
            ConsumerA a = provider.GetService<ConsumerA>();
            ConsumerB b = provider.GetService<ConsumerB>();
            ConsumerC c = provider.GetService<ConsumerC>();
            ConsumerD d = provider.GetService<ConsumerD>();

            Console.ReadLine();
        }
    }
}
