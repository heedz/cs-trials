using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CobaThread
{
    public interface Motor
    {
        int Speed { get; }

        int Run(int startPoint);
    }

    public class MotorLowSpeed:Motor
    {
        public int Speed { get; private set; }

        public MotorLowSpeed()
        {
            Speed = 1000;
        }

        public int Run(int startPoint)
        {
            Thread.Sleep(Speed);

            return startPoint + 1;
        }
    }

    public class MotorHiSpeed : Motor
    {
        public int Speed { get; private set; }

        public MotorHiSpeed()
        {
            Speed = 500;
        }

        public int Run(int startPoint)
        {
            Thread.Sleep(Speed);

            return startPoint + 1;
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Program
    {
        static Point StartPoint { get; set; }
        static Point EndPoint { get; set; }

        static Motor _motorX = new MotorHiSpeed();
        static Motor _motorY = new MotorHiSpeed();

        static object _locker = new object();
        static int _val1;

        static SemaphoreSlim _semaphore;

        static object signal = new object();

        static void Main(string[] args)
        {
            /*Console.WriteLine("Parameterized Thread");
            StartPoint = new Point(0, 0);
            EndPoint = new Point(20, 7);

            Thread tX = new Thread(delegate() { RunX(_motorX); });
            Thread tY = new Thread(delegate() { RunY(_motorY); });

            tX.Start();
            tY.Start();

            tX.Join();
            tY.Join();

            Console.ReadKey();

            Console.WriteLine("Parameterless Thread");
            Thread tZ = new Thread(OnThread);
            tZ.IsBackground = true;

            tZ.Start();

            tZ.Join();

            Console.ReadKey();

            Console.WriteLine("Thread Pool");
            ThreadPool.QueueUserWorkItem(Go);
            ThreadPool.QueueUserWorkItem(Go, "1234");

            Console.ReadKey();

            Console.WriteLine("Async Delegate");
            WorkInvoker method = Work;

            IAsyncResult cookie = method.BeginInvoke(3, null, null);
            
            int result = method.EndInvoke(cookie);
            Console.WriteLine(result);

            Console.ReadKey();

            Console.WriteLine("Checking all threads");
            Console.WriteLine("tX, isAlive = " + tX.IsAlive);
            Console.WriteLine("tY, isAlive = " + tY.IsAlive);
            Console.WriteLine("tZ, isAlive = " + tZ.IsAlive);
            Console.WriteLine("cookie, isCompleted = " + cookie.IsCompleted);

            Console.ReadKey();
            
            Console.WriteLine("Locking");
            Thread tLock1 = new Thread(LockTrial);
            Thread tLock2 = new Thread(LockTrial);

            tLock1.Start();
            tLock2.Start();

            tLock1.Join();
            tLock2.Join();

            Console.ReadKey();
            */
            Console.WriteLine("Semaphore");
            _semaphore = new SemaphoreSlim(3);

            for (int i = 0; i < 5; i++)
            {
                Thread tSem = new Thread(SemaphoreTrial);

                tSem.Start(i);
            }

            Console.ReadKey();
        }

        static void Worker()
        {
            while (true)
            {
                Monitor.Wait(signal);
                Thread.Sleep(1000);
            }
        }

        static void Requestor()
        {
        }

        static void SemaphoreTrial(object id)
        {
            Console.WriteLine(id + " is waiting!");
            _semaphore.Wait();
            Console.WriteLine(id + " is in!");
            Thread.Sleep(1000);
            Console.WriteLine(id + " is out!");
            _semaphore.Release();
        }

        static void LockTrial()
        {
            lock (_locker)
            {
                Console.WriteLine("Enter locking block");

                if (_val1 != 0)
                {
                    _val1 = 10 / _val1;
                }

                _val1 = 0;

                Thread.Sleep(2000);
                Console.WriteLine("Exit locking block");
            }
        }

        static void OnThread()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write("OT ");
                Thread.Sleep(500);
            }
        }

        static void RunX(Motor m)
        {
            while (EndPoint.X != StartPoint.X)
            {
                StartPoint.X = m.Run(StartPoint.X);

                Console.Write("("+ StartPoint.X + "," + StartPoint.Y +")");
            }
        }

        static void RunY(Motor m)
        {
            while (EndPoint.Y != StartPoint.Y)
            {
                StartPoint.Y = m.Run(StartPoint.Y);

                Console.Write("(" + StartPoint.X + "," + StartPoint.Y + ")");
            }
        }

        static void Go(object data)
        {
            Console.WriteLine("From thread pool" + data);

            Console.ReadKey();
        }

        delegate int WorkInvoker(int howMuch);
        static int Work(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Thread.Sleep(500);
            }

            return 0;
        }
    }
}
