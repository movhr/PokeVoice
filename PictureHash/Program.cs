using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speech_Recognition_test;

namespace PictureHash
{
    class Program
    {
        static void MonochromePicture(Bitmap fbm)
        {

            var sw = Stopwatch.StartNew();
            var ph = PictureRecognition.GetHash(fbm, out Bitmap rbm, new Size(resizeWidth, resizeHeight));
            sw.Stop();
            for (var i = 0; i < ph.Count; i++)
            {
                if (i % resizeWidth == 0)
                    Console.Write('\n');
                else if (i % 16 == 0)
                    Console.Write(' ');
                //Console.Write($"{ph[i]}, ");
                Console.Write(ph[i] ? 8.ToString() : " ");
            }
            Console.WriteLine();
            Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds}ms.");
            using (var nbm = new Bitmap(rbm))
            {
                for (var i = 0; i < nbm.Width; i++)
                {
                    for (var j = 0; j < nbm.Height; j++)
                    {
                        var b = nbm.GetPixel(i, j).GetBrightness();
                        //if (b < 0.3f)
                        //    nbm.SetPixel(i, j, Color.Black);
                        //else if (b < 0.7f)
                        //    nbm.SetPixel(i, j, Color.Gray);
                        //else
                        //    nbm.SetPixel(i, j, Color.White);
                        if (b < 0.5)
                            nbm.SetPixel(i, j, Color.Black);
                        else
                            nbm.SetPixel(i, j, Color.White);
                    }
                }
                fbm.Save(
                    $"original_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png",
                    ImageFormat.Png);
                nbm.Save(
                    $"monochrome_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png",
                    ImageFormat.Png);
                rbm.Dispose();

            }
        }

        static void ComparePicture(Bitmap fbm1, Bitmap fbm2)
        {
            var sw = Stopwatch.StartNew();
            var ph1 = PictureRecognition.GetHash(fbm1, new Size(resizeWidth, resizeHeight));
            var ph2 = PictureRecognition.GetHash(fbm2, new Size(resizeWidth, resizeHeight));
            var c = new bool[ph1.Count];
            for (int i = 0; i < ph1.Count; i++)
                c[i] = ph1[i] & ph2[i];
            sw.Stop();

            for (int i = 0; i < ph1.Count; i++)
            {
                if (i % resizeWidth == 0)
                    Console.Write('\n');
                else if (i % 16 == 0)
                    Console.Write(' ');
                //Console.Write($"{ph[i]}, ");
                Console.Write(c[i] ? 8.ToString() : " ");
            }

            Console.WriteLine();
            Console.WriteLine($"Time elapsed: {sw.ElapsedMilliseconds}ms.");

            using (var nbm = new Bitmap(resizeWidth, resizeHeight))
            {
                int k = 0;
                for (var i = 0; i < nbm.Width; i++)
                {
                    for (var j = 0; j < nbm.Height; j++)
                    {
                        nbm.SetPixel(i, j, c[k] ? Color.Black : Color.White);
                        k++;
                    }
                }
                nbm.Save(
                    $"comparison_monochrome_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png",
                    ImageFormat.Png);

            }
        }

        private const int resizeHeight = 80;
        private const int resizeWidth = 200;
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                using (var f = File.OpenRead(args[0]))
                {
                    using (var fbm = new Bitmap(f))
                    {
                        MonochromePicture(fbm);

                        Console.WriteLine("Monochrome picture");
                    }
                }
            }
            else if (args.Length == 2)
            {
                using (var f1 = File.OpenRead(args[0]))
                {
                    using (var f2 = File.OpenRead(args[1]))
                    {
                        using (var fbm1 = new Bitmap(f1))
                        {
                            using (var fbm2 = new Bitmap(f2))
                            {
                                ComparePicture(fbm1, fbm2);
                                Console.WriteLine("Picture comparison");
                            }
                        }
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
