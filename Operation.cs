using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhangShashaCSharp
{
    public class Operation
    {
        public enum Op { Insert, Delete, Change };
        public Op O;
        public Node N1;
        public Node N2;
    }
}
