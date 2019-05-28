using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Chroma.Behaviour.WatchAndFollowPlayer
{
    [Serializable]
    public struct WatchAndFollowPlayerComponent : IComponentData
    {
        public float3 FollowOffset;
        public float FollowSpeed;
        public float RotationSpeed;
    }
}
