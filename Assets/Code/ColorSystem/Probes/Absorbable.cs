using Chroma.ColorSystem.Probes.Builder;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chroma.ColorSystem.Probes
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public abstract class Absorbable : MonoBehaviour
    {
        private const float DefaultClusterSize = 0.5f;

        [SerializeField]
        protected AssetReference probeDataRef;

        // TODO: move color probe builder configuration to a separate script
        [SerializeField]
        [Range(0.1f, 10f)]
        protected float clusterSize = DefaultClusterSize;

        [SerializeField]
        TriangleColorCalculationMode triangleColorCalculationMode = TriangleColorCalculationMode.FromTexture;

        [SerializeField]
        Color triangleManualColor = Color.Blue;

        public AssetReference ProbeDataRef { get { return probeDataRef; } set { probeDataRef = value; } }
        public float ClusterSize { get { return clusterSize; } }
        public TriangleColorCalculationMode TriangleColorCalculationMode { get { return triangleColorCalculationMode; } }
        public Color TriangleManualColor { get { return triangleManualColor; } }
    }
}
