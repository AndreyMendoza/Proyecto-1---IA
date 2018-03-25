using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Model
{
    // Identifies the status of a BoardMatrix entry
    public enum BoardPositionStatus
    {
        Empty = 0,       
        Obstacle = 1,        
        Agent = 2,
        AgentGoal = 3
    }
}
