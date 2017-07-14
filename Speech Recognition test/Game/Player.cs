using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public partial class Game
    {
        public static partial class Player
        {
            public static List<Pokemon> Pokemons { get; private set; }
            
            public static void SwapPokemon(string swap, string with)
            {
                var fst = Pokemons.FindIndex(x => x.Name == swap);
                var snd = Pokemons.FindIndex(y => y.Name == with);
                var t = Pokemons[fst];
                Pokemons[fst] = Pokemons[snd];
                Pokemons[snd] = t;
            }

            public static void CatchPokemon(string name)
            {
                if (Pokemons.Count == 6)
                    return;
                Pokemons.Add(new Pokemon(name));
            }

            public static void SetPokemonMoves(string pokemonName, params string[] moveNames)
            {
                if (moveNames.Length > 4)
                    throw new ArgumentException("Move count cannot exceed four.");
                Pokemons.FirstOrDefault(x => x.Name == pokemonName)?.Moves.AddRange(moveNames);
            }

            public static void LearnPokemonMove(string pokemonName, string newMoveName, string oldMoveName = "")
            {
                var p = Pokemons.Find(x => x.Name == pokemonName);
                if (string.IsNullOrWhiteSpace(oldMoveName))
                    p.Moves.Add(newMoveName);
                else
                {
                    var oldMoveIndex = p.Moves.FindIndex(x => x == oldMoveName);
                    if (oldMoveIndex < 0)
                        throw new ArgumentException("Move name is not valid");
                    p.Moves[oldMoveIndex] = newMoveName;
                }
            }
        }
    }
}
