using System.Collections.Generic;
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

        [SerializeField]
        private float maxRadius = 5.0f;

        [SerializeField]
        private float growthRate = 3.0f;
        private float elapsedTime;
        private float currentRadius;

        [Inject]
        private void Inject(
            AbsorptionEffectController absroptionEffectController,
            AbsorptionRenderSystem absorptionRenderSystem,
            ColorProbeQuadrantSystem quadrantSystem,
            ColorProbeRecoverySystem recoverySystem
        )
        {
            this.absroptionEffectController = absroptionEffectController;
            this.absorptionRenderSystem = absorptionRenderSystem;
            this.quadrantSystem = quadrantSystem;
            this.recoverySystem = recoverySystem;
        }

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
            absorptionRenderSystem.UpdateCurrentAbsorptionPoint(transform.position, currentRadius);
            absroptionEffectController.Tick(transform.position, currentRadius);
        }

        public void StartAbsorption()
        {
            enabled = true;
            absroptionEffectController.StartEffect(transform.position, currentRadius);
        }

        public float ExecuteAbsobption(Color colorToAbsorb)
        {
            float absorbedAmount = 0;
            List<ColorProbe[]> quadrantsToCheck = quadrantSystem.GetCurrentAndAdjacentQuadrants(transform.position);
            absorptionRenderSystem.AddAbsorptionPoint(transform.position, currentRadius);
            AddRenderDataToDynamiCobjects();

            foreach(ColorProbe[] probes in quadrantsToCheck)
            {
                for(int i = 0; i < probes.Length; i++)
                {
                    if(Vector3.Distance(probes[i].Position, transform.position) <= currentRadius && probes[i].Color == colorToAbsorb)
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

        private void AddRenderDataToDynamiCobjects()
        {
            // TODO: change to OverlapSphereNonAlloc to avoid unnecessary allocation
            Collider[] colliders = Physics.OverlapSphere(transform.position, currentRadius, (int)dynamicObjectsLayer);

            foreach(Collider collider in colliders)
            {
                collider.GetComponent<AbsorbableRenderManager>()?.AddAbsorptionPoint(transform.position, currentRadius);
            }
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
