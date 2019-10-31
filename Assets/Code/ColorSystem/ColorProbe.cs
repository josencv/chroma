using UnityEngine;

namespace Chroma.ColorSystem
{
    public class ColorProbe : MonoBehaviour
    {
        [SerializeField]
        private Color color = Color.Blue;
        [SerializeField]
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
    }
}
