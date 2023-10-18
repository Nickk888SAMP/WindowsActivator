using System;

namespace WindowsActivator
{
    static class Printer
    {
        public static void Print(string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            Print(text, false, backgroundColor, foregroundColor);
        }

        public static void Print(string text, bool inline, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            if (Program.noOutput)
                return;

            Console.ResetColor();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            if (!inline)
                Console.WriteLine(text);
            else
                Console.Write(text);
            Console.ResetColor();
        }

        public static void Print(string text, bool inline)
        {
            if (Program.noOutput)
                return;

            Console.ResetColor();

            if (!inline)
                Console.WriteLine(text);
            else
                Console.Write(text);
        }

        public static void Print(string text)
        {
            Print(text, false);
        }

        public static void Print()
        {
            Print(string.Empty);
        }
    }
}
