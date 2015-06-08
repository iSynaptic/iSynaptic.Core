using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using iSynaptic;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class CustomerAgentRoleIdentifier : RoleIdentifier, IEquatable<CustomerAgentRoleIdentifier>
    {

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public CustomerAgentRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(CustomerAgentRoleIdentifier other)
        {
            return Equals((Object)other);
        }


        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public new class Essence : RoleIdentifier.Essence
        {

            public new CustomerAgentRoleIdentifier Create()
            {
                return (CustomerAgentRoleIdentifier)CreateValue();
            }

            protected override RoleIdentifier CreateValue()
            {
                return new CustomerAgentRoleIdentifier(BaseIdentifier, IdentifierType);
            }
        }

        public new Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected override RoleIdentifier.Essence CreateEssence()
        {
            return new Essence
            {
                BaseIdentifier = BaseIdentifier,
                IdentifierType = IdentifierType
            };
        }

    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class CustomerServiceRepresentativeRoleIdentifier : RoleIdentifier, IEquatable<CustomerServiceRepresentativeRoleIdentifier>
    {

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public CustomerServiceRepresentativeRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(CustomerServiceRepresentativeRoleIdentifier other)
        {
            return Equals((Object)other);
        }


        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public new class Essence : RoleIdentifier.Essence
        {

            public new CustomerServiceRepresentativeRoleIdentifier Create()
            {
                return (CustomerServiceRepresentativeRoleIdentifier)CreateValue();
            }

            protected override RoleIdentifier CreateValue()
            {
                return new CustomerServiceRepresentativeRoleIdentifier(BaseIdentifier, IdentifierType);
            }
        }

        public new Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected override RoleIdentifier.Essence CreateEssence()
        {
            return new Essence
            {
                BaseIdentifier = BaseIdentifier,
                IdentifierType = IdentifierType
            };
        }

    }
}
namespace iSynaptic.TestDomain
{
    public partial class Base<T> : Aggregate<T>
        where T : IEquatable<T>
    {
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        [AggregateEventVersion(1)]
        public abstract class BaseEvent : AggregateEvent<T>
        {
            private readonly String _responsibleParty;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            protected BaseEvent(String responsibleParty, T id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(responsibleParty, null)) throw new ArgumentNullException("responsibleParty");

                _responsibleParty = responsibleParty;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String ResponsibleParty { get { return _responsibleParty; } }
        }
    }
}
namespace iSynaptic.TestDomain
{
    public partial class ServiceCase : Base<ServiceCaseId>
    {
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        protected void ApplyOpened(String title, String description, ServiceCasePriority priority, String responsibleParty)
        {
            if (Version <= 0) throw new InvalidOperationException("This overload of ApplyOpened can only be called after the first event is applied.");

            ApplyEvent(new Opened(title, description, priority, responsibleParty, Id, Version + 1));
        }
        protected void ApplyOpened(String title, String description, ServiceCasePriority priority, String responsibleParty, ServiceCaseId id)
        {
            ApplyEvent(new Opened(title, description, priority, responsibleParty, id, Version + 1));
        }
        protected void ApplyCommunicationThreadStarted(Int32 threadId, String topic, String description, String responsibleParty)
        {
            if (Version <= 0) throw new InvalidOperationException("This overload of ApplyCommunicationThreadStarted can only be called after the first event is applied.");

            ApplyEvent(new CommunicationThreadStarted(threadId, topic, description, responsibleParty, Id, Version + 1));
        }
        protected void ApplyCommunicationThreadStarted(Int32 threadId, String topic, String description, String responsibleParty, ServiceCaseId id)
        {
            ApplyEvent(new CommunicationThreadStarted(threadId, topic, description, responsibleParty, id, Version + 1));
        }
        protected void ApplyCommunicationRecorded(Int32 threadId, CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration, String responsibleParty)
        {
            if (Version <= 0) throw new InvalidOperationException("This overload of ApplyCommunicationRecorded can only be called after the first event is applied.");

            ApplyEvent(new CommunicationRecorded(threadId, direction, content, communicationTime, duration, responsibleParty, Id, Version + 1));
        }
        protected void ApplyCommunicationRecorded(Int32 threadId, CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration, String responsibleParty, ServiceCaseId id)
        {
            ApplyEvent(new CommunicationRecorded(threadId, direction, content, communicationTime, duration, responsibleParty, id, Version + 1));
        }
        [AggregateEventVersion(3)]
        public class Opened : BaseEvent
        {
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Opened(String title, String description, ServiceCasePriority priority, String responsibleParty, ServiceCaseId id, Int32 version)
                : base(responsibleParty, id, version)
            {
                if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _title = title;
                _description = description;
                _priority = priority;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Title { get { return _title; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Description { get { return _description; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public ServiceCasePriority Priority { get { return _priority; } }
        }
        [AggregateEventVersion(1)]
        public class CommunicationThreadStarted : BaseEvent
        {
            private readonly Int32 _threadId;
            private readonly String _topic;
            private readonly String _description;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public CommunicationThreadStarted(Int32 threadId, String topic, String description, String responsibleParty, ServiceCaseId id, Int32 version)
                : base(responsibleParty, id, version)
            {
                if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _threadId = threadId;
                _topic = topic;
                _description = description;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Int32 ThreadId { get { return _threadId; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Topic { get { return _topic; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Description { get { return _description; } }
        }
        [AggregateEventVersion(1)]
        public class CommunicationRecorded : BaseEvent
        {
            private readonly Int32 _threadId;
            private readonly CommunicationDirection _direction;
            private readonly String _content;
            private readonly DateTime _communicationTime;
            private readonly TimeSpan _duration;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public CommunicationRecorded(Int32 threadId, CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration, String responsibleParty, ServiceCaseId id, Int32 version)
                : base(responsibleParty, id, version)
            {
                if (ReferenceEquals(content, null)) throw new ArgumentNullException("content");

                _threadId = threadId;
                _direction = direction;
                _content = content;
                _communicationTime = communicationTime;
                _duration = duration;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Int32 ThreadId { get { return _threadId; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public CommunicationDirection Direction { get { return _direction; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Content { get { return _content; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public DateTime CommunicationTime { get { return _communicationTime; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public TimeSpan Duration { get { return _duration; } }
        }
        public class Snapshot : AggregateSnapshot<ServiceCaseId>
        {
            private readonly Int32 _lastThreadId;
            private readonly IEnumerable<CommunicationThreadSnapshot> _threadSnapshots;
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Snapshot(Int32 lastThreadId, IEnumerable<CommunicationThreadSnapshot> threadSnapshots, String title, String description, ServiceCasePriority priority, ServiceCaseId id, Int32 version, DateTime takenAt)
                : base(id, version, takenAt)
            {
                if (ReferenceEquals(threadSnapshots, null)) throw new ArgumentNullException("threadSnapshots");
                if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _lastThreadId = lastThreadId;
                _threadSnapshots = threadSnapshots;
                _title = title;
                _description = description;
                _priority = priority;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Int32 LastThreadId { get { return _lastThreadId; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { get { return _threadSnapshots; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Title { get { return _title; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Description { get { return _description; } }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public ServiceCasePriority Priority { get { return _priority; } }
        }
    }
}
namespace iSynaptic.TestDomain
{
    public partial class HomogeneousRole<TRoleIdentifier> : Aggregate<TRoleIdentifier>
        where TRoleIdentifier : RoleIdentifier, IEquatable<TRoleIdentifier>
    {
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        protected void ApplyRegistered(string name)
        {
            if (Version <= 0) throw new InvalidOperationException("This overload of ApplyRegistered can only be called after the first event is applied.");

            ApplyEvent(new Registered(name, Id, Version + 1));
        }
        protected void ApplyRegistered(string name, TRoleIdentifier id)
        {
            ApplyEvent(new Registered(name, id, Version + 1));
        }
        protected void ApplyStatusChanged(HomogeneousRoleStatus status)
        {
            if (Version <= 0) throw new InvalidOperationException("This overload of ApplyStatusChanged can only be called after the first event is applied.");

            ApplyEvent(new StatusChanged(status, Id, Version + 1));
        }
        protected void ApplyStatusChanged(HomogeneousRoleStatus status, TRoleIdentifier id)
        {
            ApplyEvent(new StatusChanged(status, id, Version + 1));
        }
        [AggregateEventVersion(1)]
        public class Registered : AggregateEvent<TRoleIdentifier>
        {
            private readonly string _name;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Registered(string name, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(name, null)) throw new ArgumentNullException("name");

                _name = name;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public string Name { get { return _name; } }
        }
        [AggregateEventVersion(1)]
        public class StatusChanged : AggregateEvent<TRoleIdentifier>
        {
            private readonly HomogeneousRoleStatus _status;

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public StatusChanged(HomogeneousRoleStatus status, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {

                _status = status;
            }

            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public HomogeneousRoleStatus Status { get { return _status; } }
        }
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public partial class CommunicationThreadSnapshot : IEquatable<CommunicationThreadSnapshot>
    {
        private readonly Int32 _threadId;
        private readonly String _topic;
        private readonly String _description;

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public CommunicationThreadSnapshot(Int32 threadId, String topic, String description)
        {
            if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
            if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");
            Validate(threadId, topic, description);

            _threadId = threadId;
            _topic = topic;
            _description = description;
        }

        partial void Validate(Int32 threadId, String topic, String description);

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(CommunicationThreadSnapshot other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Boolean Equals(Object obj)
        {
            CommunicationThreadSnapshot other = obj as CommunicationThreadSnapshot;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!ThreadId.Equals(other.ThreadId)) return false;
            if (!Topic.Equals(other.Topic)) return false;
            if (!Description.Equals(other.Description)) return false;

            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + ThreadId.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Topic.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Description.GetHashCode());

            return hash;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator ==(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator !=(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public class Essence
        {
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Int32 ThreadId { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Topic { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Description { protected get; set; }

            public CommunicationThreadSnapshot Create()
            {
                return (CommunicationThreadSnapshot)CreateValue();
            }

            protected virtual CommunicationThreadSnapshot CreateValue()
            {
                return new CommunicationThreadSnapshot(ThreadId, Topic, Description);
            }
        }

        public Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected virtual CommunicationThreadSnapshot.Essence CreateEssence()
        {
            return new Essence
            {
                ThreadId = ThreadId,
                Topic = Topic,
                Description = Description
            };
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Int32 ThreadId { get { return _threadId; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String Topic { get { return _topic; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String Description { get { return _description; } }
    }
}
namespace iSynaptic.TestDomain
{
    public enum CommunicationDirection
    {
        Incoming,
        Outgoing
    }
}
namespace iSynaptic.TestDomain
{
    public enum ServiceCasePriority
    {
        Low,
        Normal,
        High
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class ServiceCaseDetails : IEquatable<ServiceCaseDetails>
    {
        private readonly Int32 _lastThreadId;
        private readonly IEnumerable<CommunicationThreadSnapshot> _threadSnapshots;
        private readonly String _title;
        private readonly String _description;
        private readonly ServiceCasePriority _serviceCasePriority;

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public ServiceCaseDetails(Int32 lastThreadId, IEnumerable<CommunicationThreadSnapshot> threadSnapshots, String title, String description, ServiceCasePriority serviceCasePriority)
        {
            if (ReferenceEquals(threadSnapshots, null)) throw new ArgumentNullException("threadSnapshots");
            if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
            if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

            _lastThreadId = lastThreadId;
            _threadSnapshots = threadSnapshots;
            _title = title;
            _description = description;
            _serviceCasePriority = serviceCasePriority;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(ServiceCaseDetails other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Boolean Equals(Object obj)
        {
            ServiceCaseDetails other = obj as ServiceCaseDetails;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!LastThreadId.Equals(other.LastThreadId)) return false;
            if (!ThreadSnapshots.SequenceEqual(other.ThreadSnapshots)) return false;
            if (!Title.Equals(other.Title)) return false;
            if (!Description.Equals(other.Description)) return false;
            if (!ServiceCasePriority.Equals(other.ServiceCasePriority)) return false;

            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + LastThreadId.GetHashCode());
            hash = ThreadSnapshots.Aggregate(hash, (h, item) => HashCode.MixJenkins32(h + item.GetHashCode()));
            hash = HashCode.MixJenkins32(hash + Title.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Description.GetHashCode());
            hash = HashCode.MixJenkins32(hash + ServiceCasePriority.GetHashCode());

            return hash;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator ==(ServiceCaseDetails left, ServiceCaseDetails right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator !=(ServiceCaseDetails left, ServiceCaseDetails right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public class Essence
        {
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public Int32 LastThreadId { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Title { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String Description { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public ServiceCasePriority ServiceCasePriority { protected get; set; }

            public ServiceCaseDetails Create()
            {
                return (ServiceCaseDetails)CreateValue();
            }

            protected virtual ServiceCaseDetails CreateValue()
            {
                return new ServiceCaseDetails(LastThreadId, ThreadSnapshots, Title, Description, ServiceCasePriority);
            }
        }

        public Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected virtual ServiceCaseDetails.Essence CreateEssence()
        {
            return new Essence
            {
                LastThreadId = LastThreadId,
                ThreadSnapshots = ThreadSnapshots,
                Title = Title,
                Description = Description,
                ServiceCasePriority = ServiceCasePriority
            };
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Int32 LastThreadId { get { return _lastThreadId; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { get { return _threadSnapshots; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String Title { get { return _title; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String Description { get { return _description; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public ServiceCasePriority ServiceCasePriority { get { return _serviceCasePriority; } }
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public partial class ServiceCaseId : IScalarValue<Guid>, IEquatable<Guid>, IEquatable<ServiceCaseId>
    {
        private readonly Guid _value;
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public ServiceCaseId(System.Guid value)
        {
            Normalize(ref value);
            Validate(value);
            _value = value;
        }

        partial void Normalize(ref System.Guid value);

        partial void Validate(System.Guid value);

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(ServiceCaseId other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(Guid other)
        {
            return Value == other;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Boolean Equals(Object obj)
        {
            ServiceCaseId other = obj as ServiceCaseId;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (Value != other.Value) return false;
            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Int32 GetHashCode()
        {
            return Value.GetHashCode();
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static implicit operator ServiceCaseId(Guid value) { return new ServiceCaseId(value); }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static implicit operator Guid(ServiceCaseId value) { return value.Value; }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator ==(ServiceCaseId left, ServiceCaseId right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator !=(ServiceCaseId left, ServiceCaseId right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public System.Guid Value { get { return _value; } }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        object IScalarValue.Value { get { return Value; } }
    }

    [ValueObject]
    public partial class SpecialServiceCaseId : ServiceCaseId, IEquatable<SpecialServiceCaseId>
    {
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public SpecialServiceCaseId(System.Guid value) : base(value)
        {
            Normalize(ref value);
            Validate(value);
        }

        partial void Normalize(ref System.Guid value);

        partial void Validate(System.Guid value);

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(SpecialServiceCaseId other)
        {
            return Equals((Object)other);
        }

    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class CustomerRoleIdentifier : RoleIdentifier, IEquatable<CustomerRoleIdentifier>
    {

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public CustomerRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(CustomerRoleIdentifier other)
        {
            return Equals((Object)other);
        }


        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public new class Essence : RoleIdentifier.Essence
        {

            public new CustomerRoleIdentifier Create()
            {
                return (CustomerRoleIdentifier)CreateValue();
            }

            protected override RoleIdentifier CreateValue()
            {
                return new CustomerRoleIdentifier(BaseIdentifier, IdentifierType);
            }
        }

        public new Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected override RoleIdentifier.Essence CreateEssence()
        {
            return new Essence
            {
                BaseIdentifier = BaseIdentifier,
                IdentifierType = IdentifierType
            };
        }

    }
}
namespace iSynaptic.TestDomain
{

}
namespace iSynaptic.TestDomain
{
    public enum HomogeneousRoleStatus
    {
        New,
        PendingApproval,
        Approved,
        Retired
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public abstract class RoleIdentifier : IEquatable<RoleIdentifier>
    {
        private readonly String _baseIdentifier;
        private readonly String _identifierType;

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        protected RoleIdentifier(String baseIdentifier, String identifierType)
        {
            if (ReferenceEquals(baseIdentifier, null)) throw new ArgumentNullException("baseIdentifier");
            if (ReferenceEquals(identifierType, null)) throw new ArgumentNullException("identifierType");

            _baseIdentifier = baseIdentifier;
            _identifierType = identifierType;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public Boolean Equals(RoleIdentifier other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Boolean Equals(Object obj)
        {
            RoleIdentifier other = obj as RoleIdentifier;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!BaseIdentifier.Equals(other.BaseIdentifier)) return false;
            if (!IdentifierType.Equals(other.IdentifierType)) return false;

            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + BaseIdentifier.GetHashCode());
            hash = HashCode.MixJenkins32(hash + IdentifierType.GetHashCode());

            return hash;
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator ==(RoleIdentifier left, RoleIdentifier right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public static Boolean operator !=(RoleIdentifier left, RoleIdentifier right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public abstract class Essence
        {
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String BaseIdentifier { protected get; set; }
            [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
            public String IdentifierType { protected get; set; }

            public RoleIdentifier Create()
            {
                return (RoleIdentifier)CreateValue();
            }

            protected abstract RoleIdentifier CreateValue();
        }

        public Essence ToEssence()
        {
            return (Essence)CreateEssence();
        }

        protected abstract RoleIdentifier.Essence CreateEssence();

        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String BaseIdentifier { get { return _baseIdentifier; } }
        [GeneratedCode("iSynaptic.Core", "0.2.0.0")]
        public String IdentifierType { get { return _identifierType; } }
    }
}

