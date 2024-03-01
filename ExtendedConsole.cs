using System.Text.RegularExpressions;
using UT.Data.Extensions;

namespace UT.Data
{
    public partial class ExtendedConsole
    {
        #region Statics
        private static bool boxingMode;
        private static int boxingSize;
        #endregion //Statics

        #region Public Methods
        public static void BoxMode(bool enabled, int? length=null)
        {
            ExtendedConsole.boxingMode = enabled;
            if(length == null)
            {
                length = 64;
            }
            if(enabled)
            {
                ExtendedConsole.boxingSize = length.Value;
            }

            int size = ExtendedConsole.boxingSize;

            Console.WriteLine("* " + ("-").Repeat(size) + " *");

            if (!enabled)
            {
                Console.WriteLine();
            }
        }

        public static void WriteLine(string text)
        {
            ConsoleColor baseColor = Console.ForegroundColor;
            if(ExtendedConsole.boxingMode)
            {
                int len = EndTagMatchRegex().Replace(StartTagMatchRegex().Replace(text, ""), "").Length;
                int padding = ExtendedConsole.boxingSize - len;

                text = "| " + text + (" ".Repeat(padding)) + " |";
            }

            List<ConsoleColor> colors = [];
            foreach(Match match in StartTagMatchRegex().Matches(text).Cast<Match>())
            {
                string txtMatch = match.Value;
                string txtColor = txtMatch[1..(txtMatch.Length - 1)];
                foreach(ConsoleColor color in ExtendedConsole.Colors())
                {
                    if(color.ToString().Equals(txtColor, StringComparison.CurrentCultureIgnoreCase))
                    {
                        colors.Add(color);
                    }
                }
            }

            string txtBuffer = text;
            string lcBuffer = txtBuffer.ToLower();
            List<Tuple<string, ConsoleColor>> buffer = [];
            foreach(ConsoleColor color in colors)
            {
                string sTag = ("<" + color + ">").ToLower();
                string eTag = ("</" + color + ">").ToLower();

                int s = lcBuffer.IndexOf(sTag);
                int e = lcBuffer.IndexOf(eTag);

                if(s > 0)
                {
                    buffer.Add(new Tuple<string, ConsoleColor>(txtBuffer[0..s], baseColor));
                }

                s += sTag.Length;
                string cut = txtBuffer[s..e];
                e += eTag.Length;

                buffer.Add(new Tuple<string, ConsoleColor>(cut, color));

                if (e > txtBuffer.Length)
                {
                    break;
                }
                txtBuffer = txtBuffer[e..txtBuffer.Length];
                lcBuffer = txtBuffer.ToLower();
            }

            if (txtBuffer.Length != 0)
            {
                buffer.Add(new Tuple<string, ConsoleColor>(txtBuffer, baseColor));
            }

            foreach (Tuple<string, ConsoleColor> tuple in buffer)
            {
                Console.ForegroundColor = tuple.Item2;
                Console.Write(tuple.Item1);
            }
            Console.WriteLine("");
            Console.ForegroundColor = baseColor;
        }
        #endregion //Public Methods

        #region Private Methods
        private static ConsoleColor[] Colors()
        {
            return
            [
                ConsoleColor.Black, 
                ConsoleColor.Blue, 
                ConsoleColor.Cyan, 
                ConsoleColor.DarkBlue, 
                ConsoleColor.DarkCyan, 
                ConsoleColor.DarkGray, 
                ConsoleColor.DarkGreen, 
                ConsoleColor.DarkMagenta, 
                ConsoleColor.DarkRed, 
                ConsoleColor.DarkYellow,
                ConsoleColor.Gray, 
                ConsoleColor.Green, 
                ConsoleColor.Magenta, 
                ConsoleColor.Red, 
                ConsoleColor.White, 
                ConsoleColor.Yellow,
            ];
        }

        [GeneratedRegex(@"\<[a-z]+\>", RegexOptions.IgnoreCase)]
        private static partial Regex StartTagMatchRegex();

        [GeneratedRegex(@"\<\/[a-z]+\>", RegexOptions.IgnoreCase)]
        private static partial Regex EndTagMatchRegex();
        #endregion //Private Methods
    }
}
