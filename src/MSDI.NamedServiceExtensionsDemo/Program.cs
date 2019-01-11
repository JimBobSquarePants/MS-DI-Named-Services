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

            // Resolve our two types. They should now output different injected types.
            Console.WriteLine("Resolved types:");
            Console.WriteLine();
            ServiceProvider provider = services.BuildServiceProvider();
            ConsumerA a = provider.GetService<ConsumerA>();
            ConsumerB b = provider.GetService<ConsumerB>();
            ConsumerC c = provider.GetService<ConsumerC>();

            Console.ReadLine();
        }
    }
}
