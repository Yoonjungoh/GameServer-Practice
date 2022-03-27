﻿
using System;
using System.Threading.Tasks;

class Program
{
    static int number = 0;
    static object _lock = new object();
    static void Thread_1()
    {
        for (int i = 0; i < 1000; i++)
            number++;
    }
    static void Thread_2()
    {
        for (int i = 0; i < 1000; i++)
            number--;
    }
    static void Main(string[] args)
    {
        Task t1 = new Task(Thread_1);
        Task t2 = new Task(Thread_2);

        t1.Start();
        t2.Start();

        Task.WaitAll(t1, t2);

        Console.WriteLine(number);

    }
}