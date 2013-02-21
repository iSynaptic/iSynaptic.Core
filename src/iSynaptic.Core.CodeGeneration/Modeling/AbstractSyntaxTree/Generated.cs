using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    using SyntacticModel;

    public static class Syntax
    {
        public static AstNodeFamily Family(String @namespace, IEnumerable<AstNode> nodes)
        {
            return new AstNodeFamily(new SyntacticModel.Internal.AstNodeFamily(@namespace, nodes.Select(x => x.GetUnderlying())));
        }

        public static AstNode Node(Boolean isAbstract, String name, String typeName, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNode(null, new SyntacticModel.Internal.AstNode(isAbstract, name, typeName, parentType, baseTypes, properties.Select(x => x.GetUnderlying())));
        }

        public static AstNodeProperty Property(String name, String type, AstNodePropertyCardinality cardinality)
        {
            return new AstNodeProperty(null, new SyntacticModel.Internal.AstNodeProperty(name, type, cardinality));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel
{
    public class AstNodeFamily : IAstConcept, IVisitable
    {
        private readonly Internal.AstNodeFamily _underlying;

        internal AstNodeFamily(Internal.AstNodeFamily underlying)
        {
            _underlying = underlying;
        }

        internal Internal.AstNodeFamily GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Nodes);
        }

        public String Namespace
        {
            get
            {
                return GetUnderlying().Namespace;
            }
        }
        public IEnumerable<AstNode> Nodes
        {
            get
            {
                return GetUnderlying().Nodes.Select(x => x.MakePublic(this));
            }
        }
    }

    public class AstNode : IAstConcept, IVisitable
    {
        private readonly AstNodeFamily _parent;
        private readonly Internal.AstNode _underlying;

        internal AstNode(AstNodeFamily parent, Internal.AstNode underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        internal Internal.AstNode GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public Boolean IsAbstract
        {
            get
            {
                return GetUnderlying().IsAbstract;
            }
        }
        public String Name
        {
            get
            {
                return GetUnderlying().Name;
            }
        }
        public String TypeName
        {
            get
            {
                return GetUnderlying().TypeName;
            }
        }
        public Maybe<String> ParentType
        {
            get
            {
                return GetUnderlying().ParentType;
            }
        }
        public IEnumerable<String> BaseTypes
        {
            get
            {
                return GetUnderlying().BaseTypes;
            }
        }
        public IEnumerable<AstNodeProperty> Properties
        {
            get
            {
                return GetUnderlying().Properties.Select(x => x.MakePublic(this));
            }
        }
    }

    public class AstNodeProperty : IAstConcept, IVisitable
    {
        private readonly AstNode _parent;
        private readonly Internal.AstNodeProperty _underlying;

        internal AstNodeProperty(AstNode parent, Internal.AstNodeProperty underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstNode Parent { get { return _parent; } }
        internal Internal.AstNodeProperty GetUnderlying() { return _underlying; }

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
        public AstNodePropertyCardinality Cardinality
        {
            get
            {
                return GetUnderlying().Cardinality;
            }
        }
    }

    namespace Internal
    {
        internal class AstNodeFamily
        {
            private readonly String _namespace;
            private readonly IEnumerable<AstNode> _nodes;

            public AstNodeFamily(String @namespace, IEnumerable<AstNode> nodes)
            {
                _namespace = @namespace;
                _nodes = nodes;
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
            public IEnumerable<AstNode> Nodes { get { return _nodes; } }
        }

        internal class AstNode
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

        internal class AstNodeProperty
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

            public SyntacticModel.AstNodeProperty MakePublic(SyntacticModel.AstNode parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNodeProperty BuildPublic(SyntacticModel.AstNode parent)
            {
                return new SyntacticModel.AstNodeProperty(parent, this);
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
            public AstNodePropertyCardinality Cardinality { get { return _cardinality; } }
        }
    }
}

