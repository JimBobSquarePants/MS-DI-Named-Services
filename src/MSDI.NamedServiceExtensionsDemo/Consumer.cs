using System;

namespace MSDI.NamedServiceExtensions
{
    public class ConsumerA
    {
        public ConsumerA(IServiceProvider provider)
        {
            string name = nameof(ServiceA);
            this.Service = provider.GetNamedService<IService>(name);
            Console.WriteLine($"Consumer Type: {this.GetType().Name}");
            Console.WriteLine($"Dependent Type: {name}, " +
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
            Console.WriteLine($"Consumer Type: {this.GetType().Name}");
            Console.WriteLine($"Dependent Type: {name}, " +
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
            Console.WriteLine($"Consumer Type: {this.GetType().Name}");
            Console.WriteLine($"Dependent Type: {name}, " +
                $"Dependent Name: {name}, " +
                $"Dependent Value: {this.Service.Date.Ticks}");
        }

        public IService Service { get; }
    }

    public class ConsumerD
    {
        public ConsumerD(IService serviceX, IService serviceY)
        {
            this.ServiceX = serviceX;
            string nameX = this.ServiceX.GetType().Name;
            this.ServiceY = serviceY;
            string nameY = this.ServiceY.GetType().Name;
            Console.WriteLine($"Consumer Type: {this.GetType().Name}");
            Console.WriteLine($"First Dependent Type: {nameX}, " +
                $"First Dependent Name: {nameof(serviceX)}, " +
                $"First Dependent Value: {this.ServiceX.Date.Ticks}");
            Console.WriteLine($"Second Dependent Type: {nameY}, " +
                $"Second Dependent Name: {nameof(serviceY)}, " +
                $"Second Dependent Value: {this.ServiceY.Date.Ticks}");
        }

        public IService ServiceX { get; }
        public IService ServiceY { get; }
    }
}
