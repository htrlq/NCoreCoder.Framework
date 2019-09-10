using System;
using System.Threading.Tasks;

namespace EmitSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var instance = new Service();
            var serviceType = typeof(Service);

            var targetMethod = serviceType.GetMethod("Invoke");

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
