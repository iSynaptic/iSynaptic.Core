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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public CustomerAgentRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(CustomerAgentRoleIdentifier other)
        {
            return Equals((Object)other);
        }


    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class CustomerServiceRepresentativeRoleIdentifier : RoleIdentifier, IEquatable<CustomerServiceRepresentativeRoleIdentifier>
    {

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public CustomerServiceRepresentativeRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(CustomerServiceRepresentativeRoleIdentifier other)
        {
            return Equals((Object)other);
        }


    }
}
namespace iSynaptic.TestDomain
{
    public partial class Base<T> : Aggregate<T>
        where T : IEquatable<T>
    {
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        protected Base(AggregateEvent<T> startEvent) { ApplyEvent(startEvent); }

    }
}
namespace iSynaptic.TestDomain
{
    public partial class ServiceCase : Base<ServiceCaseId>
    {
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        protected ServiceCase(AggregateEvent<ServiceCaseId> startEvent) : base(startEvent) { }

        [AggregateEventVersion(3)]
        public class Opened : AggregateEvent<ServiceCaseId>
        {
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public Opened(String title, String description, ServiceCasePriority priority, ServiceCaseId id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _title = title;
                _description = description;
                _priority = priority;
            }

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Title { get { return _title; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Description { get { return _description; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public ServiceCasePriority Priority { get { return _priority; } }
        }
        [AggregateEventVersion(1)]
        public class CommunicationThreadStarted : AggregateEvent<ServiceCaseId>
        {
            private readonly Int32 _threadId;
            private readonly String _topic;
            private readonly String _description;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public CommunicationThreadStarted(Int32 threadId, String topic, String description, ServiceCaseId id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _threadId = threadId;
                _topic = topic;
                _description = description;
            }

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public Int32 ThreadId { get { return _threadId; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Topic { get { return _topic; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Description { get { return _description; } }
        }
        [AggregateEventVersion(1)]
        public class CommunicationRecorded : AggregateEvent<ServiceCaseId>
        {
            private readonly Int32 _threadId;
            private readonly CommunicationDirection _direction;
            private readonly String _content;
            private readonly DateTime _communicationTime;
            private readonly TimeSpan _duration;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public CommunicationRecorded(Int32 threadId, CommunicationDirection direction, String content, DateTime communicationTime, TimeSpan duration, ServiceCaseId id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(content, null)) throw new ArgumentNullException("content");

                _threadId = threadId;
                _direction = direction;
                _content = content;
                _communicationTime = communicationTime;
                _duration = duration;
            }

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public Int32 ThreadId { get { return _threadId; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public CommunicationDirection Direction { get { return _direction; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Content { get { return _content; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public DateTime CommunicationTime { get { return _communicationTime; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public TimeSpan Duration { get { return _duration; } }
        }
        public class Snapshot : AggregateSnapshot<ServiceCaseId>
        {
            private readonly Int32 _lastThreadId;
            private readonly IEnumerable<CommunicationThreadSnapshot> _threadSnapshots;
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public Int32 LastThreadId { get { return _lastThreadId; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { get { return _threadSnapshots; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Title { get { return _title; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public String Description { get { return _description; } }
            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public ServiceCasePriority Priority { get { return _priority; } }
        }
    }
}
namespace iSynaptic.TestDomain
{
    public partial class HomogeneousRole<TRoleIdentifier> : Aggregate<TRoleIdentifier>
        where TRoleIdentifier : RoleIdentifier, IEquatable<TRoleIdentifier>
    {
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        protected HomogeneousRole(AggregateEvent<TRoleIdentifier> startEvent) { ApplyEvent(startEvent); }

        [AggregateEventVersion(1)]
        public class Registered : AggregateEvent<TRoleIdentifier>
        {
            private readonly string _name;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public Registered(string name, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(name, null)) throw new ArgumentNullException("name");

                _name = name;
            }

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public string Name { get { return _name; } }
        }
        [AggregateEventVersion(1)]
        public class StatusChanged : AggregateEvent<TRoleIdentifier>
        {
            private readonly HomogeneousRoleStatus _status;

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public StatusChanged(HomogeneousRoleStatus status, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {

                _status = status;
            }

            [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
            public HomogeneousRoleStatus Status { get { return _status; } }
        }
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public class CommunicationThreadSnapshot : IEquatable<CommunicationThreadSnapshot>
    {
        private readonly Int32 _threadId;
        private readonly String _topic;
        private readonly String _description;

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public CommunicationThreadSnapshot(Int32 threadId, String topic, String description)
        {
            if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
            if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

            _threadId = threadId;
            _topic = topic;
            _description = description;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(CommunicationThreadSnapshot other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + ThreadId.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Topic.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Description.GetHashCode());

            return hash;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator ==(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator !=(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Int32 ThreadId { get { return _threadId; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public String Topic { get { return _topic; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(ServiceCaseDetails other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator ==(ServiceCaseDetails left, ServiceCaseDetails right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator !=(ServiceCaseDetails left, ServiceCaseDetails right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Int32 LastThreadId { get { return _lastThreadId; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { get { return _threadSnapshots; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public String Title { get { return _title; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public String Description { get { return _description; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public ServiceCasePriority ServiceCasePriority { get { return _serviceCasePriority; } }
    }
}
namespace iSynaptic.TestDomain
{
    [ValueObject]
    public partial class ServiceCaseId : IScalarValue<Guid>, IEquatable<Guid>, IEquatable<ServiceCaseId>
    {
        private readonly Guid _value;
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public ServiceCaseId(System.Guid value)
        {
            Validate(value);
            _value = value;
        }

        partial void Validate(System.Guid value);

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(ServiceCaseId other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(Guid other)
        {
            return Value == other;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public override Boolean Equals(Object obj)
        {
            ServiceCaseId other = obj as ServiceCaseId;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (Value != other.Value) return false;
            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public override Int32 GetHashCode()
        {
            return Value.GetHashCode();
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static implicit operator ServiceCaseId(Guid value) { return new ServiceCaseId(value); }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static implicit operator Guid(ServiceCaseId value) { return value.Value; }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator ==(ServiceCaseId left, ServiceCaseId right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator !=(ServiceCaseId left, ServiceCaseId right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public System.Guid Value { get { return _value; } }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        object IScalarValue.Value { get { return Value; } }
    }

    [ValueObject]
    public partial class SpecialServiceCaseId : ServiceCaseId, IEquatable<SpecialServiceCaseId>
    {
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public SpecialServiceCaseId(System.Guid value)
            : base(value)
        {
            Validate(value);
        }

        partial void Validate(System.Guid value);

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public CustomerRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(CustomerRoleIdentifier other)
        {
            return Equals((Object)other);
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

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        protected RoleIdentifier(String baseIdentifier, String identifierType)
        {
            if (ReferenceEquals(baseIdentifier, null)) throw new ArgumentNullException("baseIdentifier");
            if (ReferenceEquals(identifierType, null)) throw new ArgumentNullException("identifierType");

            _baseIdentifier = baseIdentifier;
            _identifierType = identifierType;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public Boolean Equals(RoleIdentifier other)
        {
            return Equals((Object)other);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public override Boolean Equals(Object obj)
        {
            RoleIdentifier other = obj as RoleIdentifier;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!BaseIdentifier.Equals(other.BaseIdentifier)) return false;
            if (!IdentifierType.Equals(other.IdentifierType)) return false;

            return true;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + BaseIdentifier.GetHashCode());
            hash = HashCode.MixJenkins32(hash + IdentifierType.GetHashCode());

            return hash;
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator ==(RoleIdentifier left, RoleIdentifier right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public static Boolean operator !=(RoleIdentifier left, RoleIdentifier right) { return !(left == right); }

        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public String BaseIdentifier { get { return _baseIdentifier; } }
        [GeneratedCode("iSynaptic.Core", "0.1.21.0")]
        public String IdentifierType { get { return _identifierType; } }
    }
}

