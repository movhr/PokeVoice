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
        public LinkedList<EventLink> PreviousEventChain;
        public LinkedList<EventLink> CurrentEventChain;

        public bool InBattle;
        public bool HasMoves;
        public string LastMove;
        public string[] MoveList;
        public int MoveIndex = 0;
        private static readonly string[] NewlineDelimiter = { "\n" };
        private Grammar _movesGrammar;
        private Grammar _pokemonGrammar;
        private Grammar _selectionGrammar;
        private Grammar _battleGrammar;

        public static string[] BattleOptions = { "FIGHT", "PACK", "POKéMON", "RUN", "again" };
        public static readonly string[] BattleSpecific = { "enter trainer battle", "enter wild battle", "read moves" };
        public static string[] CurrentPokemon { get; private set; }
        public static string[] CurrentOptions { get; private set; }

        public int CurrentState;
        public int PreviousState;

        public OpponentType Opponent;
        public enum OpponentType
        {
            Wild,
            Trainer
        }

        //Step 1: enter battle
        public void EnterBattle(OpponentType opponent)
        {
            if (InBattle)
                return;
            InBattle = true;
            CurrentState = BattleState.Battle;
            _form.battleModeLabel.Text = "IN BATTLE";
            Opponent = opponent;

            var battleGrammar = GetBattleOptionsGrammar();
            _form.Recognizer.LoadGrammar(battleGrammar);
            currentGrammar = battleGrammar;
        }

        //Step 2: select action (fight, pack, etc)
        public void ChooseAction(string action)
        {
            CurrentEventChain.AddLast(new EventLink(() => BattleOption.Trigger(action)));
            BattleOption.Trigger(action);
        }

        public void ExitBattle()
        {
            if(_battleGrammar != null)
                _form.Recognizer.UnloadGrammar(_battleGrammar);
            HasMoves = false;
            _form.battleModeLabel.Text = "not in battle";
            CurrentState = BattleState.NotInBattle;
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

        public Grammar GetBattleOptionsGrammar()
        {
            BattleMenuCursor.Reset();
            return _battleGrammar ?? (_battleGrammar = CreateGrammar(BattleOptions));
        }

        public Grammar GetOptionsGrammar()
        {
            CurrentOptions = Ocr.GetOptionsFromRectangle(Ocr.PokemonBattleOptionsLocation, out int index);
            BattleMenuCursor.OptionsCursor = index;

            _selectionGrammar = CreateGrammar(CurrentOptions);
            return _selectionGrammar;
        }

        public Grammar GetMovesGrammar()
        {
            var str = Ocr.ReadFromRectangle(Ocr.ConsoleTextLocation);

            // UpdateGUI moves
            var lines = str.Split(NewlineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Where((x, i) => i > 0 && !string.IsNullOrWhiteSpace(x)) //Remove added confidence from OCR and check for whitelines
                .ToArray();

            if (lines.Length > 4)
                throw new IndexOutOfRangeException("Not a valid amount of moves given.");

            MoveList = lines.Select((x, i) => x.StartsWith(">") ? x.Substring(1) : x).ToArray();
            BattleMenuCursor.FightCursor = Array.IndexOf(lines, lines.FirstOrDefault((x) => x.StartsWith(">")));
            _form.listBox1.Items.Clear();
            foreach (string move in MoveList)
                _form.listBox1.Items.Add(move);
            
            _movesGrammar = CreateGrammar(MoveList);
            return _movesGrammar;
        }

        public Grammar GetPokemonGrammar()
        {
            CurrentPokemon = Pokemon.ReadPokemon().ToArray();

            _form.Pokemons.Items.Clear();
            foreach (var pok in CurrentPokemon)
                _form.Pokemons.Items.Add(pok);
            _pokemonGrammar = CreateGrammar(CurrentPokemon);
            return _pokemonGrammar;
        }
    }
}
