using Proyecto1AI.Controller;
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
            principalWindown PrincipalWindown = new principalWindown();

            Console.Write(board.BoardMatrix);


            PrincipalWindown.ShowDialog();
            
            PrincipalWindown.DrawBestPath();
            //PrincipalWindown.CleanPath();



        }
    }
}
