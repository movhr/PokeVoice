namespace Speech_Recognition_test
{
    public static class BattleMenuCursor
    {
        public static int FightCursor;
        public static Vector BattleCursor = new Vector();
        public static Vector Fight = new Vector(0, 0);
        public static Vector Pack = new Vector(0, 1);
        public static Vector Pokemon = new Vector(1, 0);
        public static Vector Run = new Vector(1, 1);

        public static void Reset()
        {
            BattleCursor = new Vector();
            FightCursor = 0;
        }

        public static void SelectAction(Vector dest)
        {
            var dX = dest.X - BattleCursor.X;
            var dY = dest.Y - BattleCursor.Y;
            if (dX < 0)
                KeySender.Up();
            else if (dX > 0)
                KeySender.Down();
            if (dY > 0)
                KeySender.Right();
            else if (dY < 0)
                KeySender.Left();
            KeySender.Confirm();
            BattleCursor = dest;
        }

        public static void SelectMove(int moveIndex, int nMaxMoves = 4)
        {
            int d = moveIndex - FightCursor;
            if (d > 0)
            {
                while (FightCursor < moveIndex)
                {
                    KeySender.Down();
                    FightCursor++;
                }
            }
            else if (d < 0)
            {
                while (FightCursor > moveIndex)
                {
                    KeySender.Up();
                    FightCursor--;
                }
            }
            KeySender.Confirm();
        }
    }
}