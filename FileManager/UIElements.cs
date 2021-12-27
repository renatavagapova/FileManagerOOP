using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager
{
    public class UIElements
    {
        static public void FirstLine()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(2, 1);
            Console.WriteLine("File Manager (C) Vagapova Renata");
            Console.SetCursorPosition(113, 1);
            Console.WriteLine($"{DateTime.Now.DayOfWeek}");

            Console.SetCursorPosition(200, 1);
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MMM-dd")}");
        }

        static public void SecondLine()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(2, 4);
            Console.WriteLine("Помощь-F1   Выход-Escape   Переход-Tab ");
        }
    }
}
