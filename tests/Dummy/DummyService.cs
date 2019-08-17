namespace Butler.Tests.Dummy
{
    internal class DummyService : IDummyService
    {
        public int SomeValue { get; set; }

        public void DoSomething()
        {
            SomeValue = 12;
        }
    }
}