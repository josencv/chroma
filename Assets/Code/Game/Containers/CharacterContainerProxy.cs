using Unity.Entities;
using UnityEngine;

namespace Chroma.Game.Containers
{
    public class CharacterContainerProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new CharacterContainer() { };
            dstManager.AddComponentData(entity, data);
        }
    }
}
