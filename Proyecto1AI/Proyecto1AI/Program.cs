﻿using Proyecto1AI.Model;
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
            Board board = new Board("Andrey", 10, 10, 10);

            int[,] BoardMatrix = new int[4, 4];

            BoardMatrix[3, 3] = 5;

            



            Console.ReadKey();
            
        }
    }
}
