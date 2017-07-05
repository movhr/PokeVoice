using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Threading;

namespace Speech_Recognition_test
{
    public partial class Game : IDisposable
    {
        public static string[] BattleOptions = { "FIGHT", "PACK", "POKéMON", "RUN", "again" };
        public static readonly string[] BattleSpecific = { "enter trainer battle", "enter wild battle", "read moves" };

        public BattleState CurrentState;
        public enum BattleState
        {
            Battle,
            Fight,
            Pack,
            Pokemon,
            NotInBattle
        }

        public OpponentType Opponent;
        public enum OpponentType
        {
            Wild,
            Trainer
        }

        private static Game _game;

        private Game(Form1 form)
        {
            _form = form;
            _movingDirection = new Vector();
            ContinueConsole = new Bitmap(File.OpenRead("continueConsole.png"));
            UpdateThread = new Thread(ThreadUpdate);
            UpdateThread.Start();
        }

        public Bitmap ContinueConsole;
        public bool InBattle;
        public bool HasMoves;
        public string LastMove;
        public string[] MoveList;
        public int MoveIndex = 0;
        private static readonly string[] NewlineDelimiter = { "\n" };
        private readonly Form1 _form;
        private Grammar _movesGrammar;
        private Vector _movingDirection;
        public Thread UpdateThread;

        public static Game Initialize(Form1 form) => _game ?? (_game = new Game(form));

        public void SetMoves(string str)
        {
            // UpdateGUI moves
            var lines = str.Split(NewlineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where((x, i) => i > 0 && !String.IsNullOrWhiteSpace(x)) //Remove added confidence from OCR and check for whitelines
                .ToArray();
            
            if (lines.Length > 4)
                throw new IndexOutOfRangeException("Not a valid amount of moves given.");

            MoveList = lines.Select( (x,i) => x.StartsWith(">") ? x.Substring(1) : x).ToArray();
            BattleMenuCursor.FightCursor = Array.IndexOf(lines, lines.FirstOrDefault((x) => x.StartsWith(">")));
            _form.listBox1.Items.Clear();
            foreach (string move in MoveList)
                _form.listBox1.Items.Add(move);

            // Create new speech grammar
            if (_movesGrammar != null)
                _form.Recognizer.UnloadGrammar(_movesGrammar);

            var choises = new Choices();
            choises.Add(MoveList);
            var gb = new GrammarBuilder();
            gb.Append(choises);
            _movesGrammar = new Grammar(gb);
            _form.Recognizer.LoadGrammar(_movesGrammar);
        }

        

        //Step 1: enter battle
        public void EnterBattle(OpponentType opponent)
        {
            BattleMenuCursor.Reset();
            InBattle = true;
            CurrentState = BattleState.Battle;
            _form.battleModeLabel.Text = "IN BATTLE";
            Opponent = opponent;
        }

        //Step 2: select action (fight, pack, etc)
        public void ChooseAction(string action)
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

        public void ChooseMove(string move)
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

        public void Run()
        {
            ExitBattle();
        }

        public void ExitBattle()
        {
            HasMoves = false;
            _form.battleModeLabel.Text = "not in battle";
            CurrentState = BattleState.NotInBattle;
        }

        // Function for form control updates
        public void UpdateGui()
        {
            if (Form1.VBA.Length == 0)
                return;

            _form.moveDirX.Text = _movingDirection.X.ToString();
            _form.moveDirY.Text = _movingDirection.Y.ToString();

            _form.gameState.Text = CurrentState.ToString();
        }
        

        // Must run continuously
        public void ThreadUpdate()
        {
            try
            {
                while (true)
                {
                    _start:
                    Thread.Sleep(100);

                    // Check if player should walk
                    if (_movingDirection.X > 0)
                        KeySender.Right();
                    else if (_movingDirection.X < 0)
                        KeySender.Left();
                    else if (_movingDirection.Y < 0)
                        KeySender.Up();
                    else if (_movingDirection.Y > 0)
                        KeySender.Down();

                    // Await window locations to perform checks on textbox if there is any
                    if (Ocr.ConsoleLocation.IsEmpty())
                        goto _start;

                    // Checks whether there is a textbox and if talk can continue
                    TextboxContinueTalking();
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        public void Dispose()
        {
            UpdateThread.Abort();
            ContinueConsole.Dispose();
        }
    }
}
