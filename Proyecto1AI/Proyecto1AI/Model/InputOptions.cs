using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1AI.Model
{
    class InputOptions
    {
        public String RecognitionLanguage { get; set; }
        public VoiceGender VoiceType { get; set; }
        public Uri RequestUri { get; set; }
        public string VoiceName { get; set; }
        public string AuthorizationToken { get; set; }
        public string Text { get; set; }
        public string OutputFormat { get; set; }


        // ----------------------------------------------------------------------------------------------------------------------------------------
        
        // Constructor
        public InputOptions()
        {
            RecognitionLanguage = ConfigurationManager.AppSettings["RecognitionLanguage"];
            OutputFormat = ConfigurationManager.AppSettings["OutputFormat"];
            VoiceName = ConfigurationManager.AppSettings["VoiceName"];
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------

        // Get or set the headers
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();
                toReturn.Add(new KeyValuePair<string, string>("Content-Type", "application/ssml+xml"));

                toReturn.Add(new KeyValuePair<string, string>("X-Microsoft-OutputFormat", OutputFormat));
                // authorization Header
                toReturn.Add(new KeyValuePair<string, string>("Authorization", AuthorizationToken));
                // Refer to the doc
                toReturn.Add(new KeyValuePair<string, string>("X-Search-AppId", "07D3234E49CE426DAA29772419F436CA"));
                // Refer to the doc
                toReturn.Add(new KeyValuePair<string, string>("X-Search-ClientID", "1ECFAE91408841A480F00935DC390960"));
                // The software originating the request
                toReturn.Add(new KeyValuePair<string, string>("User-Agent", "TTSClient"));

                return toReturn;
            }
            set
            {
                Headers = value;
            }
        }
    }
}

