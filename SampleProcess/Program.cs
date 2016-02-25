using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProcess
{
    /// <summary>
    /// This is a little command line application that our debugger will start up so we can test things out on it.
    /// </summary>
    class Program
    {
        static System.Timers.Timer timer = new System.Timers.Timer();
        static int count = 0;
        static void Main(string[] args)
        {
            var processId = Process.GetCurrentProcess().Id;
            Console.WriteLine("Process Id: " + processId);

            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 2000;
            timer.Start();
            Console.ReadLine();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Hit " + count++);
        }
    }
}
