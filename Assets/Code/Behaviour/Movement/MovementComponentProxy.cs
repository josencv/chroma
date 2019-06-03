using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Chroma.Behaviour.Movement
{
    [RequiresEntityConversion]
    public class MovementComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField]
        private float speed = 3.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new MovementComponent() { MovementVector = float3.zero, Speed = speed };
            dstManager.AddComponentData(entity, data);
        }
    }
}
