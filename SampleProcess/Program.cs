using System;

class Program
{
    static System.Timers.Timer timer = new System.Timers.Timer();
    static int count = 0;
    static void Main(string[] args)
    {
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
