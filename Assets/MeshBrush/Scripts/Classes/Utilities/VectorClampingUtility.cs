using UnityEngine;

namespace MeshBrush
{
    /// <summary>
    /// Utility that provides simple clamping functionality for vectors.
    /// </summary>
    public static class VectorClampingUtility
    {
        public static void ClampVector(ref Vector2 vector, float minX, float maxX, float minY, float maxY)
        {
            vector.x = Mathf.Clamp(vector.x, minX, maxX);
            vector.y = Mathf.Clamp(vector.y, minY, maxY);
        }

        public static void ClampVector(ref Vector3 vector, float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            vector.x = Mathf.Clamp(vector.x, minX, maxX);
            vector.y = Mathf.Clamp(vector.y, minY, maxY);
            vector.z = Mathf.Clamp(vector.z, minZ, maxZ);
        }

        public static void ClampVector(ref Vector4 vector, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, float minW, float maxW)
        {
            vector.x = Mathf.Clamp(vector.x, minX, maxX);
            vector.y = Mathf.Clamp(vector.y, minY, maxY);
            vector.z = Mathf.Clamp(vector.z, minZ, maxZ);
            vector.w = Mathf.Clamp(vector.w, minW, maxW);
        }
    }
}

// Copyright (C) Raphael Beck, 2017