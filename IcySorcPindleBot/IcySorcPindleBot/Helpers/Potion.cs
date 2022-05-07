
using System;
using System.Collections.Generic;
using System.Threading;

namespace IcySorcPindleBot.Helpers
{
    public class Potion
    {
        private ViewHelper view;
        private static Potion instance;
        private int numPotions = 16;

        private Potion()
        {
            this.view = new();
        }

        public static Potion GetInstance()
        {
            if(Potion.instance == null)
            {
                Potion.instance = new();
            }

            return Potion.instance;
        }

        public bool NeedPotions()
        {
            if(numPotions < 12)
            {
                return true;
            }

            return false;
        }

        public void AddPotion()
        {
            if(numPotions < 16)
            {
                this.numPotions++;
            }
            else
            {
                Console.WriteLine("Adding Potions when you are already full.");
            }
            
        }

        public void UsePotion(IntPtr D2handle)
        {
            if (!view.IsHealthLow(D2handle))
            {
                return;
            }

            var potionSlots = new List<(int, int)>
            {
                (1115, 1050), (1180, 1050), (1245, 1050), (1300, 1050)
            };

            var scanCodes = new List<uint>{ 0x02, 0x03, 0x04, 0x05};

            for(var i = 0; i < potionSlots.Count; i++)
            {
                var row = potionSlots[i].Item1;
                var col = potionSlots[i].Item2;
                if(!view.IsEmptyPotionSquare(D2handle, row, col))
                {
                    //hit i
                    Input.KeyboardPress(D2handle, scanCodes[i]);
                    numPotions--;
                    Thread.Sleep(100);

                    break;
                }
            }
        }
    }
}
