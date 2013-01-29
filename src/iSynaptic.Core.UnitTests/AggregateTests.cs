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
using iSynaptic.TestAggregates;

namespace iSynaptic
{
    [TestFixture]
    public class AggregateTests
    {
        [Test]
        public void CreatedPost_AppliesEvent()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            
            post.Id.Should().NotBe(Guid.Empty);
            post.Version.Should().Be(1);
            post.Title.Should().Be("Test");
            post.Description.Should().Be("This is a test.");
            post.Price.Should().Be(47);
        }

        [Test]
        public void UpdateTextCopyCommand_AppliesEvent()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.UpdateTextCopy("T2", "D2");

            post.Id.Should().NotBe(Guid.Empty);
            post.Version.Should().Be(2);
            post.Title.Should().Be("T2");
            post.Description.Should().Be("D2");
            post.Price.Should().Be(47);
        }

        [Test]
        public void UncommittedEventsRetreivable()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            post.GetEvents().Count().Should().Be(2);
        }
    }
}