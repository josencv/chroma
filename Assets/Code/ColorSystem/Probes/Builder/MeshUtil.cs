using System.Linq;
using Chroma.Utility;
using UnityEngine;

namespace Chroma.ColorSystem.Probes.Builder
{
    public static class MeshUtil
    {
        public static Color GetTriangleAverageColor(Vector2 v0, Vector2 v1, Vector2 v2, UnityEngine.Color[] pixels, int textureWidth, int textureHeight, TextureWrapMode wrapMode, UnityEngine.Color tint)
        {
            int[] colorsCount;

            if(wrapMode == TextureWrapMode.Clamp)
            {
                v0.x = Mathf.Clamp(v0.x, 0, 1);
                v0.y = Mathf.Clamp(v0.y, 0, 1);
                v1.x = Mathf.Clamp(v1.x, 0, 1);
                v1.y = Mathf.Clamp(v1.y, 0, 1);
                v2.x = Mathf.Clamp(v2.x, 0, 1);
                v2.y = Mathf.Clamp(v2.y, 0, 1);
            }
            else // For now we assume "Repeat", later we cam add support for "Mirror" and "MirrorOnce" wrap modes
            {
                // What we do here is to try to downsize the triangle if large enough, because we have a repeated pattern,
                // in which case we just need to check the smallest triangle that holds the texture at least one time
                // (which I think it's guaranteed to happen for a triangle with all sides at least 2 times the texture... could be wrong tho)
                float scaleFactor1 = (v0 - v1).magnitude / 2;
                float scaleFactor2 = (v1 - v2).magnitude / 2;
                float scaleFactor3 = (v2 - v0).magnitude / 2;

                float minScale = Mathf.Min(scaleFactor1, scaleFactor2, scaleFactor3);

                // The case where the scale is less than one means that one of the sides wasn't big enough to be able to downsize
                // to a triangle that is guaranteed to have the texture at least once
                if (minScale > 1)
                {
                    v0 = v0 / minScale;
                    v1 = v1 / minScale;
                    v2 = v2 / minScale;
                }
            }

            Vector2 textureSpace = new Vector2(textureWidth, textureHeight);

            v0.Scale(textureSpace);
            v1.Scale(textureSpace);
            v2.Scale(textureSpace);

            // Sort vertices by y component from top to bottom, using Direct3D convention
            if(v1.y < v0.y)
            {
                Algorithms.Swap(ref v0, ref v1);
            }

            if(v2.y < v1.y)
            {
                Algorithms.Swap(ref v1, ref v2);
            }

            if(v1.y < v0.y)
            {
                Algorithms.Swap(ref v0, ref v1);
            }

            if(v0.y == v1.y) // natural flat top triangle
            {
                // Sort top vertices from left to right
                if(v1.x < v0.x)
                {
                    Algorithms.Swap(ref v0, ref v1);
                }
                colorsCount = CalculateFlatTopTrianglePixelsColorCount(v0, v1, v2, pixels, textureWidth, textureHeight, tint);
            }
            else if(v1.y == v2.y) // natural flat bottom triangle
            {
                // Sort bottom vertices from left to right
                if(v2.x < v1.x)
                {
                    Algorithms.Swap(ref v1, ref v2);
                }
                colorsCount = CalculateFlatBottomTrianglePixelsColorCount(v0, v1, v2, pixels, textureWidth, textureHeight, tint);
            }
            else // general triangle
            {
                int[] colorsCountAux;
                // Split the triangle in one flat top and one flat bottom triangle:
                // Find the new vertex to split the triangle in two flat triangles
                Vector2 split = FindFlatSplittingVertex(v0, v1, v2);
                if(v1.x < split.x)
                {
                    // Count major right case
                    colorsCount = CalculateFlatBottomTrianglePixelsColorCount(v0, v1, split, pixels, textureWidth, textureHeight, tint);
                    colorsCountAux = CalculateFlatTopTrianglePixelsColorCount(v1, split, v2, pixels, textureWidth, textureHeight, tint);
                }
                else
                {
                    // Count major left case
                    colorsCount = CalculateFlatBottomTrianglePixelsColorCount(v0, split, v1, pixels, textureWidth, textureHeight, tint);
                    colorsCountAux = CalculateFlatTopTrianglePixelsColorCount(split, v1, v2, pixels, textureWidth, textureHeight, tint);
                }


                for(int i = 0; i < ColorConstants.ColorTypesCount; i++)
                {
                    colorsCount[i] += colorsCountAux[i];
                }
            }

            int maxValue = colorsCount.Max();
            int maxIndex = colorsCount.ToList().IndexOf(maxValue);

            return (Color)maxIndex;
        }

        /// <summary>
        /// Returns the vertex that divides the triangle in two flat triangles
        /// </summary>
        /// <param name="v0">Topmost uv vertex of the triangle</param>
        /// <param name="v1">Middle uv vertex of the triangle</param>
        /// <param name="v2">Bottom-most uv vertex of the triangle</param>
        /// <returns>The uv vertex that splits the triangle in two flat triangles</returns>
        private static Vector2 FindFlatSplittingVertex(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            // Linear interpolation
            float alpha = (v1.y - v0.y) / (v2.y - v0.y);
            return v0 + (v2 - v0) * alpha;
        }

        private static int[] CalculateFlatTopTrianglePixelsColorCount(Vector2 v0, Vector2 v1, Vector2 v2, UnityEngine.Color[] pixels, int textureWidth, int textureHeight, UnityEngine.Color tint)
        {
            int[] colorsCount = new int[ColorConstants.ColorTypesCount];

            float m0 = (v2.x - v0.x) / (v2.y - v0.y);
            float m1 = (v2.x - v1.x) / (v2.y - v1.y);

            // Calculate start and end scanlines (using top-left rasterization rule)
            int yStart = Mathf.CeilToInt(v0.y - 0.5f);
            int yEnd = Mathf.CeilToInt(v2.y - 0.5f); // The scanline AFTER the last line drawn

            if(yStart == yEnd)
            {
                // Hack to sample even in the case when we have degenerate UV triangles
                yEnd++;
            }

            for(int y = yStart; y < yEnd; y++)
            {
                // Calculate start and end points in x axis, adding 0.5 to y value because
                // we are calculating based on pixel centers
                float px0 = (float)(m0 * ((float)y + 0.5 - v0.y) + v0.x);
                float px1 = (float)(m1 * ((float)y + 0.5 - v1.y) + v1.x);

                int xStart = Mathf.CeilToInt(px0 - 0.5f);
                int xEnd = Mathf.CeilToInt(px1 - 0.5f);

                if(xStart == xEnd)
                {
                    // Hack to sample even in the case when we have degenerate UV triangles
                    xEnd++;
                }

                for(int x = xStart; x < xEnd; x++)
                {
                    int pixelX;
                    int pixelY;
                    if(x < 0)
                    {
                        // Note: the mod will return a negative result, so we add
                        pixelX = textureWidth + (x % textureWidth) - 1;
                    }
                    else
                    {
                        pixelX = x % textureWidth;
                    }

                    if(y < 0)
                    {
                        // Note: the mod will return a negative result, so we add
                        pixelY = textureHeight + (y % textureHeight) - 1;
                    }
                    else
                    {
                        pixelY = y % textureHeight;
                    }

                    UnityEngine.Color pixel = pixels[pixelY * textureWidth + pixelX]; // TODO: invert y
                    Color? color = GetColorFromPixel(pixel, tint);
                    if(color != null)
                    {
                        colorsCount[(int)color]++;
                    }
                }
            }

            return colorsCount;
        }

        private static int[] CalculateFlatBottomTrianglePixelsColorCount(Vector2 v0, Vector2 v1, Vector2 v2, UnityEngine.Color[] pixels, int textureWidth, int textureHeight, UnityEngine.Color tint)
        {
            int[] colorsCount = new int[ColorConstants.ColorTypesCount];

            float m0 = (v1.x - v0.x) / (v1.y - v0.y);
            float m1 = (v2.x - v0.x) / (v2.y - v0.y);

            // Calculate start and end scanlines (using top-left rasterization rule)
            int yStart = Mathf.CeilToInt(v0.y - 0.5f);
            int yEnd = Mathf.CeilToInt(v2.y - 0.5f); // The scanline AFTER the last line drawn

            if (yStart == yEnd)
            {
                // Hack to sample even in the case when we have degenerate UV triangles
                yEnd++;
            }

            for(int y = yStart; y < yEnd; y++)
            {
                // Calculate start and end points in x axis, adding 0.5 to y value because
                // we are calculating based on pixel centers
                float px0 = (float)(m0 * ((float)y + 0.5 - v0.y) + v0.x);
                float px1 = (float)(m1 * ((float)y + 0.5 - v0.y) + v0.x);

                int xStart = Mathf.CeilToInt(px0 - 0.5f);
                int xEnd = Mathf.CeilToInt(px1 - 0.5f);

                if(xStart == xEnd)
                {
                    // Hack to sample even in the case when we have degenerate UV triangles
                    xEnd++;
                }

                for(int x = xStart; x < xEnd; x++)
                {
                    int pixelX;
                    int pixelY;
                    if(x < 0)
                    {
                        // Note: the mod will return a negative result, so we add
                        pixelX = textureWidth + (x % textureWidth) - 1;
                    }
                    else
                    {
                        pixelX = x % textureWidth;
                    }

                    if(y < 0)
                    {
                        // Note: the mod will return a negative result, so we add
                        pixelY = textureHeight + (y % textureHeight) - 1;
                    }
                    else
                    {
                        pixelY = y % textureHeight;
                    }

                    UnityEngine.Color pixel = pixels[pixelY * textureWidth + pixelX]; // TODO: invert y
                    Color? color = GetColorFromPixel(pixel, tint);
                    if(color != null)
                    {
                        colorsCount[(int)color]++;
                    }
                }
            }

            return colorsCount;
        }

        private static Color? GetColorFromPixel(UnityEngine.Color pixel, UnityEngine.Color tint)
        {
            // TODO: handle white and black pixel color separately
            Color color;
            Vector3 hsv = new Vector3();
            UnityEngine.Color.RGBToHSV(pixel * tint, out hsv.x, out hsv.y, out hsv.z);

            if(hsv.y < 0.01)
            {
                return null;
            }

            if(hsv.x >= ColorConstants.YellowHueStart && hsv.x < ColorConstants.GreenHueStart)
            {
                color = Color.Yellow;
            }
            else if(hsv.x >= ColorConstants.GreenHueStart && hsv.x < ColorConstants.CyanHueStart)
            {
                color = Color.Green;
            }
            else if(hsv.x >= ColorConstants.CyanHueStart && hsv.x < ColorConstants.BlueHueStart)
            {
                color = Color.Cyan;
            }
            else if(hsv.x >= ColorConstants.BlueHueStart && hsv.x < ColorConstants.MagentaHueStart)
            {
                color = Color.Blue;
            }
            else if(hsv.x >= ColorConstants.MagentaHueStart || hsv.x < ColorConstants.RedHueStart)
            {
                color = Color.Magenta;
            }
            else
            {
                color = Color.Red;
            }

            return color;
        }
    }
}
