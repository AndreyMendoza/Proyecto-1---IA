using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Model
{
    class Agent : BoardItem
    {
        public Tuple<int, int> Goal { get; set; }
        public string Name { get; set; }
    }
}
