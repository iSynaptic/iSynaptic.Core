using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    using SyntacticModel;

    public static class Syntax
    {
        public static AstNodeFamily Family(String @namespace, IEnumerable<IAstConcept> concepts)
        {
            return new AstNodeFamily(new SyntacticModel.Internal.AstNodeFamily(@namespace, concepts.Select(x => ((IAstNode<SyntacticModel.Internal.IAstConcept>)x).GetUnderlying())));
        }


        public static AstNode Node(Boolean isAbstract, String name, String typeName, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNode(null, new SyntacticModel.Internal.AstNode(isAbstract, name, typeName, parentType, baseTypes, properties.Select(x => ((IAstNode<SyntacticModel.Internal.AstNodeProperty>)x).GetUnderlying())));
        }

        public static AstNodeContract Contract(String typeName, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNodeContract(null, new SyntacticModel.Internal.AstNodeContract(typeName, parentType, baseTypes, properties.Select(x => ((IAstNode<SyntacticModel.Internal.AstNodeProperty>)x).GetUnderlying())));
        }

        public static AstNodeProperty Property(String name, String type, AstNodePropertyCardinality cardinality)
        {
            return new AstNodeProperty(null, new SyntacticModel.Internal.AstNodeProperty(name, type, cardinality));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel
{
    internal interface IAstNode<out T> : IVisitable { T GetUnderlying(); }

    internal interface IAstUnderlyingNode<out T, in TParent> { T MakePublic(TParent parent); }

    public partial class AstNodeFamily : IAstNode<Internal.AstNodeFamily>
    {
        private readonly Internal.AstNodeFamily _underlying;

        internal AstNodeFamily(Internal.AstNodeFamily underlying)
        {
            _underlying = underlying;
        }

        Internal.AstNodeFamily IAstNode<Internal.AstNodeFamily>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Concepts);
        }

        public String Namespace
        {
            get
            {
                return ((IAstNode<Internal.AstNodeFamily>)this).GetUnderlying().Namespace;
            }
        }
        public IEnumerable<IAstConcept> Concepts
        {
            get
            {
                return ((IAstNode<Internal.AstNodeFamily>)this).GetUnderlying().Concepts.Select(x => ((IAstUnderlyingNode<IAstConcept, AstNodeFamily>)x).MakePublic(this));
            }
        }
    }

    public interface IAstConcept : IVisitable
    {
        String TypeName { get; }
        IEnumerable<String> BaseTypes { get; }
        Maybe<String> ParentType { get; }
        IEnumerable<AstNodeProperty> Properties { get; }
        AstNodeFamily Parent { get; }
    }

    public partial class AstNode : IAstConcept, IAstNode<Internal.AstNode>
    {
        private readonly AstNodeFamily _parent;
        private readonly Internal.AstNode _underlying;

        internal AstNode(AstNodeFamily parent, Internal.AstNode underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        Internal.AstNode IAstNode<Internal.AstNode>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public Boolean IsAbstract
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().IsAbstract;
            }
        }
        public String Name
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().Name;
            }
        }
        public String TypeName
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().TypeName;
            }
        }
        public Maybe<String> ParentType
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().ParentType;
            }
        }
        public IEnumerable<String> BaseTypes
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().BaseTypes;
            }
        }
        public IEnumerable<AstNodeProperty> Properties
        {
            get
            {
                return ((IAstNode<Internal.AstNode>)this).GetUnderlying().Properties.Select(x => ((IAstUnderlyingNode<AstNodeProperty, AstNode>)x).MakePublic(this));
            }
        }
    }

    public partial class AstNodeContract : IAstConcept, IAstNode<Internal.AstNodeContract>
    {
        private readonly AstNodeFamily _parent;
        private readonly Internal.AstNodeContract _underlying;

        internal AstNodeContract(AstNodeFamily parent, Internal.AstNodeContract underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        Internal.AstNodeContract IAstNode<Internal.AstNodeContract>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String TypeName
        {
            get
            {
                return ((IAstNode<Internal.AstNodeContract>)this).GetUnderlying().TypeName;
            }
        }
        public Maybe<String> ParentType
        {
            get
            {
                return ((IAstNode<Internal.AstNodeContract>)this).GetUnderlying().ParentType;
            }
        }
        public IEnumerable<String> BaseTypes
        {
            get
            {
                return ((IAstNode<Internal.AstNodeContract>)this).GetUnderlying().BaseTypes;
            }
        }
        public IEnumerable<AstNodeProperty> Properties
        {
            get
            {
                return ((IAstNode<Internal.AstNodeContract>)this).GetUnderlying().Properties.Select(x => ((IAstUnderlyingNode<AstNodeProperty, AstNodeContract>)x).MakePublic(this));
            }
        }
    }

    public partial class AstNodeProperty : IAstNode<Internal.AstNodeProperty>
    {
        private readonly IAstConcept _parent;
        private readonly Internal.AstNodeProperty _underlying;

        internal AstNodeProperty(IAstConcept parent, Internal.AstNodeProperty underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public IAstConcept Parent { get { return _parent; } }
        Internal.AstNodeProperty IAstNode<Internal.AstNodeProperty>.GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
        }

        public String Name
        {
            get
            {
                return ((IAstNode<Internal.AstNodeProperty>)this).GetUnderlying().Name;
            }
        }
        public String Type
        {
            get
            {
                return ((IAstNode<Internal.AstNodeProperty>)this).GetUnderlying().Type;
            }
        }
        public AstNodePropertyCardinality Cardinality
        {
            get
            {
                return ((IAstNode<Internal.AstNodeProperty>)this).GetUnderlying().Cardinality;
            }
        }
    }

    namespace Internal
    {
        internal class AstNodeFamily : IAstUnderlyingNode<SyntacticModel.AstNodeFamily, Object>
        {
            private readonly String _namespace;
            private readonly IEnumerable<IAstConcept> _concepts;

            public AstNodeFamily(String @namespace, IEnumerable<IAstConcept> concepts)
            {
                _namespace = @namespace;
                _concepts = concepts;
            }

            public SyntacticModel.AstNodeFamily MakePublic(Object parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNodeFamily BuildPublic(Object parent)
            {
                return new SyntacticModel.AstNodeFamily(this);
            }

            public String Namespace { get { return _namespace; } }
            public IEnumerable<IAstConcept> Concepts { get { return _concepts; } }
        }

        internal interface IAstConcept
        {
        }

        internal class AstNode : IAstConcept, IAstUnderlyingNode<SyntacticModel.AstNode, SyntacticModel.AstNodeFamily>
        {
            private readonly Boolean _isAbstract;
            private readonly String _name;
            private readonly String _typeName;
            private readonly Maybe<String> _parentType;
            private readonly IEnumerable<String> _baseTypes;
            private readonly IEnumerable<AstNodeProperty> _properties;

            public AstNode(Boolean isAbstract, String name, String typeName, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
            {
                _isAbstract = isAbstract;
                _name = name;
                _typeName = typeName;
                _parentType = parentType;
                _baseTypes = baseTypes;
                _properties = properties;
            }

            public SyntacticModel.AstNode MakePublic(SyntacticModel.AstNodeFamily parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNode BuildPublic(SyntacticModel.AstNodeFamily parent)
            {
                return new SyntacticModel.AstNode(parent, this);
            }

            public Boolean IsAbstract { get { return _isAbstract; } }
            public String Name { get { return _name; } }
            public String TypeName { get { return _typeName; } }
            public Maybe<String> ParentType { get { return _parentType; } }
            public IEnumerable<String> BaseTypes { get { return _baseTypes; } }
            public IEnumerable<AstNodeProperty> Properties { get { return _properties; } }
        }

        internal class AstNodeContract : IAstConcept, IAstUnderlyingNode<SyntacticModel.AstNodeContract, SyntacticModel.AstNodeFamily>
        {
            private readonly String _typeName;
            private readonly Maybe<String> _parentType;
            private readonly IEnumerable<String> _baseTypes;
            private readonly IEnumerable<AstNodeProperty> _properties;

            public AstNodeContract(String typeName, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
            {
                _typeName = typeName;
                _parentType = parentType;
                _baseTypes = baseTypes;
                _properties = properties;
            }

            public SyntacticModel.AstNodeContract MakePublic(SyntacticModel.AstNodeFamily parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNodeContract BuildPublic(SyntacticModel.AstNodeFamily parent)
            {
                return new SyntacticModel.AstNodeContract(parent, this);
            }

            public String TypeName { get { return _typeName; } }
            public Maybe<String> ParentType { get { return _parentType; } }
            public IEnumerable<String> BaseTypes { get { return _baseTypes; } }
            public IEnumerable<AstNodeProperty> Properties { get { return _properties; } }
        }

        internal class AstNodeProperty : IAstUnderlyingNode<SyntacticModel.AstNodeProperty, SyntacticModel.IAstConcept>
        {
            private readonly String _name;
            private readonly String _type;
            private readonly AstNodePropertyCardinality _cardinality;

            public AstNodeProperty(String name, String type, AstNodePropertyCardinality cardinality)
            {
                _name = name;
                _type = type;
                _cardinality = cardinality;
            }

            public SyntacticModel.AstNodeProperty MakePublic(SyntacticModel.IAstConcept parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNodeProperty BuildPublic(SyntacticModel.IAstConcept parent)
            {
                return new SyntacticModel.AstNodeProperty(parent, this);
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
            public AstNodePropertyCardinality Cardinality { get { return _cardinality; } }
        }
    }
}

