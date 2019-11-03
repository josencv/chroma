using Unity.Entities;
using UnityEngine;

namespace Chroma.ColorSystem.ECS
{
    [RequiresEntityConversion]
    public class PointColorProbeProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private Color color;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PointColorProbeComponent probe = new PointColorProbeComponent()
            {
                Color = color,
            };

            dstManager.AddComponentData(entity, probe);
        }
    }
}
