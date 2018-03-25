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

            BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int) BoardPositionStatus.Agent;
            BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = (int)BoardPositionStatus.AgentGoal;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Fills the Matrix with 0s and 1s(obstacles)
        private void InitBoardMatrix()
        {
            // First, init ALL the board with 0s
            for (int i = 0; i <= Size.Item1; i++)
            {
                for (int j = 0; j <= Size.Item2; j++)
                {
                    BoardMatrix[i, j] = (int) BoardPositionStatus.Empty;
                }
            }

            // Set 1s for each obstacle
            foreach (BoardItem obstacle in Obstacles)
            {
                BoardMatrix[obstacle.Position.Item1, obstacle.Position.Item2] = (int)BoardPositionStatus.Obstacle;
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
                BoardMatrix[position.Item1, position.Item2] = (int)BoardPositionStatus.Obstacle;
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

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Moves the agent in the board if possible
        public bool MoveAgent(AgentMovement Direction)
        {
            Tuple<int, int> MovementPosition = new Tuple<int, int>(-1, -1);

            // Create the tuple with the goal direction
            switch (Direction)
            {
                case AgentMovement.Up:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1, Agent.Position.Item2 - 1);
                    break;
                case AgentMovement.Down:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1, Agent.Position.Item2 + 1);
                    break;
                case AgentMovement.Right:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2);
                    break;
                case AgentMovement.Left:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2);
                    break;
                case AgentMovement.UpRight:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2 - 1);
                    break;
                case AgentMovement.UpLeft:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2 - 1);
                    break;
                case AgentMovement.DownRight:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2 + 1);
                    break;
                case AgentMovement.DownLeft:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2 + 1);
                    break;
            }

            // Updates the board with the Agent's position
            if (IsValidMovement(MovementPosition))
            {
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Position = MovementPosition;
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Agent;

                return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------
        
        // Verifies if a movement is valid
        public bool IsValidMovement(Tuple<int, int> Goal)
        {
            if (Size.Item1 <= Goal.Item1 && Size.Item2 <= Goal.Item2
                && BoardMatrix[Goal.Item1, Goal.Item2] == (int)BoardPositionStatus.Empty)
                return true;
            else
                return false;
        }
    }
}
