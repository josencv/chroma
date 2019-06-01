using UnityEngine;

namespace MeshBrush
{
    /// <summary>
    /// Mesh transformation utility useful for randomizations that require scaling, rotating and/or offsetting existing meshes.
    /// </summary>
    public static class MeshTransformationUtility
    {
        /// <summary>
        /// Applies random scale to a <see cref="Transform"/> uniformly and within a defined range.
        /// </summary>
        /// <param name="targetTransform">Target transform to randomize.</param>
        /// <param name="range">Scale range (a random number will be picked between range.x and range.y).</param>
        public static void ApplyRandomScale(Transform targetTransform, Vector2 range)
        {
            float scaleValue = Mathf.Abs(Random.Range(range.x, range.y));
            targetTransform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }

        /// <summary>
        /// Applies random scale to a <see cref="Transform"/> non-uniformly (width and height based) within defined ranges.
        /// </summary>
        /// <param name="targetTransform">Target transform to which the random scale should be applied.</param>
        /// <param name="scaleRanges">Scale ranges (x and y are the min/max width; z and w are the min/max height).</param>
        public static void ApplyRandomScale(Transform targetTransform, Vector4 scaleRanges)
        {
            float width = Random.Range(scaleRanges.x, scaleRanges.y);
            float height = Random.Range(scaleRanges.z, scaleRanges.w);

            targetTransform.localScale = new Vector3
            {
                x = Mathf.Abs(width),
                y = Mathf.Abs(height),
                z = Mathf.Abs(width)
            };
        }

        /// <summary>
        /// Applies random scale to a <see cref="Transform"/> non-uniformly and within defined ranges (for each scale axis individually).
        /// </summary>
        /// <param name="targetTransform">Target transform to which the scale modification should be applied.</param>
        /// <param name="rangeX">Minimum/maximum scale along the local x-axis.</param>
        /// <param name="rangeY">Minimum/maximum scale along the local y-axis.</param>
        /// <param name="rangeZ">Minimum/maximum scale along the local z-axis.</param>
        public static void ApplyRandomScale(Transform targetTransform, Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
        {
            targetTransform.localScale = new Vector3
            {
                x = Mathf.Abs(Random.Range(rangeX.x, rangeX.y)),
                y = Mathf.Abs(Random.Range(rangeY.x, rangeY.y)),
                z = Mathf.Abs(Random.Range(rangeZ.x, rangeZ.y))
            };
        }

        /// <summary>
        /// Adds a constant scale additively and uniformly to a target <see cref="Transform"/>.
        /// </summary>
        /// <param name="targetTransform">Target transform that should be scaled.</param>
        /// <param name="range">The min/max amount to add to the scale (uniformly across x, y and z).</param>
        public static void AddConstantScale(Transform targetTransform, Vector2 range)
        {
            float randomValue = Random.Range(range.x, range.y);

            Vector3 newScale = targetTransform.localScale + new Vector3(randomValue, randomValue, randomValue);
            newScale.x = Mathf.Abs(newScale.x); newScale.y = Mathf.Abs(newScale.y); newScale.z = Mathf.Abs(newScale.z);

            targetTransform.localScale = newScale;
        }

        /// <summary>
        /// Adds a constant scale additively and non-uniformly to a target <see cref="Transform"/>.
        /// </summary>
        /// <param name="targetTransform">Target transform that should be scaled.</param>
        /// <param name="x">The amount of scale to add to the local x axis.</param>
        /// <param name="y">The amount of scale to add to the local y axis.</param>
        /// <param name="z">The amount of scale to add to the local z axis.</param>
        public static void AddConstantScale(Transform targetTransform, float x, float y, float z)
        {
            Vector3 a = targetTransform.localScale + new Vector3
            {
                x = Mathf.Abs(x),
                y = Mathf.Abs(y),
                z = Mathf.Abs(z)
            };
            targetTransform.localScale = a;
        }

        /// <summary>
        /// Apply some random rotation (around the local X, Y and Z axes) to a <see cref="Transform"/>.
        /// </summary>
        /// <param name="targetTransform">Target transform to randomize.</param>
        /// <param name="randomRotationIntensityPercentageX">How intense should the applied random rotation to the X-Axis be? [0% - 100%]</param>
        /// <param name="randomRotationIntensityPercentageY">How intense should the applied random rotation to the Y-Axis be? [0% - 100%]</param>
        /// <param name="randomRotationIntensityPercentageZ">How intense should the applied random rotation to the Z-Axis be? [0% - 100%]</param>
        public static void ApplyRandomRotation(Transform targetTransform, float randomRotationIntensityPercentageX, float randomRotationIntensityPercentageY, float randomRotationIntensityPercentageZ)
        {
            float x = Random.Range(0.0f, 3.60f * randomRotationIntensityPercentageX);
            float y = Random.Range(0.0f, 3.60f * randomRotationIntensityPercentageY);
            float z = Random.Range(0.0f, 3.60f * randomRotationIntensityPercentageZ);
            targetTransform.Rotate(new Vector3(x, y, z));
        }

        /// <summary>
        /// Applies the mesh offset (along the local Y axis).
        /// </summary>
        /// <param name="targetTransform">Target transform to offset.</param>
        /// <param name="offset">Offset (in centimeters).</param>
        /// <param name="direction">Offset direction (in world space).</param>
        public static void ApplyMeshOffset(Transform targetTransform, float offset, Vector3 direction)
        {
            // We divide the offset by 100 since we want to use centimeters as our offset unit (because 1cm = 0.01m).
            targetTransform.Translate((direction.normalized * offset * 0.01f), Space.World);
        }
    }
}

// Copyright (C) Raphael Beck, 2017