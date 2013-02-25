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
            return new SyntaxTree(null, new SyntacticModel.Internal.SyntaxTree(usingStatements.Select(x => ((IAstNode<SyntacticModel.Internal.UsingStatementSyntax>)x).GetUnderlying()), namespaces.Select(x => ((IAstNode<SyntacticModel.Internal.NamespaceSyntax>)x).GetUnderlying())));
        }

        public static NamespaceSyntax Namespace(NameSyntax name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
        {
            return new NamespaceSyntax(null, new SyntacticModel.Internal.NamespaceSyntax(((IAstNode<SyntacticModel.Internal.NameSyntax>)name).GetUnderlying(), usingStatements.Select(x => ((IAstNode<SyntacticModel.Internal.UsingStatementSyntax>)x).GetUnderlying()), members.Select(x => ((IAstNode<SyntacticModel.Internal.INamespaceMember>)x).GetUnderlying())));
        }


        public static NamedAggregateIdentifierSyntax NamedAggregateIdentifier(TypeReferenceSyntax type)
        {
            return new NamedAggregateIdentifierSyntax(null, new SyntacticModel.Internal.NamedAggregateIdentifierSyntax(type));
        }

        public static GenericAggregateIdentifierSyntax GenericAggregateIdentifier(IdentifierNameSyntax name, Maybe<TypeReferenceSyntax> constrainedType)
        {
            return new GenericAggregateIdentifierSyntax(null, new SyntacticModel.Internal.GenericAggregateIdentifierSyntax(((IAstNode<SyntacticModel.Internal.IdentifierNameSyntax>)name).GetUnderlying(), constrainedType));
        }

        public static EnumSyntax Enum(SimpleNameSyntax simpleName, IEnumerable<EnumValueSyntax> values)
        {
            return new EnumSyntax(null, new SyntacticModel.Internal.EnumSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), values.Select(x => ((IAstNode<SyntacticModel.Internal.EnumValueSyntax>)x).GetUnderlying())));
        }

        public static EnumValueSyntax EnumValue(IdentifierNameSyntax simpleName)
        {
            return new EnumValueSyntax(null, new SyntacticModel.Internal.EnumValueSyntax(((IAstNode<SyntacticModel.Internal.IdentifierNameSyntax>)simpleName).GetUnderlying()));
        }


        public static AtomSyntax Atom(SimpleNameSyntax simpleName, TypeReferenceSyntax type)
        {
            return new AtomSyntax(null, new SyntacticModel.Internal.AtomSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), type));
        }

        public static AggregateSyntax Aggregate(SimpleNameSyntax simpleName, Maybe<AggregateIdentifierSyntax> identifier, Maybe<NameSyntax> @base, IEnumerable<AggregateEventSyntax> events)
        {
            return new AggregateSyntax(null, new SyntacticModel.Internal.AggregateSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), identifier.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateIdentifierSyntax>)x).GetUnderlying()), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), events.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateEventSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventSyntax AggregateEvent(Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms)
        {
            return new AggregateEventSyntax(null, new SyntacticModel.Internal.AggregateEventSyntax(isAbstract, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), atoms.Select(x => ((IAstNode<SyntacticModel.Internal.AtomSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventPropertySyntax AggregateEventProperty(SimpleNameSyntax simpleName, TypeReferenceSyntax type)
        {
            return new AggregateEventPropertySyntax(null, new SyntacticModel.Internal.AggregateEventPropertySyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), type));
        }

        public static ValueSyntax Value(Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms)
        {
            return new ValueSyntax(null, new SyntacticModel.Internal.ValueSyntax(isAbstract, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), atoms.Select(x => ((IAstNode<SyntacticModel.Internal.AtomSyntax>)x).GetUnderlying())));
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

    public interface INode : IVisitable
    {
        INode Parent { get; }
    }

    public interface INamespaceParent : INode
    {
    }

    public interface INamespaceMember : INode
    {
    }

    public interface IUsingsContainer : IVisitable
    {
        IEnumerable<UsingStatementSyntax> UsingStatements { get; }
    }

    public interface ISymbol : INode
    {
        NameSyntax Name { get; }
        NameSyntax FullName { get; }
    }

    public interface IType : ISymbol
    {
        SimpleNameSyntax SimpleName { get; }
        Boolean IsValueType { get; }
        Boolean HasValueSemantics { get; }
    }

    public class SyntaxTree : INamespaceParent, IUsingsContainer, IAstNode<Internal.SyntaxTree>
    {
        private readonly INode _parent;
        private readonly Internal.SyntaxTree _underlying;

        internal SyntaxTree(INode parent, Internal.SyntaxTree underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        INode INode.Parent { get { return _parent; } }
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
        INode INode.Parent { get { return _parent; } }
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

    public abstract class AggregateIdentifierSyntax : IAstNode<Internal.AggregateIdentifierSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateIdentifierSyntax _underlying;

        internal AggregateIdentifierSyntax(AggregateSyntax parent, Internal.AggregateIdentifierSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateSyntax Parent { get { return _parent; } }
        Internal.AggregateIdentifierSyntax IAstNode<Internal.AggregateIdentifierSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

    }

    public class NamedAggregateIdentifierSyntax : AggregateIdentifierSyntax, IAstNode<Internal.NamedAggregateIdentifierSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.NamedAggregateIdentifierSyntax _underlying;

        internal NamedAggregateIdentifierSyntax(AggregateSyntax parent, Internal.NamedAggregateIdentifierSyntax underlying)
            : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        Internal.NamedAggregateIdentifierSyntax IAstNode<Internal.NamedAggregateIdentifierSyntax>.GetUnderlying() { return _underlying; }

        public TypeReferenceSyntax Type
        {
            get
            {
                return ((IAstNode<Internal.NamedAggregateIdentifierSyntax>)this).GetUnderlying().Type;
            }
        }
    }

    public class GenericAggregateIdentifierSyntax : AggregateIdentifierSyntax, IAstNode<Internal.GenericAggregateIdentifierSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.GenericAggregateIdentifierSyntax _underlying;

        internal GenericAggregateIdentifierSyntax(AggregateSyntax parent, Internal.GenericAggregateIdentifierSyntax underlying)
            : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        Internal.GenericAggregateIdentifierSyntax IAstNode<Internal.GenericAggregateIdentifierSyntax>.GetUnderlying() { return _underlying; }

        public override void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { Name });
            base.AcceptChildren(dispatch);
        }

        public IdentifierNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<IdentifierNameSyntax, GenericAggregateIdentifierSyntax>)((IAstNode<Internal.GenericAggregateIdentifierSyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public Maybe<TypeReferenceSyntax> ConstrainedType
        {
            get
            {
                return ((IAstNode<Internal.GenericAggregateIdentifierSyntax>)this).GetUnderlying().ConstrainedType;
            }
        }
    }

    public partial class EnumSyntax : INamespaceMember, IType, IAstNode<Internal.EnumSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.EnumSyntax _underlying;

        internal EnumSyntax(NamespaceSyntax parent, Internal.EnumSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return _parent; } }
        Internal.EnumSyntax IAstNode<Internal.EnumSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Values);
        }

        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, EnumSyntax>)((IAstNode<Internal.EnumSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public IEnumerable<EnumValueSyntax> Values
        {
            get
            {
                return ((IAstNode<Internal.EnumSyntax>)this).GetUnderlying().Values.Select(x => ((IAstUnderlyingNode<EnumValueSyntax, EnumSyntax>)x).MakePublic(this));
            }
        }
    }

    public class EnumValueSyntax : IAstNode<Internal.EnumValueSyntax>
    {
        private readonly EnumSyntax _parent;
        private readonly Internal.EnumValueSyntax _underlying;

        internal EnumValueSyntax(EnumSyntax parent, Internal.EnumValueSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public EnumSyntax Parent { get { return _parent; } }
        Internal.EnumValueSyntax IAstNode<Internal.EnumValueSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
        }

        public IdentifierNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<IdentifierNameSyntax, EnumValueSyntax>)((IAstNode<Internal.EnumValueSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
    }

    public abstract partial class MoleculeSyntax : IType, IAstNode<Internal.MoleculeSyntax>
    {
        private readonly ISymbol _parent;
        private readonly Internal.MoleculeSyntax _underlying;

        internal MoleculeSyntax(ISymbol parent, Internal.MoleculeSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public ISymbol Parent { get { return _parent; } }
        INode INode.Parent { get { return _parent; } }
        Internal.MoleculeSyntax IAstNode<Internal.MoleculeSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Base.ToEnumerable());
            dispatch(Atoms);
        }

        public Boolean IsAbstract
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().IsAbstract;
            }
        }
        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, MoleculeSyntax>)((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public Maybe<NameSyntax> Base
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().Base.Select(x => ((IAstUnderlyingNode<NameSyntax, MoleculeSyntax>)x).MakePublic(this));
            }
        }
        public IEnumerable<AtomSyntax> Atoms
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().Atoms.Select(x => ((IAstUnderlyingNode<AtomSyntax, MoleculeSyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class AtomSyntax : ISymbol, IAstNode<Internal.AtomSyntax>
    {
        private readonly MoleculeSyntax _parent;
        private readonly Internal.AtomSyntax _underlying;

        internal AtomSyntax(MoleculeSyntax parent, Internal.AtomSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public MoleculeSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return _parent; } }
        Internal.AtomSyntax IAstNode<Internal.AtomSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
        }

        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AtomSyntax>)((IAstNode<Internal.AtomSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public TypeReferenceSyntax Type
        {
            get
            {
                return ((IAstNode<Internal.AtomSyntax>)this).GetUnderlying().Type;
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
        INode INode.Parent { get { return _parent; } }
        Internal.AggregateSyntax IAstNode<Internal.AggregateSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Identifier.ToEnumerable());
            dispatch(Base.ToEnumerable());
            dispatch(Events);
        }

        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateSyntax>)((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public Maybe<AggregateIdentifierSyntax> Identifier
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Identifier.Select(x => ((IAstUnderlyingNode<AggregateIdentifierSyntax, AggregateSyntax>)x).MakePublic(this));
            }
        }
        public Maybe<NameSyntax> Base
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Base.Select(x => ((IAstUnderlyingNode<NameSyntax, AggregateSyntax>)x).MakePublic(this));
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

    public partial class AggregateEventSyntax : MoleculeSyntax, IAstNode<Internal.AggregateEventSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateEventSyntax _underlying;

        internal AggregateEventSyntax(AggregateSyntax parent, Internal.AggregateEventSyntax underlying)
            : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public new AggregateSyntax Parent { get { return _parent; } }
        Internal.AggregateEventSyntax IAstNode<Internal.AggregateEventSyntax>.GetUnderlying() { return _underlying; }

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
        INode INode.Parent { get { return _parent; } }
        Internal.AggregateEventPropertySyntax IAstNode<Internal.AggregateEventPropertySyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(new[] { SimpleName });
        }

        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, AggregateEventPropertySyntax>)((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public TypeReferenceSyntax Type
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Type;
            }
        }
    }

    public partial class ValueSyntax : MoleculeSyntax, INamespaceMember, IAstNode<Internal.ValueSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.ValueSyntax _underlying;

        internal ValueSyntax(NamespaceSyntax parent, Internal.ValueSyntax underlying)
            : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public new NamespaceSyntax Parent { get { return _parent; } }
        Internal.ValueSyntax IAstNode<Internal.ValueSyntax>.GetUnderlying() { return _underlying; }

    }

    public class UsingStatementSyntax : IAstNode<Internal.UsingStatementSyntax>
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
        internal interface INode
        {
        }

        internal interface INamespaceParent : INode
        {
        }

        internal interface INamespaceMember : INode
        {
        }

        internal interface IUsingsContainer
        {
        }

        internal interface ISymbol : INode
        {
        }

        internal interface IType : ISymbol
        {
        }

        internal class SyntaxTree : INamespaceParent, IUsingsContainer, IAstUnderlyingNode<SyntacticModel.SyntaxTree, SyntacticModel.INode>
        {
            private readonly IEnumerable<UsingStatementSyntax> _usingStatements;
            private readonly IEnumerable<NamespaceSyntax> _namespaces;

            public SyntaxTree(IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces)
            {
                _usingStatements = usingStatements;
                _namespaces = namespaces;
            }

            public SyntacticModel.SyntaxTree MakePublic(SyntacticModel.INode parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.SyntaxTree BuildPublic(SyntacticModel.INode parent)
            {
                return new SyntacticModel.SyntaxTree(parent, this);
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

        internal abstract class AggregateIdentifierSyntax : IAstUnderlyingNode<SyntacticModel.AggregateIdentifierSyntax, SyntacticModel.AggregateSyntax>
        {

            public SyntacticModel.AggregateIdentifierSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.AggregateIdentifierSyntax BuildPublic(SyntacticModel.AggregateSyntax parent);
        }

        internal class NamedAggregateIdentifierSyntax : AggregateIdentifierSyntax, IAstUnderlyingNode<SyntacticModel.NamedAggregateIdentifierSyntax, SyntacticModel.AggregateSyntax>
        {
            private readonly TypeReferenceSyntax _type;

            public NamedAggregateIdentifierSyntax(TypeReferenceSyntax type)
            {
                _type = type;
            }

            public new SyntacticModel.NamedAggregateIdentifierSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return (SyntacticModel.NamedAggregateIdentifierSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.AggregateIdentifierSyntax BuildPublic(SyntacticModel.AggregateSyntax parent)
            {
                return new SyntacticModel.NamedAggregateIdentifierSyntax(parent, this);
            }

            public TypeReferenceSyntax Type { get { return _type; } }
        }

        internal class GenericAggregateIdentifierSyntax : AggregateIdentifierSyntax, IAstUnderlyingNode<SyntacticModel.GenericAggregateIdentifierSyntax, SyntacticModel.AggregateSyntax>
        {
            private readonly IdentifierNameSyntax _name;
            private readonly Maybe<TypeReferenceSyntax> _constrainedType;

            public GenericAggregateIdentifierSyntax(IdentifierNameSyntax name, Maybe<TypeReferenceSyntax> constrainedType)
            {
                _name = name;
                _constrainedType = constrainedType;
            }

            public new SyntacticModel.GenericAggregateIdentifierSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return (SyntacticModel.GenericAggregateIdentifierSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.AggregateIdentifierSyntax BuildPublic(SyntacticModel.AggregateSyntax parent)
            {
                return new SyntacticModel.GenericAggregateIdentifierSyntax(parent, this);
            }

            public IdentifierNameSyntax Name { get { return _name; } }
            public Maybe<TypeReferenceSyntax> ConstrainedType { get { return _constrainedType; } }
        }

        internal class EnumSyntax : INamespaceMember, IType, IAstUnderlyingNode<SyntacticModel.EnumSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly IEnumerable<EnumValueSyntax> _values;

            public EnumSyntax(SimpleNameSyntax simpleName, IEnumerable<EnumValueSyntax> values)
            {
                _simpleName = simpleName;
                _values = values;
            }

            public SyntacticModel.EnumSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.EnumSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.EnumSyntax(parent, this);
            }

            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public IEnumerable<EnumValueSyntax> Values { get { return _values; } }
        }

        internal class EnumValueSyntax : IAstUnderlyingNode<SyntacticModel.EnumValueSyntax, SyntacticModel.EnumSyntax>
        {
            private readonly IdentifierNameSyntax _simpleName;

            public EnumValueSyntax(IdentifierNameSyntax simpleName)
            {
                _simpleName = simpleName;
            }

            public SyntacticModel.EnumValueSyntax MakePublic(SyntacticModel.EnumSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.EnumValueSyntax BuildPublic(SyntacticModel.EnumSyntax parent)
            {
                return new SyntacticModel.EnumValueSyntax(parent, this);
            }

            public IdentifierNameSyntax SimpleName { get { return _simpleName; } }
        }

        internal abstract class MoleculeSyntax : IType, IAstUnderlyingNode<SyntacticModel.MoleculeSyntax, SyntacticModel.ISymbol>
        {
            private readonly Boolean _isAbstract;
            private readonly SimpleNameSyntax _simpleName;
            private readonly Maybe<NameSyntax> _base;
            private readonly IEnumerable<AtomSyntax> _atoms;

            protected MoleculeSyntax(Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms)
            {
                _isAbstract = isAbstract;
                _simpleName = simpleName;
                _base = @base;
                _atoms = atoms;
            }

            public SyntacticModel.MoleculeSyntax MakePublic(SyntacticModel.ISymbol parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent);
            public Boolean IsAbstract { get { return _isAbstract; } }
            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public Maybe<NameSyntax> Base { get { return _base; } }
            public IEnumerable<AtomSyntax> Atoms { get { return _atoms; } }
        }

        internal class AtomSyntax : ISymbol, IAstUnderlyingNode<SyntacticModel.AtomSyntax, SyntacticModel.MoleculeSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly TypeReferenceSyntax _type;

            public AtomSyntax(SimpleNameSyntax simpleName, TypeReferenceSyntax type)
            {
                _simpleName = simpleName;
                _type = type;
            }

            public SyntacticModel.AtomSyntax MakePublic(SyntacticModel.MoleculeSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AtomSyntax BuildPublic(SyntacticModel.MoleculeSyntax parent)
            {
                return new SyntacticModel.AtomSyntax(parent, this);
            }

            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public TypeReferenceSyntax Type { get { return _type; } }
        }

        internal class AggregateSyntax : INamespaceMember, IType, IAstUnderlyingNode<SyntacticModel.AggregateSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly Maybe<AggregateIdentifierSyntax> _identifier;
            private readonly Maybe<NameSyntax> _base;
            private readonly IEnumerable<AggregateEventSyntax> _events;

            public AggregateSyntax(SimpleNameSyntax simpleName, Maybe<AggregateIdentifierSyntax> identifier, Maybe<NameSyntax> @base, IEnumerable<AggregateEventSyntax> events)
            {
                _simpleName = simpleName;
                _identifier = identifier;
                _base = @base;
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

            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public Maybe<AggregateIdentifierSyntax> Identifier { get { return _identifier; } }
            public Maybe<NameSyntax> Base { get { return _base; } }
            public IEnumerable<AggregateEventSyntax> Events { get { return _events; } }
        }

        internal class AggregateEventSyntax : MoleculeSyntax, IAstUnderlyingNode<SyntacticModel.AggregateEventSyntax, SyntacticModel.AggregateSyntax>
        {

            public AggregateEventSyntax(Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms)
                : base(isAbstract, simpleName, @base, atoms)
            {
            }

            public new SyntacticModel.AggregateEventSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return (SyntacticModel.AggregateEventSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent)
            {
                return new SyntacticModel.AggregateEventSyntax((SyntacticModel.AggregateSyntax)parent, this);
            }

        }

        internal class AggregateEventPropertySyntax : ISymbol, IAstUnderlyingNode<SyntacticModel.AggregateEventPropertySyntax, SyntacticModel.AggregateEventSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly TypeReferenceSyntax _type;

            public AggregateEventPropertySyntax(SimpleNameSyntax simpleName, TypeReferenceSyntax type)
            {
                _simpleName = simpleName;
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

            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public TypeReferenceSyntax Type { get { return _type; } }
        }

        internal class ValueSyntax : MoleculeSyntax, INamespaceMember, IAstUnderlyingNode<SyntacticModel.ValueSyntax, SyntacticModel.NamespaceSyntax>
        {

            public ValueSyntax(Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms)
                : base(isAbstract, simpleName, @base, atoms)
            {
            }

            public new SyntacticModel.ValueSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return (SyntacticModel.ValueSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent)
            {
                return new SyntacticModel.ValueSyntax((SyntacticModel.NamespaceSyntax)parent, this);
            }

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

