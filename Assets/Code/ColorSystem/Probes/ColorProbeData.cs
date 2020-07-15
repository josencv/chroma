using UnityEngine;

namespace Chroma.ColorSystem.Probes
{
    public class ColorProbeData : ScriptableObject
    {
        [SerializeField]
        private ColorProbe[] probes;

        public ColorProbe[] Probes { get { return probes; } set { probes = value; } }
    }
}
