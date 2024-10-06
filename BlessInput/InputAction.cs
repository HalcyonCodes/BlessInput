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
    public class InputAction
    {
        private INPUT mouseStruct;
        private INPUT keyBdStruct;
        private INPUT[] pInput;
        private int windowXMin;
        private int windowYMin;
        private int windowXMax;
        private int windowYMax;

        private int screenWidth;
        private int screenHeight;
        public InputAction()
        {
            mouseStruct = new INPUT
            {
                mi = {
                dx = 0,
                dy = 0,
                mouseData = 0,
                dwFlags = 0,
                }
            };
            keyBdStruct = new INPUT
            {
                type = 1,
                ki = {
                wVk = 0,
                wScan = 0,
                dwFlags = 0,
                time = 0,
                dwExtraInfo = IntPtr.Zero,
                }
            };
            pInput = new INPUT[1];
        }


        private static InputAction instance;
        public static InputAction Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputAction();

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
            public InputAction.HARDWAREINPUT hi;
            [FieldOffset(8)]
            public InputAction.KEYBDINPUT ki;
            [FieldOffset(8)]
            public InputAction.MOUSEINPUT mi;
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
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

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
        [Flags]
        private enum KEYEVENTF
        {
            EXTENDEDKEY = 1,
            KEYUP = 2,
            SCANCODE = 8,
            UNICODE = 4
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
            if (gameName == "")
            {
                int[] pointB = getAScreenBound();
                Instance.screenWidth = pointB[0];
                Instance.screenHeight = pointB[1];

                Instance.windowXMin = 0;
                Instance.windowYMin = 0;
                Instance.windowXMax = pointB[0];
                Instance.windowYMax = pointB[1];

            }
            else
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
        }
        /// <summary>
        /// 按紧左键
        /// </summary>
        /// <param name="time">持续时间，为0时不放</param>
        /// <returns>1表示成功，-1表示失败</returns>
        public static int LClickDown(int time)
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
                Instance.pInput[0] = Instance.mouseStruct;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
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
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.RIGHTDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
                Instance.pInput[0] = Instance.mouseStruct;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
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
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
                Instance.pInput[0] = Instance.mouseStruct;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
                if (result == 0) return -1;
            }
            return 1;
        }/// <summary>
         /// 左键弹起
         /// </summary>
         /// <returns></returns>
        public static int LClickUp()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 右键弹起
        /// </summary>
        /// <returns></returns>
        public static int RClickUp()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 中键弹起
        /// </summary>
        /// <returns></returns>
        public static int MClickUp()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
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
            if (x >= (Instance.windowXMax - Instance.windowXMin)) x = Instance.windowXMax;
            if (y >= (Instance.windowYMax - Instance.windowYMin)) y = Instance.windowYMax;
            int InputX = (Instance.windowXMin + x) * (65335 / Instance.screenWidth);
            int InputY = (Instance.windowYMin + y) * (65335 / Instance.screenHeight);
            Instance.mouseStruct.mi.dx = InputX;
            Instance.mouseStruct.mi.dy = InputY;
            Instance.mouseStruct.mi.dwFlags = (int)(MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE);
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 在原本鼠标坐标上叠加移动鼠标
        /// </summary>
        /// <param name="x">叠加x像素</param>
        /// <param name="y">叠加y像素</param>
        /// <returns></returns>
        public static int moveToChange(int x, int y)
        {
            Instance.mouseStruct.mi.dx = x;
            Instance.mouseStruct.mi.dy = y;
            Instance.mouseStruct.mi.dwFlags = (int)(MOUSEEVENTF.MOVE);
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击中键
        /// </summary>
        /// <returns>成功返回1</returns>
        public static int MOnClick()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.MIDDLEUP;
            Instance.pInput[0] = Instance.mouseStruct;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击左键
        /// </summary>
        /// <returns>成功返回1</returns>
        public static int LOnClick()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.LEFTDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.LEFTUP;
            Instance.pInput[0] = Instance.mouseStruct;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 点击右键
        /// </summary>
        /// <returns>失败返回-1</returns>
        public static int ROnClick()
        {
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.RIGHTDOWN;
            Instance.pInput[0] = Instance.mouseStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.mouseStruct.mi.dwFlags = (int)MOUSEEVENTF.RIGHTUP;
            Instance.pInput[0] = Instance.mouseStruct;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.mouseStruct));
            if (result == 0) return -1;
            return 1;
        }
        //--------------------------------------------------------------------------------
        //键盘
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// 按下键盘
        /// </summary>
        /// <param name="key">按键码</param>
        /// <param name="time">持续时间，0时一直不放</param>
        /// <returns></returns>
        public static int keyDown(short key, int time)
        {
            Instance.keyBdStruct.ki.wScan = (short)MapVirtualKey((uint)key, 0);
            Instance.keyBdStruct.ki.dwFlags = (int)KEYEVENTF.SCANCODE;
            Instance.pInput[0] = Instance.keyBdStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.keyBdStruct));
            if (result == 0) return -1;
            if (time == 0) { }
            else
            {
                Thread.Sleep(time);
                Instance.keyBdStruct.ki.dwFlags = (int)KEYEVENTF.KEYUP | (int)KEYEVENTF.SCANCODE;
                Instance.pInput[0] = Instance.keyBdStruct;
                result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.keyBdStruct));
                if (result == 0) return -1;
            }
            return 1;
        }
        /// <summary>
        /// 按键弹起
        /// </summary>
        /// <param name="key">按键码</param>
        /// <returns></returns>
        public static int keyUp(short key)
        {
            Instance.keyBdStruct.ki.wScan = (short)MapVirtualKey((uint)key, 0);
            Instance.keyBdStruct.ki.dwFlags = (int)KEYEVENTF.KEYUP | (int)KEYEVENTF.SCANCODE;
            Instance.pInput[0] = Instance.keyBdStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.keyBdStruct));
            if (result == 0) return -1;
            return 1;
        }
        /// <summary>
        /// 按键一次
        /// </summary>
        /// <param name="key">按键码</param>
        /// <returns>成功1，-1失败</returns>
        public static int keyPress(short key)
        {
            Instance.keyBdStruct.ki.wScan = (short)MapVirtualKey((uint)key, 0);
            Instance.keyBdStruct.ki.dwFlags = (int)KEYEVENTF.SCANCODE;
            Instance.pInput[0] = Instance.keyBdStruct;
            uint result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.keyBdStruct));
            if (result == 0) return -1;
            Thread.Sleep(100);
            Instance.keyBdStruct.ki.dwFlags = (int)KEYEVENTF.KEYUP | (int)KEYEVENTF.SCANCODE;
            Instance.pInput[0] = Instance.keyBdStruct;
            result = SendInput(1, Instance.pInput, Marshal.SizeOf(Instance.keyBdStruct));
            if (result == 0) return -1;
            return 1;
        }
        //--------------------------------------------------------------------------------
        //输入
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// 发送字符串（包括英文和中文字符）
        /// </summary>
        /// <param name="str">要发送的字符串</param>
        /// <returns>成功返回1，失败返回-1</returns>
        public static int SendString(string str)
        {
            // 清空之前的输入
            InputAction.keyPress((short)Keys.LMenu);
            InputAction.keyUp((short)Keys.LMenu);

            foreach (char ch in str)
            {
                INPUT[] inputDown = new INPUT[1];
                INPUT[] inputUp = new INPUT[1];

                // 设置按键按下的参数
                inputDown[0] = new INPUT
                {
                    type = 1,
                    ki = new KEYBDINPUT
                    {
                        wScan = (short)ch,
                        dwFlags = (int)KEYEVENTF.UNICODE,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero,
                    }
                };

                // 设置按键弹起的参数
                inputUp[0] = new INPUT
                {
                    type = 1,
                    ki = new KEYBDINPUT
                    {
                        wScan = (short)ch,
                        dwFlags = (int)(KEYEVENTF.UNICODE | KEYEVENTF.KEYUP),
                        time = 0,
                        dwExtraInfo = IntPtr.Zero,
                    }
                };

                // 发送按键按下事件
                uint result = SendInput(1, inputDown, Marshal.SizeOf(typeof(INPUT)));
                if (result == 0) return -1;

                // 发送按键弹起事件
                result = SendInput(1, inputUp, Marshal.SizeOf(typeof(INPUT)));
                if (result == 0) return -1;
            }

            return 1;
        }
    }
}
