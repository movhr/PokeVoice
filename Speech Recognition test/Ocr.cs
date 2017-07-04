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
using Speech_Recognition_test;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace Speech_Recognition_test
{

    public struct MyRectangle
    {
        public int Top, Left;
        private int right, bottom;
        public int Right { get => right; set => right = value; }
        public int Bottom { get => bottom; set => bottom = value; }
        public int Width { get => right; set => right = value; }
        public int Height { get => bottom; set => bottom = value; }

        public MyRectangle(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            this.right = right;
            this.bottom = bottom;
        }

        public bool IsEmpty() => Top == 0 && Left == 0 && Right == 0 && Bottom == 0;
    }

    public static class Ocr
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Form1.Rect rectangle);

        public static readonly Form1.Rect emptyRect = new Form1.Rect();
        public static Form1.Rect oldRect;
        public static MyRectangle windowLocation;
        public static MyRectangle consoleTextLocation;
        public static MyRectangle consoleLocation;
        public static MyRectangle ShootingRegion;

        private static TesseractEngine engine;

        public static void SetWindowLocations(Form1.Rect rect)
        {
            windowLocation = new MyRectangle
            {
                Top = rect.Top + 50,
                Left = rect.Left + 15,
                Right = rect.Right - rect.Left - 30,
                Bottom = rect.Bottom - (rect.Top + 50) - 5
            };

            consoleLocation = new MyRectangle
            {
                Top  = windowLocation.Top + windowLocation.Bottom / 3 * 2,
                Left = rect.Left + 15,
                Bottom = windowLocation.Bottom / 3,
                Right = windowLocation.Right
            };

            consoleTextLocation = new MyRectangle
            {
                Top = consoleLocation.Top + windowLocation.Bottom / 145 * 6,
                Left = consoleLocation.Left + windowLocation.Right / 140 * 6,
                Bottom = consoleLocation.Bottom - 2 * (windowLocation.Bottom / 140 * 7),
                Right = consoleLocation.Right - 2 * (windowLocation.Right / 140 * 8)
            };

            ShootingRegion = consoleTextLocation;
            oldRect = rect;
        }

        public static Bitmap ShootScreen()
        {
            //Create a new bitmap.
            var rect = new Form1.Rect();
            if (Form1.VBA == null)
                throw new NullReferenceException("Could not find window");

            if (!GetWindowRect(Form1.VBA[0].MainWindowHandle, ref rect))
                throw new NullReferenceException("Could not find window location");

            if (ShootingRegion.IsEmpty() || !oldRect.Equals(rect))
                SetWindowLocations(rect);
            

            var bmpScreenshot = new Bitmap(ShootingRegion.Right, ShootingRegion.Bottom, PixelFormat.Format24bppRgb);
            
            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(
                ShootingRegion.Left,
                ShootingRegion.Top,
                0,
                0,
                new Size(ShootingRegion.Right, ShootingRegion.Bottom),
                CopyPixelOperation.SourceCopy);

            bmpScreenshot.Save($@"C:/Users/katel/Desktop/crystal_images/img_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png", ImageFormat.Png);

            return bmpScreenshot;
        }

        public static string ReadFromScreen(ref PictureBox pb)
        {
            if (engine == null)
            {
                engine = new TesseractEngine(@"./tessdata", "Pokemon", EngineMode.Default);
                engine.SetVariable("tessedit_char_whitelist", @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789é$>…'");
            }
            var sw = Stopwatch.StartNew();
            var bm = ShootScreen();
            Game.CheckMovingState(bm);
            using (var result = engine.Process(bm))
            {
                var textResult = result.GetText();
                pb.Image = bm;
                textResult = SpellingChecker.FixIchar(textResult);
                Game.RelayOcr(textResult);
                sw.Stop();
                return $"Confidence: {result.GetMeanConfidence()} | Speed: {sw.ElapsedMilliseconds}ms\n{textResult}\n";
            }

        }
    }
}
