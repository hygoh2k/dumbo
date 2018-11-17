using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestAsync
{
    class Program
    {
        static ConcurrentBag<string> _list = new ConcurrentBag<string>();

        static void Main(string[] args)
        {
            while (true)
            {
                Example("ThreadA", 200);
                Example("ThreadB", 200);
                Console.WriteLine("In Progress");
                Console.ReadLine();
                Console.WriteLine("List:" + _list.Count);
                _list.Clear();
                
            }
        }


        static async void Example(string prefix, int delaySec)
        {
            while (true)
            {
                // This method runs asynchronously.
                int t = await Task.Run(() => NotSyncFync(prefix, delaySec));
            }
            
        }

        static int NotSyncFync(string prefix, int delaySec)
        {
            Thread.Sleep(delaySec);
            Console.WriteLine(prefix + ":Time:" + DateTime.Now.ToLongTimeString());
            _list.Add(DateTime.Now.ToLongTimeString());
            return 1;
        }



        static async Task<int> HandleFileAsync()
        {

            string a = string.Empty;
            for (int i = 0; i < 1000000; i++)
            {
                a = a + " ";
            }

            return 1;
        }
    }



}
