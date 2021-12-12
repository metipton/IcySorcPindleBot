using System;
using System.Drawing;
using System.Threading;

namespace IcySorcPindleBot.Helpers
{
    public class Stash
    {
        readonly Input inputs = new();
        readonly ViewHelper views = new();
        private int currentStashTab = 0;

        public Point GetItemLocation(IntPtr D2handle)
        {

            Thread.Sleep(100);

            for (var row = 1320; row <= 1490; row += 55)
            {
                for (var col = 550; col <= 720; col += 55)
                {
                    //item border color and empty stash color are effectively the same
                    if (!views.IsEmptyStashSquare(D2handle, row, col))
                    {
                        return new Point(row, col);
                    }
                }
            }

            return new Point(0, 0);
        }

        private bool IsCurrentClear(IntPtr D2handle, int row, int col)
        {
            if (views.IsEmptyStashSquare(D2handle, row, col))
            {
                return true;
            }

            return false;
        }

        public bool InventoryHasItems(IntPtr D2handle)
        {
            OpenInventory(D2handle);

            var hasItems = false;

            for (var row = 1320; row <= 1490; row += 56)
            {
                for (var col = 550; col <= 720; col += 55)
                {
                    //item border color and empty stash color are effectively the same
                    if (!views.IsEmptyStashSquare(D2handle, row, col))
                    {
                        hasItems = true;
                    }
                }
            }

            CloseStashOrInventory(D2handle);

            return hasItems;
        }

        public void EmptyInventory(IntPtr D2handle)
        {

            ChangeStashTab(D2handle);
            var itemLocation = GetItemLocation(D2handle);

            while (itemLocation.X != 0 && itemLocation.Y != 0)
            {
                inputs.RealisticMouseMove(D2handle, itemLocation.X, itemLocation.Y, 7, 4, 1);
                Input.SendKey(0x1D, false, Input.InputType.Keyboard);
                Thread.Sleep(200);
                Input.LeftMouseClick();
                Thread.Sleep(200);
                Input.SendKey(0x1D, true, Input.InputType.Keyboard);

                if(!IsCurrentClear(D2handle, itemLocation.X, itemLocation.Y))
                {
                    this.currentStashTab++;
                    ChangeStashTab(D2handle);
                }

                itemLocation = GetItemLocation(D2handle);
            }

            CloseStashOrInventory(D2handle);
        }

        private void ChangeStashTab(IntPtr D2handle)
        {
            if(currentStashTab == 0)
            {
                return;
            }
            else if(currentStashTab == 1)
            {
                inputs.RealisticMouseMove(D2handle, 270, 120, 7, 4, 1);
                Input.LeftMouseClick();
            }
            else if(currentStashTab == 2)
            {
                inputs.RealisticMouseMove(D2handle, 400, 120, 7, 4, 1);
                Input.LeftMouseClick();
            }
            else if(currentStashTab == 3)
            {
                inputs.RealisticMouseMove(D2handle, 550, 120, 7, 4, 1);
                Input.LeftMouseClick();
            }
            else
            {
                throw new Exception("Stash is full. Empty it biiitch.");
            }
        }

        public static void OpenInventory(IntPtr D2handle)
        {
            //press i
            Input.KeyboardPress(D2handle, 0x17);
            Thread.Sleep(100);
        }

        public static void CloseStashOrInventory(IntPtr D2handle)
        {
            // Press esc
            Input.KeyboardPress(D2handle, 0x01);
            Thread.Sleep(100);
        }
    }
}
