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
using iSynaptic.Commons;

namespace iSynaptic
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
            var type5 = new LogicalType("tst", "Foo", 0, 1.ToMaybe());
            var type6 = new LogicalType("tst", "Foo", 0, 1.ToMaybe());
            var type7 = new LogicalType("tst", "Foo", 0, 2.ToMaybe());
            var type8 = new LogicalType("tst", "Foo", 1, Maybe.NoValue);
            var type9 = new LogicalType("tst", "Foo", 1, Maybe.NoValue);
            var type10 = new LogicalType("tst", "Foo", 1, 1.ToMaybe());
            var type11 = new LogicalType("tst", "Foo", 1, 1.ToMaybe());
            var type12 = new LogicalType("tst", "Foo", 1, 2.ToMaybe());
            var type13 = new LogicalType("tst", "Foo", new[] { type1 }, Maybe.NoValue);
            var type14 = new LogicalType("tst", "Foo", new[] { type1 }, Maybe.NoValue);
            var type15 = new LogicalType("tst", "Foo", new[] { type1 }, 1.ToMaybe());
            var type16 = new LogicalType("tst", "Foo", new[] { type1 }, 2.ToMaybe());
            var type17 = new LogicalType("tst", "Foo", new[] { type2 }, Maybe.NoValue);
            var type18 = new LogicalType("tst", "Foo", new[] { type1, type3 }, Maybe.NoValue);

            LogicalType nullType = null;
            
            (type1 == type2).Should().BeTrue();
            (type1 != type3).Should().BeTrue();
            (type3.Equals(type4)).Should().BeTrue();
            (type5 == type6).Should().BeTrue();
            (type1 != type5).Should().BeTrue();
            (type6 != type7).Should().BeTrue();
            (type7 != type8).Should().BeTrue();
            (type8 == type9).Should().BeTrue();
            (type9 != type10).Should().BeTrue();
            (type10 == type11).Should().BeTrue();
            (type11 != type12).Should().BeTrue();
            (type8 != type13).Should().BeTrue();
            (type13 == type14).Should().BeTrue();
            (type14 != type15).Should().BeTrue();
            (type15 != type16).Should().BeTrue();
            (type14 == type17).Should().BeTrue();
            (type13 != type18).Should().BeTrue();

            (nullType == null).Should().BeTrue();
            (type1 != nullType).Should().BeTrue();
        }

        [Test]
        public void NamespaceAlias_IsValidated()
        {
            Action act = () => new LogicalType("28f;[]';,.", "Foo");
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void TypeName_IsValidated()
        {
            Action act = () => new LogicalType("tst", "28f;[]';,.");
            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Test]
        public void QualifiedTypeName_IsPermitted()
        {
            var logicalType = new LogicalType("tst", "Outer.Inner");
            logicalType.TypeName.Should().Be("Outer.Inner");
        }

        [Test]
        public void PartiallyOpenLogicalType_ThrowsException()
        {
            Action act = () => new LogicalType("tst", "Result", new[] { new LogicalType("tst", "MainId"), new LogicalType("tst", "Obs", 1, Maybe.NoValue) }, Maybe.NoValue);
            act.ShouldThrow<ArgumentException>();

            act = () => new LogicalType("tst", "Result", new[]
            {
                new LogicalType("tst", "MainId"),
                new LogicalType("tst", "Obs", new [] { new LogicalType("tst", "Error", 2, Maybe.NoValue)}, Maybe.NoValue)
            }, Maybe.NoValue);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Parse_WithCorrectFormatAndNoVersion_ReturnsLogicalType()
        {
            String input = "tst:Foo";

            var logicalType = LogicalType.Parse(input);

            logicalType.Should().NotBeNull();
            logicalType.NamespaceAlias.Should().Be("tst");
            logicalType.TypeName.Should().Be("Foo");
            logicalType.Version.HasValue.Should().BeFalse();
        }

        [Test]
        public void Parse_WithCorrectFormatAndVersion_ReturnsLogicalType()
        {
            String input = "tst:Foo:v42";

            var logicalType = LogicalType.Parse(input);

            logicalType.Should().NotBeNull();
            logicalType.NamespaceAlias.Should().Be("tst");
            logicalType.TypeName.Should().Be("Foo");
            logicalType.Version.HasValue.Should().BeTrue();
            logicalType.Version.Value.Should().Be(42);
        }

        [Test]
        public void Parse_WithIncorrectFormat_ThrowsException()
        {
            String input = "63^*^*:8*&&(&";

            Action act = () => LogicalType.Parse(input);

            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void TryParse_WithCorrectFormatAndNoVersion_ReturnsLogicalType()
        {
            String input = "tst:Foo";

            var logicalType = LogicalType.TryParse(input);

            logicalType.HasValue.Should().BeTrue();
            logicalType.Value.NamespaceAlias.Should().Be("tst");
            logicalType.Value.TypeName.Should().Be("Foo");
            logicalType.Value.Version.HasValue.Should().BeFalse();
        }

        [Test]
        public void TryParse_WithCorrectFormatAndVersion_ReturnsLogicalType()
        {
            String input = "tst:Foo:v42";

            var logicalType = LogicalType.TryParse(input);

            logicalType.HasValue.Should().BeTrue();
            logicalType.Value.NamespaceAlias.Should().Be("tst");
            logicalType.Value.TypeName.Should().Be("Foo");
            logicalType.Value.Version.HasValue.Should().BeTrue();
            logicalType.Value.Version.Value.Should().Be(42);
        }

        [Test]
        public void TryParse_WithIncorrectFormat_ReturnsNoValue()
        {
            String input = "63^*^*:8*&&(&";

            var logicalType = LogicalType.TryParse(input);

            logicalType.HasValue.Should().BeFalse();
        }

        [Test]
        public void TryParse_WithArity_ReturnsLogicalType()
        {
            String input = "tst:Foo`2";

            var logicalType = LogicalType.TryParse(input);
            logicalType.HasValue.Should().BeTrue();

            (logicalType.Value == new LogicalType("tst", "Foo", 2, Maybe.NoValue)).Should().BeTrue();
        }

        [Test]
        public void TryParse_WithArityAndVersion_ReturnsLogicalType()
        {
            String input = "tst:Foo`2:v42";

            var logicalType = LogicalType.TryParse(input);
            logicalType.HasValue.Should().BeTrue();

            (logicalType.Value == new LogicalType("tst", "Foo", 2, 42.ToMaybe())).Should().BeTrue();
        }

        [Test]
        public void TryParse_WithOneTypeArgument_ReturnsLogicalType()
        {
            String input = "tst:Foo<sys:Int32>";

            var logicalType = LogicalType.TryParse(input);
            logicalType.HasValue.Should().BeTrue();

            var arg = new LogicalType("sys", "Int32");

            (logicalType.Value == new LogicalType("tst", "Foo", new[] {arg}, Maybe.NoValue)).Should().BeTrue();
        }

        [Test]
        public void TryParse_WithMultipleTypeArguments_ReturnsLogicalType()
        {
            String input = "tst:Foo<sys:Int32, sys:String, sys:DateTime>";

            var logicalType = LogicalType.TryParse(input);
            logicalType.HasValue.Should().BeTrue();

            var arg1 = new LogicalType("sys", "Int32");
            var arg2 = new LogicalType("sys", "String");
            var arg3 = new LogicalType("sys", "DateTime");

            (logicalType.Value == new LogicalType("tst", "Foo", new[] { arg1, arg2, arg3 }, Maybe.NoValue)).Should().BeTrue();
        }

        [Test]
        public void TryParse_SuperComplex_ReturnsLogicalType()
        {
            String input = "tst:Result<sys:Tuple<sys:Int32, tst:ComplexType:v47>, tst:Observation<tst:Error>>:v42";

            var logicalType = LogicalType.TryParse(input);
            logicalType.HasValue.Should().BeTrue();

            var expected = new LogicalType("tst", "Result", new[]
            {
                new LogicalType("sys", "Tuple", new [] { new LogicalType("sys", "Int32"), new LogicalType("tst", "ComplexType", 0, 47.ToMaybe()) }, Maybe.NoValue),
                new LogicalType("tst", "Observation", new [] { new LogicalType("tst", "Error")}, Maybe.NoValue)
            }, 42.ToMaybe());

            logicalType.Value.Equals(expected).Should().BeTrue();
        }

        [Test]
        public void NoTypeArgumentsAndZeroArity_IsNotOpenType()
        {
            var logicalType = new LogicalType("tst", "Foo");
            logicalType.IsOpenType.Should().BeFalse();
        }

        [Test]
        public void NoTypeArgumentsAndNonZeroArity_IsOpenType()
        {
            var logicalType = new LogicalType("tst", "Foo", 1, Maybe.NoValue);
            logicalType.IsOpenType.Should().BeTrue();
        }

        [Test]
        public void TypeArguments_IsNotOpenType()
        {
            var arguments = new[] 
            {
                new LogicalType("sys", "Int32"),
                new LogicalType("sys", "String")
            };

            var logicalType = new LogicalType("tst", "Foo", arguments, Maybe.NoValue);

            logicalType.IsOpenType.Should().BeFalse();
        }

        [Test]
        public void GetOpenType()
        {
            var logicalType = new LogicalType("tst", "Result", new[] { new LogicalType("sys", "Int32"), new LogicalType("sys", "String") }, Maybe.NoValue);

            var openLogicalType = logicalType.GetOpenType();
            openLogicalType.Equals(LogicalType.Parse("tst:Result`2")).Should().BeTrue();
        }

        [Test]
        public void MakeClosedType()
        {
            var openLogicalType = new LogicalType("tst", "Result", 2, Maybe.NoValue);

            var args = new[]
            {
                new LogicalType("sys", "Int32"),
                new LogicalType("sys", "String")
            };

            var logicalType = openLogicalType.MakeClosedType(args);

            logicalType.Equals(LogicalType.Parse("tst:Result<sys:Int32, sys:String>")).Should().BeTrue();
        }
    }
}
