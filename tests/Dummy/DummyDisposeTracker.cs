namespace Butler.Tests.Dummy
{
    using System;

    internal class DummyDisposeTracker : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }
}