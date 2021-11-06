
namespace IcySorcPindleBot.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using Tesseract;

    public class ViewHelper
    {
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

        public Bitmap GetScreenBitMap(IntPtr D2handle)
        {
            Rectangle screenRect;
            GetWindowRect(D2handle, out screenRect);
            var width = Math.Abs(screenRect.Right - screenRect.Left);
            var height = Math.Abs(screenRect.Bottom - screenRect.Top);
            //Console.WriteLine("Left: " + screenRect.Left + ", Right: " + screenRect.Right + ", Top: " + screenRect.Top + ", Bottom: " + screenRect.Bottom);
            //Console.WriteLine("size: " + screenRect.Size);

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                bitmap.SetResolution(100, 100);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(screenRect.Left, screenRect.Top), Point.Empty, screenRect.Size);
                }

                return bitmap;
            }
        }

        public void ReadWordsFromScreen(IntPtr D2handle)
        {
            Bitmap bitmap = GetScreenBitMap(D2handle);
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                Console.WriteLine("\nOCR - The OCR Engine has started!\n");
                using (var img = PixConverter.ToPix(bitmap))
                {
                    Console.WriteLine("\nOCR the image i fetched is: - " + img.ToString() + "\n");
                    using (var screen = engine.Process(img))
                    {
                        string text = screen.GetText();
                        Console.WriteLine("Text");

                    }
                }
            }

            Console.WriteLine("Wait");
        }

        public Point? SearchForColor(Bitmap image, uint color)
        {
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            BitmapData data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            //works for 32-bit pixel format only
            int ymin = rect.Top, ymax = Math.Min(rect.Bottom, image.Height);
            int xmin = rect.Left, xmax = Math.Max(rect.Right, image.Width) - 1;

            int strideInPixels = data.Stride / 4; //4 bytes per pixel
            unsafe
            {
                uint* dataPointer = (uint*)data.Scan0;
                for (int y = ymin; y < ymax; y++)
                    for (int x = xmin; x < xmax; x++)
                    {
                        //works independently of the data.Stride sign
                        uint* pixelPointer = dataPointer + y * strideInPixels + x;
                        uint pixel = *pixelPointer;
                        bool found = pixel == color;
                        if (found)
                        {

                            image.UnlockBits(data);
                            return new Point(x, y);

                        }
                    }
            }
            image.UnlockBits(data);
            return null;
        }

        public bool isGameLoaded(IntPtr D2handle, ClientHelper clientHelper)
        {
            while (D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = clientHelper.GetD2WinHandle();
            }

            var color1 = GetPixelColor(D2handle, 1593, 761);
            var color2 = GetPixelColor(D2handle, 611, 883);

            var color1checks = Math.Abs(color1.R - 62) <= 3 && Math.Abs(color1.G - 57) <= 3 && Math.Abs(color1.B - 47) <= 3;
            var color2checks = Math.Abs(color2.R - 82) <= 10 && Math.Abs(color2.G - 68) <= 10 && Math.Abs(color2.B - 55) <= 10;

            return color1checks && color2checks;
        }

        public bool isTempleLoaded(IntPtr D2handle, ClientHelper clientHelper)
        {
            while (D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = clientHelper.GetD2WinHandle();
            }

            var color1 = GetPixelColor(D2handle, 1664, 136);
            var color2 = GetPixelColor(D2handle, 1885, 27);

            var color1checks = Math.Abs(color1.R - 95) <= 10 && Math.Abs(color1.G - 75) <= 8 && Math.Abs(color1.B - 50) <= 5;
            var color2checks = Math.Abs(color2.R - 48) <= 5 && Math.Abs(color2.G - 39) <= 5 && Math.Abs(color2.B - 28) <= 5;

            return color1checks && color2checks;
        }
    }
}
