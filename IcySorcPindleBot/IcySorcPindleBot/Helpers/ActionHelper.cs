using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using IcySorcPindleBot.Helpers.ItemHelper;
using System.Linq;

namespace IcySorcPindleBot.Helpers
{
    public class ActionHelper
    {
        private readonly Random rnd = new();
        readonly Input inputs = new();
        readonly ItemFinder ItemFinder = new();
        readonly ViewHelper view = new();
        readonly Stash stash = new();
        readonly Merc merc = Merc.GetInstance();
        readonly Move move = new();
        readonly Potion potion = Potion.GetInstance();
        private bool hasItem = false;

        public void ManageStart(IntPtr D2handle, byte frozenHotkey)
        {
            Thread.Sleep(300);

            Input.KeyboardPress(D2handle, frozenHotkey);
            Input.RightMouseClick();
            Thread.Sleep(100);

            potion.UsePotion(D2handle);

            //Todo pick up body if death

            //merc.ResurrectMerc(D2handle);
        }

        public void CallToArms(IntPtr D2handle)
        {
            Input.KeyboardPress(D2handle, 0x11);
            Thread.Sleep(300);
            Input.KeyboardPress(D2handle, 0x3E);
            Thread.Sleep(300);
            Input.RightMouseClick();
            Thread.Sleep(1000);
            Input.KeyboardPress(D2handle, 0x3F);
            Thread.Sleep(300);
            Input.RightMouseClick();
            Thread.Sleep(500);
            Input.KeyboardPress(D2handle, 0x11);
        }

        public void MoveToPortal(IntPtr D2handle)
        {
            CheckForItems(D2handle, 15);

            if (!this.hasItem)
            {
                move.FromLoadStraightToPortal(D2handle);
            }
            else
            {
                move.FromLoadToStashDecision(D2handle);

                move.FromStashDecisionPointToStash(D2handle);

                stash.EmptyInventory(D2handle);
                this.hasItem = false;

                move.FromStashToPortal(D2handle);          
            }     

            var portalAttempts = 0;

            var hasHitLoadScreen = false;
            //try one last time to get the portal
            while (!hasHitLoadScreen && !view.IsTempleLoaded(D2handle) && portalAttempts < 2)
            {
                //take snapshot, find largest black thing
                var portalLocation = view.GetPortalLocation(D2handle);
                inputs.RealisticMouseMove(D2handle, portalLocation.X, portalLocation.Y, 12, 5, 1);
                Input.LeftMouseClick();

                var counter = 0; 

                while (!hasHitLoadScreen && counter < 25 )
                {
                    if (view.IsInLoadScreen(D2handle))
                    {
                        hasHitLoadScreen = true;
                    }
                    Thread.Sleep(100);
                    counter++;
                }
                
                if(portalAttempts > 0)
                {
                    Console.WriteLine($@"Failed to find portal one atttempt: {portalAttempts - 1}");
                }
                portalAttempts++;
            }

            if(portalAttempts >= 5)
            {
                Console.WriteLine("Failed to find portal after 5 attempts. Leaving game.");
                LeaveGame(D2handle);
                Thread.Sleep(5000);
                throw new Exception("Restarting run");
            }

            while (!view.IsTempleLoaded(D2handle))
            {
                Thread.Sleep(500);
            }
        }

        public void AttackSequence(IntPtr D2handle, byte blizzHotkey)
        {
            potion.UsePotion(D2handle);
            Thread.Sleep(500);
            Console.WriteLine("Commencing attack phase");
            Input.KeyboardPress(D2handle, blizzHotkey);
            inputs.RealisticMouseMove(D2handle, 1457, 276, 10, 5, 1);
            Input.RightMouseClick();

            inputs.RealisticMouseMove(D2handle, 1373, 289, 10, 5, 1);
            CastPrimary(2000);
            Thread.Sleep(150);
            Input.RightMouseClick();
            inputs.RealisticMouseMove(D2handle, 1325, 325, 10, 5, 1);
            Thread.Sleep(100);
            potion.UsePotion(D2handle);
            CastPrimary(3000);
        }

        private void CastPrimary(int time)
        {
            Input.SendKey(0x2A, false, Input.InputType.Keyboard);
            Thread.Sleep(50);
            Input.MouseClickDown();
            Thread.Sleep(time);
            Input.MouseClickUp();
            Thread.Sleep(100);
            Input.SendKey(0x2A, true, Input.InputType.Keyboard);
        }

        public void ExtendedAttackSequence(IntPtr D2handle)
        {

            inputs.RealisticMouseMove(D2handle, 900, 525, 10, 5, 2);
            Thread.Sleep(50);
            //select hydra
            Input.KeyboardPress(D2handle, 0x40);
            Thread.Sleep(300);

            //drop 3 hydras

            Input.RightMouseClick();
            Thread.Sleep(2000);
            Input.RightMouseClick();
            Thread.Sleep(2000);
            Input.RightMouseClick();
        }

        public void PickUpItems(IntPtr D2handle)
        {
            inputs.RealisticMouseMove(D2handle, 750, 100, 10, 10, 2);
            //Alt button
            Input.KeyboardPress(D2handle, 0x38);
            Thread.Sleep(200);
            var bitmap = ViewHelper.GetMapFromScreen(D2handle);

            var currentItems = ItemFinder.FindItemsUF(bitmap, D2handle);

            foreach (var item in currentItems)
            {
                Console.WriteLine($@"Found {item.ItemType} {item.ItemName}");
            }

            if ( currentItems.Count > 0)
            {
                this.hasItem = currentItems.Where(item => item.ItemType != "White").Any();
                Mailer.SendItemFind(currentItems);
            }

            var attempts = 0;
            while (currentItems.Count > 0 && attempts < 10)
            {
                var clickPoint = ItemFinder.GetItemClickPoint(currentItems[0]);

                inputs.RealisticMouseMove(D2handle, clickPoint.X, clickPoint.Y, 15, 0, 1);
                Thread.Sleep(300);
                Input.LeftMouseClick();

                Thread.Sleep(1000);
                inputs.RealisticMouseMove(D2handle, 950, 100, 15, 5, 1);
                bitmap = ViewHelper.GetMapFromScreen(D2handle);
                currentItems = ItemFinder.FindItemsUF(bitmap, D2handle);
                attempts++;
            }

            bitmap.Dispose();
        }

        public bool CheckForItems(IntPtr D2handle, int prob)
        {
            //going to check randomly every 15 runs or so just to make sure to drop off random shit we pick up by accident
            var randCheck = rnd.Next(prob);
            
            if (randCheck == prob - 1)
            {
                this.hasItem = this.hasItem || stash.InventoryHasItems(D2handle);
            }

            return hasItem;
        }

        public void LeaveGame(IntPtr D2handle)
        {
            Input.KeyboardPress(D2handle, 0x01);
            inputs.RealisticMouseMove(D2handle, 954, 475, 15, 6, 1);
            Input.LeftMouseClick();

            Thread.Sleep(700);

            while(view.IsInLoadScreen(D2handle))
            {
                Thread.Sleep(300);
            }
        }
    }
}
