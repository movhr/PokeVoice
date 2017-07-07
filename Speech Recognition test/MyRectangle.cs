namespace Speech_Recognition_test
{
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

        public static MyRectangle ResizeRectangle(MyRectangle r, float offsTop = 0, float offsLeft = 0, float offsBottom = 0, float offsRight = 0)
        {
            var mH = (float)Ocr.WindowLocation.Height / Ocr.DefaultScreenSize.Height;
            var mW = (float)Ocr.WindowLocation.Width / Ocr.DefaultScreenSize.Width;
            return new MyRectangle
            {
                Top = (int)(mH * offsTop + r.Top),
                Left = (int)(mW * offsLeft + r.Left),
                Bottom = (int)(mH * offsBottom + r.Bottom),
                Right = (int)(mW * offsRight + r.Right)
            };
        }

        public static MyRectangle RelativeToRectangle(MyRectangle r, float offsTop = 0, float offsLeft = 0,
            float offsBottom = 0, float offsRight = 0)
        {
            var mH = (float)Ocr.WindowLocation.Height / Ocr.DefaultScreenSize.Height;
            var mW = (float)Ocr.WindowLocation.Width / Ocr.DefaultScreenSize.Width;
            var top = (mH * offsTop + r.Top);
            var left = (mW * offsLeft + r.Left);
            var newRect = new MyRectangle
            {
                Top = (int)(top),
                Left = (int)(left),
                Bottom = (int)(mH * (offsTop + offsBottom) + top),
                Right = (int)(mW * (offsLeft + offsRight) + left)
            };
            return newRect;
        }

        public static MyRectangle RelativeToSize(MyRectangle r, float offsTop, float offsLeft, float offsWidth, float offsHeight)
        {
            var mH = (float)Ocr.WindowLocation.Height / Ocr.DefaultScreenSize.Height;
            var mW = (float)Ocr.WindowLocation.Width / Ocr.DefaultScreenSize.Width;
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
}