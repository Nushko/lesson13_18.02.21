using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace lesson13_18._02._21
{
    static class MatrixStart
    {
        public static void Start()
        {
                Console.SetWindowSize(101, 31);
                Console.CursorVisible = false;
                Console.WriteLine(
                    "Press any key to enter the Matrix" +
                    "(Press any key again to exit the Matrix)..."
                    );
                Console.ReadKey();
                Console.Clear();
                var task = Task.Run(Matrix.Start);
                Console.ReadKey();
            }
            public static class Matrix
            {
                public const int width = 100;
                public const int height = 30;
                public const int lines = 50;
                public const int delay = 10;

                public static async Task Start()
                {
                    var tasks = new List<Task>();
                    for (var i = 0; i < lines; i += 1)
                    {
                        var task = Task.Run(OutLine);
                        tasks.Add(task);
                        await Task.Delay(delay);
                    }
                }

                private static async Task OutLine()
                {
                    while (true)
                    {
                        var column = RandomHelper.Rand(0, width);
                        await MatrixLine.StartNew(column);
                    }
                }
            }

            public class MatrixLine
            {
                private readonly string symbols = "AaNnUuSsHh";
                private readonly int matcolumn;
                private readonly int length;
                private readonly int minlength = 4;
                private readonly int maxlength = 18;
                private readonly int minupdate = 10;
                private readonly int maxupdate = 15;
                private readonly int updateTime;
                private char prev1 = ' ';
                private char prev2 = ' ';
                private int row;

                private MatrixLine(int column)
                {
                    length = RandomHelper.Rand(minlength, maxlength + 1);
                    updateTime = RandomHelper.Rand(minupdate, maxupdate + 1);
                    matcolumn = column;
                }

                public static async Task StartNew(int column)
                {
                    var line = new MatrixLine(column);
                    await line.Start();
                }

                private async Task Start()
                {
                    for (var i = 0; i < Matrix.height + length; i += 1)
                    {
                        Step();
                        await Task.Delay(updateTime);
                    }
                }

                private static bool BordersCheck(int row)
                {
                    return row > 0 && row < Matrix.height;
                }

                private void Step()
                {
                    if (BordersCheck(row))
                    {
                        var symbol = symbols[RandomHelper.Rand(0, symbols.Length)];
                        ConsoleHelper.Display(new ConsoleTask(matcolumn, row, symbol, ConsoleColor.White));
                        prev1 = symbol;
                    }

                    if (BordersCheck(row - 1))
                    {
                        ConsoleHelper.Display(new ConsoleTask(matcolumn, row - 1, prev1, ConsoleColor.Green));
                        prev2 = prev1;
                    }

                    if (BordersCheck(row - 2))
                    {
                        ConsoleHelper.Display(new ConsoleTask(matcolumn, row - 2, prev2, ConsoleColor.DarkGreen));
                    }

                    if (BordersCheck(row - length))
                    {
                        ConsoleHelper.Display(new ConsoleTask(matcolumn, row - length, ' ', ConsoleColor.Black));
                    }
                    row += 1;
                }
            }

            public class ConsoleTask
            {
                public readonly ConsoleColor Color;
                public readonly int Column;
                public readonly int Row;
                public readonly char Symbol;

                public ConsoleTask(int column, int row, char symbol, ConsoleColor color)
                {
                    Color = color;
                    Column = column;
                    Row = row;
                    Symbol = symbol;
                }
            }

            public static class ConsoleHelper
            {
                private static readonly ConcurrentQueue<ConsoleTask> Queue = new ConcurrentQueue<ConsoleTask>();
                private static bool inprocess;

                public static void Display(ConsoleTask task)
                {
                    Queue.Enqueue(task);
                    DisplayCore();
                }

                private static void DisplayCore()
                {
                    while (true)
                    {
                        if (inprocess)
                        {
                            return;
                        }

                        lock (Queue)
                        {
                            if (inprocess)
                            {
                                return;
                            }

                            inprocess = true;
                        }

                        while (Queue.TryDequeue(out var task))
                        {
                            Console.SetCursorPosition(task.Column, task.Row);
                            Console.ForegroundColor = task.Color;
                            Console.Write(task.Symbol);
                        }

                        lock (Queue)
                        {
                            inprocess = false;
                            if (!Queue.IsEmpty)
                            {
                                continue;
                            }
                        }
                        break;
                    }
                }
            }

            public static class RandomHelper
            {
                private static int off = Environment.TickCount;

                private static readonly ThreadLocal<Random> Random =
                        new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref off)));

                public static int Rand(int min, int max)
                {
                    return Random.Value.Next(min, max);
                }
            }
    }
}
