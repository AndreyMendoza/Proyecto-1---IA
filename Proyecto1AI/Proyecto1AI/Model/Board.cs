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
        public bool IsDiagonal = true;
        public Tuple<int, int> Size { get; set; }
        public int ItemSize { get; set; }
        public int[,] BoardMatrix { get; set; }

        // Constructor
        public Board(string AgentName, int Height, int Width, int ItemSizeIn)
        {
            Agent.Name = AgentName;
            ItemSize = ItemSizeIn;
            Size = new Tuple<int, int>(Width- 1, Height - 1);
            BoardMatrix = new int[Width, Height];

            // Init random values because it's the first time
            InitBoardMatrix();
            GenerateRandomObstacles();
            InitAgentRandomly();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Initialize Agent Randomly
        private void InitAgentRandomly()
        {
            // Agent Position
            Agent.Position = RandomBoardPosition();
            BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Agent;

            // Goal Position
            Agent.Goal = RandomBoardPosition();
            BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = (int)BoardPositionStatus.AgentGoal;

            Agent.Path = "Sonrisas.png";  
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Add an obstacle to the board
        public bool AddObstacle(Node NewPosition)
        {
            if (IsValidMovement(NewPosition))
            {
                BoardItem NewObstacle = new BoardItem
                {
                    Position = new Tuple<int,int>(NewPosition.X, NewPosition.Y),
                    Path = "ruta"
                };
                BoardMatrix[NewPosition.X, NewPosition.Y] = (int)BoardPositionStatus.Obstacle;
                Obstacles.Add(NewObstacle);

                return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Deletes an Obstacle from the board
        public bool DeleteObstacle(Tuple<int, int> Position)
        {
            if (BoardMatrix[Position.Item1, Position.Item2] == (int)BoardPositionStatus.Obstacle)
            {
                for (int i = 0; i < Obstacles.Count; i++)
                {
                    BoardItem obstacle = Obstacles.ElementAt(i);

                    if (obstacle.Position.Item1 == Position.Item1 && obstacle.Position.Item2 == Position.Item2)
                    {
                        Obstacles.RemoveAt(i);
                        BoardMatrix[Position.Item1, Position.Item2] = (int)BoardPositionStatus.Empty;
                        return true;
                    }
                }
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Changes Agent's Position
        public bool ChangeAgentPosition(Node Position)
        {
            // Updates the board with the Agent's position
            if (IsValidMovement(Position))
            {
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Position = new Tuple<int,int>(Position.X,Position.Y);
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Agent;

                return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Changes Agent's goal position
        public bool ChangeAgentGoal(Node Position)
        {
            // Updates the board with the Agent's position
            if (IsValidMovement(Position))
            {
                BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Goal = new Tuple<int,int>(Position.X, Position.Y);
                BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = (int)BoardPositionStatus.AgentGoal;

                return true;
            }
            return false;
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
                    BoardMatrix[i, j] = (int)BoardPositionStatus.Empty;
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
            Double nObstacles = Math.Ceiling(((Size.Item1 + 1) * (Size.Item2 + 1)) * 0.5);

            while (nObstacles > 0)
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
            int Row = Roulette.Next(0, Size.Item1 + 1);
            int Column = Roulette.Next(0, Size.Item2 + 1);

            while (BoardMatrix[Row, Column] != (int)BoardPositionStatus.Empty)
            {
                Row = Roulette.Next(0, Size.Item1 + 1);
                Column = Roulette.Next(0, Size.Item2 + 1);
            }
            return new Tuple<int, int>(Row, Column);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Moves the agent in the board if possible
        public bool MoveAgent(AgentMovement Direction)
        {
            Node MovementPosition = CreateMovement(Direction);

            // Updates the board with the Agent's position
            if (IsValidMovement(MovementPosition))
            {
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Position = new Tuple<int,int>(MovementPosition.X, MovementPosition.Y);
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Agent;

                return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Generate the new position tuple from the actual position
        public Node CreateMovement(AgentMovement Direction)
        {
            Node MovementPosition = new Node { };

            // Create the tuple with the goal direction
            switch (Direction)
            {
                case AgentMovement.Up:
                    MovementPosition = new Node { X = Agent.Position.Item1 - 1, Y = Agent.Position.Item2 };
                    break;
                case AgentMovement.Down:
                    MovementPosition = new Node { X = Agent.Position.Item1 + 1, Y = Agent.Position.Item2 };
                    break;
                case AgentMovement.Right:
                    MovementPosition = new Node { X = Agent.Position.Item1, Y = Agent.Position.Item2 + 1 };
                    break;
                case AgentMovement.Left:
                    MovementPosition = new Node { X = Agent.Position.Item1, Y = Agent.Position.Item2 - 1 };
                    break;
                case AgentMovement.UpRight:
                    MovementPosition = new Node { X = Agent.Position.Item1 - 1, Y = Agent.Position.Item2 + 1 };
                    break;
                case AgentMovement.UpLeft:
                    MovementPosition = new Node { X = Agent.Position.Item1 - 1, Y = Agent.Position.Item2 - 1 };
                    break;
                case AgentMovement.DownRight:
                    MovementPosition = new Node { X = Agent.Position.Item1 + 1, Y = Agent.Position.Item2 + 1 };
                    break;
                case AgentMovement.DownLeft:
                    MovementPosition = new Node { X = Agent.Position.Item1 + 1, Y = Agent.Position.Item2 - 1 };
                    break;
            }

            return MovementPosition;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Verifies if a movement is valid
        public bool IsValidMovement(Node Goal)
        {
            if ((0 <= Goal.X && Goal.X <= Size.Item1) && (0 <= Goal.Y && Goal.Y <= Size.Item2)
                && BoardMatrix[Goal.X, Goal.Y] == (int)BoardPositionStatus.Empty)
                return true;
            else
                return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Calculates the shortest path
        public Node ShortestPath()
        {
            // Keeps the track of each node (Whether already inspected or not)
            var openedNodes = new List<Node>();
            var closedNodes = new List<Node>();
            // Here, the result of the algorithm  will be saved
            var bestRoute = new List<Node>();
            // Saves the "virtual" position of the agent while the algorithm is running
            var actualNode = new Node { X = Agent.Position.Item1, Y = Agent.Position.Item2 };
            // Saves whether keeps serching or not.
            var alreadyFoundGoal = false;
            // Add the initial position to the already opened list
            closedNodes.Add(actualNode);


            while (!alreadyFoundGoal) {

                //The goal has been reached, it's time to stop :)
                if (VerifyGoal(actualNode))
                {
                    actualNode.solution = true;
                }

                //get the adjacent nodes of the actualNode and keeps the f(n) value for every valid node in this iteration
                var adjacents = GetAdjacents(actualNode);

                //Calls the function that calculates f(n)=h(n)+g(n) for every valid path
                adjacents = AStar(adjacents);

                //Verifies if i has already been opened or closed
                foreach(Node i in adjacents)
                {
                    double parent = 0.0;
                    if (bestRoute.Count > 0) {
                        parent = bestRoute[bestRoute.Count - 1].H;
                    }
                    int index = openedNodes.FindIndex(t => t.X == i.X && t.Y == i.Y);
                    //Looks if the found cell has been already opened
                    if (index > -1)
                    {
                        //If so, looks if the new one is a better choice
                        if (i.H < openedNodes[index].H)
                        {
                            //Yes, it is, so now is added to the openedNodes list 
                            i.Parent = actualNode;
                            i.H = i.H + actualNode.H;
                            i.index += 1;
                            openedNodes.Add(i);
                        }
                        else
                        {
                            //No, it doesn't, so it's added to the closedNodes list
                            closedNodes.Add(i);
                        }
                        //Anyway, the old node has to be removed.
                        openedNodes.RemoveAt(index);
                    }
                    else {
                        //Now looks if the found cell has a spot in the closedNodes list
                        index = closedNodes.FindIndex(t => t.X == i.X && t.Y == i.Y);
                        if (index < 0) {
                            //If not, it's added to the openedNodes list
                            i.Parent = actualNode;
                            i.H = i.H + actualNode.H;
                            i.index += 1;
                            openedNodes.Add(i);
                        }
                    }
                    //Now, the list is sorted in order to have the best choice at index 0
                    openedNodes = openedNodes.OrderBy(t => t.H).ToList();
                }
                //There are no more options, it's time to stop :(
                if (openedNodes.Count() == 0) {
                    Console.WriteLine("No valid path was found :(");
                    break;
                }
                //Select the best node to continue
                actualNode = openedNodes[0];

                bestRoute.Add(openedNodes[0]);
                //Now the node is added to the closedNodes list 'cuz it's has been already visited, thus, is deleted from openedNodes list.
                closedNodes.Add(openedNodes[0]);
                openedNodes.RemoveAt(0);
            }

            bestRoute.OrderBy(t => t.index).ToList();
            foreach (Node node in bestRoute) {
                if (node.solution == true) {
                    return node;
                }
            }
            return null;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        //Calculates the adjacent nodes of a cell
        public List<Node> GetAdjacents(Node cell)
        {
            //Possible movement pattern
            var directions = new List<Tuple<int, int, double>>()
            {
                    Tuple.Create(+1, -0, 0.0),
                    Tuple.Create(-1, +0, 0.0),
                    Tuple.Create(-0, +1, 0.0),
                    Tuple.Create(+0, -1, 0.0),
            };

            // If Diagonal is active, add it to the adjacents
            if (IsDiagonal)
            {
                directions.Add(new Tuple<int, int, double>(+1, +1, 1.0));
                directions.Add(new Tuple<int, int, double>(-1, -1, 1.0));
                directions.Add(new Tuple<int, int, double>(+1, -1, 1.0));
                directions.Add(new Tuple<int, int, double>(-1, +1, 1.0));
            }

            //Possible movements (Without collision & blocked diagonals)
            var adjacents = new List<Node>();
            //Used to dertermine the blocked diagonals
            foreach (Tuple<int, int, double> i in directions)
            {
                var adjacent = new Node { X = cell.X + i.Item1, Y = cell.Y + i.Item2, H = i.Item3 };
                //Does not exclude the GOAL node if is found
                if (IsValidMovement(adjacent) || VerifyGoal(adjacent))
                {
                    adjacents.Add(adjacent);
                }
            }
            return adjacents;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        //Calculates f(n)=h(n)+g(n) 
        public List<Node> AStar(List<Node> paths)
        {
            var result = new List<Node>();
            var cathetus = ItemSize;
            var hypotenuse = Math.Sqrt(2 * (cathetus * cathetus));

            foreach (Node i in paths)
            {
                //Sizes the heuristic up
                var heuristic = (Math.Abs(Agent.Goal.Item1 - i.X) + Math.Abs(Agent.Goal.Item2 - i.Y)) * 10;

                //Sums the cost up
                var cost = 0.0;

                if (i.H == 0.0)
                    cost = cathetus;
                else
                    cost = hypotenuse;

                //Inserts the sized up path
                i.H = heuristic + cost;
                result.Add(i);
            }
            return result;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Verifies if the cell is already the goal.
        public bool VerifyGoal(Node cell)
        {
            if (cell.X == Agent.Goal.Item1 && cell.Y == Agent.Goal.Item2)            
                return true;

            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Print the Board in console
        public void Show()
        {
            Console.WriteLine("\n----------------------------------------------\n");
            for (int i = 0; i <= Size.Item1; i++)
            {
                for (int j = 0; j <= Size.Item2; j++)
                    Console.Write("{0} ",BoardMatrix[i, j]);
                Console.WriteLine();
            }
        }
    }
}
