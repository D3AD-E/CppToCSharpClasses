using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CPPtoCSharpClasses.SyntaxNode;

namespace CPPtoCSharpClasses
{
    internal class FileState
    {
        public Dictionary<string, AcessType> GettersAndSettersEncounters { get; set; }

        public FileState()
        {
            GettersAndSettersEncounters = new Dictionary<string, AcessType>();
        }
    }
}
