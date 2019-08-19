<!-- Banner -->
<a href="https://github.com/angelobreuer/Butler/">
	<img src="https://i.imgur.com/YsYyz1s.png"/>
</a>

<!-- Center badges -->
<p align="center">
	
<!-- CodeFactor.io Badge -->
<a href="https://www.codefactor.io/repository/github/angelobreuer/Butler">
	<img alt="CodeFactor.io" src="https://www.codefactor.io/repository/github/angelobreuer/Butler/badge?style=for-the-badge" />	
</a>

<!-- Releases Badge -->
<a href="https://github.com/angelobreuer/Butler/releases">
	<img alt="GitHub tag (latest SemVer)" src="https://img.shields.io/github/tag/angelobreuer/Butler.svg?label=RELEASE&style=for-the-badge">
</a>

<!-- GitHub issues Badge -->
<a href="https://github.com/angelobreuer/Butler/issues">
	<img alt="GitHub issues" src="https://img.shields.io/github/issues/angelobreuer/Butler.svg?style=for-the-badge">	
</a>

<br/>

<!-- AppVeyor CI (master) Badge 
<a href="https://ci.appveyor.com/project/angelobreuer/Butler">
	<img alt="AppVeyor" src="https://img.shields.io/appveyor/ci/angelobreuer/Butler?style=for-the-badge">
</a>	-->


<!-- AppVeyor CI (Development) Badge 
<a href="https://github.com/angelobreuer/Butler/tree/dev">
	<img alt="AppVeyor" src="https://img.shields.io/appveyor/ci/angelobreuer/Butler/dev?label=development&style=for-the-badge">
</a>-->


</p>

[Butler](https://github.com/angelobreuer/Butler) is an easy-to-use and beginner-friendly IoC *([Inversion of Control](https://en.wikipedia.org/wiki/Inversion_of_control))* container
designed for performance, low-memory consumption and easy integrability.


### Features
- 🔌 **At-Runtime Service Registration**
- ✳️ **Dependency Resolving**
- 📝 **Resolve Tracing**
- ⚡ **Advanced Debugging**
- ⏱️ **Service Lifetimes**
- ⬆ **Lightweight**

<span>&nbsp;&nbsp;&nbsp;</span>*and a lot more...*

### Minimum Requirements
- .NET Core 1.0, .NET Framework 2.0*, .NET Standard 1.0 or Mono 4.6

*Note: .NET Framework 2.0 does not support full 
reflection, since .NET Framework 3.5 all features of Butler are supported.*

### Quick Start

Let's get started with an easy example of resolving a basic service with a single dependency:

```csharp
// create a new root container
using (var container = new RootContainer())
{
  // register services (1)
  container.Register<ISomeDependency, SomeDependency>();
  container.Register<ISomeService, SomeService>();

  // resolve the service (2)
  var myService = container.Resolve<ISomeService>();
  
  // do something with the service
  myService.DoSomething();
}
```

In the above example the service and a dependency is registered *(1)*. 
Then the service is resolved. *(2)*

You can register as many services as you want, there are several methods to register a service.

#### Service Lifetimes

The container keeps care of dependency resolution and disposation of the service and dependencies. You can control the service lifetime using 
service lifetimes. Butler comes with three service-lifetimes out of the box: [`Singleton`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L29), [`Transient`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L15) and [`Scoped`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L22). By default Butler uses the
Transient lifetime which recreates the service each request / resolve.

##### Singleton

The singleton service lifetime ([`Lifetime.Singleton`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L29)) 
creates the service once, the instance is shared for all creations.

##### Scoped

The scoped service lifetime ([`Lifetime.Scoped`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L22)) creates the service for each new scope. You can set the scope key in the
resolve method when resolving the service. All resolves will retrieve the same service instance with the same scope key.

##### Transient

The transient service lifetime ([`Lifetime.Transient`](https://github.com/angelobreuer/Butler/blob/master/src/Lifetime/Lifetime.cs#L15)) creates a new service each 
resolve. This should be used for state-less small services. (*Note: By default Butler does not track / dispose 
transient services, this can be enabled using the `TrackDisposableTransients` property*)

##### Custom

You can implement lifetimes by implementing the ILifetime interface. Note that your ILifetime should be a singleton object.

#### Service Registration

##### Default Registration

The default registration is used for services that may have dependencies. This registration can have all types of lifetimes.

```csharp
// register ISomeService with SomeService as implementation as transient (default)
myContainer.Register<ISomeService, SomeService>();

// register SomeService as a singleton service
myContainer.Register<SomeService>().AsSingleton();

// register ISomeService with SomeService as implementation with a custom lifetime
myContainer.Register<ISomeService, SomeService>().WithLifetime(MyCustomLifetime);

```

##### Direct Registration

A direct registration is a registration for a service that has no dependencies and is constructed over a parameter-less constructor.

```csharp
// register direct service (Note: SomeService must have a parameter-less constructor!)
myContainer.RegisterDirect<ISomeService, SomeService>();
```

##### Factory Registration

A factory registration is a registration that is retrieved from a factory *(see: [`ServiceFactory<T>`](https://github.com/angelobreuer/Butler/blob/master/src/Resolver/ServiceFactory.cs))*.

```csharp
// register a DateTimeOffset factory which returns the current UTC time offset
myContainer.RegisterFactory<DateTimeOffset>(context => DateTimeOffset.UtcNow);

var currentTimeUtc = myContainer.Resolve<DateTimeOffset>();
Console.WriteLine($"Current Time: {currentTimeUtc} UTC.");
// > Current Time: 8/18/2019 12:36:43 PM +00:00 UTC.
```

##### Instance Registration

An instance registration is a registration for already supplied services. The registration must
keep care of disposing the instance itself.

```csharp
// create the service instance
var myService = new SomeService();

// register the service instance
myContainer.RegisterInstance<ISomeService, SomeService>(myService);

// do something with the service
myContainer.Resolve<ISomeService>().DoSomething();

// dispose the instance
myService.Dispose();
```

##### Multi-Registration

A multi-registration can contain as the name says multiple registrations in once: For example you want to register
multiple services with the same service type:

```csharp
// enable automatic multi-registration creation
myContainer.DefaultRegistrationMode = ServiceRegistrationMode.Append;

// register multiple registrations with the same type
myContainer.Register<ISomeService, SomeService>();
myContainer.Register<ISomeService, SomeOtherService>();

// resolve all
myContainer.ResolveAll<ISomeService>();
// --> Contains an instance of SomeService and SomeOtherService
```

*Note: The service instances are created when enumerating through the enumerable.*
