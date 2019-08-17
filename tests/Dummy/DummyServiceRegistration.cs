namespace Butler.Tests.Dummy
{
    using Butler.Registration;
    using Butler.Resolver;
    using System;

    internal sealed class DummyServiceRegistration : IServiceRegistration
    {
        public object Create(ServiceResolveContext context) => throw new NotImplementedException();
    }
}