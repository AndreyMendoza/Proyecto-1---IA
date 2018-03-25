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
        public bool IsDiagonal = false;
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

            // Init random values because it's the first time
            InitBoardMatrix();
            GenerateRandomObstacles();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Initialize Agent Randomly
        private void InitAgentRandomly()
        {
            Agent.Goal = RandomBoardPosition();
            Agent.Position = RandomBoardPosition();
            Agent.Path = "Sonrisas.png";

            BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = 2;
            BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = 3;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Fills the Matrix with 0s and 1s(obstacles)
        private void InitBoardMatrix()
        {
            // First, init ALL the board with 1s
            for (int i = 0; i < Size.Item1; i++)
            {
                for (int j = 0; j < Size.Item2; j++)
                {
                    BoardMatrix[i, j] = 0;
                }
            }

            // Set 0s for each obstacle
            foreach (BoardItem obstacle in Obstacles)
            {
                BoardMatrix[obstacle.Position.Item1, obstacle.Position.Item2] = 1;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Generate random obstacles
        private void GenerateRandomObstacles()
        {
            Double nObstacles = Math.Ceiling((Size.Item1 * Size.Item2) * 0.25);
            
            while (nObstacles >= 0)
            {
                Tuple<int, int> position = RandomBoardPosition();
                BoardItem NewObstacle = new BoardItem
                {
                    Position = position,
                    Path = "ruta"
                };
                BoardMatrix[position.Item1, position.Item2] = 1;
                Obstacles.Add(NewObstacle);
                nObstacles--;
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Generates a random position in the board
        private Tuple<int, int> RandomBoardPosition()
        {
            Random Roulette = new Random();
            int Row = Roulette.Next(1, Size.Item1);
            int Column = Roulette.Next(1, Size.Item2);

            while (BoardMatrix[Row, Column] != 0)
            {
                Row = Roulette.Next(1, Size.Item1);
                Column = Roulette.Next(1, Size.Item2);
            }
            return new Tuple<int, int>(Row, Column);
        }

    }
}
