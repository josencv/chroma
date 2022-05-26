﻿using System.Collections.Generic;
using Chroma.ColorSystem;
using Chroma.ColorSystem.Effects;
using Chroma.ColorSystem.Probes;
using Chroma.Game.Configuration;
using UnityEngine;
using Zenject;
using Color = Chroma.ColorSystem.Color;

namespace Chroma.Behaviour.Skills
{
    public class Absorber : MonoBehaviour
    {
        private const GameLayers dynamicObjectsLayer = GameLayers.Objects;

        private AbsorptionEffectController absroptionEffectController;
        private AbsorptionRenderSystem absorptionRenderSystem;
        private ColorProbeQuadrantSystem quadrantSystem;
        private ColorProbeRecoverySystem recoverySystem;
        private ColorUnlockSystem colorUnlockSystem;

        [SerializeField]
        private float maxRadius = 5.0f;

        [SerializeField]
        private float growthRate = 3.0f;
        private float elapsedTime;
        private float currentRadius;
        private Color currentColor;

        [Inject]
        private void Inject(
            AbsorptionEffectController absroptionEffectController,
            AbsorptionRenderSystem absorptionRenderSystem,
            ColorProbeQuadrantSystem quadrantSystem,
            ColorProbeRecoverySystem recoverySystem,
            ColorUnlockSystem colorUnlockSystem
        )
        {
            this.absroptionEffectController = absroptionEffectController;
            this.absorptionRenderSystem = absorptionRenderSystem;
            this.quadrantSystem = quadrantSystem;
            this.recoverySystem = recoverySystem;
            this.colorUnlockSystem = colorUnlockSystem;
        }

        private void Start()
        {
            enabled = false;
            // TODO: define a default color somewhere
            currentColor = Color.Blue;
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
            absorptionRenderSystem.UpdateCurrentAbsorptionPoint(transform.position, currentRadius);
            absroptionEffectController.Tick(transform.position, currentRadius);
        }

        /// <summary>
        /// Tries to change the color to be used by the absorber
        /// </summary>
        /// <param name="color"></param>
        /// <returns>True if the operation was succesful, false otherwise</returns>
        public bool TryChangeColor(Color color)
        {
            if(enabled)
            {
                return false;
            }

            currentColor = color;
            return true;
        }

        public void StartAbsorption()
        {
            enabled = true;
            absroptionEffectController.StartEffect(transform.position, currentRadius);
        }

        public float ExecuteAbsobption()
        {
            float absorbedAmount = 0;
            List<ColorProbe[]> quadrantsToCheck = quadrantSystem.GetCurrentAndAdjacentQuadrants(transform.position);
            absorptionRenderSystem.AddAbsorptionPoint(transform.position, currentRadius);
            absorbedAmount += AbsorbFromDynamicObjects(currentColor);

            foreach(ColorProbe[] probes in quadrantsToCheck)
            {
                for(int i = 0; i < probes.Length; i++)
                {
                    if(probes[i].Color == currentColor && colorUnlockSystem.IsColorUnlocked(currentColor) && Vector3.Distance(probes[i].Position, transform.position) <= currentRadius)
                    {
                        float amount = probes[i].GetAbsorbed();
                        absorbedAmount += amount;

                        // We add the probe to the recovery system only if it was full, because otherwise it means it's already
                        // in the recovery system
                        if(amount == 1)
                        {
                            recoverySystem.AddProbeHashIndexPair(ColorProbeQuadrantSystem.GetQuadrantHash(probes[i].Position), i);
                        }
                    }
                }
            }
            
            absorptionRenderSystem.ReleaseCurrentAbsorptionPoint();
            absroptionEffectController.EndEffect();
            enabled = false;
            return absorbedAmount;
        }

        private float AbsorbFromDynamicObjects(Color colorToAbsorb)
        {
            float amount = 0;
            // TODO: change to OverlapSphereNonAlloc to avoid unnecessary allocation
            Collider[] dynamicObjectsColliders = Physics.OverlapSphere(transform.position, currentRadius, (int)dynamicObjectsLayer);

            foreach(Collider collider in dynamicObjectsColliders)
            {
                AbsorbableDynamic absorbable = collider.GetComponent<AbsorbableDynamic>();
                if (absorbable != null)
                {
                    amount += absorbable.GetAbsorbed(transform.position, currentRadius, colorToAbsorb);
                }

                collider.GetComponent<AbsorbableRenderManager>()?.AddAbsorptionPoint(transform.position, currentRadius);
            }

            return amount;
        }

        private void OnDrawGizmos()
        {
            if(enabled)
            {
                Gizmos.color = UnityEngine.Color.cyan;
                Gizmos.DrawWireSphere(transform.position, currentRadius);
            }
        }
    }
}
