using UnityEngine;

namespace Chroma.ColorSystem.Effects
{
    public class AbsorptionField : MonoBehaviour
    {
        /// <summary>
        /// The radius length in unity units when the scale of the sphere is (1, 1, 1)
        /// </summary>
        [SerializeField]
        private float nativeRadiusUnits = 0.5f;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public float NativeRadius { get { return nativeRadiusUnits; } }
    }
}
