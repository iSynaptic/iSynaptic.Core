// The MIT License
// 
// Copyright (c) 2013 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestAggregates;

namespace iSynaptic
{
    [TestFixture(typeof(InMemoryAggregateRepository<ServiceCase, Guid>))]
    [TestFixture(typeof(InMemoryJsonAggregateRepository<ServiceCase, Guid>))]
    [CLSCompliant(false)]
    public class InMemoryAggregateRepositoryTests
    {
        private readonly IAggregateRepository<ServiceCase, Guid> _repo;

        public InMemoryAggregateRepositoryTests(Type repoType)
        {
            _repo = (IAggregateRepository<ServiceCase, Guid>)Activator.CreateInstance(repoType);
        }

        [Test]
        public async void RoundTrip()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);

            await _repo.Save(serviceCase);

            var reconsituted = await _repo.Get(serviceCase.Id);
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

            await _repo.Save(serviceCase);
            await _repo.SaveSnapshot(serviceCase.Id);

            var reconsituted = await _repo.Get(serviceCase.Id);
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

            await _repo.Save(serviceCase);
            await _repo.SaveSnapshot(serviceCase.Id);

            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription);
            await _repo.Save(serviceCase);

            var reconsituted = await _repo.Get(serviceCase.Id);
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

            await _repo.Save(serviceCase);
            await _repo.SaveSnapshot(serviceCase.Id);

            thread.RecordCommunication(CommunicationDirection.Incoming, ServiceCase.SampleContent.CommunicationContent, SystemClock.UtcNow);
            await _repo.Save(serviceCase);
            await _repo.SaveSnapshot(serviceCase.Id);

            var reconsituted = await _repo.Get(serviceCase.Id);
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
        public async void TakingSnapshot_ForSameVersion_MoreThanOnceFails()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                              ServiceCase.SampleContent.TopicDescription);

            await _repo.Save(serviceCase);
            await _repo.SaveSnapshot(serviceCase.Id);

            _repo.Invoking(x => x.SaveSnapshot(serviceCase.Id).Wait())
                .ShouldThrow<AggregateException>()
                .Subject.InnerException
                .Should().BeAssignableTo<InvalidOperationException>();
        }
    }
}
