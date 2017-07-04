using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public static class PictureRecognition
    {
        public static bool[] EmptyTextBox =
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, true,
            true, true, true, true, true, true, true, true, true, true, true, true, true, false
        };

        public static List<bool> GetHash(Bitmap bmpSource, out Bitmap bmpMin, Size size)
        {
            List<bool> lResult = new List<bool>(256);
            //create new image with 16x16 pixel
            bmpMin = new Bitmap(bmpSource, size);
            for (int j = 0; j < bmpMin.Width; j++)
            {
                for (int i = 0; i < bmpMin.Height; i++)
                {
                    //reduce colors to true / false
                    lResult.Add(bmpMin.GetPixel(j, i).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        public static List<bool> GetHash(Bitmap bmpSource, Size size)
        {
            var result = GetHash(bmpSource, out Bitmap bm, size);
            bm.Dispose();
            return result;
        }

        public static List<bool> GetHash(Bitmap bmpSource, int size = 16)
        {
            var result = GetHash(bmpSource, out Bitmap bm, new Size(size, size));
            bm.Dispose();
            return result;
        }

        public static float GetHashProbability(IList<bool> l1, IList<bool> l2)
        {
            if (l1.Count != l2.Count)
                throw new ArgumentException("Pictures are not the same size");

            int err = 0;
            int n = l1.Count;

            for (int i = 0; i < l1.Count; i++)
            {
                if (l1[i] != l2[i])
                    err++;
            }
            return err == 0 ? 100 : (n - (float) err) / n * 100;
        }

        public static float GetHashProbability(Bitmap bmp, bool[] barr)
        {
            var hash = GetHash(bmp);
            return GetHashProbability(hash, barr);
        }

        public static float GetHashProbability(Bitmap bmp1, Bitmap bmp2)
        {
            return GetHashProbability(GetHash(bmp1), GetHash(bmp2));
        }

        public static Bitmap ShootScreen(MyRectangle area)
        {
            //Create a new bitmap.
            var rect = new Form1.Rect();
            if (Form1.VBA == null)
                throw new NullReferenceException("Could not find window");

            if (!Ocr.GetWindowRect(Form1.VBA[0].MainWindowHandle, ref rect))
                throw new NullReferenceException("Could not find window location");

            
            var bmpScreenshot = new Bitmap(area.Width, area.Height, PixelFormat.Format24bppRgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(
                area.Left,
                area.Top,
                0,
                0,
                new Size(area.Width, area.Height),
                CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }
    }
}
