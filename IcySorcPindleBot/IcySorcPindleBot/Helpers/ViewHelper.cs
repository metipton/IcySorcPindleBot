
namespace IcySorcPindleBot.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public class ViewHelper
    {
        ColorHelper colors = new ColorHelper();

        public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, x, y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        public static Bitmap GetMapFromScreen(IntPtr D2handle)
        {
            Rectangle screenRect;
            GetWindowRect(D2handle, out screenRect);
            var width = Math.Abs(screenRect.Right - screenRect.Left);
            var height = Math.Abs(screenRect.Bottom - screenRect.Top);

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                bitmap.SetResolution(500, 500);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.InterpolationMode = InterpolationMode.Bicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CopyFromScreen(new Point(screenRect.Left, screenRect.Top), Point.Empty, screenRect.Size);
                }

                var croppedBmp = bitmap.Clone(screenRect, PixelFormat.Format32bppRgb);
                croppedBmp.Save(@"C:\Users\michael\source\repos\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\imgcapture.png", System.Drawing.Imaging.ImageFormat.Png);

                return croppedBmp;
            }
        }

        public bool IsInDifficultySelection(IntPtr D2handle)
        {
            var color1 = GetPixelColor(D2handle, 1062, 453);
            var color2 = GetPixelColor(D2handle, 855, 452);

            var color1checks = Math.Abs(color1.R - 71) <= 10 && Math.Abs(color1.G - 173) <= 10 && Math.Abs(color1.B - 253) <= 10;
            var color2checks = Math.Abs(color2.R - 95) <= 10 && Math.Abs(color2.G - 230) <= 10 && Math.Abs(color2.B - 254) <= 10;

            return color1checks && color2checks;
        }

        public bool IsInHarrogath(IntPtr D2handle)
        {
            var color1 = GetPixelColor(D2handle, 1593, 761);
            var color2 = GetPixelColor(D2handle, 611, 883);

            var color1checks = Math.Abs(color1.R - 62) <= 3 && Math.Abs(color1.G - 57) <= 3 && Math.Abs(color1.B - 47) <= 3;
            var color2checks = Math.Abs(color2.R - 82) <= 10 && Math.Abs(color2.G - 68) <= 10 && Math.Abs(color2.B - 55) <= 10;

            return color1checks && color2checks;
        }

        public bool IsInLoadScreen(IntPtr D2handle)
        {
            var color1 = GetPixelColor(D2handle, 919, 868);
            var color2 = GetPixelColor(D2handle, 9, 102);

            var color1checks = color1.R <= 10 && color1.G <= 10 && color1.B  <= 10;
            var color2checks = color2.R <= 10 && color2.G <= 10 && color2.B  <= 10;

            return color1checks && color2checks;
        }

        public bool IsTempleLoaded(IntPtr D2handle)
        {
            var color1 = GetPixelColor(D2handle, 1664, 136);
            var color2 = GetPixelColor(D2handle, 1885, 27);

            var color1checks = Math.Abs(color1.R - 95) <= 10 && Math.Abs(color1.G - 75) <= 8 && Math.Abs(color1.B - 50) <= 5;
            var color2checks = Math.Abs(color2.R - 48) <= 5 && Math.Abs(color2.G - 39) <= 5 && Math.Abs(color2.B - 28) <= 5;

            return color1checks && color2checks;
        }

        public bool IsInPandFortress(IntPtr D2handle)
        {
            var color1 = GetPixelColor(D2handle, 651, 831);
            var color2 = GetPixelColor(D2handle, 1524, 605);

            var color1checks = Math.Abs(color1.R - 97) <= 15 && Math.Abs(color1.G - 98) <= 15 && Math.Abs(color1.B - 95) <= 15;
            var color2checks = Math.Abs(color2.R - 14) <= 15 && Math.Abs(color2.G - 13) <= 15 && Math.Abs(color2.B - 10) <= 15;

                return color1checks && color2checks;
        }

        public bool IsEmptyStashSquare(IntPtr D2handle, int row, int col)
        {
            var color1 = GetPixelColor(D2handle, row, col);

            var color1checks = color1.R <= 14 && color1.G <= 14 && color1.B <= 14;

            return color1checks;
        }

        public bool IsEmptyPotionSquare(IntPtr D2handle, int row, int col)
        {
            var color = GetPixelColor(D2handle, row, col);

            var color1checks = Math.Abs(color.R - 39) <= 15 && Math.Abs(color.R - 39) <= 15 && Math.Abs(color.R - 39) <= 15;

            return color1checks;
        }

        public bool IsHealthLow(IntPtr D2handle)
        {
            var color = GetPixelColor(D2handle, 462, 920);

            return color.R < 60;
        }

        public Point GetPortalLocation(IntPtr D2handle)
        {
            Bitmap bmp = GetMapFromScreen(D2handle);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int strideInPixels = data.Stride / 4; //4 bytes per pixel

            var maxSize = 0;
            var left = 0;
            var right = 0;
            var top = 0;
            var bottom = 0;

            for (var j = 0; j < bmp.Width; j += 2)
            {
                for (var i = 0; i < bmp.Height; i += 2)
                {
                    unsafe
                    {
                        uint* dataPointer = (uint*)data.Scan0;
                        uint* pixelPointer = dataPointer + i * strideInPixels + j;
                        uint color = *pixelPointer;
                        if (ColorHelper.IsPortalBorderColor(color))
                        {
                            var possiblePortal = PortalDFS(data, i, j);
                            if (possiblePortal[0] > maxSize)
                            {
                                left = possiblePortal[1];
                                right = possiblePortal[2];
                                top = possiblePortal[3];
                                bottom = possiblePortal[4];
                                maxSize = possiblePortal[0];
                            }
                        }
                    }
                }
            }

            bmp.UnlockBits(data);
            bmp.Save(@"C:\Users\michael\source\repos\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\PortalSearch.png", System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
            return new Point((left + right) / 2, (top + bottom) / 2);
        }

        private int[] PortalDFS(BitmapData data, int row, int col)
        {
            int strideInPixels = data.Stride / 4; //4 bytes per pixel
            var width = data.Width;
            var height = data.Height;

            unsafe
            {
                Stack<Point> stack = new();
                stack.Push(new Point(row, col));
                Point current;
                uint* dataPointer = (uint*)data.Scan0;
                var left = width;
                var right = 0;
                var top = height;
                var bottom = 0;

                do
                {
                    current = stack.Pop();

                    uint* pixelPointer = dataPointer + current.X * strideInPixels + current.Y;
                    uint color = *pixelPointer;

                    if (ColorHelper.IsPortalBorderColor(color))
                    {
                        row = current.X;
                        col = current.Y;

                        SetToVisited(data, row, col);

                        if(row > bottom)
                        {
                            bottom = row;
                        }
                        if(row < top)
                        {
                            top = row;
                        }
                        if(col < left)
                        {
                            left = col;
                        }
                        if(col > right)
                        {
                            right = col;
                        }
                        if (col - 1 > 0)
                        {
                            stack.Push(new Point(row, col - 1));
                        }
                        if (col + 1 < width)
                        {
                            stack.Push(new Point(row, col + 1));
                        }
                        if (row - 1 > 0)
                        {
                            stack.Push(new Point(row - 1, col));
                        }
                        if (row + 1 < height)
                        {
                            stack.Push(new Point(row + 1, col));
                        }
                    }
                } while (stack.Count > 0);

                return new int[5] { (right - left) * (bottom - top), left, right, top, bottom };
            }
        }
        private static bool HasVisited(uint color)
        {
            return color == 0x00000000;
        }

        private static void SetToVisited(BitmapData data, int row, int col)
        {
            int strideInPixels = data.Stride / 4; //4 bytes per pixel
            unsafe
            {
                uint* dataPointer = (uint*)data.Scan0;
                uint* pixelPointer = dataPointer + row * strideInPixels + col;
                *pixelPointer = 0x00000000;
                return;
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
    }
}
