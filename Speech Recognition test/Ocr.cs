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
        public int Right;
        public int Bottom;
        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public MyRectangle(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public MyRectangle DivideAll(int d) => new MyRectangle
        {
            Top = Top / d,
            Left = Left / d,
            Right = Right / d,
            Bottom = Bottom / d
        };

        public static MyRectangle FromRect(Form1.Rect rect)
            => new MyRectangle
            {
                Top = rect.Top,
                Left = rect.Left,
                Bottom = rect.Bottom,
                Right = rect.Right
            };

        public static MyRectangle RelativeToRectangle(MyRectangle r, int offsTop, int offsLeft, int offsBottom, int offsRight)
        {
            var mH = (float)r.Height / Ocr.DefaultScreenSize.Height;
            var mW = (float)r.Width / Ocr.DefaultScreenSize.Width;
            return new MyRectangle
            {
                Top = (int)(mH * offsTop + r.Top),
                Left = (int)(mW * offsLeft + r.Left),
                Bottom = (int)(mH * offsBottom + r.Bottom),
                Right = (int)(mW * offsRight + r.Right)
            };
        }

        public static MyRectangle RelativeToSize(MyRectangle r, int offsTop, int offsLeft, int offsWidth, int offsHeight)
        {
            var mH = (float)r.Height / Ocr.DefaultScreenSize.Height;
            var mW = (float)r.Width / Ocr.DefaultScreenSize.Width;
            var top = (mH * offsTop + r.Top);
            var left = (mW * offsLeft + r.Left);
            var newRect = new MyRectangle
            {
                Top = (int)(top),
                Left = (int)(left),
                Bottom = (int)(mH * offsHeight + top),
                Right = (int)(mW * offsWidth + left)
            };
            return newRect;
        }

        public bool IsEmpty() => Top == 0 && Left == 0 && Right == 0 && Bottom == 0;
    }

    public static class Ocr
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Form1.Rect rectangle);

        public static Size DefaultScreenSize = new Size(160, 144);
        public static readonly Form1.Rect emptyRect = new Form1.Rect();
        public static Form1.Rect oldRect;
        public static MyRectangle windowLocation;
        public static MyRectangle consoleTextLocation;
        public static MyRectangle consoleLocation;
        public static MyRectangle ShootingRegion;

        private static TesseractEngine engine;

        public static void SetWindowLocations(Form1.Rect rect)
        {
            var height = rect.Bottom - rect.Top;

            windowLocation = new MyRectangle
            {
                Left = rect.Left + 8,
                Right = rect.Right - 8,
                Bottom = rect.Bottom - 8,
                Top = (rect.Bottom - 8) - (height / DefaultScreenSize.Height * DefaultScreenSize.Height)
            };

            consoleLocation = MyRectangle.RelativeToRectangle(windowLocation, DefaultScreenSize.Height / 3 * 2, 0, 0, 0);

            consoleTextLocation = MyRectangle.RelativeToRectangle(consoleLocation, 25, 5, -22, -6);

            //consoleLocation = new MyRectangle
            //{
            //    Top = windowLocation.Top + windowLocation.Bottom / 3 * 2,
            //    Left = rect.Left + ,
            //    Bottom = windowLocation.Bottom / 3,
            //    Right = windowLocation.Right
            //};

            //consoleTextLocation = new MyRectangle
            //{
            //    Top = consoleLocation.Top + windowLocation.Bottom / 144 * 6,
            //    Left = consoleLocation.Left + windowLocation.Right / 140 * 6,
            //    Bottom = consoleLocation.Bottom - 2 * (windowLocation.Bottom / 140 * 7),
            //    Right = consoleLocation.Right - 2 * (windowLocation.Right / 140 * 8)
            //};

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


            var bmpScreenshot = new Bitmap(ShootingRegion.Width, ShootingRegion.Height, PixelFormat.Format24bppRgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(
                ShootingRegion.Left,
                ShootingRegion.Top,
                0,
                0,
                new Size(ShootingRegion.Width, ShootingRegion.Height),
                CopyPixelOperation.SourceCopy);

            //bmpScreenshot.Save($@"C:/Users/katel/Desktop/crystal_images/img_{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.ffff", CultureInfo.InvariantCulture)}.png", ImageFormat.Png);

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
