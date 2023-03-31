using System;

namespace WindowsActivator
{
    static class Printer
    {
        public static void Print(string text, ConsoleColor backgroundColor, ConsoleColor foregroundColor)
        {
            Console.ResetColor();
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Print(string text)
        {
            Console.ResetColor();
            Console.WriteLine(text);
        }

        public static void Print()
        {
            Print(string.Empty);
        }
    }
}
