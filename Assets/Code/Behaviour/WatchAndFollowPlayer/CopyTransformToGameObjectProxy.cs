using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Chroma.Behaviour.WatchAndFollowPlayer
{
    public class CopyTransformToGameObjectProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CopyTransformToGameObject());
        }
    }
}
