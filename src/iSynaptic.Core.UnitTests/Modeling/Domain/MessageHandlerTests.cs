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
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Commons;
using iSynaptic.TestDomain;

namespace iSynaptic.Modeling.Domain
{
    [TestFixture]
    public class MessageHandlerTests
    {
        [Test]
        public void Handler_DispatchesMessagesCorrectly()
        {
            var serviceCase = new ServiceCase(ServiceCase.SampleContent.Title, ServiceCase.SampleContent.Description, ServiceCasePriority.Normal);
            serviceCase.StartCommunicationThread(ServiceCase.SampleContent.Topic,
                                                 ServiceCase.SampleContent.TopicDescription);

            serviceCase.StartCommunicationThread("Win", "Winning");
            serviceCase.Threads.First().RecordCommunication(CommunicationDirection.Outgoing, "Also Win", SystemClock.UtcNow);

            var handler = new ServiceCaseProjector();
            handler.HandleEvents(serviceCase.GetEvents());

            handler.Title.Should().Be(ServiceCase.SampleContent.Title);
            handler.Description.Should().Be(ServiceCase.SampleContent.Description);
            handler.Priority.Should().Be(ServiceCasePriority.Normal);

            handler.Threads.Count().Should().Be(2);

            var thread = handler.Threads.First();
            thread.Topic.Should().Be(ServiceCase.SampleContent.Topic);
            thread.Description.Should().Be(ServiceCase.SampleContent.TopicDescription);

            var comm = thread.Communications.First();
            comm.Direction.Should().Be(CommunicationDirection.Outgoing);
            comm.Content.Should().Be("Also Win");
        }
        
        [Test]
        public void Handler_ShouldThrowExceptionUponUnexpectedMessage()
        {
            var @event = new UnexpectedEvent(Guid.NewGuid(), 1);

            var handler = new ServiceCaseProjector();

            handler.Invoking(x => x.HandleEvents(new[] { @event }))
                .ShouldThrow<InvalidOperationException>();
        }

        [Test]
        public void Handler_CanIgnoreMessages()
        {
            var @event = new IgnoredEvent(Guid.NewGuid(), 1);

            var handler = new ServiceCaseProjector();
            handler.HandleEvents(new[] { @event });
        }
    }
}
