using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPtoCSharpClasses
{
    internal class SyntaxNode
    {
        public SyntaxNode? Parent { get; set; }
        public List<SyntaxNode> Children { get; set; }
        public string? Name { get; set; }
        public NodeType Type { get; set; }

        public string? EndingTrivia { get; set; }

        public ModifierType? Modifier { get; set; }
        public enum NodeType
        {
            Unknown,
            Root,
            PublicModifier,
            PrivateModifier,
            ClassModifier,
            DefaultVariableType,
            STDVariableType,
            EqualsOperator,
            Colon,
            VariableName,
            ClassName,
            Destructor,
            OpenBraketIndentation,
            CloseBraketIndentation,
            ClassBody,
            MethodBody,
            ArgumentsBody,
            PublicClassModifier,
            PrivateClassModifier,
            CSharpModification,
            Constructor
        }

        public enum ModifierType
        {
            None,
            Public,
            Private,
        }
        public enum AcessType
        {
            None,
            Get,
            Set,
            GetSet,
        }
        public SyntaxNode(NodeType type, string name, ModifierType modifier)
        {
            Type = type;
            Name = name;
            Children = new List<SyntaxNode>();
            Modifier = modifier;
        }

        public SyntaxNode(NodeType type, string name)
        {
            Type = type;
            Name = name;
            Children = new List<SyntaxNode>();
            Modifier = ModifierType.None;
        }

        public SyntaxNode(NodeType type)
        {
            Type = type;
            Children = new List<SyntaxNode>();
            Modifier = ModifierType.None;
        }

        public override string ToString()
        {
            if (Name != null)
                return Name + EndingTrivia;
            else
            {
                string result = "";
                if (Type == NodeType.ArgumentsBody)
                    result += '(';
                else if (Type == NodeType.ClassBody)
                    result += '\t';
                foreach (var child in Children)
                {
                    if(Type == NodeType.ClassBody)
                    {
                        result +=  child.ToString();
                        if (child.EndingTrivia == Environment.NewLine && child != Children.Last())
                            result += '\t';
                    }
                    else
                        result += child.ToString();
                }
                if (Type == NodeType.ArgumentsBody)
                    result += ')'+ EndingTrivia;
                return result;
            }
        }
    }        
}
