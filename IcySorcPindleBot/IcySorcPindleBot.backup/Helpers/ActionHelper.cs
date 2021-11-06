using System;
using System.Collections.Generic;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace IcySorcPindleBot.Helpers
{
    public class ActionHelper
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
        public const ushort WM_KEYDOWN = 0x0100;
        public const ushort WM_KEYUP = 0x0101;

        private Random rnd = new Random();

        public void ManageStart(IntPtr D2handle, byte frozenHotkey)
        {
            KeyboardPress(D2handle, frozenHotkey);
            RightMouseClick();
            Thread.Sleep(100);

            KeyboardPress(D2handle, frozenHotkey);
            RightMouseClick();
            Thread.Sleep(100);

            KeyboardPress(D2handle, frozenHotkey);
            RightMouseClick();
            Thread.Sleep(100);
            //Todo pick up body if death
            //Go to stash to drop off items if full
        }

        public void MoveToPortal(IntPtr D2handle)
        {
            // WP1
            RealisticMouseMove(D2handle, 606, 906, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);


            // wp2
            RealisticMouseMove(D2handle, 790, 605, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);


            //WP3
            RealisticMouseMove(D2handle, 850, 712, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP4
            RealisticMouseMove(D2handle, 1039, 673, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP5
            RealisticMouseMove(D2handle, 438, 790, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP6
            RealisticMouseMove(D2handle, 765, 731, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP7
            RealisticMouseMove(D2handle, 904, 927, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP8
            RealisticMouseMove(D2handle, 486, 809, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);

            // WP9
            RealisticMouseMove(D2handle, 556, 163, 5, 5, 1);
            LeftMouseClick();
            Thread.Sleep(1200);
        }

        public void TeleToPindle(IntPtr D2handle, byte teleHotkey)
        {
            KeyboardPress(D2handle, teleHotkey);

            // WP1
            RealisticMouseMove(D2handle, 1354, 61, 10, 5, 1);
            RightMouseClick();
            Thread.Sleep(200);


            // wp2
            RealisticMouseMove(D2handle, 1556, 148, 10, 5, 1);
            RightMouseClick();
            Thread.Sleep(200);

            //while (true)
            //{
            //    var location = GetCursorPosition();
            //    Console.WriteLine("Cursor Pos: " + location.ToString());
            //    Thread.Sleep(500);
            //}

            //WP3
            RealisticMouseMove(D2handle, 1248, 362, 10, 5, 1);
            RightMouseClick();
            Thread.Sleep(200);

            RealisticMouseMove(D2handle, 1367, 249, 10, 5, 1);
            RightMouseClick();
            Thread.Sleep(200);

        }

        public void AttackSequence(IntPtr D2handle, byte blizzHotkey)
        {
            KeyboardPress(D2handle, blizzHotkey);
            RealisticMouseMove(D2handle, 1457, 276, 10, 5, 1);
            RightMouseClick();

            RealisticMouseMove(D2handle, 1373, 289, 10, 5, 1);
            Thread.Sleep(2200);

            //while (true)
            //{
            //    var location = GetCursorPosition();
            //    Console.WriteLine("Cursor Pos: " + location.ToString());
            //    Thread.Sleep(500);
            //}

            RightMouseClick();
            //RealisticMouseMove(D2handle, 137, 370, 10, 5, 1);
            Thread.Sleep(2200);

            RightMouseClick();
            //RealisticMouseMove(D2handle, 137, 370, 10, 5, 1);
            Thread.Sleep(2200);

            RightMouseClick();
        }

        public void MoveToLoot(IntPtr D2handle)
        {
            RealisticMouseMove(D2handle, 1400, 298, 10, 5, 1);
            LeftMouseClick();
            KeyboardPress(D2handle, 0x12);
        }

        public void MoveMouseToPosition(IntPtr hwnd, int x, int y)
        {
            Point point = new Point(0, 0);
            Point windowPoint = ConvertToScreenPixel(point, hwnd);
            Console.WriteLine("Ret: " + ConvertToScreenPixel(point, hwnd));
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
                Thread.Sleep(sleeptime);
            }
        }

        public List<Point> GetIntervalPoints(int x_source, int y_source, int x_dest, int y_dest, int numBatches, int delta)
        {
            double x_curr = x_source;
            double y_curr = y_source;

            double x_step = (double)(x_dest - x_source) / numBatches;
            double y_step = (double)(y_dest - y_source) / numBatches;

            var doubleIntervals = new List<Tuple<double, double>>();

            while (numBatches > 0)
            {
                doubleIntervals.Add( new Tuple<double, double>(x_curr, y_curr));
                x_curr += x_step;
                y_curr += y_step;
                numBatches--;
            }

            var intervals = new List<Point>();

            foreach( var tuple in doubleIntervals)
            {
                intervals.Add(new Point((int)Math.Round(tuple.Item1), (int)Math.Round(tuple.Item2)));
            }

            return intervals;
        }

        public POINT GetCursorPosition()
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

        public void LeftMouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void RightMouseClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        public void MouseClickDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public void MouseClickUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void KeyboardPress(IntPtr D2handle, byte bVk)
        {
            SetActiveWindow(D2handle);
            SendMessage(D2handle, WM_KEYDOWN, bVk, 0);
            SendMessage(D2handle, WM_KEYUP, bVk, 0);
        }

        public void KeyboardPressAlt(byte bVk)
        {
            keybd_event(bVk, 0, 0, 0);
        }


        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);

        static private Point ConvertToScreenPixel(Point point, IntPtr hwnd)
        {
            Rectangle rect;

            GetWindowRect(hwnd, out rect);

            Point ret = new Point();

            ret.X = rect.Location.X + point.X;
            ret.Y = rect.Location.Y + point.Y;

            return ret;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")] //Set the active window
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")] //sends a windows message to the specified window
        public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, int lParam);

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
    }
}
