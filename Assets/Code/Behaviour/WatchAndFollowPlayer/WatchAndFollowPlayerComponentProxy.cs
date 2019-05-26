using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Chroma.Behaviour.WatchAndFollowPlayer
{
    [RequiresEntityConversion]
    public class WatchAndFollowPlayerComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private float3 followOffset = new float3(0, 1, -10);

        [SerializeField]
        private float followSpeed = 1.5f;

        [SerializeField]
        private float rotationSpeed = 1.5f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new WatchAndFollowPlayerComponent() {
                FollowOffset = followOffset,
                FollowSpeed = followSpeed,
                RotationSpeed = rotationSpeed,
            };
            dstManager.AddComponentData(entity, data);
        }
    }
}
