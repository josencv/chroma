using System;
using Unity.Entities;

namespace Chroma.Behaviour.Movement
{
    [Serializable]
    public struct MovementComponent : IComponentData
    {
        public float HorizontalValue;
        public float VerticalValue;
        public float Speed;
    }
}
