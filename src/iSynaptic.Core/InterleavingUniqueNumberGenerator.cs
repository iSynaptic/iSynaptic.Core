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
using System.Threading;
using iSynaptic.Commons;

namespace iSynaptic
{
    [CLSCompliant(false)]
    public static class UniqueNumberGeneratorExtensions
    {
        public static UInt64 Generate(this IUniqueNumberGenerator @this)
        {
            Guard.NotNull(@this, "this");
            return @this.Generate(1)[0];
        }
    }

    [CLSCompliant(false)]
    public sealed class InterleavingUniqueNumberGenerator : IUniqueNumberGenerator
    {
        private readonly IVersionedGenerator _generator;
        public InterleavingUniqueNumberGenerator(Byte version, UInt32 nodeId, DateTime epoch)
        {
            if(version > 0)
                throw new ArgumentException("Unsupported generator version", "version");

            if(epoch.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Epoch must be provided in UTC.", "epoch");

            _generator = new Version00Generator(nodeId, epoch);

        }
        public UInt64[] Generate(Byte count)
        {
            return _generator.Generate(count);
        }

        private interface IVersionedGenerator
        {
            UInt64[] Generate(Byte count);
            Byte Version { get; }
        }

        #region Version 00 Generator

        private sealed class Version00Generator : IVersionedGenerator
        {
            private const UInt64 MostSignificantBit = ((UInt64) 1) << 63;

            private readonly UInt32 _nodeId;
            private readonly DateTime _epoch;
            private SpinLock _lock = new SpinLock();

            private UInt32 _lastMinutesFromEpoch;
            private UInt32 _nextSequenceNumber;

            private readonly Component[] _components = 
                new Component[3];

            #region Helper Classes

            private struct Component
            {
                private UInt32 _value;
                private Byte _remainingBits;
                private readonly Byte _totalBits;
                private readonly ComponentWidth _width;

                public Component(UInt32 value, ComponentKind kind)
                    : this()
                {
                    _value = value;

                    if (kind == ComponentKind.Node)
                    {
                        if (value > 31)
                        {
                            _width = ComponentWidth.Wide;
                            _totalBits = 16;
                        }
                        else
                            _totalBits = 5;
                    }
                    else if (kind == ComponentKind.Sequence)
                    {
                        if (value > 255)
                        {
                            _width = ComponentWidth.Wide;
                            _totalBits = 22;
                        }
                        else
                            _totalBits = 8;
                    }
                    else { _totalBits = 23; }

                    _remainingBits = _totalBits;
                }

                public void ConsumeBit()
                {
                    _value = _value >> 1;
                    _remainingBits--;
                }

                public UInt32 Value { get { return _value; } }
                public Byte TotalBits { get { return _totalBits; } }
                public ComponentWidth Width { get { return _width; } }

                public Boolean HasRemainingBits { get { return _remainingBits > 0; } }
            }

            private enum ComponentWidth : byte
            {
                Narrow = 0,
                Wide = 1
            }

            private enum ComponentKind : byte
            {
                Time = 0,
                Node = 1,
                Sequence = 2
            }

            #endregion

            public Version00Generator(UInt32 nodeId, DateTime epoch)
            {
                if(_nodeId > 65535)
                    throw new ArgumentException("Node Id cannot be greater than 4095.", "nodeId");

                _nodeId = nodeId;
                _epoch = epoch;

                _lastMinutesFromEpoch = ComputeMinutesFromEpoch();
            }

            public UInt64[] Generate(Byte count)
            {
                bool lockTaken = false;
                try
                {
                    _lock.Enter(ref lockTaken);

                    UInt32 minutesFromEpoch = ComputeMinutesFromEpoch();
                    
                    if (minutesFromEpoch != _lastMinutesFromEpoch)
                    {
                        _lastMinutesFromEpoch = minutesFromEpoch;
                        _nextSequenceNumber = 0;
                    }

                    UInt32 sequenceNumber = _nextSequenceNumber;
                    _nextSequenceNumber += count;

                    UInt64[] ids = new UInt64[count];
                    for (UInt32 i = 0; i < count; i++)
                    {
                        ids[i] = ComputeId(minutesFromEpoch, sequenceNumber + i);
                    }

                    return ids;
                }
                finally
                {
                    if(lockTaken) _lock.Exit(false);
                }
            }

            private UInt64 ComputeId(UInt32 time, UInt32 sequence)
            {
                _components[0] = new Component(time, ComponentKind.Time);
                _components[1] = new Component(_nodeId, ComponentKind.Node);
                _components[2] = new Component(sequence, ComponentKind.Sequence);

                UInt64 value = 0;

                if (_components[1].Width == ComponentWidth.Wide)
                    value = value | MostSignificantBit;

                value = value >> 1;

                if (_components[2].Width == ComponentWidth.Wide)
                    value = value | MostSignificantBit;

                bool hasRemainingBits;
                do
                {
                    hasRemainingBits = false;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!_components[i].HasRemainingBits)
                            continue;

                        value = value >> 1;

                        if ((_components[i].Value & 1) == 1)
                            value = value | MostSignificantBit;

                        _components[i].ConsumeBit();

                        hasRemainingBits |= _components[i].HasRemainingBits;
                    }

                } while (hasRemainingBits);

                var remainingShifts = (61 - (_components[0].TotalBits + _components[1].TotalBits + _components[2].TotalBits));
                if (remainingShifts > 0)
                    value = value >> remainingShifts;

                return value;
            }

            private UInt32 ComputeMinutesFromEpoch()
            {
                return (UInt32) Math.Floor(SystemClock.UtcNow.Subtract(_epoch).TotalMinutes);
            }

            public Byte Version { get { return 0; } }
        }

        #endregion
    }
}
