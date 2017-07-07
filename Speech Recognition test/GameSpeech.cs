using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public static readonly string[] Moving = { "walk left", "walk right", "walk up", "walk down", "stop" };
        private Grammar currentGrammar;
        private Grammar previousGrammar;

        public void DoPreviousEventChain()
        {
            EventLink[] t = new EventLink[PreviousEventChain.Count];
            PreviousEventChain.CopyTo(t, 0);
            foreach (var evt in t) {
                try
                {
                    evt.Execute();
                    Thread.Sleep(250);
                }
                catch (Exception)
                {
                    // ignored
                } //Raises when loading new grammar
            }
        }

        public void OnBattle(string text)
        {
            if (MoveList != null && MoveList.Contains(text))
            {
                ChooseAction("FIGHT");
                ChooseMove(text);
            }
            else if (text == "again")
            {
                DoPreviousEventChain();
                FlushBattleState();
            }
            else
            {
                ChooseAction(text);
            }
        }

        public void SwitchGrammarAndState(Grammar newGrammar, int newState)
        {
            PreviousState = CurrentState;
            previousGrammar = currentGrammar;
            _form.Recognizer.UnloadGrammar(currentGrammar);
            currentGrammar = newGrammar;
            CurrentState = newState;
            _form.Recognizer.LoadGrammar(currentGrammar);
        }

        public void FlushBattleState()
        {
            PreviousEventChain = CurrentEventChain;
            SwitchGrammarAndState(_battleGrammar, BattleState.Battle);

        }

        public void Return()
        {
            CurrentEventChain.RemoveLast();
            KeySender.Back();
            SwitchGrammarAndState(previousGrammar, PreviousState);
        }

        public void ChooseBattleOption(Vector target, Func<Grammar> newGrammar, int newState, int sleepTime = 0)
        {
            CurrentEventChain.AddLast(new EventLink(() => ChooseBattleOption(target, newGrammar, newState, sleepTime)));
            BattleMenuCursor.SelectAction(target);
            if(sleepTime > 0)
                Thread.Sleep(sleepTime);
            SwitchGrammarAndState(newGrammar(), newState);
        }

        public void OnFight() => ChooseBattleOption(BattleMenuCursor.Fight, GetMovesGrammar, BattleState.Fight);

        public void OnPokemon() => ChooseBattleOption(BattleMenuCursor.Pokemon, GetPokemonGrammar, BattleState.Pokemon, 1000);

        public void OnPack()
        {
            BattleMenuCursor.SelectAction(BattleMenuCursor.Pack);
            CurrentState = BattleState.Pack;
        }

        public void OnRun()
        {
            BattleMenuCursor.SelectAction(BattleMenuCursor.Run);
            ExitBattle();
        }

        //Helper for taking over speech recognition regarding battle
        public bool RelayVoiceText(string text)
        {

            if (text == "read moves")
            {
                GetMovesGrammar();
                return true;
            }

            if (text == "enter trainer battle")
            {
                _game.EnterBattle(Game.OpponentType.Trainer);
                return true;
            }

            if (text == "enter wild battle")
            {
                _game.EnterBattle(Game.OpponentType.Wild);
                return true;
            }

            if (Moving.Contains(text))
            {
                if (text == "stop")
                {
                    _movingDirection.Reset();
                    return true;
                }
                if (text.Contains("right"))
                    _movingDirection = new Vector(1, 0);
                else if (text.Contains("left"))
                    _movingDirection = new Vector(-1, 0);
                else if (text.Contains("up"))
                    _movingDirection = new Vector(0, -1);
                else if (text.Contains("down"))
                    _movingDirection = new Vector(0, 1);
                else throw new Exception();
                return true;
            }

            if (InBattle)
            {
                if (Form1.Cancelation.Contains(text))
                {
                    if (PreviousState != BattleState.None && PreviousState != BattleState.NotInBattle)
                        Return();
                    return true;
                }
                switch (CurrentState)
                {
                    case BattleState.Battle:
                        OnBattle(text);
                        return true;
                    case BattleState.Fight:
                        ChooseMove(text);
                        return true;
                    case BattleState.Pack:
                        return true;
                    case BattleState.Pokemon:
                        ChoosePokemon(text);
                        return true;
                    case BattleState.SelectOptions:
                        ChooseOption(text);
                        BattleMenuCursor.BattleCursor.Reset();
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }


        private bool ChooseIndexableItem(ref int cursor, string item, string[] itemCollection, Func<bool> checker = null)
        {
            if (checker != null && !checker())
            {
                _form.statusLabel.Text = "Didnt pass check";
                return false;
            }
            _form.statusLabel.Text = item;
            var itemIndex = Array.IndexOf(itemCollection, item);
            if (itemIndex < 0)
            {
                _form.statusLabel.Text = "Could not find item index";
                KeySender.Back();
                return false;
            }

            BattleMenuCursor.SelectIndex(ref cursor, itemIndex, itemCollection.Length);
            return true;
        }

        public void ChooseMove(string move)
        {
            var success = ChooseIndexableItem(ref BattleMenuCursor.FightCursor, move, MoveList);
            if (!success)
            {
                CurrentState = BattleState.Battle;
                return;
            }
            CurrentEventChain.AddLast(new EventLink(() => ChooseMove(move)));
            LastMove = move;
            FlushBattleState();
        }

        public void ChoosePokemon(string pokemon)
        {
            var success = ChooseIndexableItem(ref BattleMenuCursor.PokemonCursor, pokemon, CurrentPokemon);
            if (!success)
            {
                CurrentState = BattleState.Battle;
                return;
            }
            CurrentEventChain.AddLast(new EventLink(() => ChoosePokemon(pokemon)));
            Thread.Sleep(500); //Let the screen load
            SwitchGrammarAndState(GetOptionsGrammar(), BattleState.SelectOptions);
        }

        public void ChooseOption(string option)
        {
            var success = ChooseIndexableItem(ref BattleMenuCursor.OptionsCursor, option, CurrentOptions);
            if (!success) return;
            CurrentEventChain.AddLast(new EventLink(() => ChooseOption(option)));
            FlushBattleState();
        }
    }
}
