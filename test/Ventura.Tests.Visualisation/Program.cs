using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Ventura.Generator;

namespace Ventura.Tests.Visualisation
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter width:");
            int width = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter height:");
            int height = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter filename:");
            string name = Console.ReadLine();

            var watch = Stopwatch.StartNew();
            string path = $@"{Directory.GetCurrentDirectory()}\{name}.png";
            DrawImage(height, width, path);

            Console.WriteLine($"Operation took { watch.ElapsedMilliseconds / 1000 } seconds");
            Console.WriteLine("Press any key to open file, ESC to exit");
            var key = Console.ReadKey().Key;

            if (key == ConsoleKey.Escape)
            {
                return;
            }
            else Process.Start(path);
            
        }

        private static void DrawImage(int height, int width, string path)
        {
            var prng = new VenturaPrng();

            using (Bitmap map = new Bitmap(width, height))
            {
                for (int i = 0; i < map.Width; i++)
                {
                    for (int j = 0; j < map.Height; j++)
                    {
                        int rn = (int)prng.GenerateData(new byte[1])[0];

                        var colour = Color.FromArgb(rn, rn, rn);
                        map.SetPixel(i, j, colour);
                        
                        map.Save(path, ImageFormat.Png);
                    }
                }
            }
        }
    }
}
