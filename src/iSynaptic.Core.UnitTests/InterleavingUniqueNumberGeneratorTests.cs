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
using iSynaptic.Commons;
using iSynaptic.Commons.Linq;
using NUnit.Framework;
using iSynaptic.Commons.Collections.Generic;

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
        [Ignore("Long running test; run manually.")]
        public void GeneratingTooManyIds_ThrowsException()
        {
            using (SystemClock.Fixed(new DateTime(2014, 1, 13, 0, 0, 0, DateTimeKind.Utc)))
            {
                double max = Math.Pow(2, 22) - 1;

                for(double i = 0; i < max; i++)
                {
                    _generator.Generate();
                }

                Assert.Throws<InvalidOperationException>(() => _generator.Generate());
            }
        }

        [Test]
        public void CanGenerate_Multithreaded()
        {
            var ids = new ConcurrentBag<KeyValuePair<Int32, UInt64>>();

            var cts = new CancellationTokenSource();
            
            ParameterizedThreadStart generateIds = (token) =>
            {
                var ct = (CancellationToken)token;

                while (!ct.IsCancellationRequested)
                {
                    var newIds = new ulong[10000];
                    for (int i = 0; i < 10000; i++)
                    {
                        newIds[i] = _generator.Generate();
                    }

                    newIds.Run(x => ids.Add(KeyValuePair.Create(Thread.CurrentThread.ManagedThreadId, x)));
                }
            };

            var threads = new[]
            {
                new Thread(generateIds),
                new Thread(generateIds),
                new Thread(generateIds),
                new Thread(generateIds)
            };

            threads.Run(t => t.Start(cts.Token));

            Thread.Sleep(1000);

            cts.Cancel();

            threads.Run(t => t.Join());

            var distinctIds = ids.Select(x => x.Value).Distinct().ToArray();
            Assert.IsTrue(ids.Count > 0, "Count wasn't greater than zero.");
            Assert.IsTrue(ids.Count == distinctIds.Count(), "Counts don't match!");

            Console.WriteLine("Id count: {0}", ids.Count);

            var distinctThreadIds = ids.Select(x => x.Key).Distinct().ToArray();
            Assert.IsTrue(distinctThreadIds.Length > 1, "Distinct threads not greater than one.");
        }
    }
}
