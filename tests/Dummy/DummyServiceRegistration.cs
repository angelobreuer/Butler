namespace Butler.Tests.Dummy
{
    using Butler.Registration;
    using System;

    internal sealed class DummyServiceRegistration : IServiceRegistration
    {
        public object Create(ServiceResolveContext context) => throw new NotImplementedException();
    }
}