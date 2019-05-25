using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Chroma.Behaviour.WatchAndFollowPlayer
{
    [RequiresEntityConversion]
    public class WatchAndFollowPlayerComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new WatchAndFollowPlayerComponent() {
                FollowOffset = new float3(0, 1, -10),
                FollowSpeed = 1.5f,
                RotationSpeed = 1.5f,
            };
            dstManager.AddComponentData(entity, data);
        }
    }
}
