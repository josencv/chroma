using UnityEngine;

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
    }
}
