using System.Drawing;

namespace IcySorcPindleBot.Helpers.ItemHelper
{
    public class Item
    {
        public string ItemName;
        public string ItemType = null;
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
        public int Area = 1;

        public Item (int row, int col)
        {
            this.Top = row;
            this.Left = col;
        }
    }
}
