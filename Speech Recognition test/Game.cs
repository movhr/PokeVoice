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
        private static Game _game;

        private Game(Form1 form)
        {
            _form = form;
            _movingDirection = new Vector();
            CurrentState = BattleState.NotInBattle;
            ContinueConsole = new Bitmap(File.OpenRead("continueConsole.png"));
            UpdateThread = new Thread(ThreadUpdate);
            UpdateThread.Start();
        }

        public Thread UpdateThread;
        public Bitmap ContinueConsole;

        private readonly Form1 _form;
        private Vector _movingDirection;
        
        public static Game Initialize(Form1 form) => _game ?? (_game = new Game(form));


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
