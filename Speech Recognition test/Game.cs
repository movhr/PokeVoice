using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Threading;
using System.Windows.Forms;

namespace Speech_Recognition_test
{
    public static class KeySender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);


        const int KEY_DOWN_EVENT = 0x0001; //Key down flag
        const int KEY_UP_EVENT = 0x0002; //Key up flag

        static void EmuKeyPress(Keys key, int holdTime = 50, int pauseAfter = 100)
        {
            foreach (var proc in Form1.VBA)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                keybd_event((byte)key, 0, KEY_DOWN_EVENT, 0);
                Thread.Sleep(holdTime);
                keybd_event((byte)key, 0, KEY_UP_EVENT, 0);
                Thread.Sleep(pauseAfter);
            }
        }

        public static void Up() => EmuKeyPress(Keys.W);
        public static void Down() => EmuKeyPress(Keys.S);
        public static void Left() => EmuKeyPress(Keys.A);
        public static void Right() => EmuKeyPress(Keys.D);
        public static void Confirm() => EmuKeyPress(Keys.V);
        public static void Back() => EmuKeyPress(Keys.C);
    }

    public class Vector : IComparable<Vector>
    {
        public int X;
        public int Y;
        public Vector()
        {
            X = Y = 0;
        }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Vector other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var xComparison = X.CompareTo(other.X);
            if (xComparison != 0) return xComparison;
            return Y.CompareTo(other.Y);
        }
    }

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

    public static class Game
    {
        public static string[] BattleOptions = { "FIGHT", "PACK", "POKéMON", "RUN" };

        public enum BattleState
        {
            Battle,
            Fight,
            Pack,
            Pokemon,
            NotInBattle
        }

        public enum OpponentType
        {
            Wild,
            Trainer
        }

        public static Bitmap ContinueConsole;
        public static OpponentType Opponent;
        public static BattleState CurrentState;
        public static bool InBattle;
        public static bool HasMoves;
        public static string LastMove;
        public static string[] MoveList;
        public static int MoveIndex = 0;
        private static readonly string[] NewlineDelimiter = { "\n" };
        private static Form1 _form;
        private static Grammar movesGrammar;
        private static Vector MovingDirection;
        public static Thread t;

        public static void SetForm(Form1 form)
        {
            _form = form;
            MovingDirection = new Vector();
            ContinueConsole = new Bitmap(File.OpenRead("continueConsole.png"));
            t = new Thread(CheckTextbox);
            t.Start();
        }

        public static void SetMoves(string str)
        {
            // Update moves
            var lines = str.Split(NewlineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where((x, i) => i > 0 && !string.IsNullOrWhiteSpace(x)) //Remove added confidence from OCR and check for whitelines
                .ToArray();
            
            if (lines.Length > 4)
                throw new IndexOutOfRangeException("Not a valid amount of moves given.");

            MoveList = lines.Select( (x,i) => x.StartsWith(">") ? x.Substring(1) : x).ToArray();
            BattleMenuCursor.FightCursor = Array.IndexOf(lines, lines.FirstOrDefault((x) => x.StartsWith(">")));
            _form.listBox1.Items.Clear();
            foreach (string move in MoveList)
                _form.listBox1.Items.Add(move);

            // Create new speech grammar
            if (movesGrammar != null)
                _form.Recognizer.UnloadGrammar(movesGrammar);

            var choises = new Choices();
            choises.Add(MoveList);
            var gb = new GrammarBuilder();
            gb.Append(choises);
            movesGrammar = new Grammar(gb);
            _form.Recognizer.LoadGrammar(movesGrammar);
        }

        public static void RelayOcr(string text)
        {
            text = text.Replace('\n', ' ');
            if (text.Contains("Wild") && text.Contains("appeared"))
                EnterBattle(OpponentType.Wild);
            else if (text.Contains("wants to battle"))
                EnterBattle(OpponentType.Trainer);
            else if (Opponent == OpponentType.Wild && (text.Contains("fainted") || text.Contains("Got away safely")))
                ExitBattle();
            else if (text.Contains("was defeated") && Opponent == OpponentType.Trainer)
                ExitBattle();
        }

        //Helper for taking over speech recognition regarding battle
        public static void RelayVoiceText(string text)
        {

            if (text == "read moves")
                SetMoves(_form.DoOcr());

            if (Form1.moving.Contains(text))
            {
                if (text == "stop")
                {
                    MovingDirection = new Vector();
                }
                else
                {
                    if (text.Contains("right"))
                        MovingDirection = new Vector(1, 0);
                    else if (text.Contains("left"))
                        MovingDirection = new Vector(-1, 0);
                    else if (text.Contains("up"))
                        MovingDirection = new Vector(0, -1);
                    else if (text.Contains("down"))
                        MovingDirection = new Vector(0, 1);
                }
            }


            switch (CurrentState)
            {
                case BattleState.Battle:
                    if (HasMoves && MoveList.Contains(text))
                    {
                        ChooseAction("FIGHT");
                        ChooseMove(text);
                    }
                    else if (text == "again")
                    {
                        ChooseAction("FIGHT");
                        ChooseMove(LastMove);
                    }
                    else
                    {
                        ChooseAction(text);
                    }
                    break;
                case BattleState.Fight:
                    if (Form1.cancelation.Contains(text))
                        CurrentState = BattleState.Battle;
                    else
                        ChooseMove(text);
                    return;
                default:
                    break;
            }
        }

        //Step 1: enter battle
        public static void EnterBattle(OpponentType opponent)
        {
            BattleMenuCursor.Reset();
            InBattle = true;
            CurrentState = BattleState.Battle;
            _form.battleModeLabel.Text = "IN BATTLE";
            Opponent = opponent;
        }

        //Step 2: select action (fight, pack, etc)
        public static void ChooseAction(string action)
        {
            switch (action)
            {
                case "FIGHT":
                    BattleMenuCursor.SelectAction(BattleMenuCursor.Fight);
                    if (!HasMoves)
                    {
                        var moves = _form.DoOcr();
                        SetMoves(moves);
                        HasMoves = true;
                    }
                    CurrentState = BattleState.Fight;
                    break;
                case "RUN":
                    BattleMenuCursor.SelectAction(BattleMenuCursor.Run);
                    Run();
                    break;
                default:
                    _form.statusLabel.Text = "Could not interpret action";
                    break;
            }
        }

        public static void ChooseMove(string move)
        {
            if (!HasMoves || !MoveList.Contains(move))
                _form.statusLabel.Text = ("Could not interpret move name");
            else
            {
                _form.statusLabel.Text = move;
                var moveIndex = Array.IndexOf(MoveList, move);
                if (moveIndex < 0)
                {
                    _form.statusLabel.Text = "Could not find move index";
                    KeySender.Back();
                }
                else
                    BattleMenuCursor.SelectMove(moveIndex);
                CurrentState = BattleState.Battle;
                LastMove = move;
            }

        }

        public static void Run()
        {
            ExitBattle();
        }

        public static void ExitBattle()
        {
            HasMoves = false;
            _form.battleModeLabel.Text = "not in battle";
            CurrentState = BattleState.NotInBattle;
        }

        public static void Update()
        {
            if (Form1.VBA == null)
                return;

            if (MovingDirection.X > 0)
                KeySender.Right();
            else if (MovingDirection.X < 0)
                KeySender.Left();
            else if (MovingDirection.Y < 0)
                KeySender.Up();
            else if (MovingDirection.Y > 0)
                KeySender.Down();

            _form.moveDirX.Text = MovingDirection.X.ToString();
            _form.moveDirY.Text = MovingDirection.Y.ToString();

            _form.gameState.Text = CurrentState.ToString();
        }
        

        // Must run continuously
        public static void CheckTextbox()
        {
            try
            {
                while (true)
                {
                    _awaitOcrLocations:
                    Thread.Sleep(100);
                    if (Ocr.consoleLocation.IsEmpty())
                        goto _awaitOcrLocations;
                    using (var bmp = PictureRecognition.ShootScreen(Ocr.consoleLocation))
                    {
                        var ph = PictureRecognition.GetHashProbability(bmp, PictureRecognition.EmptyTextBox);
                        if (ph > 75)
                        {
                            MovingDirection = new Vector();
                                using (var ss = PictureRecognition.ShootScreen(
                                    MyRectangle.RelativeToSize(Ocr.windowLocation, 136, 144, 7, 7)))
                                {
                                    //_form.continuePicture.Image = ss;
                                    ph = PictureRecognition.GetHashProbability(ss, ContinueConsole);
                                    //_form.continueProbability.Text = ph.ToString("N2");
                                    if (ph > 70)
                                    {
                                        Thread.Sleep(500);
                                        KeySender.Confirm();
                                    };
                                }
                            }
                        
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
    }
}
