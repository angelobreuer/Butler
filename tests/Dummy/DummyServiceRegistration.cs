namespace Butler.Tests.Dummy
{
    using Butler.Registration;
    using System;

    internal sealed class DummyServiceRegistration : IServiceRegistration
    {
        public object Create() => throw new NotImplementedException();
    }
}