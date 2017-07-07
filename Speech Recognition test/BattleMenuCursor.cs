namespace Speech_Recognition_test
{
    public static class BattleMenuCursor
    {
        public static int FightCursor;
        public static int PokemonCursor;
        public static int OptionsCursor;
        public static Vector BattleCursor = new Vector();
        public static Vector Fight = new Vector(0, 0);
        public static Vector Pack = new Vector(0, 1);
        public static Vector Pokemon = new Vector(1, 0);
        public static Vector Run = new Vector(1, 1);

        public static void Reset()
        {
            BattleCursor = new Vector();
            FightCursor = 0;
            PokemonCursor = 0;
            OptionsCursor = 0;
        }

        public static void SelectAction(Vector dest)
        {
            var dX = dest.X - BattleCursor.X;
            var dY = dest.Y - BattleCursor.Y;
            if (dX < 0)
                KeySender.Left();
            else if (dX > 0)
                KeySender.Right();
            if (dY > 0)
                KeySender.Down();
            else if (dY < 0)
                KeySender.Up();
            KeySender.Confirm();
            BattleCursor.X = dest.X;
            BattleCursor.Y = dest.Y;
        }

        public static void SelectIndex(ref int cursor, int itemIndex, int nMaxItems = 4, bool resetCursor = false)
        {
            int d = itemIndex - cursor;
            if (d > 0)
            {
                while (cursor < itemIndex)
                {
                    KeySender.Down();
                    cursor++;
                }
            }
            else if (d < 0)
            {
                while (cursor > itemIndex)
                {
                    KeySender.Up();
                    cursor--;
                }
            }
            KeySender.Confirm();
            if (resetCursor)
                cursor = 0;
        }
    }
}