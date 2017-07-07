using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public class BattleOption
        {
            private static readonly BattleOption Fight = new BattleOption("FIGHT", _game.OnFight);
            private static readonly BattleOption Pokemon = new BattleOption("POKéMON", _game.OnPokemon);
            private static readonly BattleOption Pack = new BattleOption("PACK", _game.OnPack);
            private static readonly BattleOption Run = new BattleOption("RUN", _game.OnRun);
            private static readonly BattleOption[] Options = {Fight, Pokemon, Pack, Run};
            public static void Trigger(string str) => Options.FirstOrDefault(x => x.Name == str)?.F();

            public string Name;
            public Action F;

            public BattleOption(string name, Action f)
            {
                Name = name;
                F = f;
            }

        }
    }
}
