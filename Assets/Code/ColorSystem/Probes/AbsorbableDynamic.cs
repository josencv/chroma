using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Chroma.ColorSystem.Probes
{
    public class AbsorbableDynamic : Absorbable
    {
        private ColorUnlockSystem colorUnlockSystem;

        private const float ProbeRadius = 0.1f;
        private ColorProbe[] probes;
        private List<int> probesBeingRecoveredIndices;

        [Inject]
        private void Inject(ColorUnlockSystem colorUnlockSystem)
        {
            this.colorUnlockSystem = colorUnlockSystem;
        }

        private void Awake()
        {
            probesBeingRecoveredIndices = new List<int>();
        }

        private IEnumerator Start()
        {
            if(!ProbeDataRef.RuntimeKeyIsValid())
            {
                yield break;
            }

            AsyncOperationHandle<ColorProbeData> handle = probeDataRef.LoadAssetAsync<ColorProbeData>();
            yield return handle;

            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                ColorProbe[] assetProbes = handle.Result.Probes;
                probes = new ColorProbe[assetProbes.Length];

                for(int i = 0; i < assetProbes.Length; i++)
                {
                    Vector3 position = assetProbes[i].Position;
                    probes[i] = new ColorProbe(position, assetProbes[i].Color);
                }

                Addressables.Release(handle);
            }
            else
            {
                throw new ApplicationException("There was a problem loading a color probe asset");
            }
        }

        private void Update()
        {
            RecoverTick(Time.deltaTime);
        }

        public float GetAbsorbed(Vector3 position, float radius, Color colorToAbsorb)
        {
            float absorbedAmount = 0;

            for(int i = 0; i < probes.Length; i++)
            {
                if(colorToAbsorb == probes[i].Color && colorUnlockSystem.IsColorUnlocked(colorToAbsorb) && Vector3.Distance(position, transform.TransformPoint(probes[i].Position)) <= radius)
                {
                    float amount = probes[i].GetAbsorbed();
                    absorbedAmount += amount;

                    // We add the probe to the recovery list only if it was full, because otherwise it means it's already
                    // in the list
                    if(amount == 1)
                    {
                        probesBeingRecoveredIndices.Add(i);
                    }
                }
            }

            return absorbedAmount;
        }

        private void RecoverTick(float deltaTime)
        {
            for(int i = probesBeingRecoveredIndices.Count - 1; i >= 0; i--)
            {
                int index= probesBeingRecoveredIndices[i];
                float amount = probes[index].Recover(deltaTime * ColorConstants.ProbesRecoverySpeed);
                if(amount == 1)
                {
                    probesBeingRecoveredIndices.RemoveAt(i);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if(probes != null)
            {
                for(int i = 0; i < probes.Length; i++)
                {
                    Vector3 position = transform.TransformPoint(probes[i].Position);
                    Handles.color = GetProbeGizmoColor(probes[i].Color, probes[i].Amount);
                    Handles.SphereHandleCap(probes[i].GetHashCode(), position, Quaternion.identity, ProbeRadius, EventType.Repaint);
                }
            }
        }

        public UnityEngine.Color GetProbeGizmoColor(Color color, float amount)
        {
            UnityEngine.Color gizmoColor;

            switch(color)
            {
                case Color.Red:
                    gizmoColor = new UnityEngine.Color(1, 0, 0, amount);
                    break;
                case Color.Blue:
                    gizmoColor = new UnityEngine.Color(0, 0, 1, amount);
                    break;
                case Color.Yellow:
                    gizmoColor = new UnityEngine.Color(1, 1, 0, amount);
                    break;
                case Color.Green:
                    gizmoColor = new UnityEngine.Color(0, 1, 0, amount);
                    break;
                case Color.Cyan:
                    gizmoColor = new UnityEngine.Color(0, 1, 1, amount);
                    break;
                case Color.Magenta:
                    gizmoColor = new UnityEngine.Color(1, 0, 1, amount);
                    break;
                default:
                    gizmoColor = new UnityEngine.Color(0, 0, 0);
                    break;
            }

            return gizmoColor;
        }
    }
}
