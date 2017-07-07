using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public class Pokemon
    {
        private static readonly int distanceBetweenNames = 16;

        private readonly List<string> PokemonNames;
        private readonly MyRectangle[] pokemonNameLocations = new MyRectangle[6];
        private static Pokemon _pokemon;

        private Pokemon()
        {
            PokemonNames = new List<string>(6);
            for (int i = 0; i < 6; i++)
            {
                pokemonNameLocations[i] =
                    MyRectangle.RelativeToSize(Ocr.WindowLocation, 7 + i * distanceBetweenNames, 24, 75, 9);
            }
        }

        public static IEnumerable<string> ReadPokemon(bool _override = false)
        {
            _pokemon = _pokemon ?? new Pokemon();
            if (!_override && _pokemon.PokemonNames.Count > 0)
                return _pokemon.PokemonNames;

            for (int i = 0; i < 6; i++)
            { 
                var text = Ocr.ReadFromRectangle(_pokemon.pokemonNameLocations[i], true);
                if (text.Contains("NCEL"))
                    break;
                _pokemon.PokemonNames.Add(text.Replace("\n", ""));
            }
            return _pokemon.PokemonNames.Where(x => !string.IsNullOrWhiteSpace(x));
        }
    }
}
