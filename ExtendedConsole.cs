﻿using System.Drawing;
using System.Text.RegularExpressions;
using UT.Data.Extensions;
using System.Linq;

namespace UT.Data
{
    public partial class ExtendedConsole
    {
        #region Statics
        private static bool boxingMode;
        private static int boxingSize;
        #endregion //Statics

        #region Enums
        public enum Alignment
        {
            Left, Right, Center
        }
        #endregion //Enums

        #region Public Methods
        public static void BoxMode(bool enabled, int? length=null)
        {
            ExtendedConsole.boxingMode = enabled;
            length ??= 64;
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

        public static void WriteLine(string text, Alignment alignment)
        {
            if(alignment == Alignment.Left)
            {
                ExtendedConsole.WriteLine(text);
            }

            string shortened = ExtendedConsoleTagReplacementRegex().Replace(text, "");
            int padding = ExtendedConsole.boxingSize - shortened.Length;
            if(alignment == Alignment.Center)
            {
                padding /= 2;
            }

            ExtendedConsole.WriteLine(" ".Repeat(padding) + text);
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
            colors.AddRange(from Match match in StartTagMatchRegex().Matches(text).Cast<Match>()
                            let txtMatch = match.Value
                            let txtColor = txtMatch[1..(txtMatch.Length - 1)]
                            from ConsoleColor color in ExtendedConsole.Colors()
                            where color.ToString().Equals(txtColor, StringComparison.CurrentCultureIgnoreCase)
                            select color);
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
        [GeneratedRegex(@"<\/?[a-z]+>", RegexOptions.IgnoreCase, "en-NL")]
        private static partial Regex ExtendedConsoleTagReplacementRegex();
        #endregion //Private Methods
    }
}
