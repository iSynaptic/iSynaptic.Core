using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

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
}

