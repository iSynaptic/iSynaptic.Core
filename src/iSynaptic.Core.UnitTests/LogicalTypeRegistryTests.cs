using FluentAssertions;
using iSynaptic.Commons;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSynaptic
{
    [TestFixture]
    public class LogicalTypeRegistryTests
    {
        [Test]
        public void SimpleMapping_BehavesCorrectly()
        {
            var logicalType = LogicalType.Parse("sys:String");
            var actualType = typeof(string);

            var registry = new LogicalTypeRegistry();
            registry.AddMapping(logicalType, actualType);

            registry.LookupActualType(logicalType).Should().Be(actualType);
            registry.LookupLogicalType(actualType).Should().Be(logicalType);
        }

        [Test]
        public void ClosedTypes_MapCorrectly()
        {
            var logicalType = LogicalType.Parse("syn:Maybe<sys:Int32>");
            var actualType = typeof(Maybe<int>);

            var registry = new LogicalTypeRegistry();
            registry.AddMapping(logicalType, actualType);

            registry.LookupActualType(logicalType).Should().Be(actualType);
            registry.LookupLogicalType(actualType).Should().Be(logicalType);
        }

        [Test]
        public void OpenTypes_MapCorrectly()
        {
            var logicalType = LogicalType.Parse("syn:Maybe`1");
            var actualType = typeof(Maybe<>);

            var registry = new LogicalTypeRegistry();
            registry.AddMapping(logicalType, actualType);

            registry.LookupActualType(logicalType).Should().Be(actualType);
            registry.LookupLogicalType(actualType).Should().Be(logicalType);
        }

        [Test]
        public void CannotMapOpenLogicalTypeToClosedActualType()
        {
            var logicalType = LogicalType.Parse("syn:Maybe`1");
            var actualType = typeof(Maybe<int>);

            var registry = new LogicalTypeRegistry();
            registry.Invoking(x => x.AddMapping(logicalType, actualType))
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void CannotMapClosedLogicalTypeToOpenActualType()
        {
            var logicalType = LogicalType.Parse("syn:Maybe<sys:Int32>");
            var actualType = typeof(Maybe<>);

            var registry = new LogicalTypeRegistry();
            registry.Invoking(x => x.AddMapping(logicalType, actualType))
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void CannotMapOpenTypesWithMismatchedArity()
        {
            var registry = new LogicalTypeRegistry();

            registry.Invoking(x => x.AddMapping(LogicalType.Parse("syn:Maybe"), typeof(Maybe<>)))
                .ShouldThrow<ArgumentException>();

            registry.Invoking(x => x.AddMapping(LogicalType.Parse("syn:Maybe`2"), typeof(Maybe<>)))
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void CannotAddDuplicateMappings()
        {
            var registry = new LogicalTypeRegistry();
            registry.AddMapping(LogicalType.Parse("sys:String"), typeof(string));

            registry.Invoking(x => x.AddMapping(LogicalType.Parse("sys:String"), typeof(int)))
                .ShouldThrow<ArgumentException>();

            registry.Invoking(x => x.AddMapping(LogicalType.Parse("tst:Description"), typeof(string)))
                .ShouldThrow<ArgumentException>();
        }

        [Test]
        public void LookupActualTypeUsingClosedLogicalType_ViaOpenMapping()
        {
            var registry = new LogicalTypeRegistry();
            registry.AddMapping(LogicalType.Parse("syn:Result`2"), typeof(Result<,>));
            registry.AddMapping(LogicalType.Parse("sys:Int32"), typeof(int));
            registry.AddMapping(LogicalType.Parse("sys:String"), typeof(string));

            registry.LookupActualType(LogicalType.Parse("syn:Result<sys:Int32, sys:String>"))
                .Should().Be(typeof(Result<int, string>));
        }

        [Test]
        public void LookupLogicalTypeUsingClosedActualType_ViaOpenMapping()
        {
            var registry = new LogicalTypeRegistry();
            registry.AddMapping(LogicalType.Parse("syn:Result`2"), typeof(Result<,>));
            registry.AddMapping(LogicalType.Parse("sys:Int32"), typeof(int));
            registry.AddMapping(LogicalType.Parse("sys:String"), typeof(string));

            registry.LookupLogicalType(typeof(Result<int, string>))
                .Should().Be(LogicalType.Parse("syn:Result<sys:Int32, sys:String>"));
        }
    }
}
