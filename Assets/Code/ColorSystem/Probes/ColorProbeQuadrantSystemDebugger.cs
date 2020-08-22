using System.Collections.Generic;
using Chroma.Game.Containers;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem.Probes
{
    /// <summary>
    /// Helper class that draws the probes of the quadrants near the player see <see cref="ColorProbeQuadrantSystem"/>
    /// The probes are drawn using the probe color with an transparency inverse to the amount of the current color of the probe
    /// (so a probe with no color left fully transparent while a fully charged probe is totally opaque)
    /// </summary>
    public class ColorProbeQuadrantSystemDebugger : MonoBehaviour
    {
        private const float ProbeRadius = 0.1f;
        private ColorProbeQuadrantSystem quadrantSystem;
        private Character character;

        [Inject]
        private void Inject(Character character, ColorProbeQuadrantSystem quadrantSystem)
        {
            this.character = character;
            this.quadrantSystem = quadrantSystem;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(quadrantSystem != null)
            {
                List<ColorProbe[]> quadrants = quadrantSystem.GetCurrentAndAdjacentQuadrants(character.transform.position);
                foreach(ColorProbe[] quadrantProbes in quadrants)
                {
                    for(int i = 0; i < quadrantProbes.Length; i++)
                    {
                        Handles.color = GetProbeGizmoColor(quadrantProbes[i].Color, quadrantProbes[i].Amount);
                        Handles.SphereHandleCap(quadrantProbes[i].GetHashCode(), quadrantProbes[i].Position, Quaternion.identity, ProbeRadius, EventType.Repaint);
                    }
                }

                Gizmos.color = UnityEngine.Color.white;
                foreach(Vector3 center in ColorProbeQuadrantSystem.GetCurrentAndAdjacentQuadrantsCenterFromPosition(character.transform.position))
                {
                    Gizmos.DrawWireCube(center, ColorProbeQuadrantSystem.QuadrantSize * Vector3.one);
                }
            }
        }

        public static UnityEngine.Color GetProbeGizmoColor(Color color, float amount)
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
#endif
    }
}
