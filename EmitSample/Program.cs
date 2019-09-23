using System;
using System.Threading.Tasks;

namespace EmitSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }
    }

    internal class Service
    {
        public object Invoke(object[] param)
        {
            return "aaaaaaaaa";
        }
    }
}
