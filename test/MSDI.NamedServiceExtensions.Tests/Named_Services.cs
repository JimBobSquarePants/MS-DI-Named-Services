using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MSDI.NamedServiceExtensions.Tests
{
    [TestFixture]
    public class Named_Services
    {
        private ServiceCollection services;

        [SetUp]
        public void Setup()
        {
            services = new ServiceCollection();

            services.AddTransient(typeof(DateTime), _ => DateTime.Now);
            services.AddTransient(typeof(TimeSpan), _ => TimeSpan.MaxValue);
        }

        [Test]
        public void Are_Resolved_Explicitly_From_Generic_Registration()
        {
            services.AddNamedService<IService,ServiceA>("A name", ServiceLifetime.Transient);
            var provider = services.BuildServiceProvider();
            var resolved = provider.GetNamedService<IService>("A name");
            Assert.That(resolved, Is.InstanceOf<ServiceA>());
        }

        [Test]
        public void Are_Resolved_Explicitly_From_Type_Registration()
        {
            services.AddNamedService(typeof(IService), typeof(ServiceA), "A name", ServiceLifetime.Transient);
            var provider = services.BuildServiceProvider();
            var resolved = provider.GetNamedService<IService>("A name");
            Assert.That(resolved, Is.InstanceOf<ServiceA>());
        }

        [Test]
        public void Are_Specified_For_Dependents()
        {
            services.AddNamedService<IService, ServiceA>("A name", ServiceLifetime.Transient);

            AssertConsumerWithNamedServiceA();
        }

        [Test]
        public void Are_Specified_Amongst_Many_For_Dependents()
        {
            services.AddNamedService<IService, ServiceA>("A name", ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceB>("Another name", ServiceLifetime.Transient);

            AssertConsumerWithNamedServiceA();
        }

        [Test]
        public void Are_Specified_Amongst_Many_For_Individual_Parameters()
        {
            AssertConsumerDWithServiceDAndE();
        }

        [Test]
        public void Ignores_Defaults()
        {
            services.AddTransient<IService, ServiceDefault>();
            services.AddTransient<IConsumerXY, ConsumerE>();

            AssertConsumerDWithServiceDAndE();
        }

        private void AssertConsumerDWithServiceDAndE()
        {
            services.AddNamedService<IService, ServiceD>(nameof(ServiceD), ServiceLifetime.Transient);
            services.AddNamedService<IService, ServiceE>(nameof(ServiceE), ServiceLifetime.Transient);

            var dependencies = new[]
            {
                new NamedDependency(typeof(IService), nameof(ServiceD), "serviceX"),
                new NamedDependency(typeof(IService), nameof(ServiceE), "serviceY")
            };

            services.AddServiceWithNamedDependencies<IConsumerXY, ConsumerD>(ServiceLifetime.Transient, dependencies);

            var provider = services.BuildServiceProvider();
            var consumer = (ConsumerD) provider.GetService<IConsumerXY>();

            Assert.That(consumer, Has
                .Property("ServiceX").InstanceOf<ServiceD>().And
                .Property("ServiceY").InstanceOf<ServiceE>()
            );
        }

        private void AssertConsumerWithNamedServiceA()
        {
            services.AddServiceWithNamedDependencies<IConsumerXY, ConsumerC>(
                ServiceLifetime.Transient,
                new NamedDependency(typeof(IService), "A name", "service")
            );

            var provider = services.BuildServiceProvider();
            var resolved = provider.GetService(typeof(IConsumerXY));
            Assert.That(((ConsumerC) resolved).Service, Is.InstanceOf<ServiceA>());
        }
    }
}
