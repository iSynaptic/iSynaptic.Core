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
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic
{
    public struct Base50EncodedUInt64
    {
        private static readonly Char[] Alphabet = "0123456789BCDFGHJKMNPQRSTVWXYZbcdfghjkmnpqrstvwxyz".ToCharArray();
        private static readonly UInt64 AlphabetLength = (UInt64)Alphabet.Length;

        private readonly UInt64 _value;

        [CLSCompliant(false)]
        public Base50EncodedUInt64(UInt64 value)
        {
            _value = value;
        }

        public Base50EncodedUInt64(String value)
        {
            var result = TryParse(value);
            if(!result.HasValue)
                throw new ArgumentException("Not a valid base 50 encoded 64-bit integer.", "value");

            _value = result.Value;
        }

        public static Maybe<Base50EncodedUInt64> TryParse(String value)
        {
            if (value == null)
                return Maybe<Base50EncodedUInt64>.NoValue;

            if (value.Length > 12)
                return Maybe<Base50EncodedUInt64>.NoValue;

            var result = FromString(value);
            return new Base50EncodedUInt64(result).ToMaybe();
        }

        private static UInt64 FromString(String value)
        {
            UInt64 result = 0;

            UInt64 length = (UInt64)value.Length;
            for (UInt64 i = 0; i < length; i++)
            {
                Char ch = value[(Int32)(length - i - 1)];
                result += (UInt64) IndexOf(ch)*(UInt64)Math.Pow(AlphabetLength, i);
            }

            return result;
        }

        private static String ToString(UInt64 value)
        {
            Char[] result = new String(Alphabet[0], 12).ToCharArray();
            UInt64 remaining = value;

            Int32 index = 11;
            while (remaining > 0)
            {
                result[index] = Alphabet[remaining % AlphabetLength];
                remaining /= AlphabetLength;
                index--;
            }

            return new String(result);
        }

        public override string ToString()
        {
            return ToString(_value);
        }

        private static Int32 IndexOf(Char value)
        {
            return Array.IndexOf(Alphabet, value);
        }

        public bool Equals(Base50EncodedUInt64 other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Base50EncodedUInt64 && Equals((Base50EncodedUInt64)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Base50EncodedUInt64 left, Base50EncodedUInt64 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Base50EncodedUInt64 left, Base50EncodedUInt64 right)
        {
            return !(left.Equals(right));
        }

        [CLSCompliant(false)]
        public static implicit operator UInt64(Base50EncodedUInt64 value)
        {
            return value._value;
        }

        [CLSCompliant(false)]
        public static implicit operator Base50EncodedUInt64(UInt64 value)
        {
            return new Base50EncodedUInt64(value);
        }

        [CLSCompliant(false)]
        public UInt64 Value { get { return _value; } }
    }
}
