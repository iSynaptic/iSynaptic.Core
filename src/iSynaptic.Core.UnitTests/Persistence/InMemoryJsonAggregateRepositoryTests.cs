using System;
using NUnit.Framework;
using iSynaptic.Core.Persistence;
using iSynaptic.Serialization;
using iSynaptic.TestAggregates;

namespace iSynaptic.Persistence
{
    [TestFixture]
    public class InMemoryJsonAggregateRepositoryTests : AggregateRepositoryTests
    {
        public InMemoryJsonAggregateRepositoryTests()
        {
            Repo = new InMemoryJsonAggregateRepository<ServiceCase, Guid>(
                JsonSerializerBuilder.Build(LogicalTypeRegistryBuilder.Build()));
        }
    }
}