using System.Text.RegularExpressions;

namespace UT.Data
{
    public class ExtendedConsole
    {
        #region Public Methods
        public static void WriteLine(string text)
        {
            ConsoleColor baseColor = Console.ForegroundColor;

            List<ConsoleColor> colors = new();
            foreach(Match match in Regex.Matches(text, @"\<[a-z]+\>", RegexOptions.IgnoreCase))
            {
                string txtMatch = match.Value;
                string txtColor = txtMatch[1..(txtMatch.Length - 1)];
                foreach(ConsoleColor color in ExtendedConsole.Colors())
                {
                    if(color.ToString().ToLower() == txtColor.ToLower())
                    {
                        colors.Add(color);
                    }
                }
            }

            string txtBuffer = text;
            string lcBuffer = txtBuffer.ToLower();
            List<Tuple<string, ConsoleColor>> buffer = new();
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
                ConsoleColor.Yellow
            ];
        }
        #endregion //Private Methods
    }
}
