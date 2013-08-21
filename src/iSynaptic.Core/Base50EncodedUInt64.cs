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
        private static readonly Char AlphabetZero = Alphabet.First();

        private static readonly UInt64 AlphabetLength = (UInt64)Alphabet.Length;

        private static readonly UInt64[] AlphabetMap = 
            Enumerable.Range(0, 123)
                .Select(i => Array.IndexOf(Alphabet, (Char) i))
                .Select(i => i == -1 ? UInt64.MaxValue : (UInt64)i)
                .ToArray();

        private static readonly Int32 MaxTextLenth = (Int32)Math.Ceiling(Math.Log(UInt64.MaxValue) / Math.Log(AlphabetLength));
        private static readonly UInt64[] Multiplier =
            Enumerable.Range(0, MaxTextLenth)
                .Select(i => (UInt64)Math.Pow(AlphabetLength, i))
                .ToArray();

        private readonly UInt64 _value;
        private readonly String _text;

        [CLSCompliant(false)]
        public Base50EncodedUInt64(UInt64 value)
        {
            _value = value;
            _text = ToString(value);
        }

        public Base50EncodedUInt64(String value)
        {
            var result = TryParse(value);
            if(!result.HasValue)
                throw new ArgumentException("Not a valid base 50 encoded 64-bit integer.", "value");

            _value = result.Value;
            _text = result.Value;
        }

        private Base50EncodedUInt64(UInt64 value, String text)
        {
            _value = value;
            _text = text;
        }

        public static Base50EncodedUInt64 Parse(String value)
        {
            return new Base50EncodedUInt64(value);
        }

        public static Maybe<Base50EncodedUInt64> TryParse(String value)
        {
            if (value == null)
                return Maybe<Base50EncodedUInt64>.NoValue;

            if (value.Length > MaxTextLenth)
                return Maybe<Base50EncodedUInt64>.NoValue;

            var result = FromString(value);

            return result.HasValue 
                ? new Maybe<Base50EncodedUInt64>(new Base50EncodedUInt64(result.Value, value)) 
                : default(Maybe<Base50EncodedUInt64>);
        }

        private static Maybe<UInt64> FromString(String value)
        {
            UInt64 result = 0;

            UInt64 length = (UInt64)value.Length;
            for (UInt64 i = 0; i < length; i++)
            {
                Char ch = value[(Int32)(length - i - 1)];
                if (ch == '0')
                    continue;

                var index = IndexOf(ch);
                if (index == UInt64.MaxValue)
                    return default(Maybe<UInt64>);

                result += index * Multiplier[i];
            }

            return new Maybe<UInt64>(result);
        }

        private static String ToString(UInt64 value)
        {
            Char[] result = new String(Alphabet[0], MaxTextLenth).ToCharArray();
            UInt64 remaining = value;

            Int32 index = MaxTextLenth -1;
            Int32 indexOfLastNonZeroChar = index;

            while (remaining > 0)
            {
                result[index] = Alphabet[remaining % AlphabetLength];
                remaining /= AlphabetLength;
                if (result[index] != AlphabetZero)
                    indexOfLastNonZeroChar = index;

                index--;
            }

            return new String(result, indexOfLastNonZeroChar, MaxTextLenth - indexOfLastNonZeroChar);
        }

        private static UInt64 IndexOf(Char value)
        {
            Int32 index = value;
            if (index >= AlphabetMap.Length || AlphabetMap[index] == UInt64.MaxValue)
                return UInt64.MaxValue;

            return AlphabetMap[index];
        }

        public override string ToString()
        {
            return _text ?? "0";
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
        public static implicit operator String(Base50EncodedUInt64 value)
        {
            return value.ToString();
        }

        [CLSCompliant(false)]
        public static explicit operator Base50EncodedUInt64(String value)
        {
            return new Base50EncodedUInt64(value);
        }

        [CLSCompliant(false)]
        public UInt64 ToUInt64()
        {
            return _value;
        }

        public String Value { get { return ToString(); } }
    }
}
