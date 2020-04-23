using System.Collections.Generic;
using Chroma.ColorSystem.Effects;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem
{
    public class AbsorptionRenderSystem : MonoBehaviour
    {
        private Texture2D absorptionDataTexture;
        private List<AbsorptionPointRenderData> absorptionDataList;
        private AbsorptionPointRenderData currentAbsorptionPoint;

        [Inject]
        private void Inject(ColorSelector colorSelector)
        {
            colorSelector.ColorChanged += UpdateEffectsColor;
            UpdateEffectsColor(colorSelector.SelectedColor);
        }

        private void Awake()
        {
            absorptionDataTexture = new Texture2D(32, 32, TextureFormat.RGBAFloat, false);
            absorptionDataList = new List<AbsorptionPointRenderData>();
        }

        private void Start()
        {
            Shader.SetGlobalTexture("_AbsorptionData", absorptionDataTexture);
        }

        private void Update()
        {
            UpdateSaturationValues();
            UpdateAbsorbableDataTexture();
            UpdateShaderAbsorptionDataLengthProperty();
        }

        public void InitializeUnlockedColorsData(Dictionary<Color, ColorInfo> colorsMap)
        {
            foreach(KeyValuePair<Color, ColorInfo> pair in colorsMap)
            {
                UpdateColorDisabled(pair.Value);
            }
        }

        public void UpdateCurrentAbsorptionPoint(Vector3 position, float radius)
        {
            if(currentAbsorptionPoint == null)
            {
                currentAbsorptionPoint = new AbsorptionPointRenderData(position, radius);
            } 
            else
            {
                currentAbsorptionPoint.Position = position;
                currentAbsorptionPoint.Radius = radius;
            }
        }

        public void AddAbsorptionPoint(Vector3 position, float radius)
        {
            AbsorptionPointRenderData absorptionData = new AbsorptionPointRenderData(position, radius);
            absorptionDataList.Add(absorptionData);
        }

        public void ReleaseCurrentAbsorptionPoint()
        {
            currentAbsorptionPoint = null;
        }

        private void UpdateColorDisabled(ColorInfo colorInfo)
        {
            float colorDisabled = colorInfo.Unlocked ? 0.0f : 1.0f;
            UnityEngine.Color pixelData = new UnityEngine.Color(colorDisabled, 0, 0, 0);
            absorptionDataTexture.SetPixel((int)colorInfo.Color, 31, pixelData);
        }

        private void UpdateSaturationValues()
        {
            for (int i = absorptionDataList.Count - 1; i >= 0; i--)
            {
                absorptionDataList[i].Saturation += ColorProbeRecoverySystem.RecoverySpeed * Time.deltaTime;
                if(absorptionDataList[i].Saturation >= 1)
                {
                    absorptionDataList.Remove(absorptionDataList[i]);
                }
            }
        }

        private void UpdateShaderAbsorptionDataLengthProperty()
        {
            int length = currentAbsorptionPoint != null ? absorptionDataList.Count + 1 : absorptionDataList.Count;
            Shader.SetGlobalInt("_AbsorptionDataLength", length);
        }

        private void UpdateAbsorbableDataTexture()
        {
            for (int i = 0; i < absorptionDataList.Count; i++)
            {
                AbsorptionPointRenderData data = absorptionDataList[i];
                SetAbsorptionPointPixelData(data, i);
            }

            if(currentAbsorptionPoint != null)
            {
                SetAbsorptionPointPixelData(currentAbsorptionPoint, absorptionDataList.Count);
            }

            absorptionDataTexture.Apply();
        }

        private void SetAbsorptionPointPixelData(AbsorptionPointRenderData data, int index)
        {
            UnityEngine.Color positionColor = new UnityEngine.Color(data.Position.x, data.Position.y, data.Position.z, 0);
            UnityEngine.Color extraDataColor = new UnityEngine.Color(data.Radius, data.Saturation, 0, 0);
            absorptionDataTexture.SetPixel(index, 0, positionColor);
            absorptionDataTexture.SetPixel(index, 1, extraDataColor);
        }

        private void UpdateEffectsColor(Color color)
        {
            Shader.SetGlobalColor("AbsorptionField_EmissionColor", EffectsTheme.GetAbsorptionFieldColor(color));
        }
    }
}
