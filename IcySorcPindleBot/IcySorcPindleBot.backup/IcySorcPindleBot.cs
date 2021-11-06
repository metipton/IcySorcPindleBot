namespace IcySorcPindleBot
{
    using System;
    using System.Drawing;
    using System.Threading;
    using Helpers;


    class IcySorcPindleBot
    {


        // Main Method
        static void Main(string[] args)
        {
            var singlePlayerManager = new SinglePlayerManager();
            var actions = new ActionHelper();
            var clientHelper = new ClientHelper();
            var views = new ViewHelper();
            bool isMulti = true;
            bool inGame = false;

            while (true)
            {
                if (isMulti)
                {
                    IntPtr D2handle = IntPtr.Zero;

                    while (D2handle == IntPtr.Zero)
                    {
                        Console.WriteLine("Finding Diablo 2 process.");
                        D2handle = clientHelper.GetD2WinHandle();
                    }

                    views.ReadWordsFromScreen(D2handle);
                    var location = actions.GetCursorPosition();
                    Console.WriteLine("Cursor Pos: " + location.ToString());
                    Console.WriteLine("Pixel Color: " + views.GetPixelColor(clientHelper.GetD2WinHandle(), location.X, location.Y));
                    Thread.Sleep(500);
                }
                else
                {
                    if (!inGame)
                    {
                        singlePlayerManager.JoinGame();
                        singlePlayerManager.ManageStart();
                        singlePlayerManager.MoveToPortal();
                        singlePlayerManager.TeleToPindle();
                        singlePlayerManager.ConductAttackSequence();
                        singlePlayerManager.MoveToLoot();
                    }
                }

                while (true)
                {
                    var location = actions.GetCursorPosition();
                    Console.WriteLine("Cursor Pos: " + location.ToString());
                    Console.WriteLine("Pixel Color: " + views.GetPixelColor(clientHelper.GetD2WinHandle(), location.X, location.Y));
                    Thread.Sleep(500);
                }
            }

        }
    }
}
