namespace IcySorcPindleBot
{
    using System;
    using System.Threading;
    using Helpers;


    class IcySorcPindleBot
    {


        // Main Method
        static void Main(string[] args)
        {
            var singlePlayerManager = new GameManager();
            var clientHelper = new ClientHelper();
            var views = new ViewHelper();
            bool isMulti = true;
            bool inGame = false;
            bool trial = false;
            int runCount = 1;
            Stash stash = new();

            while (true)
            {
                //while (true)
                //{
                //    var location = Input.GetCursorPosition();
                //    Console.WriteLine("Cursor Pos: " + location.ToString());
                //    Console.WriteLine("Pixel Color: " + views.GetPixelColor(clientHelper.GetD2WinHandle(), location.X, location.Y));
                //    Thread.Sleep(500);
                //}
                if (trial)
                {
                    IntPtr D2handle = IntPtr.Zero;
                    while (D2handle == IntPtr.Zero)
                    {
                        Console.WriteLine("Finding Diablo 2 process.");
                        D2handle = clientHelper.GetD2WinHandle();
                    }

                    stash.EmptyInventory(D2handle);

                    while (true)
                    {
                        var location = Input.GetCursorPosition();
                        Console.WriteLine("Cursor Pos: " + location.ToString());
                        Console.WriteLine("Pixel Color: " + views.GetPixelColor(clientHelper.GetD2WinHandle(), location.X, location.Y));
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    if (!inGame)
                    {
                        Console.WriteLine(@$"Starting run: {runCount}");
                        singlePlayerManager.JoinGame(isMulti);
                        singlePlayerManager.ManageStart();
                        singlePlayerManager.MoveToPortal();
                        singlePlayerManager.TeleToPindle();
                        singlePlayerManager.ConductAttackSequence();
                        //singlePlayerManager.ConductExtendedAttack();
                        singlePlayerManager.MoveToLoot();
                        singlePlayerManager.PickUpItems();
                        singlePlayerManager.LeaveGame();
                        inGame = false;
                        runCount++;
                    }
                }

                //while (true)
                //{
                //    var location = actions.GetCursorPosition();
                //    Console.WriteLine("Cursor Pos: " + location.ToString());
                //    Console.WriteLine("Pixel Color: " + views.GetPixelColor(clientHelper.GetD2WinHandle(), location.X, location.Y));
                //    Thread.Sleep(500);
                //}
            }

        }
    }
}
