using System.Collections.Generic;
using Chroma.ColorSystem.Effects;
using UnityEngine;
using Zenject;

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

        private AbsorptionRenderSystem absorptionRenderSystem;
        public Dictionary<Color, ColorInfo> ColorsMap { get; private set; }

        [Inject]
        public void Inject(AbsorptionRenderSystem absorptionRenderSystem)
        {
            this.absorptionRenderSystem = absorptionRenderSystem;
        }

        private void Awake()
        {
            InitializeColorMap();
            absorptionRenderSystem.InitializeUnlockedColorsData(ColorsMap);
        }

        private void InitializeColorMap()
        {
            ColorsMap = new Dictionary<Color, ColorInfo>();
            ColorsMap.Add(Color.Red, new ColorInfo(Color.Red, redUnlocked));
            ColorsMap.Add(Color.Yellow, new ColorInfo(Color.Yellow, yellowUnlocked));
            ColorsMap.Add(Color.Green, new ColorInfo(Color.Green, greenUnlocked));
            ColorsMap.Add(Color.Cyan, new ColorInfo(Color.Cyan, cyanUnlocked));
            ColorsMap.Add(Color.Blue, new ColorInfo(Color.Blue, blueUnlocked));
            ColorsMap.Add(Color.Magenta, new ColorInfo(Color.Magenta, magentaUnlocked));
        }
    }
}
