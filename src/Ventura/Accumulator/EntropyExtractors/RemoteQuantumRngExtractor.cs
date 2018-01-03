using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Ventura.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Paddings;

namespace Ventura.Accumulator.EntropyExtractors
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteQuantumRngExtractor : EntropyExtractorBase, IEntropyExtractor
    {
        public RemoteQuantumRngExtractor(int sourceNumber) : base(sourceNumber)
        {
        }

        protected override Task<byte[]> ExtractEntropicData()
        {
            Func<byte[]> extraction = () =>
            {
                using (WebClient wc = new WebClient())
                {
                    var jsonResponse = wc.DownloadString("https://qrng.anu.edu.au/API/jsonI.php?length=30&type=uint8");
                    var jObject = JObject.Parse(jsonResponse);

                    var tokens = jObject["data"].Children().ToList();
                    byte[] result = new byte[30];
                    int i = 0;

                    foreach (var token in tokens)
                    {
                        byte part = token.ToObject<byte>();
                        result[i] = part;
                    }

                    return result;
                }
            };

            return Task.Run(extraction);
        }
    }
}
