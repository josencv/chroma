using System;
using Unity.Entities;

namespace Chroma.Behaviour.WatchTarget
{
    [Serializable]
    public struct WatchTargetComponent : IComponentData
    {
        public Entity Target;
    }
}
