namespace IcySorcPindleBot
{
    using System;
    using System.Threading;
    using global::IcySorcPindleBot.Helpers.ItemHelper;
    using Helpers;


    class IcySorcPindleBot
    {
        // Main Method
        static void Main(string[] args)
        {
            var singlePlayerManager = new GameManager();
            var clientHelper = new ClientHelper();
            var views = new ViewHelper();
            var items = new ItemFinder();
            var actions = new ActionHelper();
            bool trial = false;
            bool isMulti = true;
            Stash stash = new();
            int stashErrorCount = 0;

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

                    actions.PickUpItems(D2handle);

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
                    try
                    {
                        Console.WriteLine(@$"Starting run: {Globals.RUN_NUMBER}");
                        singlePlayerManager.JoinGame(isMulti);
                        singlePlayerManager.ManageStart();
                        singlePlayerManager.MoveToPortal();
                        //singlePlayerManager.CallToArms();
                        singlePlayerManager.TeleToPindle();
                        singlePlayerManager.ConductAttackSequence();
                        //singlePlayerManager.ConductExtendedAttack();
                        singlePlayerManager.MoveToLoot();
                        singlePlayerManager.PickUpItems();
                        singlePlayerManager.LeaveGame();
                        stashErrorCount = 0;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        singlePlayerManager.LeaveGame();
                        if(ex.Message.Equals("Stash is full. Empty it biiitch."))
                        {
                            stashErrorCount++;
                        }
                        if(stashErrorCount > 0)
                        {
                            break;
                        }
                    }

                    Globals.RUN_NUMBER++;
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
