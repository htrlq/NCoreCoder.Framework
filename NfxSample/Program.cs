using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NCoreCoder.Aop;

namespace NfxSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ServiceCollection();

            service.AddJitAop<IMyClass, MyClass>(ServiceLifetime.Singleton);

            var serviceProvider = service.BuildServiceProvider();
            var myclass = serviceProvider.GetRequiredService<IMyClass>();

            //myclass.Invoke();
            myclass.Result();
        }
    }

    public interface IMyClass
    {
        void Invoke();
        int Result();
    }

    internal class MyClass : IMyClass
    {
        public void Invoke()
        {
            Console.WriteLine("Invoke");
        }

        public int Result()
        {
            Console.WriteLine("Result");

            return 100;
        }
    }

    internal class MyClassA : IMyClass
    {
        private IMyClass _myClass;

        public MyClassA(IMyClass myClass)
        {
            _myClass = myClass;
        }

        public void Invoke()
        {
            _myClass.Invoke();
        }

        public int Result()
        {
            return _myClass.Result();
        }
    }
}
