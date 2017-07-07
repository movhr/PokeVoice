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
        public static MyRectangle PokemonBattleOptionsLocation;
        private static MyRectangle ShootingRegion;

        private static TesseractEngine _engine;
        private static readonly char[] NewlineDelimiter = {'\n'};

        public static void SetWindowLocations(WindowRect windowWindowRect)
        {
            var height = windowWindowRect.Bottom - windowWindowRect.Top;
            var width = windowWindowRect.Right - windowWindowRect.Left;

            WindowLocation = new MyRectangle
            {
                Left = windowWindowRect.Left + 8,
                Right = windowWindowRect.Right - 8,
                Bottom = windowWindowRect.Bottom - 8,
                Top = (windowWindowRect.Bottom - 8) - (height / DefaultScreenSize.Height * DefaultScreenSize.Height)
            };

            ConsoleLocation = MyRectangle.ResizeRectangle(WindowLocation, DefaultScreenSize.Height / 3 * 2);
            ConsoleTextLocation = MyRectangle.ResizeRectangle(ConsoleLocation, 7, 5, -5, -6);

            PokemonBattleOptionsLocation = MyRectangle.RelativeToSize(WindowLocation, 95, 94, 59, 43);

            ShootingRegion = ConsoleTextLocation;
            OldWindowRect = windowWindowRect;
        }

        private static Bitmap ShootScreen()
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
                textResult = textResult.FixIchar().FixPchar();
                game.RelayOcr(textResult);
                sw.Stop();
                return $"Confidence: {result.GetMeanConfidence()} | Speed: {sw.ElapsedMilliseconds}ms\n{textResult}\n";
            }
        }

        public static string ReadFromRectangle(MyRectangle rect, bool saveScreenshot = false)
        {
            if (_engine == null)
            {
                _engine = new TesseractEngine(@"./tessdata", "Pokemon", EngineMode.Default);
                _engine.SetVariable("tessedit_char_whitelist",
                    @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789é$>…'!?:.,/");
            }
            var bm = PictureRecognition.ShootScreen(rect);
            if (saveScreenshot)
                bm.Save($"ocr_{DateTime.Now.ToMilliSecondString()}.png", ImageFormat.Png);
            using (var result = _engine.Process(bm))
            {
                var textResult = result.GetText();
                textResult = textResult.FixIchar().FixPchar();
                return textResult;
            }
        }

        public static string[] GetOptionsFromRectangle(MyRectangle rect, out int currentIndex)
        {
            var text = ReadFromRectangle(rect);
            var textLines = text.Split(NewlineDelimiter, StringSplitOptions.RemoveEmptyEntries);
            currentIndex = Array.FindIndex(textLines, x => x.StartsWith(">"));
            if (currentIndex >= 0)
            {
                var startIndex = 0;
                while (textLines[currentIndex][startIndex] == '>' || textLines[currentIndex][startIndex] == ' ')
                    startIndex++;
                textLines[currentIndex] = textLines[currentIndex].Substring(startIndex);
            }
            return textLines;
        }
    }
}
