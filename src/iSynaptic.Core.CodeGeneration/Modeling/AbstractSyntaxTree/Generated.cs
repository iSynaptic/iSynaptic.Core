using System;
using System.Collections.Generic;
using System.Linq;
using iSynaptic.Commons;

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree
{
    using SyntacticModel;

    public static class Syntax
    {
        public static AstNodeFamily Family(String @namespace, IEnumerable<AstMolecule> molecules)
        {
            return new AstNodeFamily(new SyntacticModel.Internal.AstNodeFamily(@namespace, molecules.Select(x => x.GetUnderlying())));
        }



        public static AstNode Node(Boolean isAbstract, String name, Maybe<String> parentType, String typeName, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNode(null, new SyntacticModel.Internal.AstNode(isAbstract, name, parentType, typeName, baseTypes, properties.Select(x => x.GetUnderlying())));
        }

        public static AstNodeContract Contract(String typeName, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
        {
            return new AstNodeContract(null, new SyntacticModel.Internal.AstNodeContract(typeName, baseTypes, properties.Select(x => x.GetUnderlying())));
        }

        public static AstNodeProperty Property(String name, String type, AstNodePropertyCardinality cardinality)
        {
            return new AstNodeProperty(null, new SyntacticModel.Internal.AstNodeProperty(name, type, cardinality));
        }
    }
}

namespace iSynaptic.CodeGeneration.Modeling.AbstractSyntaxTree.SyntacticModel
{
    public class AstNodeFamily : IVisitable
    {
        private readonly Internal.AstNodeFamily _underlying;

        internal AstNodeFamily(Internal.AstNodeFamily underlying)
        {
            _underlying = underlying;
        }

        internal Internal.AstNodeFamily GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Molecules);
        }

        public String Namespace
        {
            get
            {
                return GetUnderlying().Namespace;
            }
        }
        public IEnumerable<AstMolecule> Molecules
        {
            get
            {
                return GetUnderlying().Molecules.Select(x => x.MakePublic(this));
            }
        }
    }

    public interface IAstConcept
    {
        String TypeName { get; }
        IEnumerable<String> BaseTypes { get; }
        IEnumerable<AstNodeProperty> Properties { get; }
    }

    public abstract class AstMolecule : IVisitable
    {
        private readonly AstNodeFamily _parent;
        private readonly Internal.AstMolecule _underlying;

        internal AstMolecule(AstNodeFamily parent, Internal.AstMolecule underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        internal Internal.AstMolecule GetUnderlying() { return _underlying; }

        public virtual void AcceptChildren(Action<IEnumerable<IVisitable>> dispatch)
        {
            dispatch(Properties);
        }

        public String TypeName
        {
            get
            {
                return GetUnderlying().TypeName;
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

    public class AstNode : AstMolecule, IAstConcept
    {
        private readonly AstNodeFamily _parent;

        internal AstNode(AstNodeFamily parent, Internal.AstNode underlying)
            : base(parent, underlying)
        {
            _parent = parent;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        new internal Internal.AstNode GetUnderlying() { return (Internal.AstNode)base.GetUnderlying(); }

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
        public Maybe<String> ParentType
        {
            get
            {
                return GetUnderlying().ParentType;
            }
        }
    }

    public class AstNodeContract : AstMolecule, IAstConcept
    {
        private readonly AstNodeFamily _parent;

        internal AstNodeContract(AstNodeFamily parent, Internal.AstNodeContract underlying)
            : base(parent, underlying)
        {
            _parent = parent;
        }

        public AstNodeFamily Parent { get { return _parent; } }
        new internal Internal.AstNodeContract GetUnderlying() { return (Internal.AstNodeContract)base.GetUnderlying(); }

    }

    public class AstNodeProperty : IVisitable
    {
        private readonly AstMolecule _parent;
        private readonly Internal.AstNodeProperty _underlying;

        internal AstNodeProperty(AstMolecule parent, Internal.AstNodeProperty underlying)
        {
            _parent = parent;
            _underlying = underlying;
        }

        public AstMolecule Parent { get { return _parent; } }
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
            private readonly IEnumerable<AstMolecule> _molecules;

            public AstNodeFamily(String @namespace, IEnumerable<AstMolecule> molecules)
            {
                _namespace = @namespace;
                _molecules = molecules;
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
            public IEnumerable<AstMolecule> Molecules { get { return _molecules; } }
        }


        internal abstract class AstMolecule
        {
            private readonly String _typeName;
            private readonly IEnumerable<String> _baseTypes;
            private readonly IEnumerable<AstNodeProperty> _properties;

            protected AstMolecule(String typeName, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
            {
                _typeName = typeName;
                _baseTypes = baseTypes;
                _properties = properties;
            }

            public SyntacticModel.AstMolecule MakePublic(SyntacticModel.AstNodeFamily parent)
            {
                return BuildPublic(parent);
            }

            protected abstract SyntacticModel.AstMolecule BuildPublic(SyntacticModel.AstNodeFamily parent);
            public String TypeName { get { return _typeName; } }
            public IEnumerable<String> BaseTypes { get { return _baseTypes; } }
            public IEnumerable<AstNodeProperty> Properties { get { return _properties; } }
        }

        internal class AstNode : AstMolecule
        {
            private readonly Boolean _isAbstract;
            private readonly String _name;
            private readonly Maybe<String> _parentType;

            public AstNode(Boolean isAbstract, String name, Maybe<String> parentType, String typeName, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
                : base(typeName, baseTypes, properties)
            {
                _isAbstract = isAbstract;
                _name = name;
                _parentType = parentType;
            }

            public new SyntacticModel.AstNode MakePublic(SyntacticModel.AstNodeFamily parent)
            {
                return (SyntacticModel.AstNode)BuildPublic(parent);
            }

            protected override SyntacticModel.AstMolecule BuildPublic(SyntacticModel.AstNodeFamily parent)
            {
                return new SyntacticModel.AstNode(parent, this);
            }

            public Boolean IsAbstract { get { return _isAbstract; } }
            public String Name { get { return _name; } }
            public Maybe<String> ParentType { get { return _parentType; } }
        }

        internal class AstNodeContract : AstMolecule
        {

            public AstNodeContract(String typeName, IEnumerable<String> baseTypes, IEnumerable<AstNodeProperty> properties)
                : base(typeName, baseTypes, properties)
            {
            }

            public new SyntacticModel.AstNodeContract MakePublic(SyntacticModel.AstNodeFamily parent)
            {
                return (SyntacticModel.AstNodeContract)BuildPublic(parent);
            }

            protected override SyntacticModel.AstMolecule BuildPublic(SyntacticModel.AstNodeFamily parent)
            {
                return new SyntacticModel.AstNodeContract(parent, this);
            }

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

            public SyntacticModel.AstNodeProperty MakePublic(SyntacticModel.AstMolecule parent)
            {
                return BuildPublic(parent);
            }

            protected virtual SyntacticModel.AstNodeProperty BuildPublic(SyntacticModel.AstMolecule parent)
            {
                return new SyntacticModel.AstNodeProperty(parent, this);
            }

            public String Name { get { return _name; } }
            public String Type { get { return _type; } }
            public AstNodePropertyCardinality Cardinality { get { return _cardinality; } }
        }
    }
}

