using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestDomain;

namespace iSynaptic.Modeling.Domain
{
    public abstract class AggregateRepositoryTests
    {
        protected IAggregateRepository<ServiceCase, ServiceCaseId> Repo { get; set; }

        [Test]
        public async Task RoundTrip()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(1);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.ServiceCasePriority.Should().Be(serviceCase.ServiceCasePriority);

            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic, ServiceCase.SampleContent.TopicDescription, ServiceCase.SampleContent.ResponsibleParty);
            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);

            reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(3);
            var events = reconsituted.GetEvents().ToArray();

            var commRecordedEvent = events[2] as ServiceCase.CommunicationRecorded;
            commRecordedEvent.Should().NotBeNull();
            commRecordedEvent.Direction.Should().Be(CommunicationDirection.Incoming);
            commRecordedEvent.Content.Should().Be(ServiceCase.SampleContent.CommunicationContent);
            commRecordedEvent.Duration.Should().Be(ServiceCase.SampleContent.CommunicationDuration);
        }

        [Test]
        public async Task RoundTrip_UsingSnapshot()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);

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
            reconsituted.ServiceCasePriority.Should().Be(serviceCase.ServiceCasePriority);
        }

        [Test]
        public async Task RoundTrip_WithChangeAfterSnapshot()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription,
                                                 ServiceCase.SampleContent.ResponsibleParty);
            await Repo.Save(serviceCase);

            var reconsituted = await Repo.Get(serviceCase.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(serviceCase);

            reconsituted.GetEvents().Count().Should().Be(1);
            reconsituted.Id.Should().Be(serviceCase.Id);
            reconsituted.Version.Should().Be(serviceCase.Version);
            reconsituted.Title.Should().Be(serviceCase.Title);
            reconsituted.Description.Should().Be(serviceCase.Description);
            reconsituted.ServiceCasePriority.Should().Be(serviceCase.ServiceCasePriority);
        }

        [Test]
        public async Task RoundTrip_WithMultipleSnapshots()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            var thread = serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                              ServiceCase.SampleContent.TopicDescription,
                                                              ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);

            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, ServiceCase.SampleContent.ResponsibleParty);
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
            reconsituted.ServiceCasePriority.Should().Be(serviceCase.ServiceCasePriority);
        }

        [Test]
        public async Task TakingSnapshot_ForSameVersion_IsOkay()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription,
                                                 ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);
            await Repo.SaveSnapshot(serviceCase);
            await Repo.SaveSnapshot(serviceCase);
        }

        [Test]
        public async Task ConcurrecyConflict_WithTrueConflict_ThrowsException()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription,
                                                 ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);

            var winner = await Repo.Get(serviceCase.Id, Int32.MaxValue);
            var loser = await Repo.Get(serviceCase.Id, Int32.MaxValue);

            winner.StartCommunicationThread("Win", "Winning", ServiceCase.SampleContent.ResponsibleParty);
            await Repo.Save(winner);

            loser.StartCommunicationThread("Lose", "Loosing", ServiceCase.SampleContent.ResponsibleParty);

            try
            {
                await Repo.Save(loser);
                Assert.Fail("Exception was not thrown");
            }
            catch (AggregateConcurrencyException)
            {
            }
        }

        [Test]
        public async Task ConcurrencyConflict_WithFalseConflict_ResolvesConflict()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCase.Priority.Normal, ServiceCase.SampleContent.ResponsibleParty);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription,
                                                 ServiceCase.SampleContent.ResponsibleParty);

            await Repo.Save(serviceCase);

            var winner = await Repo.Get(serviceCase.Id, Int32.MaxValue);
            var secondWinner = await Repo.Get(serviceCase.Id, Int32.MaxValue);

            winner.StartCommunicationThread("Win", "Winning", ServiceCase.SampleContent.ResponsibleParty);
            await Repo.Save(winner);

            secondWinner.Threads.First().RecordCommunication(CommunicationDirection.Outgoing, "Also Win", SystemClock.UtcNow, ServiceCase.SampleContent.CommunicationDuration, ServiceCase.SampleContent.ResponsibleParty);
            await Repo.Save(secondWinner);

            secondWinner.Version.Should().Be(4);
            var events = secondWinner.GetEvents().ToArray();

            events[2].Should().BeOfType<ServiceCase.CommunicationThreadStarted>();
            events[3].Should().BeOfType<ServiceCase.CommunicationRecorded>();

            var cre = (ServiceCase.CommunicationRecorded) events[3];
            cre.Version.Should().Be(4);
            cre.Direction.Should().Be(CommunicationDirection.Outgoing);
            cre.Content.Should().Be("Also Win");
        }
    }
}