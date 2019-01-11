using System;

namespace MSDI.NamedServiceExtensions
{
    public class ConsumerA
    {
        public ConsumerA(IServiceProvider provider)
        {
            string name = nameof(ServiceA);
            this.Service = provider.GetNamedService<IService>(name);
            Console.WriteLine($"Consumer Type: {this.GetType().Name}, " +
                $"Dependent Type: {this.Service.GetType().Name}, " +
                $"Dependent Name: {name}, " +
                $"Dependent Value: {this.Service.Date.Ticks}");
        }

        public IService Service { get; }
    }

    public class ConsumerB
    {
        public ConsumerB(IServiceProvider provider)
        {
            string name = nameof(ServiceB);
            this.Service = provider.GetNamedService<IService>(name);
            Console.WriteLine($"Consumer Type: {this.GetType().Name}, " +
                $"Dependent Type: {this.Service.GetType().Name}, " +
                $"Dependent Name: {name}, " +
                $"Dependent Value: {this.Service.Date.Ticks}");
        }

        public IService Service { get; }
    }

    public class ConsumerC
    {
        public ConsumerC(IService service)
        {
            this.Service = service;
            string name = this.Service.GetType().Name;
            Console.WriteLine($"Consumer Type: {this.GetType().Name}, " +
                $"Dependent Type: {name}, " +
                $"Dependent Name: {name}, " +
                $"Dependent Value: {this.Service.Date.Ticks}");
        }

        public IService Service { get; }
    }
}
