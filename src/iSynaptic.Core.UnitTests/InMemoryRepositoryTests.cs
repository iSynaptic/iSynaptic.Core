using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.TestAggregates;

namespace iSynaptic
{
    [TestFixture]
    public class InMemoryRepositoryTests
    {
        [Test]
        public async void RoundTrip()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            var repo = new InMemoryRepository<FauxPost, Guid>();
            await repo.Save(post);

            var reconsituted = await repo.Get(post.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(post);

            reconsituted.GetEvents().Count().Should().Be(2);
            reconsituted.Id.Should().Be(post.Id);
            reconsituted.Version.Should().Be(post.Version);
            reconsituted.Title.Should().Be(post.Title);
            reconsituted.Description.Should().Be(post.Description);
            reconsituted.Price.Should().Be(post.Price);
        }

        [Test]
        public async void RoundTrip_UsingSnapshot()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            var repo = new InMemoryRepository<FauxPost, Guid>();
            await repo.Save(post);
            await repo.SaveSnapshot(post.Id);

            var reconsituted = await repo.Get(post.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(post);

            reconsituted.GetEvents().Count().Should().Be(0);
            reconsituted.Id.Should().Be(post.Id);
            reconsituted.Version.Should().Be(post.Version);
            reconsituted.Title.Should().Be(post.Title);
            reconsituted.Description.Should().Be(post.Description);
            reconsituted.Price.Should().Be(post.Price);
        }

        [Test]
        public async void RoundTrip_WithChangeAfterSnapshot()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            var repo = new InMemoryRepository<FauxPost, Guid>();
            await repo.Save(post);
            await repo.SaveSnapshot(post.Id);

            post.ChangePrice(47.42m);
            await repo.Save(post);

            var reconsituted = await repo.Get(post.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(post);

            reconsituted.GetEvents().Count().Should().Be(1);
            reconsituted.Id.Should().Be(post.Id);
            reconsituted.Version.Should().Be(post.Version);
            reconsituted.Title.Should().Be(post.Title);
            reconsituted.Description.Should().Be(post.Description);
            reconsituted.Price.Should().Be(post.Price);
        }

        [Test]
        public async void RoundTrip_WithMultipleSnapshots()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            var repo = new InMemoryRepository<FauxPost, Guid>();
            await repo.Save(post);
            await repo.SaveSnapshot(post.Id);

            post.ChangePrice(47.42m);
            await repo.Save(post);
            await repo.SaveSnapshot(post.Id);

            var reconsituted = await repo.Get(post.Id);
            reconsituted.Should().NotBeNull();
            reconsituted.Should().NotBeSameAs(post);

            reconsituted.GetEvents().Count().Should().Be(0);
            reconsituted.Id.Should().Be(post.Id);
            reconsituted.Version.Should().Be(post.Version);
            reconsituted.Title.Should().Be(post.Title);
            reconsituted.Description.Should().Be(post.Description);
            reconsituted.Price.Should().Be(post.Price);
        }

        [Test]
        public async void TakingSnapshot_ForSameVersion_MoreThanOnceFails()
        {
            var post = new FauxPost("Test", "This is a test.", 47);
            post.ChangePrice(42.47m);

            var repo = new InMemoryRepository<FauxPost, Guid>();
            await repo.Save(post);
            await repo.SaveSnapshot(post.Id);

            repo.Invoking(x => x.SaveSnapshot(post.Id).Wait())
                .ShouldThrow<AggregateException>()
                .Subject.InnerException
                .Should().BeAssignableTo<InvalidOperationException>();

        }
    }
}
