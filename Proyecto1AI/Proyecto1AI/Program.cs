using Proyecto1AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Testing the board initialization
            string AgentName = "Andrey";
            int m = 10;
            int n = 10;
            int a = 10;

            Board board = new Board(AgentName, m, n , a);

            board.Show();



            int x = Int32.Parse(Console.ReadLine());
            int y = Int32.Parse(Console.ReadLine());

            

            board.ChangeAgentGoal(new Tuple<int, int>(x, y));
            board.Show();

            Console.ReadKey();
            
        }
    }
}
