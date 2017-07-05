using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Windows.Forms;

namespace Speech_Recognition_test
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);
        
        public static List<string> Extras = new List<string>();

        // Basics
        public static readonly string[] Confirmation = { "next", "OK", "enter", "yes", "nice", "what", "cool", "oh my god", "no way", "amazing", "noice", "awesome", "ya",
            "nay", "select", "confirm", "yeah", "wicked", "wicked stuff", "ola senior", "ola seniorita" };
        public static readonly string[] Cancelation = { "exit", "back", "no" };
        public static readonly string[] NoActions = {"cut", "close", "start"};
        public static readonly string[] Navigation = { "left", "right", "up", "down" };
        public static readonly string[] OvergameMenu = { "menu" };
        public static readonly string[] Ocr = { "read screen", "picture recognition", "compare textbox" };


        public Form1()
        {
            InitializeComponent();
        }

        static string LAST_RESULT = "";
        //public const string LOG_FILE_PATH = @"C:/Users/katel/Desktop/Crystal_ocr_log.txt";
        public const string LOG_FILE_PATH = @"C:/Users/Donald/Source/Repos/PokeVoice/crystal_ocr_log.txt"; 
        public static StreamWriter LogStream;
        public static Process[] VBA;
        public SpeechRecognizer Recognizer;
        private Game _game;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a new SpeechRecognitionEngine instance.
            Recognizer = new SpeechRecognizer();

            VBA = Process.GetProcessesByName("VisualBoyAdvance");
            if (VBA.Length == 0)
                throw new NullReferenceException("VBA could not be located");
            

            Choices x = new Choices();
            x.Add(Confirmation);
            x.Add(Cancelation);
            x.Add(OvergameMenu);
            //x.Add(start_menu);
            //x.Add(battle_menu);
            x.Add(Ocr);
            x.Add(Navigation);
            x.Add(Extras.ToArray());
            x.Add(Game.BattleOptions);
            x.Add(Game.BattleSpecific);
            x.Add(NoActions);
            x.Add(Game.Moving);

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(x);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar grammar = new Grammar(gb);
            Recognizer.LoadGrammar(grammar);

            // Register a handler for the SpeechRecognized event.
            Recognizer.SpeechRecognized += (Sre_SpeechRecognized);

            FileStream logFile = File.Open(LOG_FILE_PATH, FileMode.Append);
            LogStream = new StreamWriter(logFile);

            _game = Game.Initialize(this);

        }

        public void PrependRichTextboxText(string text) => richTextBox1.Text = text + richTextBox1.Text;
        public void PrependRichTextboxText(char c) => richTextBox1.Text = c + richTextBox1.Text;
        public void PrependRichTextboxText<T>(T obj) => richTextBox1.Text = obj + richTextBox1.Text;


        public string DoOcr()
        {

            var result = Speech_Recognition_test.Ocr.ReadFromScreen(ref pictureBox1, _game);
            var str = "<read screen>\n" + result + "\n<read screen end>\n\n";
            PrependRichTextboxText(str);
            //LogStream.Write(str);
            return result;
        }

        // Create a simple handler for the SpeechRecognized event.
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            PrependRichTextboxText(e.Result.Text + '\n');

            LAST_RESULT = e.Result.Text;
            if (Confirmation.Contains(LAST_RESULT))
                KeySender.Confirm();

            if (Cancelation.Contains(LAST_RESULT))
                KeySender.Back();

            if (LAST_RESULT == "picture recognition")
            {
                var sw = Stopwatch.StartNew();
                var ssHash = PictureRecognition.GetHash(Speech_Recognition_test.Ocr.ShootScreen());
                PrependRichTextboxText(ssHash.Length);
                for (int i = 0; i < ssHash.Length; i++)
                {
                    if (i % 16 == 0)
                        PrependRichTextboxText('\n');
                    PrependRichTextboxText(ssHash[i] ? 1 : 0);
                }
                sw.Stop();
                PrependRichTextboxText(sw.ElapsedMilliseconds + "ms \n");
                PrependRichTextboxText(PictureRecognition.GetHashProbability(ssHash, Game.EmptyTextBox).ToString("N4") + '\n');
            }

            _game.RelayVoiceText(LAST_RESULT);

            if (Navigation.Contains(LAST_RESULT))
            {
                    switch (LAST_RESULT)
                    {
                        case "up":
                            KeySender.Up();
                            break;
                        case "down":
                            KeySender.Down();
                            break;
                        case "left":
                            KeySender.Left();
                            break;
                        case "right":
                            KeySender.Right();
                            break;
                        default:
                            break;
                    }
            }

            //if (OVERGAME_MENU.Contains(LAST_RESULT))
            //    EmuKeyPress(Keys.Enter);

            if (LAST_RESULT == "read screen")
            {
                DoOcr();
            }
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            DoOcr();
            _game.UpdateGui();
            //Check all navigation keys
            //if (
            //    (GetAsyncKeyState(0x56)  // v
            //    //| GetAsyncKeyState(0x41) //a
            //    //| GetAsyncKeyState(0x44) //d
            //    | GetAsyncKeyState(0x43) //c
            //    //| GetAsyncKeyState(0x53) //s
            //    //| GetAsyncKeyState(0x57) //w
            //    ) != 0)
            //{
            //    DoOcr();
            //    Thread.Sleep(500);
            //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Recognizer.Dispose();
            _game.Dispose();
            Application.Exit();
        }
    }
}
