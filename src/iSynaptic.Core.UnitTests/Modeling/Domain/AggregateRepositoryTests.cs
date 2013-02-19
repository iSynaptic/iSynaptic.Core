using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestAggregates;

namespace iSynaptic.Modeling.Domain
{
    public abstract class AggregateRepositoryTests
    {
        protected IAggregateRepository<ServiceCase, Guid> Repo { get; set; }

        [Test]
        public async void RoundTrip()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);

            await Repo.Save(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(1);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.Priority.Should().Be(serviceCase.Priority);
        }

        [Test]
        public async void RoundTrip_UsingSnapshot()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(0);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.Priority.Should().Be(serviceCase.Priority);
        }

        [Test]
        public async void RoundTrip_WithChangeAfterSnapshot()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription);
            await Repo.Save(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(1);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.Priority.Should().Be(serviceCase.Priority);
        }

        [Test]
        public async void RoundTrip_WithMultipleSnapshots()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                              ServiceCase.SampleContent.TopicDescription);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow);
            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(0);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.Priority.Should().Be(serviceCase.Priority);
        }

        [Test]
        public async void TakingSnapshot_ForSameVersion_IsOkay()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);
            await Repo.SaveSnapshot(serviceCase);
        }
    }
}