using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace taskConsoleApp
{
    class AsyncEx
    {
        private static int DoAsyncStuff()
        {
            for (int i = 5; i < 10;)
            {
                i++;
                Thread.Sleep(300);
                Console.WriteLine($"{i} в квадрате = {i * i}");
                return i * i;
            }
            Console.WriteLine("Операция завершена");
            return 0;
        }
        public static async Task<int> Start()
        {
            Thread.Sleep(300);

            var result = Task.Run(() => DoAsyncStuff());

            return await result;
        }
    }
}
