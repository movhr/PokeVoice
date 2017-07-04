using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public static class PictureRecognition
    {
        public static bool[] EmptyTextBox = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false };

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
            return err == 0 ? 100 : (n - (float)err) / n * 100;
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
    }

}
