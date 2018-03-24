using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Model
{
    class Board
    {
        public List<BoardItem> Obstacles { get; set; } = new List<BoardItem>();
        public Agent Agent { get; set; } = new Agent();
        public Tuple<int, int> Size { get; set; }
        public int ItemSize { get; set; }
        public int[,] BoardMatrix { get; set; }


        // Constructor
        public Board(string AgentName, int ItemSizeIn, int Width, int Height)
        {
            Agent.Name = AgentName;
            ItemSize = ItemSizeIn;
            Size = new Tuple<int, int>(Width, Height);
            BoardMatrix = new int[Width, Height];

            InitBoardMatrix();
        }

// ----------------------------------------------------------------------------------------------------------------------------------------

        // Fills the Matrix with 1s and 0s(obstacles)
        private void InitBoardMatrix()
        {
            // First, init ALL the board with 1s
            for (int i = 0; i < Size.Item1; i++)
            {
                for (int j = 0; j < Size.Item2; j++)
                {
                    BoardMatrix[i, j] = 1;
                }
            }

            // Set 0s for each obstacle
            foreach (BoardItem obstacle in Obstacles)
            {
                BoardMatrix[obstacle.Position.Item1, obstacle.Position.Item2] = 0;
            }
        }

// ----------------------------------------------------------------------------------------------------------------------------------------

    }
}
