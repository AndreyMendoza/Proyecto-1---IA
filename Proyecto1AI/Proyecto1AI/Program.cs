using Proyecto1AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto1AI.View;

namespace Proyecto1AI
{
    class Program
    {

        static void Main()
        {
            Board board = new Board("Paché", 5, 5, 5);
            principalWindown principalWindown = new principalWindown();

            Console.Write(board.BoardMatrix);


            principalWindown.ShowDialog();

            // Testing the board initialization
            /*
            string AgentName = "Andrey";
            int m = 5;
            int n = 5;
            int a = 100;

            Board board = new Board(AgentName, m, n , a);

            int[,] b = new int[5, 5] {  {2, 0, 1, 0, 3},
                                        {0, 1, 1, 0, 0},
                                        {0, 0, 1, 0, 0},
                                        {0, 0, 1, 0, 0},
                                        {0, 0, 0, 0, 1}};

            board.BoardMatrix = b;
            board.Agent.Position = new Tuple<int, int>(0,0);
            board.Agent.Goal = new Tuple<int, int>(0, 4);
            board.IsDiagonal = true;
            board.Show();



            int x = Int32.Parse(Console.ReadLine());
            int y = Int32.Parse(Console.ReadLine());

            

            board.ChangeAgentPosition(new Tuple<int, int>(x, y));

            x = Int32.Parse(Console.ReadLine());
            y = Int32.Parse(Console.ReadLine());

            board.ChangeAgentGoal(new Tuple<int, int>(x, y));

            board.Show();

            board.ShortestPath();

            Console.ReadKey();
            */

        }
    }
}
