using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlessInput;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //int[] aa = MouseAction.getAScreenBound();
            Thread.Sleep(2000);
            MouseAction.init("Bless Unleashed");
            MouseAction.moveTo(100, 100);
            Console.ReadKey();
        }
    }
}
