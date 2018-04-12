using Proyecto1AI.Controller;
using Proyecto1AI.Model;
using System;
using System.Threading;

namespace Proyecto1AI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Testing the board initialization
            //string AgentName = "Andrey";
            //int m = 5;
            //int n = 5;
            //int a = 100;

            //Board board = new Board(AgentName, m, n , a);

            //int[,] b = new int[5, 5] {  {2, 0, 1, 0, 3},
            //                            {0, 1, 1, 0, 0},
            //                            {0, 0, 1, 0, 0},
            //                            {0, 0, 1, 0, 0},
            //                            {0, 0, 0, 0, 1}};

            //board.BoardMatrix = b;
            //board.Agent.Position = new Tuple<int, int>(0,0);
            //board.Agent.Goal = new Tuple<int, int>(0, 4);
            //board.IsDiagonal = true;
            //board.Show();



            //int x = Int32.Parse(Console.ReadLine());
            //int y = Int32.Parse(Console.ReadLine());



            //board.ChangeAgentPosition(new Tuple<int, int>(x, y));

            //x = Int32.Parse(Console.ReadLine());
            //y = Int32.Parse(Console.ReadLine());

            //board.ChangeAgentGoal(new Tuple<int, int>(x, y));

            //board.Show();

            //board.ShortestPath();

            //Authentication auth = new Authentication();
            //string accessToken;
            //string requestUri = "https://speech.platform.bing.com/synthesize";
            //var cortana = new Synthesize();

            //try
            //{
            //    accessToken = auth.AccessToken;
            //}
            //catch (Exception ex)
            //{
            //    return;
            //}

            //cortana.OnAudioAvailable += SpeechRecognition.PlayAudio;
            ////cortana.OnError += ErrorHandler;

            //// Reuse Synthesize object to minimize latency
            //cortana.Speak(CancellationToken.None, new InputOptions()
            //{
            //    Text = //"Hello, My name is Smily Boy, please insert the next parameters in order to continue:",//
            //    "Probando la voz de Helena",
            //    RequestUri = new Uri(requestUri),
            //    AuthorizationToken = "Bearer " + accessToken,
            //}).Wait();


            SpeechRecognition mc = new SpeechRecognition();

            mc.TextToSpeech("Hola a todos");

            //Console.ReadKey();
            
        }
    }
}
