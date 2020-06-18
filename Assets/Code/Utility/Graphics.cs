using UnityEngine;

namespace Chroma.Utility
{
    public static class GraphicsUtil
    {
        /// <summary>
        /// Copies a texture into memory so it's readable.
        /// Taken from https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D CopyTexture(Texture2D texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D textureCopy = new Texture2D(texture.width, texture.height);
            textureCopy.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            textureCopy.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return textureCopy;
        }
    }
}
