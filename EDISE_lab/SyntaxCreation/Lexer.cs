using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDISE_lab
{
    internal static class Lexer
    {
        
        internal static List<SyntaxNode> Tokenize(string line)
        {
            var clearedLine = line.Trim().Replace("\t", " ")
                .Replace(";", "")
                .Replace("*", "")
                .Replace("std::vector", "List")
                .Replace("std::list", "List")
                .Replace("std::map", "Dictionary")
                .Replace("std::string", "string")
                .Replace("&", "");//convert to regex

            //divide line by spaces
            var tokens = clearedLine.Split(' ');
            var nodes = new List<SyntaxNode>();
            int currentNodeIndex = 0;
            foreach (var token in tokens)
            {
                nodes.Add(CreateNode(token));
                if(nodes[currentNodeIndex].Type == SyntaxNode.NodeType.EqualsOperator)
                {
                    nodes.RemoveAt(0);
                    nodes[0].Name = "equals";
                    nodes[0].EndingTrivia = Environment.NewLine;
                    return nodes;
                }
                if (nodes.Count > 1)
                {
                    var previousNode = nodes[currentNodeIndex - 1];
                    if ((previousNode.Type == SyntaxNode.NodeType.DefaultVariableType
                        || previousNode.Type == SyntaxNode.NodeType.STDVariableType)
                        && nodes[currentNodeIndex].Type != SyntaxNode.NodeType.EqualsOperator)
                    {
                        nodes[currentNodeIndex].Type = SyntaxNode.NodeType.VariableName;
                    }
                    else if (previousNode.Type == SyntaxNode.NodeType.ClassModifier)
                    {
                        nodes[currentNodeIndex].Type = SyntaxNode.NodeType.ClassName;
                    }
                }
                nodes[currentNodeIndex].EndingTrivia = " ";
                currentNodeIndex++;
            }
            nodes[currentNodeIndex-1].EndingTrivia = Environment.NewLine;
            return nodes;
        }

        private static SyntaxNode CreateNode(string token)
        {
            switch (token)
            {
                case ":":
                    return new SyntaxNode(SyntaxNode.NodeType.Colon, token);
                case "public:":                  
                    return new SyntaxNode(SyntaxNode.NodeType.PublicModifier, token);
                case "private:":
                    return new SyntaxNode(SyntaxNode.NodeType.PrivateModifier, token);
                case "public":
                    return new SyntaxNode(SyntaxNode.NodeType.PublicClassModifier, token);
                case "private":
                    return new SyntaxNode(SyntaxNode.NodeType.PrivateClassModifier, token);
                case "class":
                    return new SyntaxNode(SyntaxNode.NodeType.ClassModifier, token);
                case "{":
                    return new SyntaxNode(SyntaxNode.NodeType.OpenBraketIndentation, token);
                case "}":
                    return new SyntaxNode(SyntaxNode.NodeType.CloseBraketIndentation, token);
                case "int":
                case "bool":
                case "double":
                case "float":
                case "uint":
                case "char":
                case "void":                    
                    return new SyntaxNode(SyntaxNode.NodeType.DefaultVariableType, token);
                default:
                    if (token.StartsWith("std::"))
                    {
                        return new SyntaxNode(SyntaxNode.NodeType.STDVariableType, token);
                    }
                    else if (token.StartsWith("operator=="))
                    {
                        return new SyntaxNode(SyntaxNode.NodeType.EqualsOperator, token);
                    }
                    else if (token.StartsWith("~"))
                    {
                        return new SyntaxNode(SyntaxNode.NodeType.Destructor, "destructor");
                    }
                    else if (token.EndsWith(":"))
                    {
                        throw new Exception("Unsupported modifier");
                    }

                    return new SyntaxNode(SyntaxNode.NodeType.Unknown, token);
            }
        }
    }
}
