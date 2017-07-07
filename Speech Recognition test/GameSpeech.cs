using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public static readonly string[] Moving = { "walk left", "walk right", "walk up", "walk down", "stop" };

        //Helper for taking over speech recognition regarding battle
        public void RelayVoiceText(string text)
        {

            if (text == "read moves")
                SetMoves(_form.DoOcr());

            if (text == "enter trainer battle")
            {
                _game.EnterBattle(Game.OpponentType.Trainer);
                return;
            }


            if (text == "enter wild battle")
            {
                _game.EnterBattle(Game.OpponentType.Wild);
                return;
            }

            if (Moving.Contains(text))
            {
                if (text == "stop")
                {
                    _movingDirection.Reset();
                }
                else
                {
                    if (text.Contains("right"))
                        _movingDirection = new Vector(1, 0);
                    else if (text.Contains("left"))
                        _movingDirection = new Vector(-1, 0);
                    else if (text.Contains("up"))
                        _movingDirection = new Vector(0, -1);
                    else if (text.Contains("down"))
                        _movingDirection = new Vector(0, 1);
                }
                return;
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
                    if (Form1.Cancelation.Contains(text))
                        CurrentState = BattleState.Battle;
                    else
                        ChooseMove(text);
                    return;
                case BattleState.Pack:
                    break;
                case BattleState.Pokemon:
                    ChoosePokemon(text);
                    break;
                case BattleState.NotInBattle:
                    break;
                case BattleState.SelectOptions:
                    ChooseOption(text);
                    BattleMenuCursor.BattleCursor.Reset();
                    break;
                default:
                    break;
            }
        }
    }
}
