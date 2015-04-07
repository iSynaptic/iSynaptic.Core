using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.Domain
{
    using SyntacticModel;

    public static class Syntax
    {









        public static AnnotationSyntax Annotation(IdentifierNameSyntax name, IEnumerable<AnnotationPairSyntax> pairs)
        {
            return new AnnotationSyntax(null, new SyntacticModel.Internal.AnnotationSyntax(((IAstNode<SyntacticModel.Internal.IdentifierNameSyntax>)name).GetUnderlying(), pairs.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationPairSyntax>)x).GetUnderlying())));
        }

        public static AnnotationPairSyntax AnnotationPair(IdentifierNameSyntax name, String value)
        {
            return new AnnotationPairSyntax(null, new SyntacticModel.Internal.AnnotationPairSyntax(((IAstNode<SyntacticModel.Internal.IdentifierNameSyntax>)name).GetUnderlying(), value));
        }

        public static Compilation Compilation(IEnumerable<SyntaxTree> trees)
        {
            return new Compilation(null, new SyntacticModel.Internal.Compilation(trees.Select(x => ((IAstNode<SyntacticModel.Internal.SyntaxTree>)x).GetUnderlying())));
        }

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

        public static EnumSyntax Enum(Boolean isExternal, SimpleNameSyntax simpleName, IEnumerable<EnumValueSyntax> values, IEnumerable<AnnotationSyntax> annotations)
        {
            return new EnumSyntax(null, new SyntacticModel.Internal.EnumSyntax(isExternal, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), values.Select(x => ((IAstNode<SyntacticModel.Internal.EnumValueSyntax>)x).GetUnderlying()), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static EnumValueSyntax EnumValue(IdentifierNameSyntax simpleName)
        {
            return new EnumValueSyntax(null, new SyntacticModel.Internal.EnumValueSyntax(((IAstNode<SyntacticModel.Internal.IdentifierNameSyntax>)simpleName).GetUnderlying()));
        }


        public static AtomSyntax Atom(SimpleNameSyntax simpleName, TypeReferenceSyntax type, IEnumerable<AnnotationSyntax> annotations)
        {
            return new AtomSyntax(null, new SyntacticModel.Internal.AtomSyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), type, annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static AggregateSyntax Aggregate(Boolean isExternal, Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<AggregateIdentifierSyntax> identifier, Maybe<NameSyntax> @base, IEnumerable<IAggregateMember> members, IEnumerable<AnnotationSyntax> annotations)
        {
            return new AggregateSyntax(null, new SyntacticModel.Internal.AggregateSyntax(isExternal, isAbstract, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), identifier.Select(x => ((IAstNode<SyntacticModel.Internal.AggregateIdentifierSyntax>)x).GetUnderlying()), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), members.Select(x => ((IAstNode<SyntacticModel.Internal.IAggregateMember>)x).GetUnderlying()), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventSyntax AggregateEvent(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations)
        {
            return new AggregateEventSyntax(null, new SyntacticModel.Internal.AggregateEventSyntax(isExternal, isAbstract, isPartial, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), atoms.Select(x => ((IAstNode<SyntacticModel.Internal.AtomSyntax>)x).GetUnderlying()), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static AggregateSnapshotSyntax AggregateSnapshot(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations)
        {
            return new AggregateSnapshotSyntax(null, new SyntacticModel.Internal.AggregateSnapshotSyntax(isExternal, isAbstract, isPartial, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), atoms.Select(x => ((IAstNode<SyntacticModel.Internal.AtomSyntax>)x).GetUnderlying()), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static AggregateEventPropertySyntax AggregateEventProperty(SimpleNameSyntax simpleName, TypeReferenceSyntax type, IEnumerable<AnnotationSyntax> annotations)
        {
            return new AggregateEventPropertySyntax(null, new SyntacticModel.Internal.AggregateEventPropertySyntax(((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), type, annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static ScalarValueSyntax ScalarValue(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, NameSyntax @base, IEnumerable<AnnotationSyntax> annotations)
        {
            return new ScalarValueSyntax(null, new SyntacticModel.Internal.ScalarValueSyntax(isExternal, isAbstract, isPartial, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), ((IAstNode<SyntacticModel.Internal.NameSyntax>)@base).GetUnderlying(), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
        }

        public static ValueSyntax Value(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations)
        {
            return new ValueSyntax(null, new SyntacticModel.Internal.ValueSyntax(isExternal, isAbstract, isPartial, ((IAstNode<SyntacticModel.Internal.SimpleNameSyntax>)simpleName).GetUnderlying(), @base.Select(x => ((IAstNode<SyntacticModel.Internal.NameSyntax>)x).GetUnderlying()), atoms.Select(x => ((IAstNode<SyntacticModel.Internal.AtomSyntax>)x).GetUnderlying()), annotations.Select(x => ((IAstNode<SyntacticModel.Internal.AnnotationSyntax>)x).GetUnderlying())));
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
    internal interface IAstNode<out T> : IVisitableChildren { T GetUnderlying(); }

    internal interface IAstUnderlyingNode<out T, in TParent> { T MakePublic(TParent parent); }

    public interface INode : IVisitableChildren
    {
        INode Parent { get; }
    }

    public interface IAnnotatableNode : IVisitableChildren
    {
        IEnumerable<AnnotationSyntax> Annotations { get; }
        INode Parent { get; }
    }

    public interface INamespaceParent : INode
    {
    }

    public interface INamespaceMember : INode
    {
    }

    public interface IUsingsContainer : IVisitableChildren
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
        Boolean IsExternal { get; }
        SimpleNameSyntax SimpleName { get; }
        Boolean IsValueType { get; }
        Boolean HasValueSemantics { get; }
    }

    public interface ITypeWithBase : IType
    {
        Maybe<NameSyntax> Base { get; }
    }

    public interface IAggregateMember : INode
    {
    }

    public class AnnotationSyntax : INode, IAstNode<Internal.AnnotationSyntax>
    {
        private readonly INode _parent;
        private readonly Internal.AnnotationSyntax _underlying;

        internal AnnotationSyntax(INode parent, Internal.AnnotationSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public INode Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        Internal.AnnotationSyntax IAstNode<Internal.AnnotationSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { Name });
            dispatch(Pairs);
        }

        public IdentifierNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<IdentifierNameSyntax, AnnotationSyntax>)((IAstNode<Internal.AnnotationSyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public IEnumerable<AnnotationPairSyntax> Pairs
        {
            get
            {
                return ((IAstNode<Internal.AnnotationSyntax>)this).GetUnderlying().Pairs.Select(x => ((IAstUnderlyingNode<AnnotationPairSyntax, AnnotationSyntax>)x).MakePublic(this));
            }
        }
    }

    public class AnnotationPairSyntax : INode, IAstNode<Internal.AnnotationPairSyntax>
    {
        private readonly AnnotationSyntax _parent;
        private readonly Internal.AnnotationPairSyntax _underlying;

        internal AnnotationPairSyntax(AnnotationSyntax parent, Internal.AnnotationPairSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AnnotationSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        Internal.AnnotationPairSyntax IAstNode<Internal.AnnotationPairSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { Name });
        }

        public IdentifierNameSyntax Name
        {
            get
            {
                return ((IAstUnderlyingNode<IdentifierNameSyntax, AnnotationPairSyntax>)((IAstNode<Internal.AnnotationPairSyntax>)this).GetUnderlying().Name).MakePublic(this);
            }
        }
        public String Value
        {
            get
            {
                return ((IAstNode<Internal.AnnotationPairSyntax>)this).GetUnderlying().Value;
            }
        }
    }

    public class Compilation : INode, IAstNode<Internal.Compilation>
    {
        private readonly INode _parent;
        private readonly Internal.Compilation _underlying;

        internal Compilation(INode parent, Internal.Compilation underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public INode Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        Internal.Compilation IAstNode<Internal.Compilation>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(Trees);
        }

        public IEnumerable<SyntaxTree> Trees
        {
            get
            {
                return ((IAstNode<Internal.Compilation>)this).GetUnderlying().Trees.Select(x => ((IAstUnderlyingNode<SyntaxTree, Compilation>)x).MakePublic(this));
            }
        }
    }

    public class SyntaxTree : INamespaceParent, IUsingsContainer, IAstNode<Internal.SyntaxTree>
    {
        private readonly Compilation _parent;
        private readonly Internal.SyntaxTree _underlying;

        internal SyntaxTree(Compilation parent, Internal.SyntaxTree underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public Compilation Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        Internal.SyntaxTree IAstNode<Internal.SyntaxTree>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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
        INode INode.Parent { get { return Parent; } }
        Internal.NamespaceSyntax IAstNode<Internal.NamespaceSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
        }

    }

    public class NamedAggregateIdentifierSyntax : AggregateIdentifierSyntax, IAstNode<Internal.NamedAggregateIdentifierSyntax>
    {
        private readonly Internal.NamedAggregateIdentifierSyntax _underlying;

        internal NamedAggregateIdentifierSyntax(AggregateSyntax parent, Internal.NamedAggregateIdentifierSyntax underlying) : base(parent, underlying)
        {
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
        private readonly Internal.GenericAggregateIdentifierSyntax _underlying;

        internal GenericAggregateIdentifierSyntax(AggregateSyntax parent, Internal.GenericAggregateIdentifierSyntax underlying) : base(parent, underlying)
        {
            _underlying = underlying;
        }

        Internal.GenericAggregateIdentifierSyntax IAstNode<Internal.GenericAggregateIdentifierSyntax>.GetUnderlying() { return _underlying; }

        public override void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

    public partial class EnumSyntax : INamespaceMember, IType, IAnnotatableNode, IAstNode<Internal.EnumSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.EnumSyntax _underlying;

        internal EnumSyntax(NamespaceSyntax parent, Internal.EnumSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.EnumSyntax IAstNode<Internal.EnumSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Values);
            dispatch(Annotations);
        }

        public Boolean IsExternal
        {
            get
            {
                return ((IAstNode<Internal.EnumSyntax>)this).GetUnderlying().IsExternal;
            }
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
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.EnumSyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, EnumSyntax>)x).MakePublic(this));
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

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

    public abstract partial class MoleculeSyntax : ITypeWithBase, IAnnotatableNode, IAstNode<Internal.MoleculeSyntax>
    {
        private readonly ISymbol _parent;
        private readonly Internal.MoleculeSyntax _underlying;

        internal MoleculeSyntax(ISymbol parent, Internal.MoleculeSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public ISymbol Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.MoleculeSyntax IAstNode<Internal.MoleculeSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Base.ToEnumerable());
            dispatch(Atoms);
            dispatch(Annotations);
        }

        public Boolean IsExternal
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().IsExternal;
            }
        }
        public Boolean IsAbstract
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().IsAbstract;
            }
        }
        public Boolean IsPartial
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().IsPartial;
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
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.MoleculeSyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, MoleculeSyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class AtomSyntax : ISymbol, IAnnotatableNode, IAstNode<Internal.AtomSyntax>
    {
        private readonly MoleculeSyntax _parent;
        private readonly Internal.AtomSyntax _underlying;

        internal AtomSyntax(MoleculeSyntax parent, Internal.AtomSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public MoleculeSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.AtomSyntax IAstNode<Internal.AtomSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Annotations);
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
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.AtomSyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, AtomSyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class AggregateSyntax : INamespaceMember, ITypeWithBase, IAnnotatableNode, IAstNode<Internal.AggregateSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.AggregateSyntax _underlying;

        internal AggregateSyntax(NamespaceSyntax parent, Internal.AggregateSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.AggregateSyntax IAstNode<Internal.AggregateSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Identifier.ToEnumerable());
            dispatch(Base.ToEnumerable());
            dispatch(Members);
            dispatch(Annotations);
        }

        public Boolean IsExternal
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().IsExternal;
            }
        }
        public Boolean IsAbstract
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().IsAbstract;
            }
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
        public IEnumerable<IAggregateMember> Members
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Members.Select(x => ((IAstUnderlyingNode<IAggregateMember, AggregateSyntax>)x).MakePublic(this));
            }
        }
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.AggregateSyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, AggregateSyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class AggregateEventSyntax : MoleculeSyntax, IAggregateMember, IAstNode<Internal.AggregateEventSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateEventSyntax _underlying;

        internal AggregateEventSyntax(AggregateSyntax parent, Internal.AggregateEventSyntax underlying) : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public new AggregateSyntax Parent { get { return _parent; } }
        Internal.AggregateEventSyntax IAstNode<Internal.AggregateEventSyntax>.GetUnderlying() { return _underlying; }

    }

    public partial class AggregateSnapshotSyntax : MoleculeSyntax, IAggregateMember, IAstNode<Internal.AggregateSnapshotSyntax>
    {
        private readonly AggregateSyntax _parent;
        private readonly Internal.AggregateSnapshotSyntax _underlying;

        internal AggregateSnapshotSyntax(AggregateSyntax parent, Internal.AggregateSnapshotSyntax underlying) : base(parent, underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public new AggregateSyntax Parent { get { return _parent; } }
        Internal.AggregateSnapshotSyntax IAstNode<Internal.AggregateSnapshotSyntax>.GetUnderlying() { return _underlying; }

    }

    public partial class AggregateEventPropertySyntax : ISymbol, IAnnotatableNode, IAstNode<Internal.AggregateEventPropertySyntax>
    {
        private readonly AggregateEventSyntax _parent;
        private readonly Internal.AggregateEventPropertySyntax _underlying;

        internal AggregateEventPropertySyntax(AggregateEventSyntax parent, Internal.AggregateEventPropertySyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AggregateEventSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.AggregateEventPropertySyntax IAstNode<Internal.AggregateEventPropertySyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(Annotations);
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
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.AggregateEventPropertySyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, AggregateEventPropertySyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class ScalarValueSyntax : INamespaceMember, ITypeWithBase, IAnnotatableNode, IAstNode<Internal.ScalarValueSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.ScalarValueSyntax _underlying;

        internal ScalarValueSyntax(NamespaceSyntax parent, Internal.ScalarValueSyntax underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public NamespaceSyntax Parent { get { return _parent; } }
        INode INode.Parent { get { return Parent; } }
        INode IAnnotatableNode.Parent { get { return Parent; } }
        Internal.ScalarValueSyntax IAstNode<Internal.ScalarValueSyntax>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
            dispatch(new[] { SimpleName });
            dispatch(new[] { Base });
            dispatch(Annotations);
        }

        public Boolean IsExternal
        {
            get
            {
                return ((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().IsExternal;
            }
        }
        public Boolean IsAbstract
        {
            get
            {
                return ((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().IsAbstract;
            }
        }
        public Boolean IsPartial
        {
            get
            {
                return ((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().IsPartial;
            }
        }
        public SimpleNameSyntax SimpleName
        {
            get
            {
                return ((IAstUnderlyingNode<SimpleNameSyntax, ScalarValueSyntax>)((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().SimpleName).MakePublic(this);
            }
        }
        public NameSyntax Base
        {
            get
            {
                return ((IAstUnderlyingNode<NameSyntax, ScalarValueSyntax>)((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().Base).MakePublic(this);
            }
        }
        public IEnumerable<AnnotationSyntax> Annotations
        {
            get
            {
                return ((IAstNode<Internal.ScalarValueSyntax>)this).GetUnderlying().Annotations.Select(x => ((IAstUnderlyingNode<AnnotationSyntax, ScalarValueSyntax>)x).MakePublic(this));
            }
        }
    }

    public partial class ValueSyntax : MoleculeSyntax, INamespaceMember, IAstNode<Internal.ValueSyntax>
    {
        private readonly NamespaceSyntax _parent;
        private readonly Internal.ValueSyntax _underlying;

        internal ValueSyntax(NamespaceSyntax parent, Internal.ValueSyntax underlying) : base(parent, underlying)
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

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

        public virtual void AcceptChildren(Action<IEnumerable<Object>> dispatch)
        {
        }

    }

    public abstract partial class SimpleNameSyntax : NameSyntax, IAstNode<Internal.SimpleNameSyntax>
    {
        private readonly Internal.SimpleNameSyntax _underlying;

        internal SimpleNameSyntax(Internal.SimpleNameSyntax underlying) : base(underlying)
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

        internal IdentifierNameSyntax(Internal.IdentifierNameSyntax underlying) : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.IdentifierNameSyntax IAstNode<Internal.IdentifierNameSyntax>.GetUnderlying() { return _underlying; }

    }

    public partial class GenericNameSyntax : SimpleNameSyntax, IAstNode<Internal.GenericNameSyntax>
    {
        private readonly Internal.GenericNameSyntax _underlying;

        internal GenericNameSyntax(Internal.GenericNameSyntax underlying) : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.GenericNameSyntax IAstNode<Internal.GenericNameSyntax>.GetUnderlying() { return _underlying; }

        public override void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

        internal QualifiedNameSyntax(Internal.QualifiedNameSyntax underlying) : base(underlying)
        {
            _underlying = underlying;
        }

        Internal.QualifiedNameSyntax IAstNode<Internal.QualifiedNameSyntax>.GetUnderlying() { return _underlying; }

        public override void AcceptChildren(Action<IEnumerable<Object>> dispatch)
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

        internal interface IAnnotatableNode
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

        internal interface ITypeWithBase : IType
        {
        }

        internal interface IAggregateMember : INode
        {
        }

        internal class AnnotationSyntax : INode, IAstUnderlyingNode<SyntacticModel.AnnotationSyntax, SyntacticModel.INode>
        {
            private readonly IdentifierNameSyntax _name;
            private readonly AnnotationPairSyntax[] _pairs;

            public AnnotationSyntax(IdentifierNameSyntax name, IEnumerable<AnnotationPairSyntax> pairs)
            {
                _name = name;
                _pairs = pairs.ToArray();
            }

            public SyntacticModel.AnnotationSyntax MakePublic(SyntacticModel.INode parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AnnotationSyntax BuildPublic(SyntacticModel.INode parent)
            {
                return new SyntacticModel.AnnotationSyntax(parent, this);
            }

            public IdentifierNameSyntax Name { get { return _name; } }
            public IEnumerable<AnnotationPairSyntax> Pairs { get { return _pairs; } }
        }

        internal class AnnotationPairSyntax : INode, IAstUnderlyingNode<SyntacticModel.AnnotationPairSyntax, SyntacticModel.AnnotationSyntax>
        {
            private readonly IdentifierNameSyntax _name;
            private readonly String _value;

            public AnnotationPairSyntax(IdentifierNameSyntax name, String value)
            {
                _name = name;
                _value = value;
            }

            public SyntacticModel.AnnotationPairSyntax MakePublic(SyntacticModel.AnnotationSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AnnotationPairSyntax BuildPublic(SyntacticModel.AnnotationSyntax parent)
            {
                return new SyntacticModel.AnnotationPairSyntax(parent, this);
            }

            public IdentifierNameSyntax Name { get { return _name; } }
            public String Value { get { return _value; } }
        }

        internal class Compilation : INode, IAstUnderlyingNode<SyntacticModel.Compilation, SyntacticModel.INode>
        {
            private readonly SyntaxTree[] _trees;

            public Compilation(IEnumerable<SyntaxTree> trees)
            {
                _trees = trees.ToArray();
            }

            public SyntacticModel.Compilation MakePublic(SyntacticModel.INode parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.Compilation BuildPublic(SyntacticModel.INode parent)
            {
                return new SyntacticModel.Compilation(parent, this);
            }

            public IEnumerable<SyntaxTree> Trees { get { return _trees; } }
        }

        internal class SyntaxTree : INamespaceParent, IUsingsContainer, IAstUnderlyingNode<SyntacticModel.SyntaxTree, SyntacticModel.Compilation>
        {
            private readonly UsingStatementSyntax[] _usingStatements;
            private readonly NamespaceSyntax[] _namespaces;

            public SyntaxTree(IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<NamespaceSyntax> namespaces)
            {
                _usingStatements = usingStatements.ToArray();
                _namespaces = namespaces.ToArray();
            }

            public SyntacticModel.SyntaxTree MakePublic(SyntacticModel.Compilation parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.SyntaxTree BuildPublic(SyntacticModel.Compilation parent)
            {
                return new SyntacticModel.SyntaxTree(parent, this);
            }

            public IEnumerable<UsingStatementSyntax> UsingStatements { get { return _usingStatements; } }
            public IEnumerable<NamespaceSyntax> Namespaces { get { return _namespaces; } }
        }

        internal class NamespaceSyntax : INamespaceParent, INamespaceMember, IUsingsContainer, ISymbol, IAstUnderlyingNode<SyntacticModel.NamespaceSyntax, SyntacticModel.INamespaceParent>
        {
            private readonly NameSyntax _name;
            private readonly UsingStatementSyntax[] _usingStatements;
            private readonly INamespaceMember[] _members;

            public NamespaceSyntax(NameSyntax name, IEnumerable<UsingStatementSyntax> usingStatements, IEnumerable<INamespaceMember> members)
            {
                _name = name;
                _usingStatements = usingStatements.ToArray();
                _members = members.ToArray();
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

        internal class EnumSyntax : INamespaceMember, IType, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.EnumSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly Boolean _isExternal;
            private readonly SimpleNameSyntax _simpleName;
            private readonly EnumValueSyntax[] _values;
            private readonly AnnotationSyntax[] _annotations;

            public EnumSyntax(Boolean isExternal, SimpleNameSyntax simpleName, IEnumerable<EnumValueSyntax> values, IEnumerable<AnnotationSyntax> annotations)
            {
                _isExternal = isExternal;
                _simpleName = simpleName;
                _values = values.ToArray();
                _annotations = annotations.ToArray();
            }

            public SyntacticModel.EnumSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.EnumSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.EnumSyntax(parent, this);
            }

            public Boolean IsExternal { get { return _isExternal; } }
            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public IEnumerable<EnumValueSyntax> Values { get { return _values; } }
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
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

        internal abstract class MoleculeSyntax : ITypeWithBase, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.MoleculeSyntax, SyntacticModel.ISymbol>
        {
            private readonly Boolean _isExternal;
            private readonly Boolean _isAbstract;
            private readonly Boolean _isPartial;
            private readonly SimpleNameSyntax _simpleName;
            private readonly Maybe<NameSyntax> _base;
            private readonly AtomSyntax[] _atoms;
            private readonly AnnotationSyntax[] _annotations;

            protected MoleculeSyntax(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations)
            {
                _isExternal = isExternal;
                _isAbstract = isAbstract;
                _isPartial = isPartial;
                _simpleName = simpleName;
                _base = @base;
                _atoms = atoms.ToArray();
                _annotations = annotations.ToArray();
            }

            public SyntacticModel.MoleculeSyntax MakePublic(SyntacticModel.ISymbol parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent);
            public Boolean IsExternal { get { return _isExternal; } }
            public Boolean IsAbstract { get { return _isAbstract; } }
            public Boolean IsPartial { get { return _isPartial; } }
            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public Maybe<NameSyntax> Base { get { return _base; } }
            public IEnumerable<AtomSyntax> Atoms { get { return _atoms; } }
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
        }

        internal class AtomSyntax : ISymbol, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.AtomSyntax, SyntacticModel.MoleculeSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly TypeReferenceSyntax _type;
            private readonly AnnotationSyntax[] _annotations;

            public AtomSyntax(SimpleNameSyntax simpleName, TypeReferenceSyntax type, IEnumerable<AnnotationSyntax> annotations)
            {
                _simpleName = simpleName;
                _type = type;
                _annotations = annotations.ToArray();
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
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
        }

        internal class AggregateSyntax : INamespaceMember, ITypeWithBase, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.AggregateSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly Boolean _isExternal;
            private readonly Boolean _isAbstract;
            private readonly SimpleNameSyntax _simpleName;
            private readonly Maybe<AggregateIdentifierSyntax> _identifier;
            private readonly Maybe<NameSyntax> _base;
            private readonly IAggregateMember[] _members;
            private readonly AnnotationSyntax[] _annotations;

            public AggregateSyntax(Boolean isExternal, Boolean isAbstract, SimpleNameSyntax simpleName, Maybe<AggregateIdentifierSyntax> identifier, Maybe<NameSyntax> @base, IEnumerable<IAggregateMember> members, IEnumerable<AnnotationSyntax> annotations)
            {
                _isExternal = isExternal;
                _isAbstract = isAbstract;
                _simpleName = simpleName;
                _identifier = identifier;
                _base = @base;
                _members = members.ToArray();
                _annotations = annotations.ToArray();
            }

            public SyntacticModel.AggregateSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AggregateSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.AggregateSyntax(parent, this);
            }

            public Boolean IsExternal { get { return _isExternal; } }
            public Boolean IsAbstract { get { return _isAbstract; } }
            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public Maybe<AggregateIdentifierSyntax> Identifier { get { return _identifier; } }
            public Maybe<NameSyntax> Base { get { return _base; } }
            public IEnumerable<IAggregateMember> Members { get { return _members; } }
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
        }

        internal class AggregateEventSyntax : MoleculeSyntax, IAggregateMember, IAstUnderlyingNode<SyntacticModel.AggregateEventSyntax, SyntacticModel.AggregateSyntax>
        {

            public AggregateEventSyntax(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations) : base(isExternal, isAbstract, isPartial, simpleName, @base, atoms, annotations)
            {
            }

            public SyntacticModel.AggregateEventSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return (SyntacticModel.AggregateEventSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent)
            {
                return new SyntacticModel.AggregateEventSyntax((SyntacticModel.AggregateSyntax)parent, this);
            }

        }

        internal class AggregateSnapshotSyntax : MoleculeSyntax, IAggregateMember, IAstUnderlyingNode<SyntacticModel.AggregateSnapshotSyntax, SyntacticModel.AggregateSyntax>
        {

            public AggregateSnapshotSyntax(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations) : base(isExternal, isAbstract, isPartial, simpleName, @base, atoms, annotations)
            {
            }

            public SyntacticModel.AggregateSnapshotSyntax MakePublic(SyntacticModel.AggregateSyntax parent)
            {
                return (SyntacticModel.AggregateSnapshotSyntax)BuildPublic(parent);
            }

            protected override SyntacticModel.MoleculeSyntax BuildPublic(SyntacticModel.ISymbol parent)
            {
                return new SyntacticModel.AggregateSnapshotSyntax((SyntacticModel.AggregateSyntax)parent, this);
            }

        }

        internal class AggregateEventPropertySyntax : ISymbol, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.AggregateEventPropertySyntax, SyntacticModel.AggregateEventSyntax>
        {
            private readonly SimpleNameSyntax _simpleName;
            private readonly TypeReferenceSyntax _type;
            private readonly AnnotationSyntax[] _annotations;

            public AggregateEventPropertySyntax(SimpleNameSyntax simpleName, TypeReferenceSyntax type, IEnumerable<AnnotationSyntax> annotations)
            {
                _simpleName = simpleName;
                _type = type;
                _annotations = annotations.ToArray();
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
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
        }

        internal class ScalarValueSyntax : INamespaceMember, ITypeWithBase, IAnnotatableNode, IAstUnderlyingNode<SyntacticModel.ScalarValueSyntax, SyntacticModel.NamespaceSyntax>
        {
            private readonly Boolean _isExternal;
            private readonly Boolean _isAbstract;
            private readonly Boolean _isPartial;
            private readonly SimpleNameSyntax _simpleName;
            private readonly NameSyntax _base;
            private readonly AnnotationSyntax[] _annotations;

            public ScalarValueSyntax(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, NameSyntax @base, IEnumerable<AnnotationSyntax> annotations)
            {
                _isExternal = isExternal;
                _isAbstract = isAbstract;
                _isPartial = isPartial;
                _simpleName = simpleName;
                _base = @base;
                _annotations = annotations.ToArray();
            }

            public SyntacticModel.ScalarValueSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.ScalarValueSyntax BuildPublic(SyntacticModel.NamespaceSyntax parent)
            {
                return new SyntacticModel.ScalarValueSyntax(parent, this);
            }

            public Boolean IsExternal { get { return _isExternal; } }
            public Boolean IsAbstract { get { return _isAbstract; } }
            public Boolean IsPartial { get { return _isPartial; } }
            public SimpleNameSyntax SimpleName { get { return _simpleName; } }
            public NameSyntax Base { get { return _base; } }
            public IEnumerable<AnnotationSyntax> Annotations { get { return _annotations; } }
        }

        internal class ValueSyntax : MoleculeSyntax, INamespaceMember, IAstUnderlyingNode<SyntacticModel.ValueSyntax, SyntacticModel.NamespaceSyntax>
        {

            public ValueSyntax(Boolean isExternal, Boolean isAbstract, Boolean isPartial, SimpleNameSyntax simpleName, Maybe<NameSyntax> @base, IEnumerable<AtomSyntax> atoms, IEnumerable<AnnotationSyntax> annotations) : base(isExternal, isAbstract, isPartial, simpleName, @base, atoms, annotations)
            {
            }

            public SyntacticModel.ValueSyntax MakePublic(SyntacticModel.NamespaceSyntax parent)
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

            public IdentifierNameSyntax(String identifier) : base(identifier)
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
            private readonly NameSyntax[] _typeArguments;

            public GenericNameSyntax(IEnumerable<NameSyntax> typeArguments, String identifier) : base(identifier)
            {
                _typeArguments = typeArguments.ToArray();
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


