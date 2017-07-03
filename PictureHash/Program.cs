using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speech_Recognition_test;

namespace PictureHash
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: PictureHash.exe <picture_path>");
                return;
            }
            using (FileStream f = File.OpenRead(args[0]))
            {
                using (Bitmap fbm = new Bitmap(f))
                {
                    var ph = PictureRecognition.GetHash(fbm);
                    for (int i = 0; i < ph.Count; i++)
                    {
                        //if (i % 16 == 0)
                        //    Console.Write('\n');
                        Console.Write($"{ph[i]}, ");
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
