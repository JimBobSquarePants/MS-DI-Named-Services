# Microst Dependency Injection Using Named Services

A demonstration of how to use the Microsoft Dependency Injection Abstractions to register named services including registering as parameters named or otherwise.  
  
**Set**

```c#  
// Adds a service/implementation relationship to the collection, keyed to the type name of the concrete implementation. 
IServiceCollection.AddNamedService<TService, TImplementation>(ServiceLifetime lifetime);

// Adds a service/implementation relationship to the collection, keyed to the given name.
IServiceCollection.AddNamedService<TService, TImplementation>(string name, ServiceLifetime lifetime);

// Adds a service/implementation relationship to the collection for the given target type.
IServiceCollection.AddServiceFor<TService, TImplementation, TTarget>(ServiceLifetime serviceLifetime, ServiceLifetime targetLifetime);

// Adds a service to the collection specifying what named dependencies to assign to named parameters.
IServiceCollection.AddServiceWithNamedDependencies<TService, TImplementation>(ServiceLifetime lifetime, params NamedDependency[] dependencies)
```  
    
**Retrieve**   

```c#  
// Returns an instance of the service type matching the given name.
IServiceProvider.GetNamedService<TService>(string name);
```
