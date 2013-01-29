using System;
using iSynaptic.Commons;

namespace iSynaptic.TestAggregates
{
    public abstract class RoleIdentifier : IEquatable<RoleIdentifier>
    {
        private readonly string _identifierType;
        private readonly string _baseIdentifier;

        protected RoleIdentifier(String identifierType, String baseIdentifier)
        {
            _identifierType = Guard.NotNullOrWhiteSpace(identifierType, "identifierType");
            _baseIdentifier = Guard.NotNullOrWhiteSpace(baseIdentifier, "baseIdentifier");
        }

        public string IdentifierType { get { return _identifierType; } }
        public string BaseIdentifier { get { return _baseIdentifier; } }

        public bool Equals(RoleIdentifier other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!IdentifierType.Equals(other.IdentifierType)) return false;
            if (!BaseIdentifier.Equals(other.BaseIdentifier)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as RoleIdentifier;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = HashCode.MixJenkins32(hash + BaseIdentifier.GetHashCode());
            return hash;
        }

        public static bool operator ==(RoleIdentifier left, RoleIdentifier right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(RoleIdentifier left, RoleIdentifier right)
        {
            return !(left == right);
        }
    }
}