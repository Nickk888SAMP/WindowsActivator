using System;

namespace WindowsActivator
{
    public static class Printer
    {
        public static void Print(string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            Print(text, false, backgroundColor, foregroundColor);
        }

        public static void Print(string text, bool inline, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
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
