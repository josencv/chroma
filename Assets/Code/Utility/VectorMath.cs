﻿using UnityEngine;

namespace Chroma.Utility
{
    public static class VectorMath
    {
        /// <summary>
        /// Transforms a given vector to the given space, without considering the inclination (the Y axis), aka: the floor plane.
        /// </summary>
        /// <param name="targetSpace">The space in which the vector will be converted</param>
        /// <param name="vector">The vector to transform</param>
        /// <returns>The transformed vector</returns>
        public static Vector3 TransformToSpacePlane(Transform targetSpace, Vector3 vector)
        {
            vector = targetSpace.TransformDirection(vector);
            var rotation = Quaternion.AngleAxis(-targetSpace.eulerAngles.x, targetSpace.right);
            return rotation * vector;
        }

        public static Vector3 CalculateTriangleCenter(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return (v1 + v2 + v3) / 3;
        }

        public static float CalculateTriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return Vector3.Cross(v1 - v2, v1 - v3).magnitude * 0.5f;
        }
    }
}
