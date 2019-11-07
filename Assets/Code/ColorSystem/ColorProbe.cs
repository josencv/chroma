using UnityEditor;
using UnityEngine;

namespace Chroma.ColorSystem
{
    public class ColorProbe : MonoBehaviour
    {
        [SerializeField]
        private Color color = Color.Blue;

        private float maxAmount = 1.0f;
        private float currentAmount;

        private void Start()
        {
            currentAmount = maxAmount;
        }

        public float Absorb(Color colorToAbsorb)
        {
            float absorbedAmount = 0;
            if(color == colorToAbsorb)
            {
                absorbedAmount = currentAmount;
                currentAmount = 0;
            }

            return absorbedAmount;
        }

        private void OnDrawGizmos()
        {
            Handles.color = ColorProbeQuadrantSystemDebugger.GetProbeGizmoColor(color, maxAmount);
            Handles.SphereHandleCap(GetHashCode(), transform.position, Quaternion.identity, 0.1f, EventType.Repaint);
        }

        public Color Color { get { return color; } }
    }
}
