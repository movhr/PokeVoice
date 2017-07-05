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
    public struct WindowRect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public struct MyRectangle
    {

        public int Top, Left, Right, Bottom;
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

        public static MyRectangle FromRect(WindowRect windowRect)
            => new MyRectangle
            {
                Top = windowRect.Top,
                Left = windowRect.Left,
                Bottom = windowRect.Bottom,
                Right = windowRect.Right
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
        public static extern bool GetWindowRect(IntPtr hwnd, ref WindowRect rectangle);

        public static Size DefaultScreenSize = new Size(160, 144);
        public static readonly WindowRect EmptyWindowRect = new WindowRect();
        public static WindowRect OldWindowRect;
        public static MyRectangle WindowLocation;
        public static MyRectangle ConsoleTextLocation;
        public static MyRectangle ConsoleLocation;
        public static MyRectangle ShootingRegion;

        private static TesseractEngine _engine;

        public static void SetWindowLocations(WindowRect windowWindowRect)
        {
            var height = windowWindowRect.Bottom - windowWindowRect.Top;

            WindowLocation = new MyRectangle
            {
                Left = windowWindowRect.Left + 8,
                Right = windowWindowRect.Right - 8,
                Bottom = windowWindowRect.Bottom - 8,
                Top = (windowWindowRect.Bottom - 8) - (height / DefaultScreenSize.Height * DefaultScreenSize.Height)
            };

            ConsoleLocation = MyRectangle.RelativeToRectangle(WindowLocation, DefaultScreenSize.Height / 3 * 2, 0, 0, 0);
            ConsoleTextLocation = MyRectangle.RelativeToRectangle(ConsoleLocation, 25, 5, -22, -6);
            
            ShootingRegion = ConsoleTextLocation;
            OldWindowRect = windowWindowRect;
        }

        public static Bitmap ShootScreen()
        {
            //Create a new bitmap.
            var rect = new WindowRect();
            if (Form1.VBA == null)
                throw new NullReferenceException("Could not find window");

            if (!GetWindowRect(Form1.VBA[0].MainWindowHandle, ref rect))
                throw new NullReferenceException("Could not find window location");

            if (ShootingRegion.IsEmpty() || !OldWindowRect.Equals(rect))
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

        public static string ReadFromScreen(ref PictureBox pb, Game game)
        {
            if (_engine == null)
            {
                _engine = new TesseractEngine(@"./tessdata", "Pokemon", EngineMode.Default);
                _engine.SetVariable("tessedit_char_whitelist",
                    @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789é$>…'!?:.,/");
            }
            var sw = Stopwatch.StartNew();
            var bm = ShootScreen();
            using (var result = _engine.Process(bm))
            {
                var textResult = result.GetText();
                pb.Image = bm;
                textResult = SpellingChecker.FixIchar(textResult);
                game.RelayOcr(textResult);
                sw.Stop();
                return $"Confidence: {result.GetMeanConfidence()} | Speed: {sw.ElapsedMilliseconds}ms\n{textResult}\n";
            }
        }
    }
}
