using System;
using System.Collections.Generic;
using System.Text;

namespace Ventura
{
    public class PRNGVenturaServiceSettings
    {
        public Cipher Cipher { get; set; }
        public byte[] Seed { get; set; }
        public ReseedEntropySources Sources { get; set; }
        public Output Output { get; set; }
    }
}
