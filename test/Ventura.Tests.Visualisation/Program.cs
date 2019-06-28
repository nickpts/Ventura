using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
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
	        //TestMethod();
			OutputRandomNumbers();
	        Console.ReadLine();
        }

        private static void TestMethod()
        {
	        using (var prng = PrngVenturaFactory.Create(new MemoryStream()))
	        {
		        for (int i = 0; i <= 10; i++)
		        {
			        Thread.Sleep(100);
			        var data = new byte[1024000];
			        prng.GetRandomData(data);
			        Debug.WriteLine($"Data generated: {i}");
		        }
	        }
        }

        public static void OutputRandomNumbers()
        {
	        using (var prng = PrngVenturaFactory.Create(new MemoryStream()))
	        {
		        for (int i = 0; i <= 1000; i++)
		        {
			        int result = prng.GetRandomNumber(100, 346464);
			        Console.WriteLine($"Data generated: {result}");
		        }
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
	        using (var prng = PrngVenturaFactory.Create(new MemoryStream()))
	        {
		        int length = width * height;
		        
		        var bytes = prng.GetRandomData(new byte[length]);

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
}
