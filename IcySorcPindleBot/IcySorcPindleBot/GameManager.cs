using IcySorcPindleBot.Helpers;
using System;
using System.Threading;

namespace IcySorcPindleBot
{
    public class GameManager
    {
        ActionHelper actions = new ActionHelper();
        ViewHelper views = new ViewHelper();
        ClientHelper client = new ClientHelper();
        readonly Input inputs = new();
        readonly Move move = new();

        IntPtr D2handle = IntPtr.Zero;


        public GameManager()
        {
            while (D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = client.GetD2WinHandle();
            }
        }

        public void JoinGame(bool isMulti)
        {
            while(D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = client.GetD2WinHandle();
            }

            if (isMulti)
            {
                var isInDifficultySelection = views.IsInDifficultySelection(D2handle);
                while (!isInDifficultySelection)
                {
                    inputs.RealisticMouseMove(D2handle, 850, 972, 14, 2, 1);
                    Input.LeftMouseClick();
                    Thread.Sleep(2000);
                    isInDifficultySelection = views.IsInDifficultySelection(D2handle);
                }

                inputs.RealisticMouseMove(D2handle, 954, 570, 14, 2, 21);
                Thread.Sleep(500);
                Input.LeftMouseClick();
                Thread.Sleep(2000);

                var inLoadScreen = views.IsInLoadScreen(D2handle);
                while (!inLoadScreen && !views.IsInHarrogath(D2handle))
                {
                    Input.LeftMouseClick();
                    Thread.Sleep(300);
                    Input.LeftMouseClick();
                    Thread.Sleep(300);
                    Thread.Sleep(3000);
                    inLoadScreen = views.IsInLoadScreen(D2handle);
                }
            }
            else
            {
                inputs.RealisticMouseMove(D2handle, 954, 972, 14, 2, 1);
                Thread.Sleep(500);
                Input.LeftMouseClick();
                Thread.Sleep(1000);
                inputs.RealisticMouseMove(D2handle, 954, 570, 14, 2, 21);
                Thread.Sleep(500);
                Input.LeftMouseClick();
                Thread.Sleep(500);
            }

            while (!views.IsInHarrogath(D2handle))
            {
                Thread.Sleep(500);
            }
            Console.WriteLine("Game has been joined.");
        }

        public void ManageStart()
        {
            actions.ManageStart(D2handle, 0x41);
            Console.WriteLine("Start actions complete.");
        }

        public void MoveToPortal()
        {
            while (D2handle == IntPtr.Zero)
            {
                Console.WriteLine("Finding Diablo 2 process.");
                D2handle = client.GetD2WinHandle();
            }

            actions.MoveToPortal(D2handle);
        }

        public void TeleToPindle()
        {
            move.TeleToPindle(D2handle, 0x3C);
        }

        public void CallToArms()
        {
            actions.CallToArms(D2handle); 
        }

        public void ConductAttackSequence()
        {
            actions.AttackSequence(D2handle, 0x3B);
        }

        public void ConductExtendedAttack()
        {
            actions.ExtendedAttackSequence(D2handle);
        }

        public void MoveToLoot()
        {
            move.MoveToLoot(D2handle);
        }

        public void PickUpItems()
        {
            actions.PickUpItems(D2handle);
        }

        public void LeaveGame()
        {
            actions.LeaveGame(D2handle);
        }
    }
}
