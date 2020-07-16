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
        [Tooltip("The cluster size to use to cluster the probes. A lower value means higher density.")]
        protected float clusterSize = DefaultClusterSize;

        [SerializeField]
        [Range(1, 20f)]
        [Tooltip("The mipmap level used for the probe calculation algorithm when the TriangleColorCalculationMode is set to FromTexture. A higher value will be much faster, but may produce a few wrong probe colors.")]
        protected int mipmapLevel = 3;

        [SerializeField]
        [Tooltip("The mode to use when setting the color of a triangle of the mesh. 'FromTexture' means the algorithm will try to calculate it from the texture directly (can be very expensive, so be careful," +
            "as the algorithm is blocks the UI thread). 'SetColor' means the same color will be set for every triangle, configured in the 'triangleManualColor' variable.")]
        TriangleColorCalculationMode triangleColorCalculationMode = TriangleColorCalculationMode.FromTexture;

        [SerializeField]
        [Tooltip("The color to use for the triangles in the probe building algorithm when the mode is set to 'SetColor'.")]
        Color triangleManualColor = Color.Blue;

        public AssetReference ProbeDataRef { get { return probeDataRef; } set { probeDataRef = value; } }
        public float ClusterSize { get { return clusterSize; } }
        public int MipmapLevel { get { return mipmapLevel; } }
        public TriangleColorCalculationMode TriangleColorCalculationMode { get { return triangleColorCalculationMode; } }
        public Color TriangleManualColor { get { return triangleManualColor; } }
    }
}
