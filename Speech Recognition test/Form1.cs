using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Speech_Recognition_test
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);



        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        
        public static List<string> Extras = new List<string>();

        // Basics
        public static string[] confirmation = new string[] { "next", "OK", "enter", "yes", "nice", "what", "cool", "oh my god", "no way", "amazing", "noice", "awesome", "ya",
            "nay", "select", "confirm", "yeah", "wicked", "wicked stuff", "ola senior", "ola seniorita" };
        public static string[] cancelation = new string[] { "exit", "back", "no" };
        public static string[] noActions = new string[] {"cut", "close", "start"};
        public static string[] navigation = new string[] { "left", "right", "up", "down" };
        public static string[] BattleOptions = { "FIGHT", "PACK", "POKéMON", "RUN" };
        public static string[] BattleSpecific = { "enter trainer battle", "enter wild battle", "read moves" };
        public static string[] OVERGAME_MENU = new string[] { "menu" };
        public static string[] ocr = new string[] { "read screen", "picture recognition", "compare textbox" };

        public static string[] moving = {"walk left", "walk right", "walk up", "walk down", "stop"};


        public Form1()
        {
            InitializeComponent();
        }

        static string LAST_RESULT = "";
        public const string LOG_FILE_PATH = @"C:/Users/katel/Desktop/Crystal_ocr_log.txt";
        //public const string LOG_FILE_PATH = @"C:/Users/Donald/Source/Repos/PokeVoice/crystal_ocr_log.txt"; 
        public static StreamWriter LogStream;
        public static Process[] VBA;
        public SpeechRecognizer Recognizer;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a new SpeechRecognitionEngine instance.
            Recognizer = new SpeechRecognizer();

            VBA = Process.GetProcessesByName("VisualBoyAdvance");
            if (VBA.Length == 0)
                throw new NullReferenceException("VBA could not be located");
            

            Choices x = new Choices();
            x.Add(confirmation);
            x.Add(cancelation);
            x.Add(OVERGAME_MENU);
            //x.Add(start_menu);
            //x.Add(battle_menu);
            x.Add(ocr);
            x.Add(navigation);
            x.Add(Extras.ToArray());
            x.Add(BattleOptions);
            x.Add(BattleSpecific);
            x.Add(noActions);
            x.Add(moving);

            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(x);

            // Create the Grammar instance and load it into the speech recognition engine.
            Grammar grammar = new Grammar(gb);
            Recognizer.LoadGrammar(grammar);

            // Register a handler for the SpeechRecognized event.
            Recognizer.SpeechRecognized += (sre_SpeechRecognized);

            FileStream logFile = File.Open(LOG_FILE_PATH, FileMode.Append);
            LogStream = new StreamWriter(logFile);

            Game.SetForm(this);

        }



        public string DoOcr()
        {

            var result = Ocr.ReadFromScreen(ref pictureBox1);
            var str = "<read screen>\n" + result + "\n<read screen end>\n\n" +
                      richTextBox1.Text;
            richTextBox1.Text = str;
            //LogStream.Write(str);
            return result;
        }

        // Create a simple handler for the SpeechRecognized event.
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            richTextBox1.Text = e.Result.Text + '\n' + richTextBox1.Text;

            LAST_RESULT = e.Result.Text;
            if (confirmation.Contains(LAST_RESULT))
                KeySender.Confirm();

            if (cancelation.Contains(LAST_RESULT))
                KeySender.Back();

            if (LAST_RESULT == "picture recognition")
            {
                var sw = Stopwatch.StartNew();
                var ssHash = PictureRecognition.GetHash(Ocr.ShootScreen());
                richTextBox1.Text = ssHash.Count.ToString() + richTextBox1.Text;
                for (int i = 0; i < ssHash.Count; i++)
                {
                    if (i % 16 == 0)
                        richTextBox1.Text = '\n' + richTextBox1.Text;
                    richTextBox1.Text = (ssHash[i] ? 1 : 0).ToString() + richTextBox1.Text;
                }
                sw.Stop();
                richTextBox1.Text = sw.ElapsedMilliseconds + "ms \n" + richTextBox1.Text;
                richTextBox1.Text = PictureRecognition.GetHashProbability(ssHash, PictureRecognition.EmptyTextBox).ToString("N4") + '\n' + richTextBox1.Text;

            }

            if (LAST_RESULT == "enter trainer battle")
            {
                Game.EnterBattle(Game.OpponentType.Trainer);
                return;
            }


            if (LAST_RESULT == "enter wild battle")
            {
                Game.EnterBattle(Game.OpponentType.Wild);
                return;
            }

            Game.RelayVoiceText(LAST_RESULT);
            if (navigation.Contains(LAST_RESULT))
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


        private void timer1_Tick(object sender, EventArgs e)
        {
            DoOcr();
            Game.Update();
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
            //Game.t.Abort();
            Application.Exit();
        }
    }
}
