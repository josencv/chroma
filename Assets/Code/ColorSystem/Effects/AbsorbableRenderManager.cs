using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem.Effects
{
    [RequireComponent(typeof(Renderer))]
    public class AbsorbableRenderManager : MonoBehaviour
    {
        private new Renderer renderer;

        private Texture2D absorptionDataTexture;
        private List<AbsorptionPointRenderData> absorptionDataList;

        [Inject]
        private void Inject(Renderer renderer)
        {
            this.renderer = renderer;
        }

        private void Awake()
        {
            absorptionDataList = new List<AbsorptionPointRenderData>();
        }

        private void Start()
        {
            absorptionDataTexture = new Texture2D(8, 8, TextureFormat.RGBAFloat, false);
            renderer.material.SetTexture("_AbsorptionDataInstanced", absorptionDataTexture);
        }

        private void Update()
        {
            UpdateSaturationValues();
            UpdateAbsorbableDataTexture();
            UpdateShaderAbsorptionDataLengthProperty();
        }

        public void AddAbsorptionPoint(Vector3 position, float radius)
        {
            position = transform.InverseTransformPoint(position);
            AbsorptionPointRenderData absorptionData = new AbsorptionPointRenderData(position, radius);
            absorptionDataList.Add(absorptionData);
        }

        private void UpdateSaturationValues()
        {
            for(int i = absorptionDataList.Count - 1; i >= 0; i--)
            {
                absorptionDataList[i].Saturation += ColorConstants.ProbesRecoverySpeed * Time.deltaTime;
                if(absorptionDataList[i].Saturation >= 1)
                {
                    absorptionDataList.Remove(absorptionDataList[i]);
                }
            }
        }

        private void UpdateShaderAbsorptionDataLengthProperty()
        {
            renderer.material.SetInt("_AbsorptionDataInstancedLength", absorptionDataList.Count);
        }

        private void UpdateAbsorbableDataTexture()
        {
            for(int i = 0; i < absorptionDataList.Count; i++)
            {
                AbsorptionPointRenderData data = absorptionDataList[i];
                SetAbsorptionPointPixelData(data, i);
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
    }
}
