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
            //InputAction.init("Bless Unleashed");
            //InputAction.MoveTo(100,100);
            //int d = InputAction.keyPress(65);
            InputAction.LClickDown(0);
            Thread.Sleep(200);
            int d = InputAction.LClickUp();
            Console.WriteLine(d);
            Console.ReadKey();
        }
    }
}
