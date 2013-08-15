using System;
using NUnit.Framework;
using iSynaptic.Core.Persistence;
using iSynaptic.Modeling;
using iSynaptic.Modeling.Domain;
using iSynaptic.Serialization;
using iSynaptic.TestDomain;

namespace iSynaptic.Persistence
{
    [TestFixture]
    public class InMemoryJsonAggregateRepositoryTests : AggregateRepositoryTests
    {
        public InMemoryJsonAggregateRepositoryTests()
        {
            Repo = new InMemoryJsonAggregateRepository<ServiceCase, ServiceCaseId>(
                JsonSerializerBuilder.Build(LogicalTypeRegistryBuilder.Build()));
        }
    }
}