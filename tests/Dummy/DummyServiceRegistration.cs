namespace Butler.Tests.Dummy
{
    using Butler.Lifetime;
    using Butler.Registration;
    using Butler.Resolver;
    using System;

    internal sealed class DummyServiceRegistration : IServiceRegistration
    {
        public IServiceLifetime ServiceLifetime => Lifetime.Transient;

        public object Create(ServiceResolveContext context) => throw new NotImplementedException();
    }
}