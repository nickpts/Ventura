using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ventura;
using Ventura.Generator;

namespace Ventura.Tests.Visualiser
{
    public class Program
    {
        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();

            GenerateImage();

            Console.WriteLine($"Finished writing image in {watch.ElapsedMilliseconds / 1000 } seconds");
            Console.ReadLine();
        }

        //@""
        public static void GenerateImage()
        {
            var gen = new VenturaPrng();

            using (Bitmap b = new Bitmap(3840, 2160))
            {
                for (int i = 0; i < b.Width; i++)
                {
                    for (int j = 0; j < b.Height; j++)
                    {
                        var pix = b.GetPixel(i, j);
                        var colour = new Color();

                        var rgb = gen.GenerateData(new byte[1]).First();
                        int rgbInt = Convert.ToInt32(rgb);

                        colour = Color.FromArgb(rgbInt, rgbInt, rgbInt);
                        b.SetPixel(i, j, colour);
                    }
                }

                b.Save($@"{ Directory.GetCurrentDirectory() }\test.png", ImageFormat.Png);
            }
        }
    }
}


