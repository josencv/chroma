using UnityEngine;

namespace Chroma.Utility
{
    public static class MaterialUtil
    {
        public static Color GetMaterialTint(Material material)
        {
            Color color;
            if(material.HasProperty("_MainCol"))
            {
                color = material.GetColor("_MainCol");
            }
            else if(material.HasProperty("_Tint"))
            {
                color = material.GetColor("_Tint");
            }
            else
            {
                color = Color.white;
            }

            return color;
        }
    }
}
