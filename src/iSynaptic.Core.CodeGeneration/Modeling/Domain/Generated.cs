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

        public static NamespaceSyntax Namespace(NameSyntax name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
        {
            return new NamespaceSyntax(null, new SyntacticModel.Internal.NamespaceSyntax(((IAstNode<SyntacticModel.Internal.NameSyntax>)name).GetUnderlying(), usingStatements.Select(x => ((IAstNode<SyntacticModel.Internal.UsingStatementSyntax>)x).GetUnderlying()), members.Select(x => ((IAstNode<SyntacticModel.Internal.INamespaceMember>)x).GetUnderlying())));
        }

        public static AggregateSyntax Aggregate(SimpleNameSyntax name, Maybe<NameSyntax> identifierType, Maybe<NameSyntax> baseAggregate, IEnumerable<AggregateEventSyntax> events)
        {
            return new AggregateSyntax(null, new SyntacticModel.Internal.AggregateSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)name).GetUnderlying(), identifierType.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), baseAggregate.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), events.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateEventSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventSyntax AggregateEvent(SimpleNameSyntax name, IEnumerable<AggregateEventPropertySyntax> properties)
        {
            return new AggregateEventSyntax(null, new SyntacticModel.Internal.AggregateEventSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)name).GetUnderlying(), properties.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateEventPropertySyntax>)x).GetUnderlying())));
        }

        public static AggregateEventPropertySyntax AggregateEventProperty(SimpleNameSyntax name, SimpleNameSyntax type)
        {
            return new AggregateEventPropertySyntax(null, new SyntacticModel.Internal.AggregateEventPropertySyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)name).GetUnderlying(), ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)type).GetUnderlying()));
        }

        public static ValueSyntax Value(SimpleNameSyntax name, IEnumerable<ValuePropertySyntax> properties)
        {
            return new ValueSyntax(null, new SyntacticModel.Internal.ValueSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)name).GetUnderlying(), properties.Select(x => ((IAstNode<SyntacticModel.Internal.ValuePropertySyntax>)x).GetUnderlying())));
        }

        public static ValuePropertySyntax ValueProperty(SimpleNameSyntax name, SimpleNameSyntax type)
        {
            return new ValuePropertySyntax(null, new SyntacticModel.Internal.ValuePropertySyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)name).GetUnderlying(), ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)type).GetUnderlying()));
        }

        public static UsingStatementSyntax UsingStatement(NameSyntax @namespace)
        {
            return new UsingStatementSyntax(new SyntacticModel.Internal.UsingStatementSyntax(((IAstNode<SyntacticModel.Internal.NameSyntax>)@namespace).GetUnderlying()));
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
    }

    public interface IUsingsContainer : IVisitable
    {
        IEnumerable<UsingStatementSyntax> UsingStatements { get; }
    }

    public interface ISymbol : IVisitable
    {
        NameSyntax FullName { get; }
    }

    public interface IType : ISymbol, IVisitable
    {
        SimpleNameSyntax Name { get; }
    }

    public partial class SyntaxTree : INamespaceParent, IUsingsContainer, IAstNode<Internal.SyntaxTree>
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

    public partial class NamespaceSyntax : INamespaceParent, INamespaceMember, IUsingsContainer, ISymbol, IAstNode<Internal.NamespaceSyntax>
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
            dispatch(new[] { Name });
            dispatch(UsingStatements);
            dispatch(Members);
        }

        public NameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<NameSyntax, NamespaceSyntax>)((IAstNode<Internal.NamespaceSyntax>)this).GetUnderlying().Name).MakePublic(this);
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

    public partial class AggregateSyntax : INamespaceMember, IType, IAstNode<Internal.AggregateSyntax>
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
            dispatch(new[] { Name });
            dispatch(IdentifierType.ToEnumerable());
            dispatch(BaseAggregate.ToEnumerable());
            dispatch(Events);
        }

        public SimpleNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateSyntax>)((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public Maybe<NameSyntax> IdentifierType
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().IdentifierType.Select(x => ((IAstUnderlyingNode<NameSyntax, AggregateSyntax>)x).MakePublic(this));
            }
        }
        public Maybe<NameSyntax> BaseAggregate
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().BaseAggregate.Select(x => ((IAstUnderlyingNode<NameSyntax, AggregateSyntax>)x).MakePublic(this));
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

    public partial class AggregateEventSyntax : ISymbol, IAstNode<Internal.AggregateEventSyntax>
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
            dispatch(new[] { Name });
            dispatch(Properties);
        }

        public SimpleNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateEventSyntax>)((IAstNode<Internal.AggregateEventSyntax>)this).GetUnderlying().Name).MakePublic(this);
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

    public partial class AggregateEventPropertySyntax : ISymbol, IAstNode<Internal.AggregateEventPropertySyntax>
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
            dispatch(new[] { Name });
            dispatch(new[] { Type });
        }

        public SimpleNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateEventPropertySyntax>)((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public SimpleNameSyntax Type
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateEventPropertySyntax>)((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Type).MakePublic(this);
            }
        }
    }

    public partial class ValueSyntax : INamespaceMember, IType, IAstNode<Internal.ValueSyntax>
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
            dispatch(new[] { Name });
            dispatch(Properties);
        }

        public SimpleNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, ValueSyntax>)((IAstNode<Internal.ValueSyntax>)this).GetUnderlying().Name).MakePublic(this);
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

    public partial class ValuePropertySyntax : ISymbol, IAstNode<Internal.ValuePropertySyntax>
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
            dispatch(new[] { Name });
            dispatch(new[] { Type });
        }

        public SimpleNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, ValuePropertySyntax>)((IAstNode<Internal.ValuePropertySyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public SimpleNameSyntax Type
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, ValuePropertySyntax>)((IAstNode<Internal.ValuePropertySyntax>)this).GetUnderlying().Type).MakePublic(this);
            }
        }
    }

    public partial class UsingStatementSyntax : IAstNode<Internal.UsingStatementSyntax>
    {
        private readonly Internal.UsingStatementSyntax _underlying;

        internal UsingStatementSyntax(Internal.UsingStatementSyntax underlying)
        {
            _underlying = underlying;
        }

        Internal.UsingStatementSyntax IAstNode<Internal.UsingStatementSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { Namespace });
        }

        public NameSyntax Namespace
        {
            get
            {
                return ((IAstUnderlyingNode<NameSyntax, UsingStatementSyntax>)((IAstNode<Internal.UsingStatementSyntax>)this).GetUnderlying().Namespace).MakePublic(this);
            }
        }
    }

    public abstract partial class NameSyntax : IAstNode<Internal.NameSyntax>
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

    public abstract partial class SimpleNameSyntax : NameSyntax, IAstNode<Internal.SimpleNameSyntax>
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

    public partial class IdentifierNameSyntax : SimpleNameSyntax, IAstNode<Internal.IdentifierNameSyntax>
    {
        private readonly Internal.IdentifierNameSyntax _underlying;

        internal IdentifierNameSyntax(Internal.IdentifierNameSyntax underlying)
            : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.IdentifierNameSyntax IAstNode<Internal.IdentifierNameSyntax>.GetUnderlying() { return _underlying; }

    }

    public partial class GenericNameSyntax : SimpleNameSyntax, IAstNode<Internal.GenericNameSyntax>
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

    public partial class QualifiedNameSyntax : NameSyntax, IAstNode<Internal.QualifiedNameSyntax>
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

        internal interface IUsingsContainer
        {
        }

        internal interface ISymbol
        {
        }

        internal interface IType : ISymbol
        {
        }

        internal class SyntaxTree : INamespaceParent, IUsingsContainer, IAstUnderlyingNode<SyntacticModel.SyntaxTree, Object>
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

        internal class NamespaceSyntax : INamespaceParent, INamespaceMember, IUsingsContainer, ISymbol, IAstUnderlyingNode<SyntacticModel.NamespaceSyntax, SyntacticModel.INamespaceParent>
        {
            private readonly NameSyntax _name;
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<INamespaceMember> _members;

            public NamespaceSyntax(NameSyntax name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
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

            public NameSyntax Name { get { return _name; } }
            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<INamespaceMember> Members { get { return _members; } }
        }

        internal class AggregateSyntax : INamespaceMember, IType, IAstUnderlyingNode<SyntacticModel.AggregateSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly SimpleNameSyntax _name;
            private readonly Maybe<NameSyntax> _identifierType;
            private readonly Maybe<NameSyntax> _baseAggregate;
            private readonly IEnumerable<AggregateEventSyntax> _events;

            public AggregateSyntax(SimpleNameSyntax name, Maybe<NameSyntax> identifierType, Maybe<NameSyntax> baseAggregate, IEnumerable<AggregateEventSyntax> events)
            {
                _name = name;
                _identifierType = identifierType;
                _baseAggregate = baseAggregate;
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

            public SimpleNameSyntax Name { get { return _name; } }
            public Maybe<NameSyntax> IdentifierType { get { return _identifierType; } }
            public Maybe<NameSyntax> BaseAggregate { get { return _baseAggregate; } }
            public IEnumerable<AggregateEventSyntax> Events { get { return _events; } }
        }

        internal class AggregateEventSyntax : ISymbol, IAstUnderlyingNode<SyntacticModel.AggregateEventSyntax, SyntacticModel.AggregateSyntax>
        {
            private readonly SimpleNameSyntax _name;
            private readonly IEnumerable<AggregateEventPropertySyntax> _properties;

            public AggregateEventSyntax(SimpleNameSyntax name, IEnumerable<AggregateEventPropertySyntax> properties)
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

            public SimpleNameSyntax Name { get { return _name; } }
            public IEnumerable<AggregateEventPropertySyntax> Properties { get { return _properties; } }
        }

        internal class AggregateEventPropertySyntax : ISymbol, IAstUnderlyingNode<SyntacticModel.AggregateEventPropertySyntax, SyntacticModel.AggregateEventSyntax>
        {
            private readonly SimpleNameSyntax _name;
            private readonly SimpleNameSyntax _type;

            public AggregateEventPropertySyntax(SimpleNameSyntax name, SimpleNameSyntax type)
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

            public SimpleNameSyntax Name { get { return _name; } }
            public SimpleNameSyntax Type { get { return _type; } }
        }

        internal class ValueSyntax : INamespaceMember, IType, IAstUnderlyingNode<SyntacticModel.ValueSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly SimpleNameSyntax _name;
            private readonly IEnumerable<ValuePropertySyntax> _properties;

            public ValueSyntax(SimpleNameSyntax name, IEnumerable<ValuePropertySyntax> properties)
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

            public SimpleNameSyntax Name { get { return _name; } }
            public IEnumerable<ValuePropertySyntax> Properties { get { return _properties; } }
        }

        internal class ValuePropertySyntax : ISymbol, IAstUnderlyingNode<SyntacticModel.ValuePropertySyntax, SyntacticModel.ValueSyntax>
        {
            private readonly SimpleNameSyntax _name;
            private readonly SimpleNameSyntax _type;

            public ValuePropertySyntax(SimpleNameSyntax name, SimpleNameSyntax type)
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

            public SimpleNameSyntax Name { get { return _name; } }
            public SimpleNameSyntax Type { get { return _type; } }
        }

        internal class UsingStatementSyntax : IAstUnderlyingNode<SyntacticModel.UsingStatementSyntax, Object>
        {
            private readonly NameSyntax _namespace;

            public UsingStatementSyntax(NameSyntax @namespace)
            {
                _namespace = @namespace;
            }

            public SyntacticModel.UsingStatementSyntax MakePublic(Object parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.UsingStatementSyntax BuildPublic(Object parent)
            {
                return new SyntacticModel.UsingStatementSyntax(this);
            }

            public NameSyntax Namespace { get { return _namespace; } }
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

