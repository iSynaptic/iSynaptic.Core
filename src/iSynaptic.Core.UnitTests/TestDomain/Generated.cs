using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;
using iSynaptic.Modeling.Domain;

namespace iSynaptic.TestDomain
{
    public abstract class RoleIdentifier : IEquatable<RoleIdentifier>
    {
        private readonly String _baseIdentifier;
        private readonly String _identifierType;

        protected RoleIdentifier(String baseIdentifier, String identifierType)
        {
            if (ReferenceEquals(baseIdentifier, null)) throw new ArgumentNullException("baseIdentifier");
            if (ReferenceEquals(identifierType, null)) throw new ArgumentNullException("identifierType");

            _baseIdentifier = baseIdentifier;
            _identifierType = identifierType;
        }

        public Boolean Equals(RoleIdentifier other)
        {
            return Equals((Object)other);
        }

        public override Boolean Equals(Object obj)
        {
            RoleIdentifier other = obj as RoleIdentifier;

            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!BaseIdentifier.Equals(other.BaseIdentifier)) return false;
            if (!IdentifierType.Equals(other.IdentifierType)) return false;

            return true;
        }

        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + BaseIdentifier.GetHashCode());
            hash = HashCode.MixJenkins32(hash + IdentifierType.GetHashCode());

            return hash;
        }

        public static bool operator ==(RoleIdentifier left, RoleIdentifier right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(RoleIdentifier left, RoleIdentifier right) { return !(left == right); }

        public String BaseIdentifier { get { return _baseIdentifier; } }
        public String IdentifierType { get { return _identifierType; } }
    }

    public class CustomerRoleIdentifier : RoleIdentifier, IEquatable<CustomerRoleIdentifier>
    {

        public CustomerRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        public Boolean Equals(CustomerRoleIdentifier other)
        {
            return Equals((Object)other);
        }


    }

    public class CustomerAgentRoleIdentifier : RoleIdentifier, IEquatable<CustomerAgentRoleIdentifier>
    {

        public CustomerAgentRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        public Boolean Equals(CustomerAgentRoleIdentifier other)
        {
            return Equals((Object)other);
        }


    }

    public class CustomerServiceRepresentativeRoleIdentifier : RoleIdentifier, IEquatable<CustomerServiceRepresentativeRoleIdentifier>
    {

        public CustomerServiceRepresentativeRoleIdentifier(String baseIdentifier, String identifierType)
            : base(baseIdentifier, identifierType)
        {

        }

        public Boolean Equals(CustomerServiceRepresentativeRoleIdentifier other)
        {
            return Equals((Object)other);
        }


    }

    public partial class Base<T> : Aggregate<T>
        where T : IEquatable<T>
    {
        protected Base(AggregateEvent<T> startEvent) { ApplyEvent(startEvent); }
    }

    public partial class ServiceCase : Base<Guid>
    {
        protected ServiceCase(AggregateEvent<Guid> startEvent) : base(startEvent) { }
        public class Opened : AggregateEvent<Guid>
        {
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            public Opened(String title, String description, ServiceCasePriority priority, Guid id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");
                if (ReferenceEquals(priority, null)) throw new ArgumentNullException("priority");

                _title = title;
                _description = description;
                _priority = priority;
            }

            public String Title { get { return _title; } }
            public String Description { get { return _description; } }
            public ServiceCasePriority Priority { get { return _priority; } }
        }
        public class CommunicationThreadStarted : AggregateEvent<Guid>
        {
            private readonly Int32 _threadId;
            private readonly String _topic;
            private readonly String _description;

            public CommunicationThreadStarted(Int32 threadId, String topic, String description, Guid id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

                _threadId = threadId;
                _topic = topic;
                _description = description;
            }

            public Int32 ThreadId { get { return _threadId; } }
            public String Topic { get { return _topic; } }
            public String Description { get { return _description; } }
        }
        public class CommunicationRecorded : AggregateEvent<Guid>
        {
            private readonly Int32 _threadId;
            private readonly CommunicationDirection _direction;
            private readonly String _content;
            private readonly DateTime _communicationTime;

            public CommunicationRecorded(Int32 threadId, CommunicationDirection direction, String content, DateTime communicationTime, Guid id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(direction, null)) throw new ArgumentNullException("direction");
                if (ReferenceEquals(content, null)) throw new ArgumentNullException("content");

                _threadId = threadId;
                _direction = direction;
                _content = content;
                _communicationTime = communicationTime;
            }

            public Int32 ThreadId { get { return _threadId; } }
            public CommunicationDirection Direction { get { return _direction; } }
            public String Content { get { return _content; } }
            public DateTime CommunicationTime { get { return _communicationTime; } }
        }
        public class Snapshot : AggregateSnapshot<Guid>
        {
            private readonly Int32 _lastThreadId;
            private readonly IEnumerable<CommunicationThreadSnapshot> _threadSnapshots;
            private readonly String _title;
            private readonly String _description;
            private readonly ServiceCasePriority _priority;

            public Snapshot(Int32 lastThreadId, IEnumerable<CommunicationThreadSnapshot> threadSnapshots, String title, String description, ServiceCasePriority priority, Guid id, Int32 version, DateTime takenAt)
                : base(id, version, takenAt)
            {
                if (ReferenceEquals(threadSnapshots, null)) throw new ArgumentNullException("threadSnapshots");
                if (ReferenceEquals(title, null)) throw new ArgumentNullException("title");
                if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");
                if (ReferenceEquals(priority, null)) throw new ArgumentNullException("priority");

                _lastThreadId = lastThreadId;
                _threadSnapshots = threadSnapshots;
                _title = title;
                _description = description;
                _priority = priority;
            }

            public Int32 LastThreadId { get { return _lastThreadId; } }
            public IEnumerable<CommunicationThreadSnapshot> ThreadSnapshots { get { return _threadSnapshots; } }
            public String Title { get { return _title; } }
            public String Description { get { return _description; } }
            public ServiceCasePriority Priority { get { return _priority; } }
        }
    }

    public partial class HomogeneousRole<TRoleIdentifier> : Aggregate<TRoleIdentifier>
        where TRoleIdentifier : RoleIdentifier, IEquatable<TRoleIdentifier>
    {
        protected HomogeneousRole(AggregateEvent<TRoleIdentifier> startEvent) { ApplyEvent(startEvent); }
        public class Registered : AggregateEvent<TRoleIdentifier>
        {
            private readonly String _name;

            public Registered(String name, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(name, null)) throw new ArgumentNullException("name");

                _name = name;
            }

            public String Name { get { return _name; } }
        }
        public class StatusChanged : AggregateEvent<TRoleIdentifier>
        {
            private readonly HomogeneousRoleStatus _status;

            public StatusChanged(HomogeneousRoleStatus status, TRoleIdentifier id, Int32 version)
                : base(id, version)
            {
                if (ReferenceEquals(status, null)) throw new ArgumentNullException("status");

                _status = status;
            }

            public HomogeneousRoleStatus Status { get { return _status; } }
        }
    }

    public class CommunicationThreadSnapshot : IEquatable<CommunicationThreadSnapshot>
    {
        private readonly Int32 _threadId;
        private readonly String _topic;
        private readonly String _description;

        public CommunicationThreadSnapshot(Int32 threadId, String topic, String description)
        {
            if (ReferenceEquals(topic, null)) throw new ArgumentNullException("topic");
            if (ReferenceEquals(description, null)) throw new ArgumentNullException("description");

            _threadId = threadId;
            _topic = topic;
            _description = description;
        }

        public Boolean Equals(CommunicationThreadSnapshot other)
        {
            return Equals((Object)other);
        }

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

        public override Int32 GetHashCode()
        {
            int hash = 1;

            hash = HashCode.MixJenkins32(hash + ThreadId.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Topic.GetHashCode());
            hash = HashCode.MixJenkins32(hash + Description.GetHashCode());

            return hash;
        }

        public static bool operator ==(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(CommunicationThreadSnapshot left, CommunicationThreadSnapshot right) { return !(left == right); }

        public Int32 ThreadId { get { return _threadId; } }
        public String Topic { get { return _topic; } }
        public String Description { get { return _description; } }
    }

    public enum CommunicationDirection
    {
        Incoming,
        Outgoing
    }

    public enum ServiceCasePriority
    {
        Low,
        Normal,
        High
    }

    public enum HomogeneousRoleStatus
    {
        New,
        PendingApproval,
        Approved,
        Retired
    }
}

