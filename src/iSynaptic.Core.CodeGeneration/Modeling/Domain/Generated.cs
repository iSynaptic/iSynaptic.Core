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
            return new SyntaxTree(new SyntacticModel.Internal.SyntaxTree(usingStatements.Select(x => ((IAstNode<SyntacticModel.Internal.UsingStatementSyntax>)x).GetUnderlying()), namespaces.Select(x => ((IAstNode<SyntacticModel.Internal.NamespaceSyntax>)x).GetUnderlying())));
        }

        public static NamespaceSyntax Namespace(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
        {
            return new NamespaceSyntax(null, new SyntacticModel.Internal.NamespaceSyntax(name, usingStatements.Select(x => ((IAstNode<SyntacticModel.Internal.UsingStatementSyntax>)x).GetUnderlying()), members.Select(x => ((IAstNode<SyntacticModel.Internal.INamespaceMember>)x).GetUnderlying())));
        }

        public static AggregateSyntax Aggregate(String name, IEnumerable<AggregateEventSyntax> events)
        {
            return new AggregateSyntax(null, new SyntacticModel.Internal.AggregateSyntax(name, events.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateEventSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventSyntax AggregateEvent(String name, IEnumerable<AggregateEventPropertySyntax> properties)
        {
            return new AggregateEventSyntax(null, new SyntacticModel.Internal.AggregateEventSyntax(name, properties.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateEventPropertySyntax>)x).GetUnderlying())));
        }

        public static AggregateEventPropertySyntax AggregateEventProperty(String name, String type)
        {
            return new AggregateEventPropertySyntax(null, new SyntacticModel.Internal.AggregateEventPropertySyntax(name, type));
        }

        public static ValueSyntax Value(String name, IEnumerable<ValuePropertySyntax> properties)
        {
            return new ValueSyntax(null, new SyntacticModel.Internal.ValueSyntax(name, properties.Select(x => ((IAstNode<SyntacticModel.Internal.ValuePropertySyntax>)x).GetUnderlying())));
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
            return new GenericNameSyntax(new SyntacticModel.Internal.GenericNameSyntax(typeArguments.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), identifier));
        }

        public static QualifiedNameSyntax QualifiedName(NameSyntax left, SimpleNameSyntax right)
        {
            return new QualifiedNameSyntax(new SyntacticModel.Internal.QualifiedNameSyntax(((IAstNode<SyntacticModel.Internal.NameSyntax>)left).GetUnderlying(), ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)right).GetUnderlying()));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.Domain.SyntacticModel
{
    internal interface IAstNode<out T> : IVisitable { T GetUnderlying(); }

    internal interface IAstUnderlyingNode<out T, in TParent> { T MakePublic(TParent parent); }

    public interface INamespaceParent : IVisitable
    {
    }

    public interface INamespaceMember : IVisitable
    {
        INamespaceParent Parent { get; }
    }

    public class SyntaxTree : INamespaceParent, IAstNode<Internal.SyntaxTree>
    {
        private readonly Internal.SyntaxTree _underlying;

        internal SyntaxTree(Internal.SyntaxTree underlying)
        {
            _underlying = underlying;
        }

        Internal.SyntaxTree IAstNode<Internal.SyntaxTree>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(UsingStatements);
            dispatch(Namespaces);
        }

        public IEnumerable<UsingStatementSyntax> UsingStatements
        {
            get
            {
                return ((IAstNode<Internal.SyntaxTree>)this).GetUnderlying().UsingStatements.Select(x => ((IAstUnderlyingNode<UsingStatementSyntax, SyntaxTree>)x).MakePublic(this));
            }
        }
        public IEnumerable<NamespaceSyntax> Namespaces
        {
            get
            {
                return ((IAstNode<Internal.SyntaxTree>)this).GetUnderlying().Namespaces.Select(x => ((IAstUnderlyingNode<NamespaceSyntax, SyntaxTree>)x).MakePublic(this));
            }
        }
    }

    public class NamespaceSyntax : INamespaceParent, INamespaceMember, IAstNode<Internal.NamespaceSyntax>
    {
        private readonly INamespaceParent _parent;
        private readonly Internal.NamespaceSyntax _underlying;

        internal NamespaceSyntax(INamespaceParent parent, Internal.NamespaceSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public INamespaceParent Parent { get { return _parent; } }
        Internal.NamespaceSyntax IAstNode<Internal.NamespaceSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(UsingStatements);
            dispatch(Members);
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.NamespaceSyntax>)this).GetUnderlying().Name;
            }
        }
        public IEnumerable<UsingStatementSyntax> UsingStatements
        {
            get
            {
                return ((IAstNode<Internal.NamespaceSyntax>)this).GetUnderlying().UsingStatements.Select(x => ((IAstUnderlyingNode<UsingStatementSyntax, NamespaceSyntax>)x).MakePublic(this));
            }
        }
        public IEnumerable<INamespaceMember> Members
        {
            get
            {
                return ((IAstNode<Internal.NamespaceSyntax>)this).GetUnderlying().Members.Select(x => ((IAstUnderlyingNode<INamespaceMember, NamespaceSyntax>)x).MakePublic(this));
            }
        }
    }

    public class AggregateSyntax : IAstNode<Internal.AggregateSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.AggregateSyntax _underlying;

        internal AggregateSyntax(NamespaceSyntax parent, Internal.AggregateSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        Internal.AggregateSyntax IAstNode<Internal.AggregateSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Events);
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Name;
            }
        }
        public IEnumerable<AggregateEventSyntax> Events
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Events.Select(x => ((IAstUnderlyingNode<AggregateEventSyntax, AggregateSyntax>)x).MakePublic(this));
            }
        }
    }

    public class AggregateEventSyntax : IAstNode<Internal.AggregateEventSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateEventSyntax _underlying;

        internal AggregateEventSyntax(AggregateSyntax parent, Internal.AggregateEventSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateSyntax Parent { get { return _parent; } }
        Internal.AggregateEventSyntax IAstNode<Internal.AggregateEventSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventSyntax>)this).GetUnderlying().Name;
            }
        }
        public IEnumerable<AggregateEventPropertySyntax> Properties
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventSyntax>)this).GetUnderlying().Properties.Select(x => ((IAstUnderlyingNode<AggregateEventPropertySyntax, AggregateEventSyntax>)x).MakePublic(this));
            }
        }
    }

    public class AggregateEventPropertySyntax : IAstNode<Internal.AggregateEventPropertySyntax>
    {
        private readonly AggregateEventSyntax _parent;
        private readonly Internal.AggregateEventPropertySyntax _underlying;

        internal AggregateEventPropertySyntax(AggregateEventSyntax parent, Internal.AggregateEventPropertySyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateEventSyntax Parent { get { return _parent; } }
        Internal.AggregateEventPropertySyntax IAstNode<Internal.AggregateEventPropertySyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Name;
            }
        }
        public String Type
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Type;
            }
        }
    }

    public class ValueSyntax : IAstNode<Internal.ValueSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.ValueSyntax _underlying;

        internal ValueSyntax(NamespaceSyntax parent, Internal.ValueSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        Internal.ValueSyntax IAstNode<Internal.ValueSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.ValueSyntax>)this).GetUnderlying().Name;
            }
        }
        public IEnumerable<ValuePropertySyntax> Properties
        {
            get
            {
                return ((IAstNode<Internal.ValueSyntax>)this).GetUnderlying().Properties.Select(x => ((IAstUnderlyingNode<ValuePropertySyntax, ValueSyntax>)x).MakePublic(this));
            }
        }
    }

    public class ValuePropertySyntax : IAstNode<Internal.ValuePropertySyntax>
    {
        private readonly ValueSyntax _parent;
        private readonly Internal.ValuePropertySyntax _underlying;

        internal ValuePropertySyntax(ValueSyntax parent, Internal.ValuePropertySyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public ValueSyntax Parent { get { return _parent; } }
        Internal.ValuePropertySyntax IAstNode<Internal.ValuePropertySyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.ValuePropertySyntax>)this).GetUnderlying().Name;
            }
        }
        public String Type
        {
            get
            {
                return ((IAstNode<Internal.ValuePropertySyntax>)this).GetUnderlying().Type;
            }
        }
    }

    public class UsingStatementSyntax : IAstNode<Internal.UsingStatementSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.UsingStatementSyntax _underlying;

        internal UsingStatementSyntax(NamespaceSyntax parent, Internal.UsingStatementSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        Internal.UsingStatementSyntax IAstNode<Internal.UsingStatementSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Namespace
        {
            get
            {
                return ((IAstNode<Internal.UsingStatementSyntax>)this).GetUnderlying().Namespace;
            }
        }
    }

    public abstract class NameSyntax : IAstNode<Internal.NameSyntax>
    {
        private readonly Internal.NameSyntax _underlying;

        internal NameSyntax(Internal.NameSyntax underlying)
        {
            _underlying = underlying;
        }

        Internal.NameSyntax IAstNode<Internal.NameSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

    }

    public abstract class SimpleNameSyntax : NameSyntax, IAstNode<Internal.SimpleNameSyntax>
    {
        private readonly Internal.SimpleNameSyntax _underlying;

        internal SimpleNameSyntax(Internal.SimpleNameSyntax underlying)
            : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.SimpleNameSyntax IAstNode<Internal.SimpleNameSyntax>.GetUnderlying() { return _underlying; }

        public String Identifier
        {
            get
            {
                return ((IAstNode<Internal.SimpleNameSyntax>)this).GetUnderlying().Identifier;
            }
        }
    }

    public class IdentifierNameSyntax : SimpleNameSyntax, IAstNode<Internal.IdentifierNameSyntax>
    {
        private readonly Internal.IdentifierNameSyntax _underlying;

        internal IdentifierNameSyntax(Internal.IdentifierNameSyntax underlying)
            : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.IdentifierNameSyntax IAstNode<Internal.IdentifierNameSyntax>.GetUnderlying() { return _underlying; }

    }

    public class GenericNameSyntax : SimpleNameSyntax, IAstNode<Internal.GenericNameSyntax>
    {
        private readonly Internal.GenericNameSyntax _underlying;

        internal GenericNameSyntax(Internal.GenericNameSyntax underlying)
            : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.GenericNameSyntax IAstNode<Internal.GenericNameSyntax>.GetUnderlying() { return _underlying; }

        public override void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(TypeArguments);
            base.AcceptChildren(dispatch);
        }

        public IEnumerable<NameSyntax> TypeArguments
        {
            get
            {
                return ((IAstNode<Internal.GenericNameSyntax>)this).GetUnderlying().TypeArguments.Select(x => ((IAstUnderlyingNode<NameSyntax, GenericNameSyntax>)x).MakePublic(this));
            }
        }
    }

    public class QualifiedNameSyntax : NameSyntax, IAstNode<Internal.QualifiedNameSyntax>
    {
        private readonly Internal.QualifiedNameSyntax _underlying;

        internal QualifiedNameSyntax(Internal.QualifiedNameSyntax underlying)
            : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.QualifiedNameSyntax IAstNode<Internal.QualifiedNameSyntax>.GetUnderlying() { return _underlying; }

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
                return ((IAstUnderlyingNode<NameSyntax, QualifiedNameSyntax>)((IAstNode<Internal.QualifiedNameSyntax>)this).GetUnderlying().Left).MakePublic(this);
            }
        }
        public SimpleNameSyntax Right
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, QualifiedNameSyntax>)((IAstNode<Internal.QualifiedNameSyntax>)this).GetUnderlying().Right).MakePublic(this);
            }
        }
    }

    namespace Internal
    {
        internal interface INamespaceParent
        {
        }

        internal interface INamespaceMember
        {
        }

        internal class SyntaxTree : INamespaceParent, IAstUnderlyingNode<SyntacticModel.SyntaxTree, Object>
        {
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<NamespaceSyntax> _namespaces;

            public SyntaxTree(IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces)
            {
                _usingStatements = usingStatements;
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

            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<NamespaceSyntax> Namespaces { get { return _namespaces; } }
        }

        internal class NamespaceSyntax : INamespaceParent, INamespaceMember, IAstUnderlyingNode<SyntacticModel.NamespaceSyntax, SyntacticModel.INamespaceParent>
        {
            private readonly String _name;
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<INamespaceMember> _members;

            public NamespaceSyntax(String name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
            {
                _name = name;
                _usingStatements = usingStatements;
                _members = members;
            }

            public SyntacticModel.NamespaceSyntax MakePublic(SyntacticModel.INamespaceParent parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.NamespaceSyntax BuildPublic(SyntacticModel.INamespaceParent parent)
            {
                return new SyntacticModel.NamespaceSyntax(parent, this);
            }

            public String Name { get { return _name; } }
            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<INamespaceMember> Members { get { return _members; } }
        }

        internal class AggregateSyntax : IAstUnderlyingNode<SyntacticModel.AggregateSyntax, SyntacticModel.NamespaceSyntax>
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

        internal class AggregateEventSyntax : IAstUnderlyingNode<SyntacticModel.AggregateEventSyntax, SyntacticModel.AggregateSyntax>
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

        internal class AggregateEventPropertySyntax : IAstUnderlyingNode<SyntacticModel.AggregateEventPropertySyntax, SyntacticModel.AggregateEventSyntax>
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

        internal class ValueSyntax : IAstUnderlyingNode<SyntacticModel.ValueSyntax, SyntacticModel.NamespaceSyntax>
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

        internal class ValuePropertySyntax : IAstUnderlyingNode<SyntacticModel.ValuePropertySyntax, SyntacticModel.ValueSyntax>
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

        internal class UsingStatementSyntax : IAstUnderlyingNode<SyntacticModel.UsingStatementSyntax, SyntacticModel.NamespaceSyntax>
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

        internal abstract class NameSyntax : IAstUnderlyingNode<SyntacticModel.NameSyntax, Object>
        {

            public SyntacticModel.NameSyntax MakePublic(Object parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.NameSyntax BuildPublic(Object parent);
        }

        internal abstract class SimpleNameSyntax : NameSyntax, IAstUnderlyingNode<SyntacticModel.SimpleNameSyntax, Object>
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

        internal class IdentifierNameSyntax : SimpleNameSyntax, IAstUnderlyingNode<SyntacticModel.IdentifierNameSyntax, Object>
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

        internal class GenericNameSyntax : SimpleNameSyntax, IAstUnderlyingNode<SyntacticModel.GenericNameSyntax, Object>
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

        internal class QualifiedNameSyntax : NameSyntax, IAstUnderlyingNode<SyntacticModel.QualifiedNameSyntax, Object>
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

