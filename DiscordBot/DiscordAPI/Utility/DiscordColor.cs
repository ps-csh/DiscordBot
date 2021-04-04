using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DiscordBot.DiscordAPI.Utility
{
    public static class DiscordColor
    {
        public static int ToInteger(Color color)
        {
            int rgb = color.R;
            rgb = (rgb << 8) + color.G;
            rgb = (rgb << 8) + color.B;
            return rgb;
        }

        public static Color FromInteger(int colorInt)
        {
            byte r = (byte)(colorInt >> 16 & 255);
            byte g = (byte)((colorInt >> 8) & 255);
            byte b = (byte)((colorInt) & 255);
            return Color.FromArgb(r, g, b);
        }
    }
}
