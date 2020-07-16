using UnityEngine;

namespace Chroma.Utility
{
    public static class Algorithms
    {
        public static void Swap<T>(ref T vo, ref T v1)
        {
            T aux = vo;
            vo = v1;
            v1 = aux;
        }
    }
}
