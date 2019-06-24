using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ventura.Accumulator;
using Ventura.Accumulator.EntropyExtractors;
using Ventura.Generator;
using Ventura.Interfaces;

namespace Ventura.Tests.Visualisation
{
    class Program
    {
        private static void Main(string[] args)
        {
            //VisualiseRandomness();
            TestMethod();
        }

        private static void TestMethod()
        {

			var prng = PrngVenturaFactory.CreatePrng(Cipher.Aes, ReseedEntropySourceGroup.Local);

			for (int i = 0; i <= 1000; i++)
			{
				Thread.Sleep(100);
				var data = prng.GetRandomData(new byte[1024000]);
				Debug.WriteLine($"Data generated: {i}");
			}
		}

        private static void VisualiseRandomness()
        {
            Console.Write("Enter width:");
            int width = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter height:");
            int height = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter filename:");
            string name = Console.ReadLine();

            var watch = Stopwatch.StartNew();
            string path = $@"{Directory.GetCurrentDirectory()}\{name}.png";
            DrawImage(width, height, path);

            Console.WriteLine($"Operation took { watch.ElapsedMilliseconds / 1000 } seconds");
            Console.WriteLine("Press any key to open file, ESC to exit");
            var key = Console.ReadKey().Key;

            if (key == ConsoleKey.Escape)
            {
                return;
            }
            else Process.Start(path);
        }

        private static void DrawImage(int width, int height, string path)
        {
            var prng = new VenturaGenerator();
            int index = width * height;
            var bytes = prng.GenerateData(new byte[index]);
            Color colour;
            int temp = 0;

            using (Bitmap map = new Bitmap(width, height))
            {
                for (int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        int rn = bytes[temp];
                        colour = Color.FromArgb(rn, rn, rn);
                        map.SetPixel(i, j, colour);

                        temp++;
                    }
                }

                map.Save(path, ImageFormat.Png);
            }
        }
    }
}
