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
using FluentAssertions;
using NUnit.Framework;

namespace iSynaptic.Serialization
{
    [TestFixture]
    public class LogicalTypeTests
    {
        [Test]
        public void Equivalence_ImplementedCorrectly()
        {
            var type1 = new LogicalType("tst", "Foo");
            var type2 = new LogicalType("tst", "foo");
            var type3 = new LogicalType("sts", "Bar");
            Object type4 = new LogicalType("sts", "Bar");

            LogicalType nullType = null;
            
            (type1 == type2).Should().BeTrue();
            (type1 != type3).Should().BeTrue();
            (type3.Equals(type4)).Should().BeTrue();

            (nullType == null).Should().BeTrue();
            (type1 != nullType).Should().BeTrue();
        }

        [Test]
        public void NamespaceAlias_IsValidated()
        {
            Action act = () => new LogicalType("28f;[]';,.", "Foo");
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TypeName_IsValidated()
        {
            Action act = () => new LogicalType("tst", "28f;[]';,.");
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Parse_WithCorrectFormat_ReturnsLogicalType()
        {
            String input = "tst:Foo";
            LogicalType.Format.IsMatch(input).Should().BeTrue();

            var logicalType = LogicalType.Parse(input);

            logicalType.Should().NotBeNull();
            logicalType.NamespaceAlias.Should().Be("tst");
            logicalType.TypeName.Should().Be("Foo");
        }

        [Test]
        public void Parse_WithIncorrectFormat_ThrowsException()
        {
            String input = "63^*^*:8*&&(&";
            LogicalType.Format.IsMatch(input).Should().BeFalse();

            Action act = () => LogicalType.Parse(input);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TryParse_WithCorrectFormat_ReturnsLogicalType()
        {
            String input = "tst:Foo";
            LogicalType.Format.IsMatch(input).Should().BeTrue();

            var logicalType = LogicalType.TryParse(input);

            logicalType.HasValue.Should().BeTrue();
            logicalType.Value.NamespaceAlias.Should().Be("tst");
            logicalType.Value.TypeName.Should().Be("Foo");
        }

        [Test]
        public void TryParse_WithIncorrectFormat_ReturnsNoValue()
        {
            String input = "63^*^*:8*&&(&";
            LogicalType.Format.IsMatch(input).Should().BeFalse();

            var logicalType = LogicalType.TryParse(input);

            logicalType.HasValue.Should().BeFalse();
        }
    }
}
