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

namespace iSynaptic.Modeling.Domain
{
    [Serializable]
    public abstract class AggregateSnapshot : IAggregateSnapshot
    {
        protected AggregateSnapshot(object id, Int32 version, DateTime takenAt)
        {
            if (version <= 0)
                throw new ArgumentOutOfRangeException("version", "Version must be greater than 0.");

            if (takenAt.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime must be of UTC kind.", "takenAt");

            SnapshotId = Guid.NewGuid();
            Id = id;
            Version = version;
            TakenAt = takenAt;
        }

        public Guid SnapshotId { get; private set; }
        public object Id { get; private set; }
        public Int32 Version { get; private set; }
        public DateTime TakenAt { get; private set; }
    }
}