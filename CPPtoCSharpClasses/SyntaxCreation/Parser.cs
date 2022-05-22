using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPPtoCSharpClasses.SyntaxNode;

namespace CPPtoCSharpClasses
{
    internal class Parser
    {
        internal SyntaxTree Parse(List<string> fileContents)
        {
            //parse file contens and build a syntax tree
            SyntaxTree syntaxTree = new SyntaxTree();
            syntaxTree.Root = new SyntaxNode(NodeType.Root);
            syntaxTree.Root.Type = NodeType.Root;
            syntaxTree.Root.Parent = null;
            SyntaxNode _currentNode = syntaxTree.Root;
            var currentModifier = _currentNode.Modifier;
            //parse file contents
            foreach (string line in fileContents)
            {
                //skip empty lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                //divide line into tokens
                List<SyntaxNode> tokens = Lexer.Tokenize(line);

                foreach (var token in tokens)
                {
                    if (token.Name.Contains("("))
                    {
                        var moreTokens = token.Name.Split("(");
                        if(moreTokens[1]!= ")")
                        {
                            var node = new SyntaxNode(NodeType.VariableName, moreTokens[0]);
                            node.Parent = _currentNode;
                            _currentNode.Children.Add(node);
                            var nodeInternal = new SyntaxNode(NodeType.ArgumentsBody);
                            nodeInternal.Parent = _currentNode;
                            _currentNode.Children.Add(nodeInternal);
                            _currentNode = nodeInternal;
                            var nodeInternalBody = new SyntaxNode(NodeType.DefaultVariableType, moreTokens[1]+" ");
                            nodeInternalBody.Parent = _currentNode;
                            _currentNode.Children.Add(nodeInternalBody);
                            if (moreTokens[1].Contains("{}"))
                            {
                                nodeInternalBody.Name = "";
                                _currentNode = _currentNode.Parent;
                                
                            }
                            continue;
                        }
                    }
                    else if (token.Name.Contains(")"))
                    {
                        var moreTokens = token.Name.Split(")");
                        if (moreTokens[1] == "")
                        {
                            var node = new SyntaxNode(NodeType.VariableName, moreTokens[0]);
                            node.Parent = _currentNode;
                            _currentNode.Children.Add(node);
                            _currentNode = _currentNode.Parent;
                            token.Name="";
                        }
                    }
                    token.Parent = _currentNode;
                    _currentNode.Children.Add(token);
                    if (token == tokens.First() 
                        && token.Type == NodeType.Unknown 
                        && token?.Parent?.Type == NodeType.ClassBody)
                    {
                        if (tokens.Count == 2)
                        {
                            token.Type = NodeType.DefaultVariableType;
                            tokens[1].Type = NodeType.VariableName;
                        }
                        else if(tokens.Count == 1 && token.Name.Contains("()"))
                        {
                            token.Type = NodeType.Constructor;
                        }
                    }
                    switch (token.Type)
                    {
                        case NodeType.OpenBraketIndentation:
                            //create new node
                            var type = NodeType.Unknown;
                            if (_currentNode.Type == NodeType.Root)
                                type = NodeType.ClassBody;
                            else if (_currentNode.Type == NodeType.ClassBody)//fix class not ckeing internal
                                type = NodeType.MethodBody;
                            SyntaxNode newNode = new SyntaxNode(type);
                            newNode.Parent = _currentNode;
                            _currentNode.Children.Add(newNode);
                            _currentNode = newNode;
                            break;
                        case NodeType.CloseBraketIndentation:
                            _currentNode.Children.RemoveAt(_currentNode.Children.Count - 1);
                            token.Parent = _currentNode.Parent;
                            _currentNode = _currentNode.Parent;
                            _currentNode.Children.Add(token);
                           
                            break;
                        case NodeType.PublicModifier:
                            currentModifier = ModifierType.Public;
                            break;
                        case NodeType.PrivateModifier:
                            currentModifier = ModifierType.Private;
                            break;
                        case NodeType.DefaultVariableType:
                        case NodeType.STDVariableType:
                        case NodeType.Destructor:
                            token.Modifier = currentModifier;
                            break;

                    }
                }                    
            }
            if (_currentNode.Type != NodeType.Root)
            {
                throw new Exception("Incorrect indentation");
            }
            return syntaxTree;
        }
    }
}
