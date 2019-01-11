using System;

namespace MSDI.NamedServiceExtensions
{
    public interface IService
    {
        DateTime Date { get; }
    }

    public class ServiceA : IService
    {
        public DateTime Date { get; }

        public ServiceA(DateTime date) => this.Date = date;
    }

    public class ServiceB : IService
    {
        public DateTime Date { get; }

        public ServiceB(DateTime date) => this.Date = date;
    }

    public class ServiceC : IService
    {
        public DateTime Date { get; }

        public ServiceC(DateTime date) => this.Date = date;
    }
}
