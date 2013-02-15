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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using iSynaptic.Commons.Collections.Generic;
using iSynaptic.Commons.Linq;

namespace iSynaptic
{
    [TestFixture]
    public class InterleavingUniqueNumberGeneratorTests
    {
        private static readonly DateTime _epoch = DateTime.Parse("2013-01-30T16:00:00.000-00:00").ToUniversalTime();

        private readonly IUniqueNumberGenerator _generator
            = new InterleavingUniqueNumberGenerator(0, 0, _epoch);

        [Test]
        public void CanGenerateAFewIds()
        {
            var ids = Enumerable.Range(1, 256)
                .Select(x => _generator.Generate())
                .Distinct()
                .ToArray();

            Assert.AreEqual(256, ids.Length);
        }

        [Test]
        public void CanGenerateAFewIds_InBulk()
        {
            var ids = _generator.Generate(255)
                .Distinct()
                .ToArray();

            Assert.AreEqual(255, ids.Length);
        }

        [Test]
        public void CanGenerate_Multithreaded()
        {
            var ids = new ConcurrentBag<KeyValuePair<Int32, UInt64>>();
            Parallel.For(0, 10000, x => ids.Add(KeyValuePair.Create(Thread.CurrentThread.ManagedThreadId, _generator.Generate())));

            var distinctIds = ids.Select(x => x.Value).Distinct().ToArray();
            Assert.AreEqual(10000, distinctIds.Length);


            var distinctThreadIds = ids.Select(x => x.Key).Distinct().ToArray();
            Assert.IsTrue(distinctThreadIds.Length > 1);
        }
    }
}
