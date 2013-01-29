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
using iSynaptic.Commons;

namespace iSynaptic.TestAggregates
{
    public class CustomerRoleIdentifier : RoleIdentifier, IEquatable<CustomerRoleIdentifier>
    {
        private readonly string _customerIdentifier;

        public CustomerRoleIdentifier(string baseIdentifier, string customerIdentifier) 
            : base("CR", baseIdentifier)
        {
            _customerIdentifier = Guard.NotNullOrWhiteSpace(customerIdentifier, "customerIdentifier");
        }

        public string CustomerIdentifier { get { return _customerIdentifier; } }

        public bool Equals(CustomerRoleIdentifier other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (GetType() != other.GetType()) return false;

            if (!CustomerIdentifier.Equals(other.CustomerIdentifier)) return false;

            return Equals((RoleIdentifier)other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as RoleIdentifier;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash = HashCode.MixJenkins32(hash + CustomerIdentifier.GetHashCode());
            return hash;
        }

        public static bool operator ==(CustomerRoleIdentifier left, CustomerRoleIdentifier right)
        {
            if (ReferenceEquals(left, null) != ReferenceEquals(right, null)) return false;
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        public static bool operator !=(CustomerRoleIdentifier left, CustomerRoleIdentifier right)
        {
            return !(left == right);
        }
    }
}