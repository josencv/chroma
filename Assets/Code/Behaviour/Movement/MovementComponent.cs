using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Chroma.Behaviour.Movement
{
    [Serializable]
    public struct MovementComponent : IComponentData
    {
        public float3 MovementVector;
        public float Speed;
    }
}
