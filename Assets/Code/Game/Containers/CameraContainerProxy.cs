using Unity.Entities;
using UnityEngine;

namespace Chroma.Game.Containers
{
    public class CameraContainerProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new CameraContainer() { };
            dstManager.AddComponentData(entity, data);
        }
    }
}
