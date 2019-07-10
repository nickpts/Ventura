using CommandLine;

namespace Ventura.Cli
{
	public class Options
	{
		[Option('s', "seed", Required = true, HelpText = "Path to the seed file, if a seed file does not exist it will be created.")] //TODO: where?
		public string SeedPath { get; set; }

		[Option('c', "cipher", Required = false, HelpText = "Aes, TwoFish, Serpent, Blowfish supported. Default: Aes")]
		public Cipher Cipher { get; set; }

		[Option('e', "entropy", Required = false, HelpText = "Local, Remote or Full entropy sources. Default: local")]
		public ReseedEntropySourceGroup EntropyGroup { get; set; }

		[Option('i', "min", Required = true, HelpText = "Lowest possible random number")]
		public int Min { get; set; }

		[Option('x', "max", Required = true, HelpText = "Highest possible random number")]
		public int Max { get; set; }

	}
}
