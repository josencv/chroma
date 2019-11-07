using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem
{
    /// <summary>
    /// This class iterates over all probes added to the recovery list each frame
    /// and it recovers an amount of color equal to Time.deltaTime * RecoverySpeed.
    /// After the probe is fully recovered, it is removed from the list.
    /// </summary>
    public class ColorProbeRecoverySystem : MonoBehaviour
    {
        private const float RecoverySpeed = 0.1f;
        private ColorProbeQuadrantSystem quadrantSystem;
        private List<HashIndexPair> probesHashIndexList;

        private void Awake()
        {
            probesHashIndexList = new List<HashIndexPair>();
        }

        [Inject]
        private void Inject(ColorProbeQuadrantSystem quadrantSystem)
        {
            this.quadrantSystem = quadrantSystem;
        }

        private void Update()
        {
            for(int i = probesHashIndexList.Count - 1; i >= 0; i--)
            {
                HashIndexPair pair = probesHashIndexList[i];
                float amount = quadrantSystem.GetQuadrantFromHash(pair.Hash)[pair.Index].Recover(Time.deltaTime * RecoverySpeed);
                if(amount == 1)
                {
                    probesHashIndexList.RemoveAt(i);
                }
            }
        }

        public void AddProbeHashIndexPair(int hash, int index)
        {
            probesHashIndexList.Add(new HashIndexPair(hash, index));
        }
    }
}
