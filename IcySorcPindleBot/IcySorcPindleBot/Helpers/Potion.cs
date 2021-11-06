
using System;
using System.Collections.Generic;
using System.Threading;

namespace IcySorcPindleBot.Helpers
{
    public class Potion
    {
        private ViewHelper view;
        public bool PotionPickedUpThisRun = false;
        private static Potion instance;

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

        public void TogglePotions(IntPtr D2handle)
        {
            //open the potions
            Input.KeyboardPress(D2handle, 0x29);
            Thread.Sleep(200);
        }

        public bool NeedPotions(IntPtr D2handle)
        {
            if (this.PotionPickedUpThisRun)
            {
                return false;
            }

            TogglePotions(D2handle);

            for(var i = 1115; i <= 1340; i += 62)
            {
                for(var j = 874; j <= 1045; j += 57)
                {
                    if(view.IsEmptyPotionSquare(D2handle, i, j))
                    {
                        TogglePotions(D2handle);
                        return true;
                    }
                }
            }

            TogglePotions(D2handle);

            return false;
        }

        public void UsePotion(IntPtr D2handle)
        {
            this.PotionPickedUpThisRun = false;

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
                    Thread.Sleep(100);

                    break;
                }
            }
        }
    }
}
