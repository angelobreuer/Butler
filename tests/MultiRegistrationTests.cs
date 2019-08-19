namespace Butler.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Butler.Registration;
    using Butler.Tests.Dummy;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="MultiRegistration"/> class.
    /// </summary>
    public sealed class MultiRegistrationTests
    {
        [Fact]
        public void TestInconsistence()
        {
            // an enumerable containing inconsistent service registrations
            var enumerable = new[] {
                new DirectRegistration<DummyService>().AsScoped(),
                new DirectRegistration<DummyService>().AsSingleton()
            };

            Assert.Throws<InvalidOperationException>(
                () => new MultiRegistration(enumerable));
        }

        [Fact]
        public void TestConsistence()
        {
            // an enumerable containing consistent service registrations
            var enumerable = new[] {
                new DirectRegistration<DummyService>().AsSingleton(),
                new DirectRegistration<DummyService>().AsSingleton()
            };

            // should not fail, because all service registrations are consistent
            new MultiRegistration(enumerable);
        }

        [Fact]
        public void TestAddRange()
        {
            // create registration
            var registration = new MultiRegistration(new[] { new DirectRegistration<DummyService>() });

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);

            // Add new registrations
            registration.AddRange((IEnumerable<IServiceRegistration>)new[] {
                new DirectRegistration<DummyService>(),
                new DirectRegistration<DummyService>() });

            // assert that item count increased
            Assert.Equal(3, registration.Registrations.Count);
        }

        [Fact]
        public void TestAddRangeInconsistent()
        {
            // create registration
            var registration = new MultiRegistration(new[] {
                new DirectRegistration<DummyService>().AsSingleton() });

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);

            // Add new registrations
            Assert.Throws<InvalidOperationException>(
                () => registration.AddRange((IEnumerable<IServiceRegistration>)new[] {
                new DirectRegistration<DummyService>().AsScoped(),
                new DirectRegistration<DummyService>().AsTransient() }));

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);
        }

        [Fact]
        public void TestAddInconsistent()
        {
            // create registration
            var registration = new MultiRegistration(new[] {
                new DirectRegistration<DummyService>().AsTransient()
            });

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);

            // Add new registration
            Assert.Throws<InvalidOperationException>(
                () => registration.Add(new DirectRegistration<DummyService>().AsScoped()));

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);
        }

        [Fact]
        public void TestAdd()
        {
            // create registration
            var registration = new MultiRegistration(new[] { new DirectRegistration<DummyService>() });

            // assert that there is only a single item in the registration
            Assert.Single(registration.Registrations);

            // Add new registration
            registration.Add(new DirectRegistration<DummyService>());

            // assert that item count increased
            Assert.Equal(2, registration.Registrations.Count);
        }

        [Fact]
        public void TestRemove()
        {
            var registration = new DirectRegistration<DummyService>().AsSingleton();

            // an enumerable containing consistent service registrations
            var enumerable = new[] {
                registration,
                new DirectRegistration<DummyService>().AsSingleton()
            };

            Assert.True(new MultiRegistration(enumerable).Remove(registration));
        }

        [Fact]
        public void TestRemoveToEmpty()
        {
            var registration = new DirectRegistration<DummyService>().AsSingleton();

            Assert.Throws<InvalidOperationException>(
                () => new MultiRegistration(new[] { registration }).Remove(registration));
        }

        [Fact]
        public void TestInconsistenceAfterConstruction()
        {
            var registration1 = new DirectRegistration<DummyService>().AsSingleton();
            var registration2 = new DirectRegistration<DummyService>().AsSingleton();
            var registrations = new IServiceRegistration[] { registration1, registration2 };
            var multiRegistration = new MultiRegistration(registrations);

            // change the lifetime of a service
            registration1.AsScoped();

            using (var container = new RootContainer())
            {
                container.Register<IDummyService>(multiRegistration);
                var enumerable = container.ResolveAll<IDummyService>();

                // create services
                Assert.Throws<InvalidOperationException>(enumerable.ToArray);
            }
        }

        [Fact]
        public void TestNoRegistrations()
        {
            Assert.Throws<ArgumentException>(
                () => new MultiRegistration(Array.Empty<IServiceRegistration>()));
        }
    }
}