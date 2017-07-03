using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Speech_Recognition_test
{
    public static class Ocr
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Form1.Rect rectangle);


        private static TesseractEngine engine;
        public static Bitmap ShootScreen()
        {
            //Create a new bitmap.
            Form1.Rect rect = new Form1.Rect();
            if(Form1.VBA == null)
                throw new NullReferenceException("Could not find window");
            if (!GetWindowRect(Form1.VBA[0].MainWindowHandle, ref rect))
                throw new NullReferenceException("Could not find window location");


            int textboxLocationTop = rect.Top + ((rect.Bottom - rect.Top) / 3 * 2) + 20;
            int width = rect.Right - rect.Left - 10;
            int height = rect.Bottom - textboxLocationTop-10;
            //int height = rect.Bottom - rect.Top - 60;
            var bmpScreenshot = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(rect.Left+5,
                textboxLocationTop,
                0,
                0,
                new Size(width, height),
                CopyPixelOperation.SourceCopy);

            //bmpScreenshot.Save($@"C:/Users/katel/Desktop/crystal_images/img_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png", ImageFormat.Png);

            return bmpScreenshot;
        }

        public static string ReadFromScreen(ref PictureBox pb)
        {
            if (engine == null)
            {
                engine = new TesseractEngine(@"./tessdata", "Pokemon", EngineMode.Default);
                engine.SetVariable("tessedit_char_whitelist", @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789é'");
            }
            var sw = Stopwatch.StartNew();
            var bm = ShootScreen();
            Game.CheckMovingState(bm);
            using (var result = engine.Process(bm))
            {
                var textResult = result.GetText();
                pb.Image = bm;
                //textResult = SpellingChecker.Check(textResult);
                Game.RelayOcr(textResult);
                sw.Stop();
                return $"Confidence: {result.GetMeanConfidence()} | Speed: {sw.ElapsedMilliseconds}ms\n{textResult}\n";
            }

        }
    }
}
