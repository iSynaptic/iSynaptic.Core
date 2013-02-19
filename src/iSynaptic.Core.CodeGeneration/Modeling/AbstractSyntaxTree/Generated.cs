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

        public static AstNode Node(String name, String typeName, Boolean isVisitable, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNode(null, new SyntacticModel.Internal.AstNode(name, typeName, isVisitable, parentType, baseTypes, properties.Select(x => x.GetUnderlying())));
        }

        public static AstNodeProperty Property(String name, String type, Boolean isNode, Boolean isMany)
        {
            return new AstNodeProperty(null, new SyntacticModel.Internal.AstNodeProperty(name, type, isNode, isMany));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel
{
    public partial class AstNodeFamily : IVisitable
    {
        private readonly Internal.AstNodeFamily _underlying;

        internal AstNodeFamily(Internal.AstNodeFamily underlying)
        {
            _underlying = underlying;
        }

        internal Internal.AstNodeFamily GetUnderlying() { return _underlying; }

        public void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Nodes);
        }
        public String Namespace
        {
            get
            {
                return _underlying.Namespace;
            }
        }
        public IEnumerable<AstNode> Nodes
        {
            get
            {
                return _underlying.Nodes.Select(x => new AstNode(this, x));
            }
        }
    }

    public partial class AstNode : IVisitable
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
        public String TypeName
        {
            get
            {
                return _underlying.TypeName;
            }
        }
        public Boolean IsVisitable
        {
            get
            {
                return _underlying.IsVisitable;
            }
        }
        public Maybe<String> ParentType
        {
            get
            {
                return _underlying.ParentType;
            }
        }
        public IEnumerable<String> BaseTypes
        {
            get
            {
                return _underlying.BaseTypes;
            }
        }
        public IEnumerable<AstNodeProperty> Properties
        {
            get
            {
                return _underlying.Properties.Select(x => new AstNodeProperty(this, x));
            }
        }
    }

    public partial class AstNodeProperty : IVisitable
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
        public Boolean IsNode
        {
            get
            {
                return _underlying.IsNode;
            }
        }
        public Boolean IsMany
        {
            get
            {
                return _underlying.IsMany;
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

            public String Namespace { get { return _namespace; } }
            public IEnumerable<AstNode> Nodes { get { return _nodes; } }
        }

        internal class AstNode
        {
            private readonly String _name;
            private readonly String _typeName;
            private readonly Boolean _isVisitable;
            private readonly Maybe<String> _parentType;
            private readonly IEnumerable<String> _baseTypes;
            private readonly IEnumerable<AstNodeProperty> _properties;

            public AstNode(String name, String typeName, Boolean isVisitable, Maybe<String> parentType, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
            {
                _name = name;
                _typeName = typeName;
                _isVisitable = isVisitable;
                _parentType = parentType;
                _baseTypes = baseTypes;
                _properties = properties;
            }

            public String Name { get { return _name; } }
            public String TypeName { get { return _typeName; } }
            public Boolean IsVisitable { get { return _isVisitable; } }
            public Maybe<String> ParentType { get { return _parentType; } }
            public IEnumerable<String> BaseTypes { get { return _baseTypes; } }
            public IEnumerable<AstNodeProperty> Properties { get { return _properties; } }
        }

        internal class AstNodeProperty
        {
            private readonly String _name;
            private readonly String _type;
            private readonly Boolean _isNode;
            private readonly Boolean _isMany;

            public AstNodeProperty(String name, String type, Boolean isNode, Boolean isMany)
            {
                _name = name;
                _type = type;
                _isNode = isNode;
                _isMany = isMany;
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
            public Boolean IsNode { get { return _isNode; } }
            public Boolean IsMany { get { return _isMany; } }
        }
    }
}

