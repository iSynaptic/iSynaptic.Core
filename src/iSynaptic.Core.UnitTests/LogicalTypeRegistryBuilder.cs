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
using System.Collections.Generic;
using iSynaptic.Modeling;
using iSynaptic.Serialization;
using iSynaptic.TestAggregates;

namespace iSynaptic
{
    public static class LogicalTypeRegistryBuilder
    {
        public static LogicalTypeRegistry Build()
        {
            var registry = new LogicalTypeRegistry();

            registry.AddMapping(new LogicalType("sys", "Guid"), typeof(Guid));
            registry.AddMapping(new LogicalType("sys", "StringToStringDictionary"), typeof(Dictionary<String, String>));

            registry.AddMapping(new LogicalType("tst", "AggregateMemento.Guid"), typeof(AggregateMemento<Guid>));
            registry.AddMapping(new LogicalType("tst", "AggregateMemento.RoleIdentifier"), typeof(AggregateMemento<RoleIdentifier>));

            registry.AddMapping(new LogicalType("tst", "CustomerRoleIdentifier"), typeof(CustomerRoleIdentifier));
            registry.AddMapping(new LogicalType("tst", "CustomerServiceRepresentativeRoleIdentifier"), typeof(CustomerServiceRepresentativeRoleIdentifier));
            registry.AddMapping(new LogicalType("tst", "CustomerAgentRoleIdentifier"), typeof(CustomerAgentRoleIdentifier));

            registry.AddMapping(new LogicalType("tst", "CustomerRole"), typeof(HomogeneousRole<CustomerRoleIdentifier>));
            registry.AddMapping(new LogicalType("tst", "CustomerServiceRepresentativeRole"), typeof(HomogeneousRole<CustomerServiceRepresentativeRoleIdentifier>));
            registry.AddMapping(new LogicalType("tst", "CustomerAgentRole"), typeof(HomogeneousRole<CustomerAgentRoleIdentifier>));

            registry.AddMapping(new LogicalType("tst", "CustomerRole.Registered"), typeof(HomogeneousRole<CustomerRoleIdentifier>.Registered));
            registry.AddMapping(new LogicalType("tst", "CustomerServiceRepresentativeRole.Registered"), typeof(HomogeneousRole<CustomerServiceRepresentativeRoleIdentifier>.Registered));
            registry.AddMapping(new LogicalType("tst", "CustomerAgentRole.Registered"), typeof(HomogeneousRole<CustomerAgentRoleIdentifier>.Registered));

            registry.AddMapping(new LogicalType("tst", "ServiceCase"), typeof(ServiceCase));
            registry.AddMapping(new LogicalType("tst", "ServiceCase.Opened"), typeof(ServiceCase.Opened));
            registry.AddMapping(new LogicalType("tst", "ServiceCase.Snapshot"), typeof(ServiceCase.Snapshot));
            registry.AddMapping(new LogicalType("tst", "ServiceCase.CommunicationThreadStarted"), typeof(ServiceCase.CommunicationThreadStarted));
            registry.AddMapping(new LogicalType("tst", "ServiceCase.CommunicationThreadSnapshot"), typeof(ServiceCase.CommunicationThreadSnapshot));
            registry.AddMapping(new LogicalType("tst", "ServiceCase.CommunicationRecorded"), typeof(ServiceCase.CommunicationRecorded));

            return registry;
        }
    }
}
