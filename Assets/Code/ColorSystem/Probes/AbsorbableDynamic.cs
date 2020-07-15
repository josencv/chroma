using UnityEngine.ResourceManagement.AsyncOperations;

namespace Chroma.ColorSystem.Probes
{
    public class AbsorbableDynamic : Absorbable
    {
        private ColorProbe[] probes;

        private void Awake()
        {
            AsyncOperationHandle<ColorProbeData> probeDataHandle = probeDataRef.LoadAssetAsync<ColorProbeData>();
        }
    }
}
