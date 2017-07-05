using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public static readonly BitArray EmptyTextBox = new BitArray(new []
        {
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, true,
            true, true, true, true, true, true, true, true, true, true, true, true, true, false
        });

        public void RelayOcr(string text)
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

        public void TextboxContinueTalking()
        {
            using (var bmp = PictureRecognition.ShootScreen(Ocr.ConsoleLocation))
            {
                var ph = PictureRecognition.GetHashProbability(bmp, EmptyTextBox);
                if (ph > 75)
                {
                    _movingDirection = new Vector();
                    using (var ss = PictureRecognition.ShootScreen(
                        MyRectangle.RelativeToSize(Ocr.WindowLocation, 136, 144, 7, 7)))
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
}
