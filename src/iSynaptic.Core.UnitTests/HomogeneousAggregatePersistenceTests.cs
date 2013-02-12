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

using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using iSynaptic.Core.Persistence;
using iSynaptic.Serialization;
using iSynaptic.TestAggregates;

namespace iSynaptic
{
    [TestFixture]
    public class HomogeneousAggregatePersistenceTests
    {
        private static readonly CustomerRoleIdentifier _cId =
            new CustomerRoleIdentifier("123", "CR456");

        private static readonly CustomerServiceRepresentativeRoleIdentifier _csrId =
            new CustomerServiceRepresentativeRoleIdentifier("123", "CSR456");

        private static readonly CustomerAgentRoleIdentifier _caId =
            new CustomerAgentRoleIdentifier("123", "CA456");

        [Test]
        public async void Save_ViaInMemoryRepo()
        {
            var customer = new HomogeneousRole<CustomerRoleIdentifier>(_cId, "Joe Customer");
            var serviceRep = new HomogeneousRole<CustomerServiceRepresentativeRoleIdentifier>(_csrId, "Jane Rep");
            var agent = new HomogeneousRole<CustomerAgentRoleIdentifier>(_caId, "Jill Agent");

            IAggregateRepository<IHomogeneousRole<RoleIdentifier>, RoleIdentifier> repo
                = new InMemoryJsonAggregateRepository<IHomogeneousRole<RoleIdentifier>, RoleIdentifier>(
                    JsonSerializerBuilder.Build(
                        LogicalTypeRegistryBuilder.Build()));

            await SaveAndTest(repo, customer, serviceRep, agent);
        }

        private async Task SaveAndTest(IAggregateRepository<IHomogeneousRole<RoleIdentifier>, RoleIdentifier> repo, params IHomogeneousRole<RoleIdentifier>[] roles)
        {
            foreach (var role in roles)
                await repo.Save(role);

            foreach (var role in roles)
            {
                var reconstituted = await repo.Get(role.Id);

                reconstituted.Should().NotBeNull();
                reconstituted.Should().NotBeSameAs(role);
                reconstituted.Id.Should().Be(role.Id);
                reconstituted.Version.Should().Be(role.Version);
                reconstituted.Name.Should().Be(role.Name);
                reconstituted.Status.Should().Be(role.Status);
            }
        }
    }
}
