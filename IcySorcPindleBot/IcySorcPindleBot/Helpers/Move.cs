using System;
using System.Threading;


namespace IcySorcPindleBot.Helpers
{
    public class Move
    {
        readonly Input inputs = new();
        readonly ViewHelper views = new();

        public void FromLoadToStashDecision(IntPtr D2handle)
        {
            // WP1
            inputs.RealisticMouseMove(D2handle, 606, 906, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(900);


            // wp2
            inputs.RealisticMouseMove(D2handle, 790, 605, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(900);


            //WP3
            inputs.RealisticMouseMove(D2handle, 850, 712, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(900);

            // WP4
            inputs.RealisticMouseMove(D2handle, 1039, 673, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(900);
        }

        public void FromStashDecisionToPortal(IntPtr D2handle)
        {
            // WP5
            inputs.RealisticMouseMove(D2handle, 438, 790, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(900);

            // WP6
            inputs.RealisticMouseMove(D2handle, 765, 731, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            // WP7
            inputs.RealisticMouseMove(D2handle, 975, 970, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            // WP8
            inputs.RealisticMouseMove(D2handle, 200, 550, 12, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);
        }

        public void FromStashDecisionPointToStash(IntPtr D2handle)
        {
            //Move to stash
            inputs.RealisticMouseMove(D2handle, 1298, 560, 8, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(500);
        }

        public void FromStashToPortal(IntPtr D2handle)
        {
            //Move to portal
            inputs.RealisticMouseMove(D2handle, 980, 809, 8, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1300);

            inputs.RealisticMouseMove(D2handle, 25, 985, 8, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1300);

            inputs.RealisticMouseMove(D2handle, 360, 837, 8, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1300);
        }

        public void TeleToPindle(IntPtr D2handle, byte teleHotkey)
        {
            Console.WriteLine("Temple loaded.");
            Input.KeyboardPress(D2handle, teleHotkey);

            // WP1
            inputs.RealisticMouseMove(D2handle, 1354, 61, 10, 5, 2);
            Input.RightMouseClick();
            Thread.Sleep(200);

            // wp2
            inputs.RealisticMouseMove(D2handle, 1556, 148, 10, 5, 2);
            Input.RightMouseClick();
            Thread.Sleep(200);

            //WP3
            inputs.RealisticMouseMove(D2handle, 1248, 362, 10, 5, 2);
            Input.RightMouseClick();
            Thread.Sleep(200);

            inputs.RealisticMouseMove(D2handle, 1367, 249, 10, 5, 2);
            Input.RightMouseClick();
            Thread.Sleep(200);
        }

        public void ToTyrael(IntPtr D2handle)
        {
            // Move 1
            inputs.RealisticMouseMove(D2handle, 578, 930, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            // Near waypoint
            inputs.RealisticMouseMove(D2handle, 579, 851, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            // Hit Waypoint
            inputs.RealisticMouseMove(D2handle, 865, 662, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            // Click act 4
            inputs.RealisticMouseMove(D2handle, 440, 140, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(100);

            // Select Pand Fortress
            inputs.RealisticMouseMove(D2handle, 417, 189, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(500);

            while (!views.IsInPandFortress(D2handle))
            {
                Thread.Sleep(500);
            }

            // Move near tyrael
            inputs.RealisticMouseMove(D2handle, 300, 487, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1200);

            //while (true)
            //{
            //    var location = Input.GetCursorPosition();
            //    Console.WriteLine("Cursor Pos: " + location.ToString());
            //    Thread.Sleep(500);
            //}

            // Click on tyrael
            inputs.RealisticMouseMove(D2handle, 904, 86, 10, 5, 2);
            Input.LeftMouseClick();
            Thread.Sleep(1600);
        }

        public void MoveToLoot(IntPtr D2handle)
        {
            Thread.Sleep(500);

            //Teleport forward
            inputs.RealisticMouseMove(D2handle, 1213, 414, 10, 5, 2);
            Thread.Sleep(200);
            Input.LeftMouseClick();
            Thread.Sleep(500);
            //Get mouse out of way for the screen shot
            inputs.RealisticMouseMove(D2handle, 750, 199, 10, 5, 2);
            Thread.Sleep(200);
        }
    }
}
