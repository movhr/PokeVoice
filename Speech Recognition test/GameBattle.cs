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
        public bool InBattle;
        public bool HasMoves;
        public string LastMove;
        public string[] MoveList;
        public int MoveIndex = 0;
        private static readonly string[] NewlineDelimiter = { "\n" };
        private Grammar _movesGrammar;
        private Grammar _pokemonGrammar;
        private Grammar _selectionGrammar;

        public static string[] BattleOptions = { "FIGHT", "PACK", "POKéMON", "RUN", "again" };
        public static readonly string[] BattleSpecific = { "enter trainer battle", "enter wild battle", "read moves" };
        public static string[] CurrentPokemon { get; private set; }
        public static string[] CurrentOptions { get; private set; }

        public BattleState CurrentState;
        public enum BattleState
        {
            Battle,
            Fight,
            Pack,
            Pokemon,
            NotInBattle,
            SelectOptions
        }

        public OpponentType Opponent;
        public enum OpponentType
        {
            Wild,
            Trainer
        }

        public static Grammar CreateGrammar(params string[] phrases)
        {
            var choises = new Choices();
            choises.Add(phrases);
            var gb = new GrammarBuilder();
            gb.Append(choises);
            var g = new Grammar(gb);
            return g;
        }

        public void SetOptions()
        {
            CurrentOptions = Ocr.GetOptionsFromRectangle(Ocr.PokemonBattleOptionsLocation, out int index);
            BattleMenuCursor.OptionsCursor = index;
            if (_selectionGrammar != null)
                _form.Recognizer.UnloadGrammar(_selectionGrammar);

            _selectionGrammar = CreateGrammar(CurrentOptions);
            _form.Recognizer.LoadGrammar(_selectionGrammar);
        }

        public void SetMoves(string str)
        {
            // UpdateGUI moves
            var lines = str.Split(NewlineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where((x, i) => i > 0 && !String.IsNullOrWhiteSpace(x)) //Remove added confidence from OCR and check for whitelines
                .ToArray();

            if (lines.Length > 4)
                throw new IndexOutOfRangeException("Not a valid amount of moves given.");

            MoveList = lines.Select((x, i) => x.StartsWith(">") ? x.Substring(1) : x).ToArray();
            BattleMenuCursor.FightCursor = Array.IndexOf(lines, lines.FirstOrDefault((x) => x.StartsWith(">")));
            _form.listBox1.Items.Clear();
            foreach (string move in MoveList)
                _form.listBox1.Items.Add(move);

            // Create new speech grammar
            if (_movesGrammar != null)
                _form.Recognizer.UnloadGrammar(_movesGrammar);

            _movesGrammar = CreateGrammar(MoveList);
            _form.Recognizer.LoadGrammar(_movesGrammar);
        }

        public void SetPokemon()
        {
            CurrentPokemon = Pokemon.ReadPokemon().ToArray();

            if (_pokemonGrammar != null)
                _form.Recognizer.UnloadGrammar(_pokemonGrammar);

            _pokemonGrammar = CreateGrammar(CurrentPokemon);
            _form.Recognizer.LoadGrammar(_pokemonGrammar);

            _form.Pokemons.Items.Clear();
            foreach (var pok in CurrentPokemon)
                _form.Pokemons.Items.Add(pok);
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
                        var moves = Ocr.ReadFromRectangle(Ocr.ConsoleTextLocation);
                        SetMoves(moves);
                        HasMoves = true;
                    }
                    CurrentState = BattleState.Fight;
                    break;
                case "POKéMON":
                    BattleMenuCursor.SelectAction(BattleMenuCursor.Pokemon);
                    Thread.Sleep(1000); //Let the screen load
                    SetPokemon();
                    CurrentState = BattleState.Pokemon;
                    break;
                case "PACK":
                    BattleMenuCursor.SelectAction(BattleMenuCursor.Pack);
                    CurrentState = BattleState.Pack;
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
            var success = ChooseIndexableItem(ref BattleMenuCursor.FightCursor, move, MoveList, () => HasMoves);
            if (!success)
            {
                CurrentState = BattleState.Battle;
                return;
            }
            LastMove = move;
            CurrentState = BattleState.Battle;
        }

        public void ChoosePokemon(string pokemon)
        {
            var success = ChooseIndexableItem(ref BattleMenuCursor.PokemonCursor, pokemon, CurrentPokemon);
            if (!success)
            {
                CurrentState = BattleState.Battle;
                return;
            }
            Thread.Sleep(500); //Let the screen load
            SetOptions();
            CurrentState = BattleState.SelectOptions;
        }

        public void ChooseOption(string option)
        {
            var success = ChooseIndexableItem(ref BattleMenuCursor.OptionsCursor, option, CurrentOptions);
            CurrentState = success ? BattleState.Battle : BattleState.Pokemon;
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
    }
}
