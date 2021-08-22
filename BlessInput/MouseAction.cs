using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BlessInput
{
    public class Action
    {
        private INPUT structure;
        private INPUT[] pInput;
        private int windowXMin;
        private int windowYMin;
        private int windowXMax;
        private int windowYMax;

        private int screenWidth;
        private int screenHeight;
        public Action()
        {
            structure = new INPUT
            {
                mi = {
                dx = 0,
                dy = 0,
                mouseData = 0,
                dwFlags = 0,
                }
            };
            pInput = new INPUT[1];
        }


        private static Action instance;
        public static Action Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Action();

                }
                return instance;
            }
        }
        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(8)]
            public Action.HARDWAREINPUT hi;
            [FieldOffset(8)]
            public Action.KEYBDINPUT ki;
            [FieldOffset(8)]
            public Action.MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [Flags]
        private enum MOUSEEVENTF
        {
            ABSOLUTE = 0x8000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            MOVE = 0x0001,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //----------------------------------------------------------------
        /// <summary>
        /// 得到屏幕分辨率
        /// </summary>
        /// <returns>1维数组[0]X,[1]Y</returns>
        public static int[] getAScreenBound()
        {
            int[] result = new int[2];
            result[1] = Screen.PrimaryScreen.Bounds.Height;
            result[0] = Screen.PrimaryScreen.Bounds.Width;
            return result;
        }

        /// <summary>
        /// 获取图像的原点
        /// </summary>
        /// <param name="hWnd">句柄</param>
        /// <returns>数组，[0]x,[1]y</returns>
        public static int[] getWindowBasePoint(IntPtr hWnd)
        {
            RECT rc = new RECT();
            GetWindowRect(hWnd, ref rc);
            int[] result = new int[4];
            result[0] = rc.Left;
            result[1] = rc.Top;
            result[2] = rc.Right;
            result[3] = rc.Bottom;
            return result;
        }
        /// <summary>
        /// 延时
        /// </summary>
        /// <param name="ms"></param>
        public static void Delay(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }
        public static void init(String gameName)
        {
            IntPtr hWnd = FindWindow(null, gameName);
            int[] point = getWindowBasePoint(hWnd);
            Instance.windowXMin = point[0];
            Instance.windowYMin = point[1];
            Instance.windowXMax = point[2];
            Instance.windowYMax = point[3];
            
            int[] pointB = getAScreenBound();
            Instance.screenWidth = pointB[0];
            Instance.screenHeight = pointB[1];
        }
        /// <summary>
        /// 按紧左键
        /// </summary>
        /// <param name="time">持续时间，为0时不放</param>
        /// <returns>1表示成功，-1表示失败</returns>
        public static int LClickDown(int time)
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            if(time == 0) { } else
            {
                Thread.Sleep(time);
                Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
                Instance.pInput[0] = Instance.structure;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
                if (result == 0) return -1;
            }
            return 1;
        }
        /// <summary>
        /// 按紧右键
        /// </summary>
        /// <param name="time">按紧时间</param>
        /// <returns>1成功，-1失败</returns>
        public static int RClickDown(int time)
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.RIGHTDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
                Instance.pInput[0] = Instance.structure;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
                if (result == 0) return -1;
            }
            return 1;
        }
        /// <summary>
        /// 按紧中键
        /// </summary>
        /// <param name="time">按紧时间</param>
        /// <returns>1成功，-1失败</returns>
        public static int MClickDown(int time)
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
                Instance.pInput[0] = Instance.structure;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
                if (result == 0) return -1;
            }
            return 1;
        }/// <summary>
        /// 左键弹起
        /// </summary>
        /// <returns></returns>
        public static int LClickUp()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 右键弹起
        /// </summary>
        /// <returns></returns>
        public static int RClickUp()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 中键弹起
        /// </summary>
        /// <returns></returns>
        public static int MClickUp()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 移动鼠标
        /// </summary>
        /// <param name="x">图片x坐标</param>
        /// <param name="y">图片y坐标</param>
        /// <returns>成功返回1，失败返回-1</returns>
        public static int moveTo(int x, int y)
        {
            if( x >= (Instance.windowXMax - Instance.windowXMin)) x = Instance.windowXMax;
            if (y >= (Instance.windowYMax - Instance.windowYMin)) y = Instance.windowYMax;
            int InputX = (Instance.windowXMin + x) * (65335 / Instance.screenWidth);
            int InputY = (Instance.windowYMin + y) * (65335 / Instance.screenHeight);
            Instance.structure.mi.dx = InputX;
            Instance.structure.mi.dy = InputY;
            Instance.structure.mi.dwFlags = (int)(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE);
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击中键
        /// </summary>
        /// <returns>成功返回1</returns>
        public static int MOnClick()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
            Instance.pInput[0] = Instance.structure;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击左键
        /// </summary>
        /// <returns>成功返回1</returns>
        public static int LOnClick()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
            Instance.pInput[0] = Instance.structure;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击右键
        /// </summary>
        /// <returns>失败返回-1</returns>
        public static int ROnClick()
        {
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.RIGHTDOWN;
            Instance.pInput[0] = Instance.structure;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.structure.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
            Instance.pInput[0] = Instance.structure;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.structure));
            if (result == 0) return -1;
            return 1;
        }
        //--------------------------------------------------------------------------------
        //键盘
        //-----------------------------------------------------------------------------------
        public static int keyDown(int time)
        {

        }
    }
}
