using Chroma.Game.Configuration;
using UnityEngine;

namespace Chroma.ColorSystem
{
    public class Absorber : MonoBehaviour
    {
        [SerializeField]
        private float maxRadius = 5.0f;

        [SerializeField]
        private float growthRate = 3.0f;
        private float elapsedTime;
        private float currentRadius;

        private void Start()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            elapsedTime = 0;
            currentRadius = 0;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            // The radius increases inverse exponentially (square root of the elapsed time)
            // until a maximum radius has been reached
            currentRadius = Mathf.Min(maxRadius, Mathf.Sqrt(elapsedTime * growthRate));
        }

        public void StartAbsorption()
        {
            enabled = true;
        }

        public float ExecuteAbsobption(Color colorToAbsorb)
        {
            float absorbedAmount = 0;
            GameLayers layerMask = GameLayers.ColorProbes;
            // TODO: consider using OverlapSphereNonAlloc if this needs to be called very frequently
            Collider[] colorProbes = Physics.OverlapSphere(transform.position, currentRadius, (int)layerMask);
            foreach(Collider probe in colorProbes)
            {
                absorbedAmount += probe.GetComponent<ColorProbe>().Absorb(colorToAbsorb);
            }
            
            enabled = false;
            return absorbedAmount;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }
    }
}
