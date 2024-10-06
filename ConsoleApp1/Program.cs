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
            Thread.Sleep(3000);
            //InputAction.init("广发操盘手(易淘金PC版)");
            InputAction.moveTo(500,500);
            //int d = InputAction.keyPress(65);
            //InputAction.LClickDown(0);
            //Thread.Sleep(200);
            //int d = InputAction.LClickUp();
            //InputAction.SendString("3000022");
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
