using Microsoft.CognitiveServices.SpeechRecognition;
using System.Configuration;
using System;
using System.Media;
using System.IO;
using System.Threading;
using Proyecto1AI.Model;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Proyecto1AI.View;
using System.Text.RegularExpressions;

namespace Proyecto1AI.Controller
{
    public class SpeechRecognition
    {
        private MicrophoneRecognitionClient Microphone;
        private string RecognitionLanguage;
        private string LuisEndpointURL;
        private string SpeechAPISubscriptionKey;
        private Authentication AuthorizationToken;
        private Board Board;
        private principalWindown MainFrame;
        int m = -1, n = -1, a = -1;

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor:
        // Read the values from the configuration file and set them in the variables
        public SpeechRecognition()
        {
            RecognitionLanguage = ConfigurationManager.AppSettings["RecognitionLanguage"];
            LuisEndpointURL = ConfigurationManager.AppSettings["LuisEndpointURL"];
            SpeechAPISubscriptionKey = ConfigurationManager.AppSettings["SpeechAPISubscriptionKey"];
            AuthorizationToken = new Authentication();
            CreateMicrophoneRecoClientWithIntent();


            Board = new Board("Paché", 6, 5, 5);
            Board.Show();
            MainFrame = new principalWindown(Board);
            MainFrame.ShowDialog();
            //MainFrame.DrawBestPath();

        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                Console.WriteLine("No phrase response is available.");
            }
            else
            {
                Console.WriteLine("********* Final n-BEST Results *********");
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {
                    Console.WriteLine(
                        "[{0}] Confidence={1}, Text=\"{2}\"",
                        i,
                        e.PhraseResponse.Results[i].Confidence,
                        e.PhraseResponse.Results[i].DisplayText);
                }

                Console.WriteLine();
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        void OnIntentHandler(object sender, SpeechIntentEventArgs e)
        {
            Console.WriteLine("--- Intent received by OnIntentHandler() ---");
            Console.WriteLine("{0}", e.Payload);

            LuisAPIAnswer answer = new JavaScriptSerializer().Deserialize<LuisAPIAnswer>(e.Payload);

            if (!answer.topScoringIntent.Equals("None"))
            {
                MainFrame.CleanPath();
                switch (answer.topScoringIntent.intent)
                {
                    case "Movement":
                        MovementEntityHandler(answer.entities);
                        break;
                    case "ChangeObjectPosition":
                        ChangeObjectPositionHandler(answer.entities);
                        break;
                    case "Questions":
                        QuestionsHandler(answer.entities);
                        break;
                    case "InputParameters":
                        InputParametersHandler(answer.entities);
                        break;
                    case "None":
                        TextToSpeech("No entiendo lo que quieres decir. Intenta de nuevo.");
                        break;
                }
            }
            else
                TextToSpeech("Esa acción no la conozco. Intenta de nuevo!");
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        private void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            Console.WriteLine("--- Microphone status change received by OnMicrophoneStatus() ---");
            Console.WriteLine("********* Microphone status: {0} *********", e.Recording);
            if (e.Recording)
            {
                Console.WriteLine("Please start speaking.");
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            Console.WriteLine("--- Partial result received by OnPartialResponseReceivedHandler() ---");
            Console.WriteLine("{0}", e.PartialResult);
            Console.WriteLine();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Console.WriteLine("--- OnMicShortPhraseResponseReceivedHandler ---");

            // we got the final result, so it we can end the mic reco.  No need to do this
            // for dataReco, since we already called endAudio() on it as soon as we were done
            // sending all the data.
            Microphone.EndMicAndRecognition();

            WriteResponseResult(e);

            CreateMicrophoneRecoClientWithIntent();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
            Console.WriteLine("--- Error received by OnConversationErrorHandler() ---");
            Console.WriteLine("Error code: {0}", e.SpeechErrorCode.ToString());
            Console.WriteLine("Error text: {0}", e.SpeechErrorText);
            Console.WriteLine();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Starts the microphone and set events that are wroking until the user stops
        public void CreateMicrophoneRecoClientWithIntent()
        {
            Console.WriteLine("--- Start microphone dictation with Intent detection ----");

            Microphone =
                SpeechRecognitionServiceFactory.CreateMicrophoneClientWithIntentUsingEndpointUrl(
                    RecognitionLanguage,
                    SpeechAPISubscriptionKey,
                    LuisEndpointURL);

            Microphone.OnIntent += OnIntentHandler;

            // Event handlers for speech recognition results
            //micClient.OnMicrophoneStatus += OnMicrophoneStatus;
            Microphone.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            Microphone.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            Microphone.OnConversationError += OnConversationErrorHandler;

            Microphone.StartMicAndRecognition();

        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handler that conert from text to voice and reproduce it
        public void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            SoundPlayer player = new SoundPlayer(args.EventData);

            player.PlaySync();
            args.EventData.Dispose();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handler to manage errors during the process of TTS
        private static void ErrorHandler(object sender, GenericEventArgs<Exception> e)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", e.ToString());
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Send the text to the API in order to generate the WAV file to reproduce
        public void TextToSpeech(string InputText)
        {
            Synthesize Cortana = new Synthesize();
            
            Cortana.OnAudioAvailable += PlayAudio;
            Cortana.OnError += ErrorHandler;

            // Reuse Synthesize object to minimize latency
            Cortana.Speak(CancellationToken.None, new InputOptions()
            {
                Text = InputText,
                AuthorizationToken = "Bearer " + AuthorizationToken.AccessToken,
            }).Wait();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handle Movement entity
        public void MovementEntityHandler(List<Entity> Entities)
        {
            string TextDirection = "";
            AgentMovement FinalDirection;

            // Verify if the movement is diagonal
            foreach (Entity entity in Entities)
            {
                if (entity.type.Equals("DireccionDiagonal"))
                    TextDirection = entity.entity;
            }

            // TextDirection is empty, which means it is not a diagonal.
            if (TextDirection.Equals(""))
            {
                foreach (Entity entity in Entities)
                {
                    if (entity.type.Equals("Direccion"))
                        TextDirection = entity.entity;
                }
            }

            switch (TextDirection)
            {
                case "arriba":
                    FinalDirection = AgentMovement.Up;
                    break;
                case "abajo":
                    FinalDirection = AgentMovement.Down;
                    break;
                case "izquierda":
                    FinalDirection = AgentMovement.Left;
                    break;
                case "derecha":
                    FinalDirection = AgentMovement.Right;
                    break;
                case "arriba derecha":
                case "derecha arriba":
                    FinalDirection = AgentMovement.UpRight;
                    break;
                case "arriba izquierda":
                case "izquierda arriba":
                    FinalDirection = AgentMovement.UpLeft;
                    break;
                case "abajo derecha":
                case "derecha abajo":
                    FinalDirection = AgentMovement.DownRight;
                    break;
                case "abajo izquierda":
                case "izquierda abajo":
                    FinalDirection = AgentMovement.DownLeft;
                    break;
                default:
                    TextToSpeech("No reconozco la dirección a la que quieres ir.");
                    return;
            }

            // Save the previous position in order to clear the draw
            Node PreviousAgentPosition = new Node()
            {
                X = Board.Agent.Position.Item1,
                Y = Board.Agent.Position.Item2
            };
            Node NewPosition = Board.CreateMovement(FinalDirection);

            if (Board.IsValidMovement(NewPosition))
            {
                List<Node> positions = new List<Node>();
                positions.Add(PreviousAgentPosition);
                positions.Add(NewPosition);

                MainFrame.UpdateItemPosition(positions);
                Board.Show();
                TextToSpeech("Me he movido!");
                Board.MoveAgent(FinalDirection);
            }
            else
                TextToSpeech("No puedo moverme a esa posición, está en uso!");
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handle Movement entity
        public void ChangeObjectPositionHandler(List<Entity> Entities)
        {
            string ObjectiveText = "";
            string Origin = "";
            string Destiny = "";
            Node OriginPositions;
            Node DestinyPositions;

            // Extract the entities
            foreach (Entity entity in Entities)
            {
                switch (entity.type)
                {
                    case "objetivo":
                        ObjectiveText = entity.entity;
                        break;
                    case "Desde":
                        Origin = entity.entity;
                        break;
                    case "Hacia":
                        Destiny = entity.entity;
                        break;
                }
            }

            List<Node> Positions = new List<Node>();
            Node ActualPosition;
            switch (ObjectiveText)
            {
                // Change agent's position
                case "agente":
                case "gente":
                    ActualPosition = new Node()
                    {
                        X = Board.Agent.Position.Item1,
                        Y = Board.Agent.Position.Item2
                    };
                    DestinyPositions = ExtractCoordinates(Destiny, "destino");

                    if (Board.IsValidMovement(DestinyPositions))
                    {
                        Positions.Add(ActualPosition);
                        Positions.Add(DestinyPositions);
                        MainFrame.UpdateItemPosition(Positions);
                        Board.ChangeAgentPosition(DestinyPositions);
                        TextToSpeech("¡Wow!, ¿me has teletransportado?");
                    }
                    else
                        TextToSpeech("No puedo moverme " + Destiny + ". Fíjate que esté libre y dentro del tablero.");
                    break;

                // Change agent's goal
                case "meta":
                    ActualPosition = new Node()
                    {
                        X = Board.Agent.Goal.Item1,
                        Y = Board.Agent.Goal.Item2
                    };
                    DestinyPositions = ExtractCoordinates(Destiny, "destino");

                    if (Board.IsValidMovement(DestinyPositions))
                    {
                        Positions.Add(ActualPosition);
                        Positions.Add(DestinyPositions);
                        MainFrame.UpdateItemPosition(Positions);
                        Board.ChangeAgentGoal(DestinyPositions);
                        TextToSpeech("¡Vaya!, ¡que indeciso eres!");
                    }
                    else
                        TextToSpeech("No podemos ir " + Destiny + ". Fíjate que esté libre y dentro del tablero.");
                    break;
                case "obstáculo":
                case "obstáculos":
                    // This means delete object
                    if (!Origin.Equals("") && Destiny.Equals(""))
                    {
                        OriginPositions = ExtractCoordinates(Origin, "origen");
                        if (Board.DeleteObstacle(new Tuple<int, int>(OriginPositions.X, OriginPositions.Y)))
                        {
                            MainFrame.UpDateMatrix(OriginPositions.X, OriginPositions.Y, 0);
                            TextToSpeech("¡Bien! Tengo más claro mi camino!");
                        }
                        else
                            TextToSpeech("No hay obstáculo en esa posición");
                    }
                    // Add obstacle
                    else if (Origin.Equals("") && !Destiny.Equals(""))
                    {
                        DestinyPositions = ExtractCoordinates(Destiny, "destino");
                        if (Board.AddObstacle(DestinyPositions))
                        {
                            MainFrame.UpDateMatrix(DestinyPositions.X, DestinyPositions.Y, 1);
                            TextToSpeech("¡Demonios! Ahora la tengo más difícil!");
                        }
                    }
                    break;
            }            
            Board.Show();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handle Movement entity
        public void QuestionsHandler(List<Entity> Entities)
        {
            string ObjectiveText = "";

            // Extract the entities
            foreach (Entity entity in Entities)
            {
                switch (entity.type)
                {
                    case "objetivo":
                        ObjectiveText = entity.entity;
                        break;
                }
            }

            switch (ObjectiveText)
            {
                // Change agent's position
                case "en diagonal":
                    if (Board.IsDiagonal)
                        TextToSpeech("Sí puedo!");
                    else
                        TextToSpeech("Sí puedo, pero me lo tienes prohibido.");
                    break;
                case "tu nombre":
                    TextToSpeech("Me llamo " + Board.Agent.Name);
                    break;
            }

            Board.Show();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Handles the InputParameters entity
        public void InputParametersHandler(List<Entity> Entities)
        {
            string ObjectiveText = "";
            string Decimal = "@";

            // Extract the entities
            foreach (Entity entity in Entities)
            {
                switch (entity.type)
                {
                    case "objetivo":
                        ObjectiveText = entity.entity;
                        break;
                    case "igual":
                        Decimal = entity.entity;
                        break;
                }
            }

            bool result = false;
            switch (ObjectiveText)
            {
                // Change agent's position
                case "crear tablero":
                    if (m > 0 && n > 0 && a > 0)
                    {
                        Board = new Board("Pache", m, n, a);
                        MainFrame.DisposeFrame();
                        MainFrame = new principalWindown(Board);
                        TextToSpeech("Tablero de " + m + " por " + n + " con cuadrículas de tamaño " + a + "creado!");
                        MainFrame.ShowDialog();
                    }
                    else
                    {
                        string ErrorMessage = "Me falta conocer ";
                        if (m == -1)
                            ErrorMessage += "el largo";
                        if (n == -1)
                            ErrorMessage += "el ancho";
                        if (a == -1)
                            ErrorMessage += "el tamaño de cada cuadrícula";
                        TextToSpeech(ErrorMessage + " para poder construirlo.");
                    }
                    break;
                case "tamaño":
                    result = Int32.TryParse(Regex.Match(Decimal, @"\d+").Value, out a);
                    if (!result || a == 0)
                        a = -1;
                    else
                        TextToSpeech("Listo.Ahora el tamaño de cada cuadícula es de ." + a);
                    break;
                case "largo":
                    result = Int32.TryParse(Regex.Match(Decimal, @"\d+").Value, out m);
                    if (!result || m == 0)
                        m = -1;
                    else
                        TextToSpeech("Listo.Ahora el largo es de ." + m);
                    break;
                case "ancho":
                    result = Int32.TryParse(Regex.Match(Decimal, @"\d+").Value, out n);
                    if (!result || n == 0)
                        a = -1;
                    else
                        TextToSpeech("Listo.Ahora el ancho es de ." + n);
                    break;
                case "mejor ruta":
                    if (Board.ShortestPath() == null)
                        TextToSpeech("Rayos, estoy encerrado. No hay ruta a la meta fijada!");
                    else
                        TextToSpeech("¡Hostia, llegamos a la meta!");
                    MainFrame.DrawBestPath();
                    break;
                case "limpiar tablero":
                    MainFrame.CleanPath();
                    break;
                case "cambiar diagonal":
                    if (Board.IsDiagonal)
                    {
                        Board.IsDiagonal = false;
                        TextToSpeech("¿Qué demonios? La tienes contra mí?.");
                    }
                    else
                    {
                        Board.IsDiagonal = true;
                        TextToSpeech("Bien! Ahora me será más fácil llegar a la meta.");
                    }
                    break;
            }

            Board.Show();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Extract two decimals from a string and return it
        private Node ExtractCoordinates(string input, string Type)
        {
            MatchCollection toPositions = Regex.Matches(input, @"\d+");
            Node DestinyPositions = new Node();
            try
            {
                DestinyPositions.X = Int32.Parse(toPositions[0].Value) - 1;
                DestinyPositions.Y = Int32.Parse(toPositions[1].Value) - 1;
            }
            catch (ArgumentNullException e)
            {
                TextToSpeech("No has ingresado la posición de " + Type + ". Prueba con 1,2 por ejemplo.");
            }
            catch (FormatException e)
            {
                TextToSpeech("No he podido entender la posición de " + Type + ". Prueba con 3,4 por ejemplo.");
            }
            catch (Exception e)
            {
                DestinyPositions.X = -1;
                DestinyPositions.Y = -1;
            }
            return DestinyPositions;
        }
    }
}
