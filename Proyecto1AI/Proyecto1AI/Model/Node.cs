using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Model
{
    class Node
    {
        public int X;
        public int Y;
        public double H = 0.0;
        public Node Parent;
        public int index = 0;
        public Boolean solution = false;
    }
}
