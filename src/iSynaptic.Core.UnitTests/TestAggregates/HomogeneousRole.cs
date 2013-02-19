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
using iSynaptic.Modeling;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.TestAggregates
{
    // This is a poor example of an aggregate since it is devoid of much logic and exposes state.
    // It exists only to facilitate testing of the framework around aggregates
    public class HomogeneousRole<TRoleIdentifier> : Aggregate<TRoleIdentifier>, IHomogeneousRole<TRoleIdentifier>
        where TRoleIdentifier : RoleIdentifier, IEquatable<TRoleIdentifier>
    {
        public HomogeneousRole(TRoleIdentifier id, String name)
        {
            ApplyEvent(new Registered(id, name));
        }

        #region Events
        
        public class Registered : AggregateEvent<TRoleIdentifier>
        {
            public Registered(TRoleIdentifier id, String name) : base(id, 1)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }

        public class StatusChanged : AggregateEvent<TRoleIdentifier>
        {
            public StatusChanged(TRoleIdentifier id, int version, HomogeneousRoleStatus status) : base(id, version)
            {
                Status = status;
            }

            public HomogeneousRoleStatus Status { get; private set; }
        }

        #endregion

        public void StartApproval()
        {
            ApplyEvent((id, ver) => new StatusChanged(id, ver, HomogeneousRoleStatus.PendingApproval));
        }

        public void Approve()
        {
            ApplyEvent((id, ver) => new StatusChanged(id, ver, HomogeneousRoleStatus.Approved));
        }

        public void Retire()
        {
            ApplyEvent((id, ver) => new StatusChanged(id, ver, HomogeneousRoleStatus.Retired));
        }

        private void On(Registered @event)
        {
            Name = @event.Name;
            Status = HomogeneousRoleStatus.New;
        }

        private void On(StatusChanged @event)
        {
            Status = @event.Status;
        }

        // these exists ONLY for testing purposes; aggregates should not expose state
        public String Name { get; private set; }
        public HomogeneousRoleStatus Status { get; private set; }
    }

    public interface IHomogeneousRole<out TRoleIdentifier> : IAggregate<TRoleIdentifier>
        where TRoleIdentifier : RoleIdentifier, IEquatable<TRoleIdentifier>
    {
        void StartApproval();
        void Approve();
        void Retire();

        // these exists ONLY for testing purposes; aggregates should not expose state
        String Name { get; }
        HomogeneousRoleStatus Status { get; }
    }

    public enum HomogeneousRoleStatus
    {
        New,
        PendingApproval,
        Approved,
        Retired
    }
}