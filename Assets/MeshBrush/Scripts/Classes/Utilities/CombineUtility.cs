using UnityEngine;

namespace MeshBrush
{
    public static class CombineUtility
    {
        static int vertexCount = 0;
        static int triangleCount = 0;
        static int stripCount = 0;

        static int curStripCount;

        static Vector3[] vertices;
        static Vector3[] normals;
        static Vector4[] tangents;
        static Vector2[] uv;
        static Vector2[] uv1;
        static Color[] colors;

        static int[] triangles;
        static int[] strip;

        static int offset = 0;

        static int triangleOffset = 0;
        static int stripOffset = 0;
        static int vertexOffset = 0;

        static Matrix4x4 invTranspose;

        public const string combinedMeshName = "Combined Mesh";

        public struct MeshInstance
        {
            public Mesh mesh;
            public int subMeshIndex;
            public Matrix4x4 transform;
        }

        public static Mesh Combine(MeshInstance[] combines, bool generateStrips)
        {
            vertexCount = 0;
            triangleCount = 0;
            stripCount = 0;

            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                {
                    vertexCount += combine.mesh.vertexCount;

                    if (generateStrips)
                    {
                        curStripCount = combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                        if (curStripCount != 0)
                        {
                            if (stripCount != 0)
                            {
                                if ((stripCount & 1) == 1)
                                    stripCount += 3;
                                else
                                    stripCount += 2;
                            }
                            stripCount += curStripCount;
                        }
                        else generateStrips = false;
                    }
                }
            }

            // Precomputed how many triangles we need instead
            if (!generateStrips)
            {
                foreach (MeshInstance combine in combines)
                {
                    if (combine.mesh != null)
                        triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                }
            }

            vertices = new Vector3[vertexCount];
            normals = new Vector3[vertexCount];
            tangents = new Vector4[vertexCount];
            uv = new Vector2[vertexCount];
            uv1 = new Vector2[vertexCount];
            colors = new Color[vertexCount];

            triangles = new int[triangleCount];
            strip = new int[stripCount];

            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                    Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
            }

            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                {
                    invTranspose = combine.transform;
                    invTranspose = invTranspose.inverse.transpose;
                    CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
                }

            }
            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                {
                    invTranspose = combine.transform;
                    invTranspose = invTranspose.inverse.transpose;
                    CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
                }

            }
            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                    Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
            }

            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                    Copy(combine.mesh.vertexCount, combine.mesh.uv2, uv1, ref offset);
            }

            offset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                    CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset);
            }

            triangleOffset = 0;
            stripOffset = 0;
            vertexOffset = 0;
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh != null)
                {
                    if (generateStrips)
                    {
                        int[] inputstrip = combine.mesh.GetTriangles(combine.subMeshIndex);
                        if (stripOffset != 0)
                        {
                            if ((stripOffset & 1) == 1)
                            {
                                strip[stripOffset + 0] = strip[stripOffset - 1];
                                strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                                strip[stripOffset + 2] = inputstrip[0] + vertexOffset;
                                stripOffset += 3;
                            }
                            else
                            {
                                strip[stripOffset + 0] = strip[stripOffset - 1];
                                strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                                stripOffset += 2;
                            }
                        }

                        for (int i = 0; i < inputstrip.Length; i++)
                        {
                            strip[i + stripOffset] = inputstrip[i] + vertexOffset;
                        }
                        stripOffset += inputstrip.Length;
                    }
                    else
                    {
                        int[] inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                        for (int i = 0; i < inputtriangles.Length; i++)
                        {
                            triangles[i + triangleOffset] = inputtriangles[i] + vertexOffset;
                        }
                        triangleOffset += inputtriangles.Length;
                    }

                    vertexOffset += combine.mesh.vertexCount;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = combinedMeshName;
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.colors = colors;
            mesh.uv = uv;
            mesh.uv2 = uv1;
            mesh.tangents = tangents;

            if (generateStrips)
                mesh.SetTriangles(strip, 0);
            else
                mesh.triangles = triangles;

            return mesh;
        }

        static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < src.Length; i++)
                dst[i + offset] = transform.MultiplyPoint(src[i]);
            offset += vertexcount;
        }

        static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < src.Length; i++)
                dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
            offset += vertexcount;
        }

        static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
        {
            for (int i = 0; i < src.Length; i++)
                dst[i + offset] = src[i];
            offset += vertexcount;
        }

        static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
        {
            for (int i = 0; i < src.Length; i++)
                dst[i + offset] = src[i];
            offset += vertexcount;
        }

        static Vector4 p4;
        static Vector3 p;

        static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
        {
            for (int i = 0; i < src.Length; i++)
            {
                p4 = src[i];
                p = new Vector3(p4.x, p4.y, p4.z);
                p = transform.MultiplyVector(p).normalized;
                dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
            }

            offset += vertexcount;
        }
    }
}