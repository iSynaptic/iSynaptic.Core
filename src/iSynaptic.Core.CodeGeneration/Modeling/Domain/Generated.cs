using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    using SyntacticModel;

    public static class Syntax
    {
        public static SyntaxTree SyntaxTree(IEnumerable<NamespaceSyntax> namespaces)
        {
            return new SyntaxTree(new SyntacticModel.Internal.SyntaxTree(namespaces.Select(x => x.GetUnderlying())));
        }

        public static NamespaceSyntax Namespace(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<AggregateSyntax> aggregates, IEnumerable<ValueSyntax> values)
        {
            return new NamespaceSyntax(null, new SyntacticModel.Internal.NamespaceSyntax(name, usingStatements.Select(x => x.GetUnderlying()), aggregates.Select(x => x.GetUnderlying()), values.Select(x => x.GetUnderlying())));
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



        public static IdentifierNameSyntax IdentifierName(String identifier)
        {
            return new IdentifierNameSyntax(new SyntacticModel.Internal.IdentifierNameSyntax(identifier));
        }

        public static GenericNameSyntax GenericName(IEnumerable<NameSyntax> typeArguments, String identifier)
        {
            return new GenericNameSyntax(new SyntacticModel.Internal.GenericNameSyntax(typeArguments.Select(x => x.GetUnderlying()), identifier));
        }

        public static QualifiedNameSyntax QualifiedName(NameSyntax left, SimpleNameSyntax right)
        {
            return new QualifiedNameSyntax(new SyntacticModel.Internal.QualifiedNameSyntax(left.GetUnderlying(), right.GetUnderlying()));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    public class SyntaxTree : INamespaceParent, IVisitable
    {
        private readonly Internal.SyntaxTree _underlying;

        internal SyntaxTree(Internal.SyntaxTree underlying)
        {
            _underlying = underlying;
        }

        internal Internal.SyntaxTree GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Namespaces);
        }

        public IEnumerable<NamespaceSyntax> Namespaces
        {
            get
            {
                return GetUnderlying().Namespaces.Select(x => x.MakePublic(this));
            }
        }
    }

    public class NamespaceSyntax : IVisitable
    {
        private readonly SyntaxTree _parent;
        private readonly Internal.NamespaceSyntax _underlying;

        internal NamespaceSyntax(SyntaxTree parent, Internal.NamespaceSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public SyntaxTree Parent { get { return _parent; } }
        internal Internal.NamespaceSyntax GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(UsingStatements);
            dispatch(Aggregates);
            dispatch(Values);
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public IEnumerable<UsingStatementSyntax> UsingStatements
        {
            get
            {
                return GetUnderlying().UsingStatements.Select(x => x.MakePublic(this));
            }
        }
        public IEnumerable<AggregateSyntax> Aggregates
        {
            get
            {
                return GetUnderlying().Aggregates.Select(x => x.MakePublic(this));
            }
        }
        public IEnumerable<ValueSyntax> Values
        {
            get
            {
                return GetUnderlying().Values.Select(x => x.MakePublic(this));
            }
        }
    }

    public class AggregateSyntax : IVisitable
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

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Events);
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public IEnumerable<AggregateEventSyntax> Events
        {
            get
            {
                return GetUnderlying().Events.Select(x => x.MakePublic(this));
            }
        }
    }

    public class AggregateEventSyntax : IVisitable
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

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public IEnumerable<AggregateEventPropertySyntax> Properties
        {
            get
            {
                return GetUnderlying().Properties.Select(x => x.MakePublic(this));
            }
        }
    }

    public class AggregateEventPropertySyntax : IVisitable
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

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public String Type
        {
            get
            {
                return GetUnderlying().Type;
            }
        }
    }

    public class ValueSyntax : IVisitable
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

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public IEnumerable<ValuePropertySyntax> Properties
        {
            get
            {
                return GetUnderlying().Properties.Select(x => x.MakePublic(this));
            }
        }
    }

    public class ValuePropertySyntax : IVisitable
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

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public String Type
        {
            get
            {
                return GetUnderlying().Type;
            }
        }
    }

    public class UsingStatementSyntax : IVisitable
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.UsingStatementSyntax _underlying;

        internal UsingStatementSyntax(NamespaceSyntax parent, Internal.UsingStatementSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        internal Internal.UsingStatementSyntax GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Namespace
        {
            get
            {
                return GetUnderlying().Namespace;
            }
        }
    }

    public abstract class NameSyntax : IVisitable
    {
        private readonly Internal.NameSyntax _underlying;

        internal NameSyntax(Internal.NameSyntax underlying)
        {
            _underlying = underlying;
        }

        internal Internal.NameSyntax GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

    }

    public abstract class SimpleNameSyntax : NameSyntax
    {

        internal SimpleNameSyntax(Internal.SimpleNameSyntax underlying)
            : base(underlying)
        {
        }

        new internal Internal.SimpleNameSyntax GetUnderlying() { return (Internal.SimpleNameSyntax)base.GetUnderlying(); }

        public String Identifier
        {
            get
            {
                return GetUnderlying().Identifier;
            }
        }
    }

    public class IdentifierNameSyntax : SimpleNameSyntax
    {

        internal IdentifierNameSyntax(Internal.IdentifierNameSyntax underlying)
            : base(underlying)
        {
        }

        new internal Internal.IdentifierNameSyntax GetUnderlying() { return (Internal.IdentifierNameSyntax)base.GetUnderlying(); }

    }

    public class GenericNameSyntax : SimpleNameSyntax
    {

        internal GenericNameSyntax(Internal.GenericNameSyntax underlying)
            : base(underlying)
        {
        }

        new internal Internal.GenericNameSyntax GetUnderlying() { return (Internal.GenericNameSyntax)base.GetUnderlying(); }

        public override void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(TypeArguments);
            base.AcceptChildren(dispatch);
        }

        public IEnumerable<NameSyntax> TypeArguments
        {
            get
            {
                return GetUnderlying().TypeArguments.Select(x => x.MakePublic(this));
            }
        }
    }

    public class QualifiedNameSyntax : NameSyntax
    {

        internal QualifiedNameSyntax(Internal.QualifiedNameSyntax underlying)
            : base(underlying)
        {
        }

        new internal Internal.QualifiedNameSyntax GetUnderlying() { return (Internal.QualifiedNameSyntax)base.GetUnderlying(); }

        public override void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { Left });
            dispatch(new[] { Right });
            base.AcceptChildren(dispatch);
        }

        public NameSyntax Left
        {
            get
            {
                return GetUnderlying().Left.MakePublic(this);
            }
        }
        public SimpleNameSyntax Right
        {
            get
            {
                return GetUnderlying().Right.MakePublic(this);
            }
        }
    }

    namespace Internal
    {
        internal class SyntaxTree
        {
            private readonly IEnumerable<NamespaceSyntax> _namespaces;

            public SyntaxTree(IEnumerable<NamespaceSyntax> namespaces)
            {
                _namespaces = namespaces;
            }

            public SyntacticModel.SyntaxTree MakePublic(Object parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.SyntaxTree BuildPublic(Object parent)
            {
                return new SyntacticModel.SyntaxTree(this);
            }

            public IEnumerable<NamespaceSyntax> Namespaces { get { return _namespaces; } }
        }

        internal class NamespaceSyntax
        {
            private readonly String _name;
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<AggregateSyntax> _aggregates;
            private readonly IEnumerable<ValueSyntax> _values;

            public NamespaceSyntax(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<AggregateSyntax> aggregates, IEnumerable<ValueSyntax> values)
            {
                _name = name;
                _usingStatements = usingStatements;
                _aggregates = aggregates;
                _values = values;
            }

            public SyntacticModel.NamespaceSyntax MakePublic(SyntacticModel.SyntaxTree parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.NamespaceSyntax BuildPublic(SyntacticModel.SyntaxTree parent)
            {
                return new SyntacticModel.NamespaceSyntax(parent, this);
            }

            public String Name { get { return _name; } }
            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
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

            public SyntacticModel.AggregateSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AggregateSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.AggregateSyntax(parent, this);
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

            public SyntacticModel.AggregateEventSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AggregateEventSyntax BuildPublic(SyntacticModel.AggregateSyntax parent)
            {
                return new SyntacticModel.AggregateEventSyntax(parent, this);
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

            public SyntacticModel.AggregateEventPropertySyntax MakePublic(SyntacticModel.AggregateEventSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AggregateEventPropertySyntax BuildPublic(SyntacticModel.AggregateEventSyntax parent)
            {
                return new SyntacticModel.AggregateEventPropertySyntax(parent, this);
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

            public SyntacticModel.ValueSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.ValueSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.ValueSyntax(parent, this);
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

            public SyntacticModel.ValuePropertySyntax MakePublic(SyntacticModel.ValueSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.ValuePropertySyntax BuildPublic(SyntacticModel.ValueSyntax parent)
            {
                return new SyntacticModel.ValuePropertySyntax(parent, this);
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

            public SyntacticModel.UsingStatementSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.UsingStatementSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.UsingStatementSyntax(parent, this);
            }

            public String Namespace { get { return _namespace; } }
        }

        internal abstract class NameSyntax
        {

            public SyntacticModel.NameSyntax MakePublic(Object parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.NameSyntax BuildPublic(Object parent);
        }

        internal abstract class SimpleNameSyntax : NameSyntax
        {
            private readonly String _identifier;

            protected SimpleNameSyntax(String identifier)
            {
                _identifier = identifier;
            }

            public new SyntacticModel.SimpleNameSyntax MakePublic(Object parent)
            {
                return (SyntacticModel.SimpleNameSyntax)BuildPublic(parent);
            }

            public String Identifier { get { return _identifier; } }
        }

        internal class IdentifierNameSyntax : SimpleNameSyntax
        {

            public IdentifierNameSyntax(String identifier)
                : base(identifier)
            {
            }

            public new SyntacticModel.IdentifierNameSyntax MakePublic(Object parent)
            {
                return (SyntacticModel.IdentifierNameSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.NameSyntax BuildPublic(Object parent)
            {
                return new SyntacticModel.IdentifierNameSyntax(this);
            }

        }

        internal class GenericNameSyntax : SimpleNameSyntax
        {
            private readonly IEnumerable<NameSyntax> _typeArguments;

            public GenericNameSyntax(IEnumerable<NameSyntax> typeArguments, String identifier)
                : base(identifier)
            {
                _typeArguments = typeArguments;
            }

            public new SyntacticModel.GenericNameSyntax MakePublic(Object parent)
            {
                return (SyntacticModel.GenericNameSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.NameSyntax BuildPublic(Object parent)
            {
                return new SyntacticModel.GenericNameSyntax(this);
            }

            public IEnumerable<NameSyntax> TypeArguments { get { return _typeArguments; } }
        }

        internal class QualifiedNameSyntax : NameSyntax
        {
            private readonly NameSyntax _left;
            private readonly SimpleNameSyntax _right;

            public QualifiedNameSyntax(NameSyntax left, SimpleNameSyntax right)
            {
                _left = left;
                _right = right;
            }

            public new SyntacticModel.QualifiedNameSyntax MakePublic(Object parent)
            {
                return (SyntacticModel.QualifiedNameSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.NameSyntax BuildPublic(Object parent)
            {
                return new SyntacticModel.QualifiedNameSyntax(this);
            }

            public NameSyntax Left { get { return _left; } }
            public SimpleNameSyntax Right { get { return _right; } }
        }
    }
}

