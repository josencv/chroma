using System.Collections.Generic;

namespace Chroma.ColorSystem.Effects
{
    public static class EffectsTheme
    {
        private static Dictionary<Color, UnityEngine.Color> colorMap = new Dictionary<Color, UnityEngine.Color>() 
        {
            { Color.Red, UnityEngine.Color.red },
            { Color.Yellow, UnityEngine.Color.yellow },
            { Color.Green, UnityEngine.Color.green },
            { Color.Cyan, UnityEngine.Color.cyan },
            { Color.Blue, UnityEngine.Color.blue },
            { Color.Magenta, UnityEngine.Color.magenta },
        };

        public static UnityEngine.Color GetAbsorptionFieldColor(Color color)
        {
            if(colorMap.ContainsKey(color))
            {
                return colorMap[color];
            }
            else
            {
                return UnityEngine.Color.white;
            }
        }
    }
}
