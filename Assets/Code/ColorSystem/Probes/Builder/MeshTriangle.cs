using UnityEngine;

namespace Chroma.ColorSystem.Probes.Builder
{
    public class MeshTriangle
    {
        public Vector3[] Vertices { get; private set; }
        public Vector2[] UVs { get; private set; }
        public int[] VertexIndices { get; private set; }

        public MeshTriangle(Vector3[] vertices, Vector2[] uvs, int[] vertexIndices)
        {
            Vertices = vertices;
            UVs = uvs;
            VertexIndices = vertexIndices;
        }
    }
}
