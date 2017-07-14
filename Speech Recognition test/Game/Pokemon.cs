using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public class Pokemon
        {
            public string Name { get; private set; }
            public List<string> Moves { get; private set; }
            public string HeldItem { get; private set; }

            public Pokemon(string name, string heldItem = null, params string[] moves)
            {
                Name = name;
                HeldItem = heldItem;
                Moves = new List<string>(4);
                Moves.AddRange(moves);
            }

        }
    }
}
