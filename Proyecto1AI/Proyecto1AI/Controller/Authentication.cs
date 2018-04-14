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
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;


namespace Proyecto1AI.Controller
{
    class Authentication
    {
        public string AccessURI { get; }
        private string SpeechAPISubscriptionKey;
        public string AccessToken { get; set; }
        private Timer AccessTokenRenewer;
        private const int RefreshTokenDuration = 9;

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Constructor
        public Authentication()
        {
            AccessURI = ConfigurationManager.AppSettings["AccessURI"];
            SpeechAPISubscriptionKey = ConfigurationManager.AppSettings["SpeechAPISubscriptionKey"];
            AccessToken = HttpPost();

            // renew the token every specfied minutes
            AccessTokenRenewer = 
                new Timer ( new TimerCallback(OnTokenExpiredCallback),
                            this,
                            TimeSpan.FromMinutes(RefreshTokenDuration),
                            TimeSpan.FromMilliseconds(-1));
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Renew the access token every RefreshTokenDuration time set
        private void RenewAccessToken()
        {
            // Creates a new token because the last is about to expire
            AccessToken = HttpPost();
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Is the callback used by the AccesTokenRenewer in order to renew the token
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }

            // Set the renewer to execute after the next RefreshTokenDuration has passed
            finally
            {
                try
                {
                    AccessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Creates the necessary HTTP request with the generated token
        private string HttpPost()
        {
            // Prepare OAuth request
            WebRequest webRequest = WebRequest.Create(AccessURI);
            webRequest.Method = "POST";
            webRequest.ContentLength = 0;
            webRequest.Headers["Ocp-Apim-Subscription-Key"] = SpeechAPISubscriptionKey;

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] waveBytes = null;
                        int count = 0;
                        do
                        {
                            byte[] buf = new byte[1024];
                            count = stream.Read(buf, 0, 1024);
                            ms.Write(buf, 0, count);
                        } while (stream.CanRead && count > 0);

                        waveBytes = ms.ToArray();

                        return Encoding.UTF8.GetString(waveBytes);
                    }
                }
            }
        }
    }
}
