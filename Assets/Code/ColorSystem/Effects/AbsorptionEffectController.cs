using UnityEngine;

namespace Chroma.ColorSystem.Effects
{
    public class AbsorptionEffectController
    {
        private AbsorptionField field;
        private float currentRadius;
        private Vector3 currentPosition;

        public AbsorptionEffectController(AbsorptionField field)
        {
            this.field = field;
        }

        public void StartEffect(Vector3 startingPosition, float startingRadius)
        {
            field.gameObject.SetActive(true);
            currentPosition = startingPosition;
            currentRadius = startingRadius;
            UpdateFieldScale();
        }

        public void Tick(Vector3 position, float radius)
        {
            currentPosition = position;
            currentRadius = radius;
            UpdateFieldScale();
        }

        public void EndEffect()
        {
            field.gameObject.SetActive(false);
        }

        private void UpdateFieldScale()
        {
            field.transform.localScale = Vector3.one * currentRadius / field.NativeRadius;
            field.transform.position = currentPosition;
        }
    }
}
