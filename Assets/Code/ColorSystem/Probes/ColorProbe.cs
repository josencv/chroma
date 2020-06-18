using UnityEngine;

namespace Chroma.ColorSystem.Probes
{
    public class ColorProbe : MonoBehaviour
    {
        [SerializeField]
        protected Color color = Color.Blue;
        public Color Color { get { return color; } set { color = value; } }
    }
}
