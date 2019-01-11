using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSDI.NamedServiceExtensions
{
    /// <summary>
    /// A factory for resolving named services.
    /// </summary>
    internal interface INamedServiceFactory
    {
        /// <summary>
        /// Gets the service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// Resolves the service type matching the given name. 
        /// </summary>
        /// <param name="name">The name the service type is registered as.</param>
        /// <param name="provider">The service provider for retrieving service objects.</param>
        /// <returns>The <see cref="object"/></returns>
        object Resolve(string name, IServiceProvider provider);
    }

    /// <summary>
    /// A factory for resolving named services.
    /// </summary>
    /// <typeparam name="TService">The type of service to resolve.</typeparam>
    internal class NamedServiceFactory<TService> : INamedServiceFactory
    {
        private readonly ConcurrentDictionary<CompositeStringTypeKey, Type> namedServices = new ConcurrentDictionary<CompositeStringTypeKey, Type>();

        /// <inheritdoc/>
        public Type ServiceType { get; } = typeof(TService);

        /// <summary>
        /// Registers the given implementation type with the name.
        /// </summary>
        /// <typeparam name="TImplementation">The implemention type.</typeparam>
        /// <param name="name">The name to register the type as.</param>
        public void Register<TImplementation>(string name)
            where TImplementation : class
            => this.namedServices.TryAdd(new CompositeStringTypeKey(name, typeof(TService)), typeof(TImplementation));

        /// <summary>
        /// Resolves the service type matching the given name. 
        /// </summary>
        /// <param name="provider">The service provider for retrieving service objects.</param>
        /// <param name="name">The name the service type is registered as.</param>
        /// <returns>The <see cref="TService"/></returns>
        public TService Resolve(IServiceProvider provider, string name) => (TService)this.Resolve(name, provider);

        /// <inheritdoc/>
        public object Resolve(string name, IServiceProvider provider)
        {
            if (!this.namedServices.TryGetValue(new CompositeStringTypeKey(name, typeof(TService)), out Type implementation))
            {
                throw new InvalidOperationException($"No service for type {typeof(TService)} named '{name}' has been registered.");
            }

            return provider.GetService(implementation);
        }

        private readonly struct CompositeStringTypeKey : IEquatable<CompositeStringTypeKey>
        {
            public CompositeStringTypeKey(string name, Type type)
            {
                this.Name = name;
                this.Type = type;
            }

            public string Name { get; }
            public Type Type { get; }

            public override bool Equals(object obj) => obj is CompositeStringTypeKey nameTypeKey && this.Equals(nameTypeKey);

            public bool Equals(CompositeStringTypeKey other) => this.Name == other.Name && EqualityComparer<Type>.Default.Equals(this.Type, other.Type);

            public override int GetHashCode() => HashCode.Combine(this.Name, this.Type);
        }
    }
}
