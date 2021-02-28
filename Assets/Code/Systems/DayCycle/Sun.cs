using UnityEngine;

namespace Chroma.Systems.DayCycle
{
    public class Sun : MonoBehaviour
    {
        private const float hoursPerDay = 24.0f;

        [SerializeField]
        [Range(5, 2500)]
        private float cyclePeriodInSeconds = 360.0f;

        [SerializeField]
        [Range(0, hoursPerDay)]
        private float timeOfDay = 12.0f;

        private void Update()
        {
            timeOfDay += Time.deltaTime / cyclePeriodInSeconds * hoursPerDay;
            if (timeOfDay >= hoursPerDay)
            {
                timeOfDay = timeOfDay % hoursPerDay;
            }

            UpdateSunPosition();
        }

        private void UpdateSunPosition()
        {
            transform.rotation = Quaternion.Euler(Mathf.Lerp(-90, 270, timeOfDay / hoursPerDay), transform.rotation.y, transform.rotation.z);
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateSunPosition();
        }
#endif
    }
}
