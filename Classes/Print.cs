using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsActivator
{
    static class Print
    {
        public static void Write(string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            Console.ResetColor();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Write(string text)
        {
            Console.ResetColor();
            Console.WriteLine(text);
        }

        public static void Write()
        {
            Write(string.Empty);
        }
    }
}
