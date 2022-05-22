using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDISE_lab
{
    public class Rewriter
    {
        private Parser _parser;
        private SyntaxTree? _syntaxTree;
        private FileState? _fileState;
        public Rewriter()
        {
            _parser = new Parser();
        }
        public void BuildTree(List<string> fileContents)
        {
            //build the syntax tree
            _syntaxTree = _parser.Parse(fileContents);
        }

        public string Rewrite()
        {
            _fileState = new FileState();
            Rewrite(_syntaxTree?.Root);
            PostProcessing(_syntaxTree?.Root);
            Console.WriteLine(_syntaxTree?.Root.ToString());
            return _syntaxTree?.Root.ToString();
        }

        private void Rewrite(SyntaxNode node)
        {
            if (node.Type == SyntaxNode.NodeType.ArgumentsBody)
            {
                int index = node.Parent.Children.IndexOf(node);
                node.EndingTrivia = Environment.NewLine;
                AppendEmptyMethodBody(node.Parent, index+1);
                return;
            }
            for (int i = 0; i < node.Children.Count; i++)
            {
                var currentNode = node.Children[i];
                if(currentNode.Type == SyntaxNode.NodeType.CSharpModification)
                {
                    continue;
                }
                if ((currentNode.Type == SyntaxNode.NodeType.VariableName || currentNode.Type == SyntaxNode.NodeType.Unknown)
                    && currentNode.Name == String.Empty)
                {
                    node.Children.RemoveAt(i);
                    i--;
                }
                if (currentNode.Type == SyntaxNode.NodeType.MethodBody)
                {
                    currentNode.Children.Clear();
                    var newChild = new SyntaxNode(SyntaxNode.NodeType.CSharpModification, "\t//TODO: implement this method");
                    newChild.EndingTrivia = Environment.NewLine +"\t";
                    currentNode.Children.Add(newChild);
                }
                if (i>0 && (currentNode.Type == SyntaxNode.NodeType.PublicClassModifier
                    || currentNode.Type == SyntaxNode.NodeType.PrivateClassModifier)
                    && node.Children[i - 1].Type == SyntaxNode.NodeType.Colon)
                {
                    node.Children.RemoveAt(i);
                    i--;
                }
                else if (currentNode.Type == SyntaxNode.NodeType.PublicModifier
                    || currentNode.Type == SyntaxNode.NodeType.PrivateModifier)
                {
                    node.Children.RemoveAt(i);
                    i--;
                }
                else if(currentNode.Type == SyntaxNode.NodeType.DefaultVariableType)
                {
                    i = AppendModifier(node, i, currentNode.Modifier);
                }
                else if (currentNode.Type == SyntaxNode.NodeType.VariableName)
                {
                    if (currentNode.Name.StartsWith("get_"))
                    {
                        i = PreTransformAccessField(node, currentNode, i, SyntaxNode.AcessType.Get);
                    }
                    else if (currentNode.Name.StartsWith("set_"))
                    {
                        i = PreTransformAccessField(node, currentNode, i, SyntaxNode.AcessType.Set);
                    }
                }
                else if (currentNode.Type == SyntaxNode.NodeType.Destructor)
                {
                    node.Children.RemoveAt(i);
                }
                else if(currentNode.Type == SyntaxNode.NodeType.Constructor)
                {
                    i = AppendModifier(node, i, currentNode.Modifier);
                    i = AppendEmptyMethodBody(node, ++i);
                }
                else if (currentNode.Type == SyntaxNode.NodeType.EqualsOperator)
                {
                    node.Children.RemoveAt(i);
                    i = InsertCSharpSyntax(node, i, "public override bool Equals(Object other)");
                    i = AppendEmptyMethodBody(node, i);
                    i = InsertCSharpSyntax(node, i, "public override int GetHashCode()");
                    i = AppendEmptyMethodBody(node, i);
                    i--;
                }
                if (currentNode.Children.Count > 0)
                {
                    Rewrite(currentNode);
                }
            }
        }

        private int AppendModifier(SyntaxNode node, int index, SyntaxNode.ModifierType? modifier)
        {
            if(modifier == SyntaxNode.ModifierType.None)
                return index;
            if (modifier == SyntaxNode.ModifierType.Public)
            {
                var newNode = new SyntaxNode(SyntaxNode.NodeType.PublicClassModifier, "public ");
                newNode.Parent = node;
                node.Children.Insert(index, newNode);
            }
            else if (modifier == SyntaxNode.ModifierType.Private)
            {
                var newNode = new SyntaxNode(SyntaxNode.NodeType.PrivateClassModifier, "private ");
                newNode.Parent = node;
                node.Children.Insert(index, newNode);
            }
            index++;
            return index;
        }

        private int AppendEmptyMethodBody(SyntaxNode node, int index)
        {
            index = InsertCSharpSyntax(node, index, "{");
            index = InsertCSharpSyntax(node, index, "\t//TODO: implement this method");
            index = InsertCSharpSyntax(node, index, "}");
            return index;
        }

        private static int InsertCSharpMultilineSyntax(SyntaxNode node, int index, string syntax)
        {
            var lines = syntax.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                index = InsertCSharpSyntax(node, index, line);
            }
            return index;
        }
        private static int InsertCSharpSyntax(SyntaxNode node, int index, string syntax, bool endLine = true)
        {
            var newNode = new SyntaxNode(SyntaxNode.NodeType.CSharpModification, syntax);
            newNode.Parent = node;
            if (endLine)
                newNode.EndingTrivia = Environment.NewLine;
            node.Children.Insert(index, newNode);
            index++;
            return index;
        }

        private int PreTransformAccessField(SyntaxNode node, SyntaxNode currentNode, int index, SyntaxNode.AcessType expectedAccess)
        {
            currentNode.Name = currentNode.Name.Substring(4);
            currentNode.Name = currentNode.Name.Replace("()", "");

            if (_fileState.GettersAndSettersEncounters.ContainsKey(currentNode.Name))
                _fileState.GettersAndSettersEncounters[currentNode.Name] = SyntaxNode.AcessType.GetSet;
            else
                _fileState.GettersAndSettersEncounters.Add(currentNode.Name, expectedAccess);

            index = ClearLine(node, index);

            return index;
        }

        private int ClearLine(SyntaxNode node, int index)
        {
            index = ClearLineBefore(node, index);
            index = ClearLineAfter(node, index);
            return index;
        }
        private int ClearLineBefore(SyntaxNode node, int index)
        {
            index--;
            while (node.Children[index].EndingTrivia != Environment.NewLine)
            {
                node.Children.RemoveAt(index);
                index--;
            }
            index++;
            return index;
        }

        private int ClearLineAfter(SyntaxNode node, int index)
        {
            while (node.Children[index].EndingTrivia != Environment.NewLine)
            {
                node.Children.RemoveAt(index);
            }
            node.Children.RemoveAt(index);
            index--;
            return index;
        }

        private void PostProcessing(SyntaxNode node)
        {
            for (int i = 1; i < node.Children.Count; i++)
            {
                var currentNode = node.Children[i];
                if(currentNode.Type == SyntaxNode.NodeType.DefaultVariableType)
                {
                    if (node.Children[i - 1].Type == SyntaxNode.NodeType.PrivateClassModifier)
                    {
                        var variableNode = node.Children[i + 1];
                        _fileState.GettersAndSettersEncounters.TryGetValue(variableNode?.Name, out SyntaxNode.AcessType acessType);
                        if (acessType == SyntaxNode.AcessType.None)
                        {
                            variableNode.Name = RewriterData.ConvertToPascalCase(variableNode.Name);
                            continue;
                        }
                         
                        variableNode.Name = RewriterData.ConvertToPascalCase(variableNode.Name);
                     
                        var prevNode = node.Children[i - 1];
                        prevNode.Name = "public ";
                        prevNode.Type = SyntaxNode.NodeType.PublicClassModifier;
                        
                        if (acessType == SyntaxNode.AcessType.GetSet)
                        {
                            i = AppendAccessModifier(node, variableNode, i, "{get; set;}");
                        }
                        else if (acessType == SyntaxNode.AcessType.Get)
                        {
                            i = AppendAccessModifier(node, variableNode, i, "{get;}");
                        }
                        else
                        {
                            i = AppendAccessModifier(node, variableNode, i, "{set;}");
                        }
                        variableNode.EndingTrivia = " ";                        
                    }
                }
                if (currentNode.Children.Count > 0)
                {
                    PostProcessing(currentNode);
                }
            }
        }

        private static int AppendAccessModifier(SyntaxNode node,  SyntaxNode variableNode, int index, string toAppend)
        {
            var newNode = new SyntaxNode(SyntaxNode.NodeType.CSharpModification, toAppend);
            newNode.EndingTrivia = variableNode.EndingTrivia;
            variableNode.EndingTrivia = "";
            newNode.Parent = node;
            node.Children.Insert(index + 2, newNode);
            index++;
            return index;
        }
    }
}
