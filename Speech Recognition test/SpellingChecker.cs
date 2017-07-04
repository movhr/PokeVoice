using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speech_Recognition_test
{
    public static class SpellingChecker
    {
        public static string Fix(string input)
        {
            // Common mistakes
            // u sea -> used
            // u sed -> used
            // F I GH T -> FIGHT
            // [*]a -> [*]d  (e.g.)
            // >  ana -> and
            // > shoula -> should

            string nospace = input.Replace(" ", "");
            return input;

        }

        public static string FixIchar(string str)
        {
            return str.Replace(" I ", "I");
        }

        private static char[] ignoredCharacters = {'\n', '!', '?'};

        public static string CheckAndReplaceLastChar(string str, char oldChar, char newChar, out bool hasChanged, bool shouldReplaceIfAlone = false)
        {
            if (str.Length == 0 || (str.Length == 1 && !shouldReplaceIfAlone))
            {
                hasChanged = false;
                return str;
            }

            int lastCharPtr = str.Length - 1;
            while (ignoredCharacters.Contains(str[lastCharPtr]))
            {
                lastCharPtr--;

                if (lastCharPtr <= 0)
                {
                    hasChanged = false;
                    return str;
                }
            }

            if (str[lastCharPtr] == oldChar)
            {
                hasChanged = true;
                var chArr = str.ToCharArray();
                chArr[lastCharPtr] = newChar;
                bool b;
                var newStr = new string(chArr);
                do newStr = CheckAndReplaceLastChar(newStr, oldChar, newChar, out b, shouldReplaceIfAlone); while (b);
                return newStr;
            }
            hasChanged = false;
            return str;
        }

        public static string Check(string str)
        {
            var splStr = str.Split(' ');
            var strList = splStr.Select(x => x.Split('\n'));
            splStr = strList.SelectMany( x => x).ToArray();

            bool b;
            for (int i = 0; i < splStr.Length; i++)
            {
                splStr[i] = CheckAndReplaceLastChar(splStr[i], 'S', '!', out b, true);
                if (!b)
                    splStr[i] = CheckAndReplaceLastChar(splStr[i], '9', '!', out b, true);
                splStr[i] = CheckAndReplaceLastChar(splStr[i], 'a', 'd', out b);
                
                if (splStr[i] == "POKSMON")
                    splStr[i] = "POKeMON";
            }
            return string.Join(" ", splStr);
        }


        public static IEnumerable<string> Choises(string str)
        {
            var splStr = str.Split(new char[' '], StringSplitOptions.RemoveEmptyEntries);
            foreach (var split in splStr)
            {
                if (split[0] == 'I')
                    yield return split.Substring(1);
                yield return split;
            }
        }
    }
}
