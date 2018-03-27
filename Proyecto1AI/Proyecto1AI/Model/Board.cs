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
        public bool AddObstacle(Tuple<int, int> NewPosition)
        {
            if (IsValidMovement(NewPosition))
            {
                BoardItem NewObstacle = new BoardItem
                {
                    Position = NewPosition,
                    Path = "ruta"
                };
                BoardMatrix[NewPosition.Item1, NewPosition.Item2] = (int)BoardPositionStatus.Obstacle;
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
        public bool ChangeAgentPosition(Tuple<int, int> Position)
        {
            // Updates the board with the Agent's position
            if (IsValidMovement(Position))
            {
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Position = Position;
                BoardMatrix[Agent.Position.Item1, Agent.Position.Item2] = (int)BoardPositionStatus.Agent;

                return true;
            }
            return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Changes Agent's goal position
        public bool ChangeAgentGoal(Tuple<int, int> Position)
        {
            // Updates the board with the Agent's position
            if (IsValidMovement(Position))
            {
                BoardMatrix[Agent.Goal.Item1, Agent.Goal.Item2] = (int)BoardPositionStatus.Empty;
                Agent.Goal = Position;
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
            Double nObstacles = Math.Ceiling(((Size.Item1 + 1) * (Size.Item2 + 1)) * 0.25);

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
            Tuple<int, int> MovementPosition = CreateMovement(Direction);

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

        // Generate the new position tuple from the actual position
        public Tuple<int, int> CreateMovement(AgentMovement Direction)
        {
            Tuple<int, int> MovementPosition = new Tuple<int, int>(-1, -1);

            // Create the tuple with the goal direction
            switch (Direction)
            {
                case AgentMovement.Up:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2);
                    break;
                case AgentMovement.Down:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2);
                    break;
                case AgentMovement.Right:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1, Agent.Position.Item2 + 1);
                    break;
                case AgentMovement.Left:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1, Agent.Position.Item2 - 1);
                    break;
                case AgentMovement.UpRight:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2 + 1);
                    break;
                case AgentMovement.UpLeft:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 - 1, Agent.Position.Item2 - 1);
                    break;
                case AgentMovement.DownRight:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2 + 1);
                    break;
                case AgentMovement.DownLeft:
                    MovementPosition = new Tuple<int, int>(Agent.Position.Item1 + 1, Agent.Position.Item2 - 1);
                    break;
            }

            return MovementPosition;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Verifies if a movement is valid
        public bool IsValidMovement(Tuple<int, int> Goal)
        {
            if ((0 <= Goal.Item1 && Goal.Item1 <= Size.Item1) && (0 <= Goal.Item2 && Goal.Item2 <= Size.Item2)
                && BoardMatrix[Goal.Item1, Goal.Item2] == (int)BoardPositionStatus.Empty)
                return true;
            else
                return false;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Calculates the shortest path
        public void ShortestPath()
        {
            //Keeps the track of each node (Whether already inspected or not)
            var openedNodes = new List<Tuple<int, int, double>>();
            var closedNodes = new List<Tuple<int, int, double>>();
            //Here, the result of the algorithm  will be saved
            var bestRoute = new List<Tuple<int, int, double>>();
            //Saves the "virtual" position of the agent while the algorithm is running
            var actualNode = Agent.Position;
            //Saves whether keeps serching or not.
            var alreadyFindedAGoal = false;
            //Add the initial position to the already opened list
            closedNodes.Add(new Tuple<int, int, double>(actualNode.Item1, actualNode.Item2, 0.0));

            while (!alreadyFindedAGoal) {
                //The goal has been reached, it's time to stop :)
                if (VerifyGoal(actualNode)) {
                    Console.WriteLine("A valid path was found. YEY!");
                    alreadyFindedAGoal = true;
                    break;
                }
                //get the adjacent nodes of the actualNode and keeps the f(n) value for every valid node in this iteration
                var adjacents = GetAdjacents(actualNode);

                //Calls the function that calculates f(n)=h(n)+g(n) for every valid path
                adjacents = AStar(adjacents);

                //Verifies if i has already been opened or closed
                foreach(Tuple < int, int, double> i in adjacents)
                {
                    int index;
                    index = openedNodes.FindIndex(t => t.Item1 == i.Item1 && t.Item2 == i.Item2);
                    //Looks if the found cell has been already opened
                    if (index > -1)
                    {
                        //If so, looks if the new one is a better choice
                        if (i.Item3 < openedNodes[index].Item3)
                        {
                            //Yes, it is, so now is added to the openedNodes list 
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
                        index = closedNodes.FindIndex(t => t.Item1 == i.Item1 && t.Item2 == i.Item2);
                        if (index < 0) {
                            //If not, it's added to the openedNodes list
                            openedNodes.Add(i);
                        }
                    }
                    //Now, the list is sorted in order to have the best choice at index 0
                    openedNodes = openedNodes.OrderBy(t => t.Item3).ToList();
                }
                //There are no more options, it's time to stop :(
                if (openedNodes.Count() == 0) {
                    Console.WriteLine("No valid path was found :(");
                    break;
                }
                //Select the best node to continue
                actualNode = new Tuple<int, int>(openedNodes[0].Item1, openedNodes[0].Item2);
                //Rewrites the best route
                while (bestRoute.Count>0)
                {
                    //If the new cell is way too far from the last inserted cell in bestRoute, it means that a rollback was made
                    if (Math.Abs(bestRoute[bestRoute.Count - 1].Item1 - openedNodes[0].Item1) > 1 || Math.Abs(bestRoute[bestRoute.Count - 1].Item2 - openedNodes[0].Item2) > 1){
                        bestRoute.RemoveAt(bestRoute.Count - 1);
                    }
                    //The cell could be near, but if the movement wasn't made diagonally, it was 'cuz it was illegal. 
                    else{
                        var cellAdjacents = GetAdjacents(new Tuple<int, int>(bestRoute[bestRoute.Count - 1].Item1, bestRoute[bestRoute.Count - 1].Item2));
                        var index = cellAdjacents.FindIndex(t => t.Item1 == openedNodes[0].Item1 && t.Item2 == openedNodes[0].Item2);
                        //The movement was illegal, the traceback keeps going on.
                        if (index < 0) { bestRoute.RemoveAt(bestRoute.Count - 1); }
                        //The movement was legal, it's time to continue.
                        else { break; }
                    }
                        
                }
                bestRoute.Add(openedNodes[0]);
                //Now the node is added to the closedNodes list 'cuz it's has been already visited, thus, is deleted from openedNodes list.
                closedNodes.Add(openedNodes[0]);
                openedNodes.RemoveAt(0);
            }

            Console.WriteLine("SOLUTION:");
            foreach (Tuple<int, int, double> i in bestRoute) {
                Console.WriteLine("(" + i.Item1 + "," + i.Item2 + ") => "+i.Item3);
            }
            return;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        //Calculates the adjacent nodes of a cell
        public List<Tuple<int, int, double>> GetAdjacents(Tuple<int, int> cell)
        {
            //Possible movement pattern
            var directions = new List<Tuple<int, int, double>>()
            {
                    Tuple.Create(+1,-0, 0.0),
                    Tuple.Create(-1,+0, 0.0),
                    Tuple.Create(-0,+1, 0.0),
                    Tuple.Create(+0,-1, 0.0),
                    Tuple.Create(+1,+1, 1.0),
                    Tuple.Create(-1,-1, 1.0),
                    Tuple.Create(+1,-1, 1.0),
                    Tuple.Create(-1,+1, 1.0)
            };

            //Possible movements (Without collision & blocked diagonals)
            var adjacents = new List<Tuple<int, int, double>>();
            //Used to dertermine the blocked diagonals
            var invalidAdjacents = new List<Tuple<int, int>>();
            //Movements without collitions
            foreach (Tuple<int, int, double> i in directions)
            {
                var adjacent = new Tuple<int, int>(cell.Item1 + i.Item1, cell.Item2 + i.Item2);
                //Does not exclude the GOAL node if is found
                if (IsValidMovement(adjacent)||VerifyGoal(adjacent))
                {
                    adjacents.Add(new Tuple<int, int, double>(cell.Item1 + i.Item1, cell.Item2 + i.Item2, i.Item3));
                }
                else {
                    invalidAdjacents.Add(new Tuple<int, int>(adjacent.Item1, adjacent.Item2));
                }
            }
            //Deletes the blocked diagonals from the valid adjacents
            foreach(Tuple < int, int > invalid in invalidAdjacents) {
                for (int limit = 0; limit< 4; limit++) {
                    //Takes the invalid adjacents and sizes up the posible invalid diagonals 
                    var excluded = new Tuple<int, int>(directions[limit].Item1 + invalid.Item1, directions[limit].Item2 + invalid.Item2);

                    var index = adjacents.FindIndex(t => t.Item1 == excluded.Item1 && t.Item2 == excluded.Item2);
                    //If excluded cell shares a side (top,bottom,right or left) with a diagonal of the actual position, it is deleted.
                    if (index > -1 && adjacents[index].Item3>0) { adjacents.RemoveAt(index); }
                }

            }
            return adjacents;

        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        //Calculates f(n)=h(n)+g(n) 
        public List<Tuple<int, int, double>> AStar(List<Tuple<int, int, double>> paths)
        {
            var result = new List<Tuple<int, int, double>>();
            var cathetus = ItemSize;
            var hypotenuse = Math.Sqrt(2 * (cathetus * cathetus));

            foreach (Tuple<int, int, double> i in paths)
            {
                //Sizes the heuristic up
                var heuristic = (Math.Abs(Agent.Goal.Item1 - i.Item1) + Math.Abs(Agent.Goal.Item2 - i.Item2))*10;

                //Sums the cost up
                var cost = 0.0;
                if (i.Item3 == 0.0) { cost = cathetus; }
                else { cost = hypotenuse; }

                //Inserts the sized up path
                result.Add(new Tuple<int, int, double>(i.Item1, i.Item2, heuristic + cost));
            }
            return result;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Verifies if the cell is already the goal.
        public bool VerifyGoal(Tuple<int,int> cell)
        {
            if (cell.Item1 == Agent.Goal.Item1 && cell.Item2 == Agent.Goal.Item2) { return true; }
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
