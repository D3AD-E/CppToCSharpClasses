using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDISE_lab.SyntaxNode;

namespace EDISE_lab
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
