namespace IcySorcPindleBot.Helpers
{
    public class ColorHelper
    {
        readonly uint Unique = 0xFFCDBA82;
        readonly uint Set = 0xFF00FC00;
        readonly uint Rune = 0xFFFFB000;
        readonly uint Rare = 0xFFFFFF70;
        readonly uint Magical = 0xFF7979FF;
        readonly uint White = 0xFFF2F2F2;

    public static bool IsItemBorderColor(uint color)
        {
            var checkRed = (color & 0x00FF0000) >> 16 <= 0x05;
            var checkGreen = (color & 0x0000FF00) >> 8 <= 0x05;
            var checkBlue = (color & 0x000000FF) <= 0x05;

            return checkRed && checkGreen && checkBlue;
        }
        public static bool IsWithinBorderColorRange(uint color)
        {
            var isWhite = (color & 0x00FF0000) >> 16 == 0xFF && (color & 0x0000FF00) >> 8 == 0xFF && (color & 0x000000FF) == 0xFF;
            var checkRed = (color & 0x00FF0000) >> 16 <= 0x30;
            var checkGreen = (color & 0x0000FF00) >> 8 <= 0x30;
            var checkBlue = (color & 0x000000FF) <= 0x30;

            return isWhite || (checkRed && checkGreen && checkBlue);
        }

        public static bool IsPortalBorderColor(uint color)
        {
            var checkRed = (color & 0x00FF0000) >> 16 == 0xFF;
            var checkGreen = (color & 0x0000FF00) >> 8 == 0xFF;
            var checkBlue = (color & 0x000000FF) == 0xFF;

            return checkRed && checkGreen && checkBlue;
        }


        public static bool IsMercHealthyColor(uint color)
        {
            var checkRed = (color & 0x00FF0000) >> 16 == 0x00;
            var checkGreen = (color & 0x0000FF00) >> 8 == 0x95;
            var checkBlue = (color & 0x000000FF) == 0x00;

            return checkRed && checkGreen && checkBlue;
        }

        public string IsSpecialColor(uint color)
        {
            if (color == Unique)
            {
                return "Unique";
            }
            else if (color == Set)
            {
                return "Set";
            }
            else if (color == Rune)
            {
                return "Rune";
            }
            else if (color == Rare)
            {
                return "Rare";
            }
            else if (color == Magical)
            {
                return "Magical";
            }
            else if (color == White)
            {
                return "White";
            }

            return null;
        }

        public uint GetColorByItemType(string itemType)
        {
            if (itemType.Equals("Rune"))
            {
                return Rune;
            }
            else if (itemType.Equals("Rare"))
            {
                return Rare;
            }
            else if (itemType.Equals("Unique"))
            {
                return Unique;
            }
            else if (itemType.Equals("Magical"))
            {
                return Magical;
            }
            else if (itemType.Equals("White"))
            {
                return White;
            }
            else if (itemType.Equals("Set"))
            {
                return Set;
            }

            return 0xFF000000;
        }
    }
}
