using IronOcr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace IcySorcPindleBot.Helpers.ItemHelper
{
    public class ItemFinder
    {
        List<Item> Items;
        ColorHelper colors = new ColorHelper();
        Potion potion = Potion.GetInstance();
        readonly List<string> keepRunes = new List<string>() { "LEMRUNE", "PULRUNE", "UMRUNE", "MALRUNE", "ISTRUNE", "GULRUNE", "VEXRUNE", "OHMRUNE", "LORUNE", "SURRUNE", "BERRUNE", "JAHRUNE", "CHAMRUNE", "ZODRUNE" };
        readonly List<string> keepRares = new List<string>() { "CIRCLET", "TIARA", "DIADEM", "CORONET", "CORONA", "AMULET", "JEWEL", "RING" };
        readonly List<string> keepMagicals = new List<string>() { "SMALLCHARM", "GRANDCHARM" };
        readonly List<string> keepWhites = new List<string>() { "SUPERHEALINGPOTION"};
        readonly List<string> keepUnique = new List<string>() { "VAMPIREBONEGLOVES","HIEROPHANTTROPHY","UNEARTHEDWAND","ELDRITCHORB","DIMENSIONALSHARD","GILDEDSHIELD","SCARABSHELLBOOTS","RUSSETARMOR", "MESHARMOR", "DUSKSHROUD", "KRAKENSHELL", "CUIRASS", "BALROGSKIN", "SHACO", "SWIRLINGCRYSTAL", "RING", "AMULET", "SERPENTSKINARMOR", "BATTLEBOOTS", "CHAINGLOVES","DEMONHEAD", "CORONA", "DIADEM", "SPIREDHELM", "GRIMHELM" , "JEWEL", "BERSERKERAXE", "HYDRABOW", "OGREAXE", "THRESHER", "YARI"   };
        readonly List<string> uniqueExclusion = new List<string>() { "ARBALEST", "ATAGHAN", "BATTLEAXE", "BATTLEHAMMER", "BILL", "BRANDISTOCK", "BREASTPLATE", "CAP", "CEDARBW", "CHAINBOOTS", "CINQUEDEAS", "CHAINMAIL", "CLAYMORE", "CROWN", "CUTLESS", "DEFENDER", "DEVILSTAR", "DIRK", "DIVINESCEPTER", "DOUBLEAXE", "FERALCLAWS", "FLAMBERGE", "FULLHELM", "GIANTAXE", "GRANDSCEPTER", "GREATAXE", "GOTHICSWORD", "HALBERD", "HEAVYBTS", "KNOUT", "KRIS", "LEGENDARYMALLET", "LONGSWORD", "MACE", "MATRIARCHALBOW", "MILITARYPICK", "OGREMAUL", "DGREMAVL", "PARTIZAN", "PAVISE", "PIKE", "PLATEMAIL", "SALLET", "SASH", "SCALEMAIL", "SCUTUM", "SHAMSHIR", "SHARKSKINBELT", "SHRTWARB", "SKULLCAP", "SPIKEDCLUB", "SPIKEDSHIELD", "SPETUM", "SPLINTMAIL", "STUDDEDLEATHER", "VULGE", "WARCLUB", "WARSCEPTER", "WARSWORD", "WINGEDHELM" };
        readonly List<string> keepSet = new List<string>() { "AMULET", "LACQUEREDPLATE", "BRAMBLEMITTS", "WINGEDHELM", "BATTLEBOOTS", "CADUCEUS", "VORTEXSHIELD", "CORONA", "SACREDARMOR", "HEAVYBRACERS"};
        int Width = 0;
        int Height = 0;

        private Random rnd = new();

        public List<Item> FindItems(Bitmap bmp, IntPtr D2handle)
        {
            this.Items = new List<Item>();
            this.Width = bmp.Width;
            this.Height = bmp.Height;
            var currItemIndex = 0;

            //var cloneBmp = bmp.Clone(new Rectangle(0, 0, Width, Height), PixelFormat.Format32bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int strideInPixels = data.Stride / 4; //4 bytes per pixel

            for (var j = 400; j < this.Width; j++)
            {
                for (var i = 0; i < 800; i++)
                {
                    unsafe
                    {
                        uint* dataPointer = (uint*)data.Scan0;
                        uint* pixelPointer = dataPointer + i * strideInPixels + j;
                        uint color = *pixelPointer;
                        if (ColorHelper.IsItemBorderColor(color))
                        {
                            Items.Add(CreateNewItem(i, j));
                            DFS(data, currItemIndex, i, j);
                            currItemIndex++;
                        }
                    }
                }
            }

            //cloneBmp.UnlockBits(data);
            //cloneBmp.Dispose();

            bmp.Save(@"C:\Users\michael\source\repos\PixelBot\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\finditems.png", System.Drawing.Imaging.ImageFormat.Png);
            var items = GetKeepItems(D2handle, ClearFakeItems(bmp, this.Items));
            bmp.Dispose();

            return items;
        }

        public List<Item> GetAllItems(Bitmap bmp, IntPtr D2handle)
        {
            this.Items = new List<Item>();
            this.Width = bmp.Width;
            this.Height = bmp.Height;
            var currItemIndex = 0;

            //var cloneBmp = bmp.Clone(new Rectangle(0, 0, Width, Height), PixelFormat.Format32bppRgb);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int strideInPixels = data.Stride / 4; //4 bytes per pixel

            for (var j = 400; j < this.Width; j++)
            {
                for (var i = 0; i < 800; i++)
                {
                    unsafe
                    {
                        uint* dataPointer = (uint*)data.Scan0;
                        uint* pixelPointer = dataPointer + i * strideInPixels + j;
                        uint color = *pixelPointer;
                        if (ColorHelper.IsItemBorderColor(color))
                        {
                            Items.Add(CreateNewItem(i, j));
                            DFS(data, currItemIndex, i, j);
                            currItemIndex++;
                        }
                    }
                }
            }

            //cloneBmp.UnlockBits(data);
            //cloneBmp.Dispose();

            bmp.Save(@"C:\Users\michael\source\repos\PixelBot\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\finditems.png", System.Drawing.Imaging.ImageFormat.Png);
            var items = ClearFakeItems(bmp, this.Items);
            bmp.Dispose();

            return items;
        }

        public Point GetItemClickPoint(Item item)
        {
            int x = (item.Right + item.Left) / 2;
            int y = (item.Top + item.Bottom) / 2;

            return new Point(x, y);
        }

        private void DFS(BitmapData data, int parent, int row, int col)
        {
            int strideInPixels = data.Stride / 4; //4 bytes per pixel

            unsafe
            {
                Stack<Point> stack = new();
                stack.Push(new Point(row, col));
                Point current;
                uint* dataPointer = (uint*)data.Scan0;

                do
                {
                    var shouldVisit = false;
                    current = stack.Pop();

                    uint* pixelPointer = dataPointer + current.X * strideInPixels + current.Y;
                    uint color = *pixelPointer;

                    if (ColorHelper.IsItemBorderColor(color))
                    {
                        row = current.X;
                        col = current.Y;
                        SetToVisited(data, row, col);
                        UpdateParentItemBounds(parent, row, col);
                        shouldVisit = true;
                    }
                    else
                    {
                        var special = colors.IsSpecialColor(color);
                        if (special != null && Items[parent].ItemType == null)
                        {
                            Items[parent].ItemType = special;
                            shouldVisit = true;
                        }
                        SetToVisited(data, row, col);
                    }
                    if (shouldVisit)
                    {
                        if (col - 1 > 0)
                        {
                            stack.Push(new Point(row, col - 1));
                        }
                        if (col + 1 < this.Width)
                        {
                            stack.Push(new Point(row, col + 1));
                        }
                        if (row - 1 > 0)
                        {
                            stack.Push(new Point(row - 1, col));
                        }
                        if (row + 1 < this.Height)
                        {
                            stack.Push(new Point(row + 1, col));
                        }

                    }
                } while (stack.Count > 0);
            }
        }

        private Item CreateNewItem(int row, int col)
        {
            var item = new Item(row , col);
            return item;
        }

        private bool HasVisited(uint color)
        {
                return color == 0xffffff;
        }

        private void SetToVisited(BitmapData data, int row, int col)
        {
            int strideInPixels = data.Stride / 4; //4 bytes per pixel
            unsafe
            {
                uint* dataPointer = (uint*)data.Scan0;
                uint* pixelPointer = dataPointer + row * strideInPixels + col;
                *pixelPointer = 0xffffffff;
                return;
            }
        }



        private void UpdateParentItemBounds(int parent, int row, int col)
        {
            var parentItem = Items[parent];
            if(row > parentItem.Bottom)
            {
                parentItem.Bottom = row;
            }
            if(col > parentItem.Right)
            {
                parentItem.Right = col;
            }
        }

        public void GetItemNames(Bitmap bmp, List<Item> items)
        {
            var counter = 0;
            foreach (var item in items)
            {
                Rectangle rect = new Rectangle(new Point(item.Left, item.Top), new Size(item.Right - item.Left, item.Bottom - item.Top));

                var croppedBmp = bmp.Clone(rect, PixelFormat.Format32bppRgb);

                ConvertBitmapToBlackWhite(croppedBmp, item.ItemType);
                croppedBmp.Save($@"C:\Users\michael\source\repos\PixelBot\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\itemname{counter}.png", System.Drawing.Imaging.ImageFormat.Png);
                item.ItemName = ReadWordFromScreen(croppedBmp);
                counter++;
                croppedBmp.Dispose();
            }
        }

        public void ConvertBitmapToBlackWhite(Bitmap bmp, string itemType)
        {
            this.Width = bmp.Width;
            this.Height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int strideInPixels = data.Stride / 4;

            for (var j = 0; j < this.Width; j++)
            {
                for (var i = 0; i < this.Height; i++)
                {
                    unsafe
                    {
                        uint* dataPointer = (uint*)data.Scan0;
                        uint* pixelPointer = dataPointer + i * strideInPixels + j;
                        uint color = *pixelPointer;
                        if (ColorHelper.IsWithinBorderColorRange(color))
                        {
                            *pixelPointer = 0xFFFFFFFF;
                        }
                        else
                        {
                            *pixelPointer = 0xFF000000;
                        }
                    }
                }
            }

            bmp.UnlockBits(data);
        }

        public string ReadWordFromScreen(Bitmap bmp)
        {

            var ocr = new IronTesseract();

            ocr.Language = OcrLanguage.English;
            ocr.Configuration.WhiteListCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using (var input = new OcrInput(bmp))
            {

                return ocr.Read(input).Text.Trim();
            }
        }

        private bool ShouldKeep(IntPtr D2handle, Item item)
        {
            if (item.ItemType == "Unique" && !CheckAllEditDistance(item.ItemName, uniqueExclusion, 1))
            {
                return true;
            }

            if (item.ItemType == "Rune")
            {
                return true;
            }

            if (item.ItemType == "Set" && CheckAllEditDistance(item.ItemName, keepSet, 3))
            {
                return true;
            }

            if (item.ItemType == "Rare" && CheckAllEditDistance(item.ItemName, keepRares, 1))
            {
                return true;
            }

            if (item.ItemType == "Magical" && CheckAllEditDistance(item.ItemName, keepMagicals, 3))
            {
                return true;
            }

            if(item.ItemType == "White" && CheckAllEditDistance(item.ItemName, keepWhites, 2)
                && potion.NeedPotions(D2handle))
            {
                potion.PotionPickedUpThisRun = true;
                return true;
            }

            //if (item.ItemType == "White" && CheckAllEditDistance(item.ItemName, keepWhites, 5))
            //{
            //    return true;
            //}
            return false;
        }

        private static List<Item> ShouldConsider(List<Item> items)
        {
            var considerItems = new List<Item>();
            foreach(var item in items)
            {
                var itemArea = (item.Bottom - item.Top) * (item.Right - item.Left);
                item.Area = itemArea;

                if( itemArea > 500 )
                {
                    considerItems.Add(item);
                }
            }
            return considerItems;
        }

        private List<Item> HasItemType(List<Item> items)
        {
            var itemTypeList = new List<Item>();
            foreach(var item in items)
            {
                if (item.ItemType != null)
                {
                    itemTypeList.Add(item);
                }
            }

            return itemTypeList;
        }


        private List<Item> ClearFakeItems(Bitmap bmp, List<Item> initialItems)
        {
            var actualItems = new List<Item>();
            var considerItems = ShouldConsider(initialItems);
            considerItems = HasItemType(considerItems);

            GetItemNames(bmp, considerItems);

            foreach(var item in considerItems)
            {
                if (item.ItemName != null)
                {
                    actualItems.Add(item);
                }
            }

            return actualItems;
        }

        private List<Item> GetKeepItems(IntPtr D2handle, List<Item> items)
        {
            var keepItems = new List<Item>();
            foreach (var item in items)
            {
                if (ShouldKeep(D2handle, item))
                {
                    keepItems.Add(item);
                }
            }

            return keepItems;
        }


        public static bool CheckAllEditDistance(string ocrVersion, List<string> itemList, int maxDist)
        {
            foreach (var item in itemList)
            {
                if (LevenshteinDistance(ocrVersion, item) <= maxDist)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistance(string s, string t)
        {
            s = Regex.Replace(s, @"\s+", "");
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
