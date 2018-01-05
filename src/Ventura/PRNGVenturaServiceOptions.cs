using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public class PRNGVenturaServiceOptions
    {
        /// <summary>
        /// Indicates type of entropy sources used to 
        /// reseed the generator. 
        /// Local: only sources from the local system used
        /// Remote: only remote sources (e.g RemoteQUantumRngExtractor) used
        /// Full: both types used
        /// </summary>
        public enum ReseedEntropySources
        {
            Local,
            Remote,
            Full = Local | Remote
        }

        public enum Output
        {
            Byte,
            Int32,
            String,
            Hex
        }

        public Cipher Cipher { get; set; }
        public byte[] Seed { get; set; }
        public ReseedEntropySources Options { get; set; }
    }
}
