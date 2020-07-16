using System.Collections.Generic;
using UnityEngine;

namespace Chroma.ColorSystem
{
    public class ColorUnlockSystem : MonoBehaviour
    {
        [SerializeField]  private bool redUnlocked = false;
        [SerializeField]  private bool yellowUnlocked = false;
        [SerializeField]  private bool greenUnlocked = false;
        [SerializeField]  private bool cyanUnlocked = false;
        [SerializeField]  private bool blueUnlocked = false;
        [SerializeField]  private bool magentaUnlocked = false;

        private Dictionary<Color, ColorInfo> colorsMap;

        private void Awake()
        {
            InitializeColorMap();
        }

        public bool IsColorUnlocked(Color color)
        {
            return colorsMap[color].Unlocked;
        }

        public ColorInfo GetColorInfo(Color color)
        {
            return colorsMap[color];
        }

        private void InitializeColorMap()
        {
            colorsMap = new Dictionary<Color, ColorInfo>();
            colorsMap.Add(Color.Red, new ColorInfo(Color.Red, redUnlocked));
            colorsMap.Add(Color.Yellow, new ColorInfo(Color.Yellow, yellowUnlocked));
            colorsMap.Add(Color.Green, new ColorInfo(Color.Green, greenUnlocked));
            colorsMap.Add(Color.Cyan, new ColorInfo(Color.Cyan, cyanUnlocked));
            colorsMap.Add(Color.Blue, new ColorInfo(Color.Blue, blueUnlocked));
            colorsMap.Add(Color.Magenta, new ColorInfo(Color.Magenta, magentaUnlocked));
        }
    }
}
