using System;
using System.Threading;
using System.Threading.Tasks;

namespace SvnTools
{
    public static class Threading
    {
        public static void DoThread(Action action)
        {
            Task task = new Task(action);
            task.Start();

            if (Console.CursorLeft > 0)
            {
                Console.WriteLine();
            }

            Console.Write("Working");

            // Animate some dots appearing in order
            int dotsStart = Console.CursorLeft;
            const int kDotCount = 3;
            int currentDot = 0;
            while (!task.IsCompleted)
            {
                if (currentDot >= kDotCount)
                {
                    Console.SetCursorPosition(dotsStart, Console.CursorTop);
                    Console.Write("          ");
                    Console.SetCursorPosition(dotsStart, Console.CursorTop);
                    currentDot = 0;
                }

                Console.Write(".");
                ++currentDot;

                Thread.Sleep(1000);
            }

            // Clear the line and return cursor to the start
            int numChars = Console.CursorLeft;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new String(' ', numChars));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}
