using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    using SyntacticModel;

    public static class Syntax
    {
        public static SyntaxTree SyntaxTree(IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces)
        {
            return new SyntaxTree(new SyntacticModel.Internal.SyntaxTree(usingStatements.Select(x => x.GetUnderlying()), namespaces.Select(x => x.GetUnderlying())));
        }

        public static NamespaceSyntax Namespace(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces, IEnumerable<AggregateSyntax> aggregates, IEnumerable<ValueSyntax> values)
        {
            return new NamespaceSyntax(null, new SyntacticModel.Internal.NamespaceSyntax(name, usingStatements.Select(x => x.GetUnderlying()), namespaces.Select(x => x.GetUnderlying()), aggregates.Select(x => x.GetUnderlying()), values.Select(x => x.GetUnderlying())));
        }

        public static AggregateSyntax Aggregate(String name, IEnumerable<AggregateEventSyntax> events)
        {
            return new AggregateSyntax(null, new SyntacticModel.Internal.AggregateSyntax(name, events.Select(x => x.GetUnderlying())));
        }

        public static AggregateEventSyntax AggregateEvent(String name, IEnumerable<AggregateEventPropertySyntax> properties)
        {
            return new AggregateEventSyntax(null, new SyntacticModel.Internal.AggregateEventSyntax(name, properties.Select(x => x.GetUnderlying())));
        }

        public static AggregateEventPropertySyntax AggregateEventProperty(String name, String type)
        {
            return new AggregateEventPropertySyntax(null, new SyntacticModel.Internal.AggregateEventPropertySyntax(name, type));
        }

        public static ValueSyntax Value(String name, IEnumerable<ValuePropertySyntax> properties)
        {
            return new ValueSyntax(null, new SyntacticModel.Internal.ValueSyntax(name, properties.Select(x => x.GetUnderlying())));
        }

        public static ValuePropertySyntax ValueProperty(String name, String type)
        {
            return new ValuePropertySyntax(null, new SyntacticModel.Internal.ValuePropertySyntax(name, type));
        }

        public static UsingStatementSyntax UsingStatement(String @namespace)
        {
            return new UsingStatementSyntax(null, new SyntacticModel.Internal.UsingStatementSyntax(@namespace));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public partial class SyntaxTree : INamespaceParent, IVisitable
    {
        private readonly Internal.SyntaxTree _underlying;

        internal SyntaxTree(Internal.SyntaxTree underlying)
        {
            _underlying = underlying;
        }

        internal Internal.SyntaxTree GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(UsingStatements);
            dispatch(Namespaces);
        }
        public IEnumerable<UsingStatementSyntax> UsingStatements
        {
            get
            {
                return _underlying.UsingStatements.Select(x => new UsingStatementSyntax(this, x));
            }
        }
        public IEnumerable<NamespaceSyntax> Namespaces
        {
            get
            {
                return _underlying.Namespaces.Select(x => new NamespaceSyntax(this, x));
            }
        }
    }

    public partial class NamespaceSyntax : INamespaceParent, IVisitable
    {
        private readonly INamespaceParent _parent;
        private readonly Internal.NamespaceSyntax _underlying;

        internal NamespaceSyntax(INamespaceParent parent, Internal.NamespaceSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public INamespaceParent Parent { get { return _parent; } }
        internal Internal.NamespaceSyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(UsingStatements);
            dispatch(Namespaces);
            dispatch(Aggregates);
            dispatch(Values);
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public IEnumerable<UsingStatementSyntax> UsingStatements
        {
            get
            {
                return _underlying.UsingStatements.Select(x => new UsingStatementSyntax(this, x));
            }
        }
        public IEnumerable<NamespaceSyntax> Namespaces
        {
            get
            {
                return _underlying.Namespaces.Select(x => new NamespaceSyntax(this, x));
            }
        }
        public IEnumerable<AggregateSyntax> Aggregates
        {
            get
            {
                return _underlying.Aggregates.Select(x => new AggregateSyntax(this, x));
            }
        }
        public IEnumerable<ValueSyntax> Values
        {
            get
            {
                return _underlying.Values.Select(x => new ValueSyntax(this, x));
            }
        }
    }

    public partial class AggregateSyntax : IVisitable
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.AggregateSyntax _underlying;

        internal AggregateSyntax(NamespaceSyntax parent, Internal.AggregateSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        internal Internal.AggregateSyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Events);
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public IEnumerable<AggregateEventSyntax> Events
        {
            get
            {
                return _underlying.Events.Select(x => new AggregateEventSyntax(this, x));
            }
        }
    }

    public partial class AggregateEventSyntax : IVisitable
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateEventSyntax _underlying;

        internal AggregateEventSyntax(AggregateSyntax parent, Internal.AggregateEventSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateSyntax Parent { get { return _parent; } }
        internal Internal.AggregateEventSyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public IEnumerable<AggregateEventPropertySyntax> Properties
        {
            get
            {
                return _underlying.Properties.Select(x => new AggregateEventPropertySyntax(this, x));
            }
        }
    }

    public partial class AggregateEventPropertySyntax : IVisitable
    {
        private readonly AggregateEventSyntax _parent;
        private readonly Internal.AggregateEventPropertySyntax _underlying;

        internal AggregateEventPropertySyntax(AggregateEventSyntax parent, Internal.AggregateEventPropertySyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateEventSyntax Parent { get { return _parent; } }
        internal Internal.AggregateEventPropertySyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public String Type
        {
            get
            {
                return _underlying.Type;
            }
        }
    }

    public partial class ValueSyntax : IVisitable
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.ValueSyntax _underlying;

        internal ValueSyntax(NamespaceSyntax parent, Internal.ValueSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        internal Internal.ValueSyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public IEnumerable<ValuePropertySyntax> Properties
        {
            get
            {
                return _underlying.Properties.Select(x => new ValuePropertySyntax(this, x));
            }
        }
    }

    public partial class ValuePropertySyntax : IVisitable
    {
        private readonly ValueSyntax _parent;
        private readonly Internal.ValuePropertySyntax _underlying;

        internal ValuePropertySyntax(ValueSyntax parent, Internal.ValuePropertySyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public ValueSyntax Parent { get { return _parent; } }
        internal Internal.ValuePropertySyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }
        public String Name
        {
            get
            {
                return _underlying.Name;
            }
        }
        public String Type
        {
            get
            {
                return _underlying.Type;
            }
        }
    }

    public partial class UsingStatementSyntax : IVisitable
    {
        private readonly INamespaceParent _parent;
        private readonly Internal.UsingStatementSyntax _underlying;

        internal UsingStatementSyntax(INamespaceParent parent, Internal.UsingStatementSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public INamespaceParent Parent { get { return _parent; } }
        internal Internal.UsingStatementSyntax GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }
        public String Namespace
        {
            get
            {
                return _underlying.Namespace;
            }
        }
    }

    namespace Internal
    {
        internal class SyntaxTree
        {
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<NamespaceSyntax> _namespaces;

            public SyntaxTree(IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces)
            {
                _usingStatements = usingStatements;
                _namespaces = namespaces;
            }

            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<NamespaceSyntax> Namespaces { get { return _namespaces; } }
        }

        internal class NamespaceSyntax
        {
            private readonly String _name;
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<NamespaceSyntax> _namespaces;
            private readonly IEnumerable<AggregateSyntax> _aggregates;
            private readonly IEnumerable<ValueSyntax> _values;

            public NamespaceSyntax(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces, IEnumerable<AggregateSyntax> aggregates, IEnumerable<ValueSyntax> values)
            {
                _name = name;
                _usingStatements = usingStatements;
                _namespaces = namespaces;
                _aggregates = aggregates;
                _values = values;
            }

            public String Name { get { return _name; } }
            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<NamespaceSyntax> Namespaces { get { return _namespaces; } }
            public IEnumerable<AggregateSyntax> Aggregates { get { return _aggregates; } }
            public IEnumerable<ValueSyntax> Values { get { return _values; } }
        }

        internal class AggregateSyntax
        {
            private readonly String _name;
            private readonly IEnumerable<AggregateEventSyntax> _events;

            public AggregateSyntax(String name, IEnumerable<AggregateEventSyntax> events)
            {
                _name = name;
                _events = events;
            }

            public String Name { get { return _name; } }
            public IEnumerable<AggregateEventSyntax> Events { get { return _events; } }
        }

        internal class AggregateEventSyntax
        {
            private readonly String _name;
            private readonly IEnumerable<AggregateEventPropertySyntax> _properties;

            public AggregateEventSyntax(String name, IEnumerable<AggregateEventPropertySyntax> properties)
            {
                _name = name;
                _properties = properties;
            }

            public String Name { get { return _name; } }
            public IEnumerable<AggregateEventPropertySyntax> Properties { get { return _properties; } }
        }

        internal class AggregateEventPropertySyntax
        {
            private readonly String _name;
            private readonly String _type;

            public AggregateEventPropertySyntax(String name, String type)
            {
                _name = name;
                _type = type;
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
        }

        internal class ValueSyntax
        {
            private readonly String _name;
            private readonly IEnumerable<ValuePropertySyntax> _properties;

            public ValueSyntax(String name, IEnumerable<ValuePropertySyntax> properties)
            {
                _name = name;
                _properties = properties;
            }

            public String Name { get { return _name; } }
            public IEnumerable<ValuePropertySyntax> Properties { get { return _properties; } }
        }

        internal class ValuePropertySyntax
        {
            private readonly String _name;
            private readonly String _type;

            public ValuePropertySyntax(String name, String type)
            {
                _name = name;
                _type = type;
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
        }

        internal class UsingStatementSyntax
        {
            private readonly String _namespace;

            public UsingStatementSyntax(String @namespace)
            {
                _namespace = @namespace;
            }

            public String Namespace { get { return _namespace; } }
        }
    }
}

