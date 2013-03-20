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
using NUnit.Framework;

namespace iSynaptic
{
    [TestFixture]
    public class Base50EncodedUInt64Tests
    {
        [Test]
        public void Zero_RoundTrips()
        {
            AssertRoundTrip(0);
        }

        [Test]
        public void One_RoundTrips()
        {
            AssertRoundTrip(1);
        }

        [Test]
        public void FourtyNine_RoundTrips()
        {
            AssertRoundTrip(49);
        }
        
        [Test]
        public void Fifty_RoundTrips()
        {
            AssertRoundTrip(50);
        }

        [Test]
        public void FiftyOne_RoundTrips()
        {
            AssertRoundTrip(51);
        }

        [Test]
        public void UInt32BoundaryMinusOne_RoundTrips()
        {
            AssertRoundTrip(UInt32.MaxValue - 1);
        }

        [Test]
        public void UInt32Boundary_RoundTrips()
        {
            AssertRoundTrip(UInt32.MaxValue);
        }

        [Test]
        public void UInt32BoundaryPlusOne_RoundTrips()
        {
            AssertRoundTrip((UInt64)UInt32.MaxValue + 1);
        }

        [Test]
        public void MaxValue_RoundTrips()
        {
            AssertRoundTrip(UInt64.MaxValue);
        }

        [Test]
        public void RountTrip_Sampling()
        {
            const ulong step = UInt64.MaxValue / 65535;

            UInt64 value = 0;

            while (value <= UInt64.MaxValue)
            {
                AssertRoundTrip(value);

                if (value + step < value)
                    break;

                value += step;
            }
        }

        private void AssertRoundTrip(UInt64 startValue)
        {
            var encodedNumber = new Base50EncodedUInt64(startValue);
            String textRepresentation = encodedNumber.ToString();

            var roundTripedNumber = new Base50EncodedUInt64(textRepresentation);

            Assert.AreEqual(startValue, roundTripedNumber.ToUInt64());
            Assert.AreEqual(encodedNumber.Value, roundTripedNumber.Value);
        }
    }
}
