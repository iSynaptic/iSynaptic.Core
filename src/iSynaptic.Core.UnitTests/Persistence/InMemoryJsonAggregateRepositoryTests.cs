﻿using System;
using NUnit.Framework;
using iSynaptic.Core.Persistence;
using iSynaptic.Modeling;
using iSynaptic.Modeling.Domain;
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