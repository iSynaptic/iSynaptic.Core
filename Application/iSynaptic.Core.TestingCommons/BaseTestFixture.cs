using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace iSynaptic.Core.TestingCommons
{
    public abstract class BaseTestFixture
    {
        private IEnumerable<ITestFixtureBehavior> _FixtureBehaviors = null;
        private IEnumerable<ITestBehavior> _TestBehaviors = null;

        [TestFixtureSetUp]
        protected virtual void BeforeTestFixture()
        {
            _FixtureBehaviors = GetAttributes<ITestFixtureBehavior>()
                .ToArray();

            _TestBehaviors = GetAttributes<ITestBehavior>()
                .ToArray();

            foreach (var behavior in _FixtureBehaviors)
                behavior.BeforeTestFixture();
        }

        [TestFixtureTearDown]
        protected virtual void AfterTestFixture()
        {
            foreach (var behavior in _FixtureBehaviors)
                behavior.AfterTestFixture();
        }

        [SetUp]
        protected virtual void BeforeTest()
        {
            foreach (var behavior in _TestBehaviors)
                behavior.BeforeTest();
        }

        [TearDown]
        protected virtual void AfterTest()
        {
            foreach (var behavior in _TestBehaviors)
                behavior.AfterTest();
        }

        protected IEnumerable<T> GetAttributes<T>()
        {
            Type fixtureType = GetType();
            return fixtureType.GetCustomAttributes(true)
                .Where(x => typeof(T).IsAssignableFrom(x.GetType()))
                .Cast<T>();
        }
    }
}
