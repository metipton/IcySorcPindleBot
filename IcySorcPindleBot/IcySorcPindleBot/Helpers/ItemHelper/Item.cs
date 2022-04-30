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

        public Item(int row, int col, string itemType)
        {
            this.Top = row;
            this.Bottom = row;
            this.Right = col;
            this.Left = col;
            this.ItemType = itemType;
        }

        public void ExpandBounds(int row, int col)
        {
            if( row > Bottom)
            {
                Bottom = row;
            }
            else if (row < Top)
            {
                Top = row;
            }

            if(col < Left)
            {
                Left = col;
            }
            else if( col > Right)
            {
                Right = col;
            }
        }
    }
}
