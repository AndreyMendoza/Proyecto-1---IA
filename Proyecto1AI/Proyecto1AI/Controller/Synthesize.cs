//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/Cognitive-Speech-TTS
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.


using Proyecto1AI.Model;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Proyecto1AI.Controller
{
    class Synthesize
    {

        private HttpClient Client;
        private HttpClientHandler Handler;
        public event EventHandler<GenericEventArgs<Stream>> OnAudioAvailable;
        public event EventHandler<GenericEventArgs<Exception>> OnError;

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor
        public Synthesize()
        {
            var cookieContainer = new System.Net.CookieContainer();
            Handler = new HttpClientHandler() { CookieContainer = new CookieContainer(), UseProxy = false };
            Client = new HttpClient(Handler);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // SSML: Speech Synthesis Markup Language. It's the specification used to generate a better understand audio.
        private string GenerateSSML(string locale, string gender, string name, string text)
        {
            var ssmlDoc = new XDocument(
                              new XElement("speak",
                                  new XAttribute("version", "1.0"),
                                  new XAttribute(XNamespace.Xml + "lang", "es-ES"),
                                  new XElement("voice",
                                      new XAttribute(XNamespace.Xml + "lang", locale),
                                      new XAttribute(XNamespace.Xml + "gender", gender),
                                      new XAttribute("name", name),
                                      text)));
            return ssmlDoc.ToString();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------
        
        // Sends the specified text to be spoken to the TTS service and saves the response audio to a file.
        public Task Speak(CancellationToken cancellationToken, InputOptions inputOptions)
        {
            var GenderValue = "";

            Client.DefaultRequestHeaders.Clear();
            foreach (var header in inputOptions.Headers)
            {
                Client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            
            switch (inputOptions.VoiceType)
            {
                case VoiceGender.Male:
                    GenderValue = "Male";
                    break;

                case VoiceGender.Female:
                default:
                    GenderValue = "Female";
                    break;
            }


            // Generates the HTTP request based in the input options and the SSML
            var request = new HttpRequestMessage(HttpMethod.Post, inputOptions.RequestUri)
            {
                Content = 
                    new StringContent(
                            GenerateSSML(
                                    inputOptions.RecognitionLanguage, 
                                    GenderValue, 
                                    inputOptions.VoiceName, 
                                    inputOptions.Text))
            };

            var httpTask = Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            Console.WriteLine("Response status code: [{0}]", httpTask.Result.StatusCode);

            var saveTask = httpTask.ContinueWith(
                async (responseMessage, token) =>
                {
                    try
                    {
                        if (responseMessage.IsCompleted && responseMessage.Result != null && responseMessage.Result.IsSuccessStatusCode)
                        {
                            var httpStream = await responseMessage.Result.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            this.AudioAvailable(new GenericEventArgs<Stream>(httpStream));
                        }
                        else
                        {
                            this.Error(new GenericEventArgs<Exception>(new Exception(String.Format("Service returned {0}", responseMessage.Result.StatusCode))));
                        }
                    }
                    catch (Exception e)
                    {
                        this.Error(new GenericEventArgs<Exception>(e.GetBaseException()));
                    }
                    finally
                    {
                        responseMessage.Dispose();
                        request.Dispose();
                    }
                },
                TaskContinuationOptions.AttachedToParent,
                cancellationToken);

            return saveTask;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Event when the audio is available.
        private void AudioAvailable(GenericEventArgs<Stream> e)
        {
            OnAudioAvailable?.Invoke(this, e);
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Event when something wrong happens.
        private void Error(GenericEventArgs<Exception> e)
        {
            OnError?.Invoke(this, e);
        }
    }
}
