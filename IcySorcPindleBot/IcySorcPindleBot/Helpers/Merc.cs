
using System;
using System.Threading;

namespace IcySorcPindleBot.Helpers
{
    public class Merc
    {
        readonly ViewHelper views;
        readonly Input input;
        readonly Move move;
        private static Merc instance;

        private Merc()
        {
            this.views = new();
            this.input = new();
            this.move = new();
        }

        public static Merc GetInstance()
        {
            if(Merc.instance == null)
            {
                Merc.instance = new();
            }

            return Merc.instance;
        }

        public void GiveMercHealthPot(IntPtr D2handle)
        {

        }

        public bool IsMercHealthy(IntPtr D2handle)
        {
            var mercCheckColor = views.GetPixelColor(D2handle, 70, 25);

            return Math.Abs(mercCheckColor.G - 134) < 10;
        }

        public void ResurrectMerc(IntPtr D2handle)
        {
            if (IsMercHealthy(D2handle))
            {
                return;
            }

            this.move.ToTyrael(D2handle);

            // Res merc
            input.RealisticMouseMove(D2handle, 953, 272, 10, 5, 1);
            Input.LeftMouseClick();
            Thread.Sleep(1500);

            // Travel to harrogath
            input.RealisticMouseMove(D2handle, 942, 237, 10, 5, 1);
            Input.LeftMouseClick();
            Thread.Sleep(500);

            while (!views.IsInHarrogath(D2handle))
            {
                Thread.Sleep(500);
            }
        }
    }
}
