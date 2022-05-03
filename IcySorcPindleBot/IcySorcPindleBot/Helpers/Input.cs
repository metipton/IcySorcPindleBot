using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace IcySorcPindleBot.Helpers
{
    public class Input
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
        public const ushort WM_KEYDOWN = 0x0100;
        public const ushort WM_KEYUP = 0x0101;

        private Random rnd = new Random();

        public static void MoveMouseToPosition(IntPtr hwnd, int x, int y)
        {
            Point point = new Point(0, 0);
            Point windowPoint = ConvertToScreenPixel(point, hwnd);
            SetCursorPos(windowPoint.X + x, windowPoint.Y + y);
        }

        public void RealisticMouseMove(IntPtr hwnd, int x, int y, int distanceScalar, int randomDelta, int sleeptime)
        {
            POINT current;
            GetCursorPos(out current);
            var numBatches = (int)Math.Round(Math.Sqrt((Math.Pow(current.X - x, 2) + Math.Pow(current.Y - y, 2))) / distanceScalar);
            var intervals = GetIntervalPoints(current.X, current.Y, x, y, numBatches, randomDelta);

            foreach (var interval in intervals)
            {
                var randomized = GetRandomizedPoint(interval.X, interval.Y, randomDelta);
                MoveMouseToPosition(hwnd, randomized.X, randomized.Y);
                Thread.Sleep(10);
            }
        }

        public static List<Point> GetIntervalPoints(int x_source, int y_source, int x_dest, int y_dest, int numBatches, int delta)
        {
            double x_curr = x_source;
            double y_curr = y_source;

            double x_step = (double)(x_dest - x_source) / numBatches;
            double y_step = (double)(y_dest - y_source) / numBatches;

            var doubleIntervals = new List<Tuple<double, double>>();

            while (numBatches > 0)
            {
                doubleIntervals.Add(new Tuple<double, double>(x_curr, y_curr));
                x_curr += x_step;
                y_curr += y_step;
                numBatches--;
            }

            var intervals = new List<Point>();

            foreach (var tuple in doubleIntervals)
            {
                intervals.Add(new Point((int)Math.Round(tuple.Item1), (int)Math.Round(tuple.Item2)));
            }

            return intervals;
        }

        public static POINT GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        public Point GetRandomizedPoint(int x, int y, int delta)
        {
            return new Point(rnd.Next(x - delta, x + delta), rnd.Next(y - delta, y + delta));
        }

        public static void LeftMouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void LeftMouseDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public static void LeftMouseUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void RightMouseClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public static void MouseClickDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public static void MouseClickUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void KeyboardPress(IntPtr D2handle, uint scan)
        {
            SetActiveWindow(D2handle);

            SendKey(scan, false, InputType.Keyboard);
            SendKey(scan, true, InputType.Keyboard);
        }

        /// <summary>
        /// Sends a directx key.
        /// http://www.gamespp.com/directx/directInputKeyboardScanCodes.html
        /// </summary>
        /// <param name="key"></param>
        /// <param name="KeyUp"></param>
        /// <param name="inputType"></param>
        public static void SendKey(uint key, bool KeyUp, InputType inputType)
        {
            uint flagtosend;
            if (KeyUp)
            {
                flagtosend = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode);
            }
            else
            {
                flagtosend = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode);
            }

            InputStruct[] inputs =
            {
            new InputStruct
            {
                type = (int) inputType,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = (ushort) key,
                        dwFlags = flagtosend,
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
        };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(InputStruct)));
        }

        static private Point ConvertToScreenPixel(Point point, IntPtr hwnd)
        {
            Rectangle rect;

            GetWindowRect(hwnd, out rect);

            Point ret = new Point();

            ret.X = rect.Location.X + point.X;
            ret.Y = rect.Location.Y + point.Y;

            return ret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }

            public override string ToString()
            {
                return "(" + X.ToString() + ", " + Y.ToString() + ")";
            }
        }

        [Flags]
        public enum KeyEventF
        {
            KeyDown = 0x0000,
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            Scancode = 0x0008,
        }

        [Flags]
        public enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        public struct InputStruct
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public readonly MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public readonly HardwareInput hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public readonly int dx;
            public readonly int dy;
            public readonly uint mouseData;
            public readonly uint dwFlags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public readonly uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, InputStruct[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")] //sends a windows message to the specified window
        public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, int lParam);
    }
}
