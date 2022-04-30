using IronOcr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;

namespace IcySorcPindleBot.Helpers.ItemHelper
{
    public class ItemFinder
    {
        List<Item> Items;
        ColorHelper colors = new ColorHelper();
        Potion potion = Potion.GetInstance();
        readonly List<string> keepRunes = new List<string>() { "LEMRUNE", "PULRUNE", "UMRUNE", "MALRUNE", "ISTRUNE", "GULRUNE", "VEXRUNE", "OHMRUNE", "LORUNE", "SURRUNE", "BERRUNE", "JAHRUNE", "CHAMRUNE", "ZODRUNE" };
        readonly List<string> runeExclusionList = new List<string>() { "ELRUNE", "ELDRUNE", "TIRRUNE", "NEFRUNE", "ETHRUNE", "ITHRUNE", "TALRUNE", "RALRUNE", "RTRUNE", "THULRUNE", "AMNRUNE", "SLRUNE", "SHAELRUNE", "DLRUNE", "HELRUNE", "IRUNE", "KRUNE" };
        readonly List<string> keepRares = new List<string>() { "CIRCLET", "TIARA", "DIADEM", "CORONET", "CORONA", "AMULET", "RING" };
        //readonly List<string> keepRares = new List<string>() { "CIRCLET", "TIARA", "DIADEM", "CORONET", "CORONA", "AMULET", "RING", "LEATHERGLOVES", "HEAVYGLOVES", "CHAINGLOVES", "LIGHTGAUNTLETS", "GAUNTLETS", "DEMONHIDEGLOVES", "SHARKSKINGLOVES", "HEAVYBRACERS", "BATTLEGAUNTLETS", "WARGAUNTLETS", "BRAMBLEMITTS", "VAMPIREBONEGLOVES", "VAMBRACES", "CRUSADERGAUNTLETS", "OGREGAUNTLETS" };
        readonly List<string> keepMagicals = new List<string>() { "SMALLCHARM", "LARGECHARM", "GRANDCHARM", "MONARCH", "JEWEL" };
        readonly List<string> MagicalTrashList = new List<string> { "RNDEL", "HEL", "DGEL" };
        readonly List<string> keepWhites = new List<string>() { "SUPERHEALINGPOTION"};
        readonly List<string> keepUnique = new List<string>() { "VAMPIREBONEGLOVES","HIEROPHANTTROPHY","UNEARTHEDWAND","ELDRITCHORB","DIMENSIONALSHARD","GILDEDSHIELD","SCARABSHELLBOOTS","RUSSETARMOR", "MESHARMOR", "DUSKSHROUD", "KRAKENSHELL", "CUIRASS", "BALROGSKIN", "SHACO", "SWIRLINGCRYSTAL", "RING", "AMULET", "SERPENTSKINARMOR", "BATTLEBOOTS", "CHAINGLOVES","DEMONHEAD", "CORONA", "DIADEM", "SPIREDHELM", "GRIMHELM" , "JEWEL", "BERSERKERAXE", "HYDRABOW", "OGREAXE", "THRESHER", "YARI"   };
        readonly List<string> uniqueExclusion = new List<string>() { "AEGIS", "ANCIENTARMR", "ANCIENTAXE", "ANCIENTSHIELD", "ARBALEST", "ARMET", "ATAGHAN", "AXE", "BALLISTA", "BALRGBLADE", "BALRGSPEAR", "BARBEDCLUB", "BARBEDSHIELD", "BARDICHE", "BASINET", "BASTARDSWORD", "BATTLEAXE", "BATTLEBELT", "BATTLECESTUS", "BATTLEDART", "BATTLEHAMMER", "BATTLESCYTHE", "BATTLESTAFF", "BATTLESWRD", "BEARDEDAXE", "BECDECRBIN", "BELT", "BILL", "BLADE", "BALDEBARRIER", "BONEHELM", "BONESHIELD", "BONEWAND", "BRANDISTOCK", "BREASTPLATE", "BROADAXE", "BROADSWORD", "CAP", "CEDARBW", "CEREMONIALPIKE", "CHAINBTS", "CHAINMAIL", "CHAMPINSWORD", "CHASSARMOR", "CHUKNU", "CINQUEDEAS", "CHAINMAIL", "CHAMPIONAXE", "CLAYMORE", "CLEAVER", "CLSCROSSBOW", "CROSSBOW", "CROWN", "CRYPTICSWORD", "CUDGEL", "CUTLESS", "DACIANFALX", "DEATHMASK", "DEATHIMMASK", "DECAPITATOR", "DEFENDER", "DEMONHIDEGLOVES", "DEMONHIDESASH", "DEVILSTAR", "DIMENSIONALBLADE", "DIRK", "DIVINESCEPTER", "DOUBLEAXE", "DRAGONSHIELD", "EDGEBW", "ELDERSTAFF", "ELEGANTBLADE", "EMBOSSEDPLATE", "ESPANDN", "ETTINAXE", "EXECUTIONERSWORD", "FALCHION", "FERALCLAWS", "FIELDPLATE", "FLAIL", "FLAMBERGE", "FLANGEDMACE", "FLYINGAXE", "FRANCISCA", "FULLHELM", "FULLPLATEMAIL", "FULLTPLATEIMAIL", "FUSCINA", "GAUNTLETS", "GHOSTARMOR", "GIANTAXE", "GIANTSWRD", "GLADIUS", "GRANDSCEPTER", "GREATAXE", "GREATHELM", "GREATMAUL", "GREATSWORD", "GRIMWAND", "GOTHICAXE", "GTHICBW", "GTHICPLATE", "GOTHICSWORD", "GRANDCROWN", "GREAVES", "GRIMSCYTHE", "HALBERD", "HARDLEATHERARMOR", "HATCHET", "HEAVYBTS", "HEAVYCROSSBOW", "HEAVYGLOVES", "HELM", "HUNTERSBOW", "HYPERINSPEAR", "JAGGEDSTAR", "KITESHIELD", "KNOUT", "KRAKENSHELL", "KRIS", "LANCE", "LARGEAXE", "ARGESHIELD", "LEGENDARYMALLET", "LIGHTBELT", "LIGHTCROSSBOW", "LIGHTPLATE", "LIGHTPLATEDBTS", "LINKEDMAIL", "LOCHABERAXE", "LNGBATTLEBW", "LONGSTAFF", "LONGSWORD", "LNGBW", "LNGWARB", "LUNA", "MACE", "MANCATCHER", "ITANCATCHER", "MAGEPLATE", "MARTELDEFER", "MASK", "MATRIARCHALBOW", "MORNINGSTAR", "MAUL", "NAGA", "MESHBELT", "MILITARYPICK", "OGREMAUL", "DGREMAVL", "PARTIZAN", "PAVISE", "PHASEBLADE", "PIKE", "PLATEDBELT", "PLATEMAIL", "POIGNARD", "POLEAXE", "QUARTERSTAFF", "RAZRBW", "REPEATINGCROSSBOW", "RINGMAIL", "RNDEL", "RUNDSHIELD", "RUNESCEPTER", "RUNESTAFF", "RUNESWORD", "SABRE", "SALLET", "SASH", "SCALEMAIL", "SCUTUM", "SCYTHE", "SHADOWPLATE", "SHAMSHIR", "SHARKSKINBELT", "SHARKTOOTHARMOR", "SHRTBATTLEB", "SHORTSWORD", "SHRTWARB", "SIEGECROSSBOW", "SILVEREDGEDAXE", "SKULLCAP", "SALLSHIELD", "SPEAR", "SPIKEDCLUB", "SPIKEDSHIELD", "SPETUM", "SPLINTMAIL", "STILETTO", "STUDDEDLEATHER", "TABAR", "TEMPLARCOAT", "THUNDERMAUL", "TOMAHAWK", "TWERSHIELD", "TRELLISEDARMOR", "TRIDENT", "TROLLNEST", "TRUNCHEON", "TWINAXE", "TWHANDEDSWORD", "TYRANTCLUB", "VULGE", "WARAXE", "WARCLUB", "WARDBW", "WARGAUNTLETS", "WARFORK", "WARHAMMER", "WARHAT", "WARSCEPTER", "WARSCYTHE", "WARSPEAR", "WARSPIKE", "WARSTAFF", "WARSWORD", "WINGEDAXE", "WINGEDHELM", "WINGEDKNIFE", "ZAKARUMSHIELD" };
        readonly List<string> keepSet = new List<string>() { "AMULET", "LACQUEREDPLATE", "SWIRLINGCRYSTAL", "DEATHMASK", "BATTLEBOOTS"};
        //readonly List<string> keepSet = new List<string>() { "AMULET", "LACQUEREDPLATE", "BRAMBLEMITTS", "WINGEDHELM", "BATTLEBOOTS", "CADUCEUS", "VORTEXSHIELD", "CORONA", "SACREDARMOR", "HEAVYBRACERS" };
        int Width = 0;
        int Height = 0;

        private Random rnd = new();

        public List<Item> FindItemsUF(Bitmap bmp, IntPtr D2handle)
        {
            this.Items = new List<Item>();
            this.Width = bmp.Width;
            this.Height = bmp.Height;

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int strideInPixels = data.Stride / 4; //4 bytes per pixel

            var uniquePixels = new List<(int, int)>();
            var setPixels = new List<(int, int)>();
            var rarePixels = new List<(int, int)>();
            var runePixels = new List<(int, int)>();
            var magicalPixels = new List<(int, int)>();
            var whitePixels = new List<(int, int)>();

            for (var j = 200; j < this.Width - 200; j++)
            {
                for (var i = 150; i < this.Height - 300; i++)
                {
                    unsafe
                    {
                        uint* dataPointer = (uint*)data.Scan0;
                        uint* pixelPointer = dataPointer + i * strideInPixels + j;
                        uint color = *pixelPointer;
                        if (colors.IsSpecialColor(color) == null)
                        {
                            continue;
                        }
                        else if(colors.IsSpecialColor(color).Equals("Unique"))
                        {
                            uniquePixels.Add((j, i));
                        }
                        else if (colors.IsSpecialColor(color).Equals("Set"))
                        {
                            setPixels.Add((j, i));
                        }
                        else if (colors.IsSpecialColor(color).Equals("Rare"))
                        {
                            rarePixels.Add((j, i));
                        }
                        else if (colors.IsSpecialColor(color).Equals("Rune"))
                        {
                            runePixels.Add((j, i));
                        }
                        else if (colors.IsSpecialColor(color).Equals("Magical"))
                        {
                            magicalPixels.Add((j, i));
                        }
                        else if (colors.IsSpecialColor(color).Equals("White"))
                        {
                            whitePixels.Add((j, i));
                        }
                    }
                }
            }

            var whites = GetUnionFindItems(UnionFind(whitePixels), whitePixels, "White");
            var uniques = GetUnionFindItems(UnionFind(uniquePixels), uniquePixels, "Unique");
            var rares = GetUnionFindItems(UnionFind(rarePixels), rarePixels, "Rare");
            var sets = GetUnionFindItems(UnionFind(setPixels), setPixels, "Set");
            var runes = GetUnionFindItems(UnionFind(runePixels), runePixels, "Rune");
            var magical = GetUnionFindItems(UnionFind(magicalPixels), magicalPixels, "Magical");

            this.Items = whites.Concat(uniques).Concat(rares).Concat(sets).Concat(runes).Concat(magical).ToList();

            GetItemNames(bmp, this.Items);

            bmp.Save(@"C:\Users\michael\source\repos\PixelBot\IcySorcPindleBot\IcySorcPindleBot\tessdata\ScreenCapture\finditems.png", System.Drawing.Imaging.ImageFormat.Png);

            bmp.Dispose();

            var items = GetKeepItems(D2handle, ShouldConsider(this.Items));

            return items;
        }

        public List<Item> GetUnionFindItems(int[] unions, List<(int, int)> pixels, string itemType)
        {
            var itemDict = new Dictionary<int, Item>();

            for(var i = 0; i < unions.Length; i++)
            {
                if (!itemDict.ContainsKey(Find(unions,i)))
                {
                    itemDict[Find(unions, i)] = new Item(pixels[i].Item2, pixels[i].Item1, itemType);
                }
                else
                {
                    itemDict[Find(unions, i)].ExpandBounds(pixels[i].Item2, pixels[i].Item1);
                }
            }

            var items = new List<Item>();
            foreach(var item in itemDict.Keys)
            {
                items.Add(itemDict[item]);
            }

            return items;
        }

        public int[] UnionFind(List<(int, int)> initialPixels)
        {
            var parents = new int[initialPixels.Count];

            for (var i = 0; i < parents.Length; i++)
            {
                parents[i] = i;
            }

            for(var i = 0; i < initialPixels.Count; i++)
            {
                for(var j = 0; j < initialPixels.Count; j++)
                {
                    if(i != j)
                    {
                        var xDistance = Math.Abs(initialPixels[i].Item1 - initialPixels[j].Item1);
                        var yDistance = Math.Abs(initialPixels[i].Item2 - initialPixels[j].Item2);
                        if (xDistance < 25 && yDistance < 8)
                        {
                            Union(parents, i, j);
                        }
                    }
                }          
            }

            return parents;
        }

        public int Find(int[] parents, int y)
        {
            if(parents[y] != y)
            {
                return Find(parents, parents[y]);
            }

            return y;
        }

        public void Union(int[] parents, int x, int y)
        {
            parents[Find(parents,x)] = Find(parents, y);
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
                var left = item.Left - 5 >= 0 ? item.Left - 5 : item.Left;
                var right = item.Right + 5 < bmp.Width ? item.Right + 5 : item.Right;
                var top = item.Top - 5 >= 0 ? item.Top - 5 : item.Top;
                var bottom = item.Bottom + 5 < bmp.Height ? item.Bottom + 5 : item.Bottom;

                Rectangle rect = new Rectangle(new Point(left, top), new Size(right - left, bottom - top));

                if(rect.Width <= 0 || rect.Height <= 0)
                {
                    return;
                }
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

            if (item.ItemType == "Rune" )
            {
                //&& !runeExclusionList.Contains(item.ItemName)
                return true;
            }

            if (item.ItemType == "Set" )
            {
                //&& CheckAllEditDistance(item.ItemName, keepSet, 3)
                return true;
            }

            if (item.ItemType == "Rare" && CheckAllEditDistance(item.ItemName, keepRares, 1))
            {
                return true;
            }

            if (item.ItemType == "Magical" && CheckAllEditDistance(item.ItemName, keepMagicals, 3) && !MagicalTrashList.Contains(item.ItemName))
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
