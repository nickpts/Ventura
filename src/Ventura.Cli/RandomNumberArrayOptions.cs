using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Ventura.Cli
{
	[Verb("rns", HelpText = "Generates a sequence of random numbers")]
	public class RandomNumberArrayOptions: Options
	{
		[Option('n', "length", Required = true, HelpText = "How many random numbers to generate")]
		public int ArrayLength { get; set; }
	}
}
