﻿using Unity.Entities;
using UnityEngine;

namespace Chroma.Behaviour.WatchTarget
{
    [RequiresEntityConversion]
    public class WatchTargetComponentProxy : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var data = new WatchTargetComponent() {};
            dstManager.AddComponentData(entity, data);
        }
    }
}
