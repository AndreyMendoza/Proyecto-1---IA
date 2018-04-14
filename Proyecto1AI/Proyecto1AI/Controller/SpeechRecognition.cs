using Microsoft.CognitiveServices.SpeechRecognition;
using System.Configuration;
using System;
using System.Media;
using System.IO;
using System.Threading;
using Proyecto1AI.Model;

namespace Proyecto1AI.Controller
{
    class SpeechRecognition
    {
        private MicrophoneRecognitionClient Microphone;
        private string RecognitionLanguage;
        private string LuisEndpointURL;
        private string SpeechAPISubscriptionKey;
        Authentication AuthorizationToken;

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor:
        // Read the values from the configuration file and set them in the variables
        public SpeechRecognition()
        {
            RecognitionLanguage = ConfigurationManager.AppSettings["RecognitionLanguage"];
            LuisEndpointURL = ConfigurationManager.AppSettings["LuisEndpointURL"];
            SpeechAPISubscriptionKey = ConfigurationManager.AppSettings["SpeechAPISubscriptionKey"];
            AuthorizationToken = new Authentication();
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
            Console.WriteLine();
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
                    "es-mx",
                    "dde02807d8774dda941ad6b233e7f983", // Subscription Key
                    "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/35d7e441-af3b-4d93-851c-b6b61f44306b?subscription-key=7c8d53150eef43278ac4f89b956c06e8&verbose=true&timezoneOffset=0&q=");

            Microphone.AuthenticationUri = "";
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
        public static void PlayAudio(object sender, GenericEventArgs<Stream> args)
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

    }
}
