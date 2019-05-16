using Unity.Entities;
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
            var data = new MovementComponent() { HorizontalValue = 0, VerticalValue = 0, Speed = speed };
            dstManager.AddComponentData(entity, data);
        }
    }
}
