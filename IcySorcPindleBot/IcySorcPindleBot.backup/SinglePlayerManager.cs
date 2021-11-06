using IcySorcPindleBot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IcySorcPindleBot
{
    class SinglePlayerManager
    {
        ActionHelper actions = new ActionHelper();
        ViewHelper views = new ViewHelper();
        ClientHelper client = new ClientHelper();

        IntPtr D2handle = IntPtr.Zero;

        public void JoinGame()
        {
            while(D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = client.GetD2WinHandle();
            }

            actions.RealisticMouseMove(D2handle, 1800, 54, 8, 2, 2);
            Thread.Sleep(500);
            actions.LeftMouseClick();
            Thread.Sleep(500);
            actions.RealisticMouseMove(D2handle, 954, 972, 8, 2, 2);
            Thread.Sleep(500);
            actions.LeftMouseClick();
            Thread.Sleep(1000);
            actions.RealisticMouseMove(D2handle, 954, 570, 8, 2, 2);
            Thread.Sleep(500);
            actions.LeftMouseClick();
            Thread.Sleep(500);

            while (!views.isGameLoaded(D2handle, client))
            {
                var location = actions.GetCursorPosition();
                Console.WriteLine("Cursor Pos: " + location.ToString());
                Console.WriteLine("Pixel Color: " + views.GetPixelColor(D2handle, location.X, location.Y));
                Thread.Sleep(500);
            }
        }

        public void ManageStart()
        {
            actions.ManageStart(D2handle, 0x76);
        }

        public void MoveToPortal()
        {
            while (D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = client.GetD2WinHandle();
            }

            actions.MoveToPortal(D2handle);

            //Todo - if not in load screen, exit game and restart

            while (!views.isTempleLoaded(D2handle, client))
            {
                var location = actions.GetCursorPosition();
                Console.WriteLine("Cursor Pos: " + location.ToString());
                Console.WriteLine("Pixel Color: " + views.GetPixelColor(D2handle, location.X, location.Y));
                Thread.Sleep(500);
            }
        }

        public void TeleToPindle()
        {
            actions.TeleToPindle(D2handle, 0x71);
        }

        public void ConductAttackSequence()
        {
            actions.AttackSequence(D2handle, 0x70);
        }

        public void MoveToLoot()
        {
            actions.MoveToLoot(D2handle);
        }
    }
}
