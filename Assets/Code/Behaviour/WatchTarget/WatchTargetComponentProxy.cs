using Unity.Entities;
using UnityEngine;

namespace Chroma.Behaviour.WatchTarget
{
    [RequiresEntityConversion]
    public class WatchTargetComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private Entity target;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new WatchTargetComponent() { Target = target };
            dstManager.AddComponentData(entity, data);
        }
    }
}
