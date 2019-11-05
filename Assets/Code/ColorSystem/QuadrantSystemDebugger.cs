using Chroma.Game.Containers;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem
{
    public class QuadrantSystemDebugger : MonoBehaviour
    {
        private const float ProbeRadius = 0.1f;
        private QuadrantSystem quadrantSystem;
        private Character character;

        [Inject]
        private void Inject(Character character, QuadrantSystem quadrantSystem)
        {
            this.character = character;
            this.quadrantSystem = quadrantSystem;
        }

        private void OnDrawGizmos()
        {
            if(quadrantSystem != null)
            {
                ColorProbeData[] probes = quadrantSystem.GetProbesFromPosition(character.transform.position);
                foreach(ColorProbeData probe in probes)
                {
                    Handles.color = GetProbeGizmoColor(probe);
                    Handles.SphereHandleCap(probe.GetHashCode(), probe.Position, Quaternion.identity, ProbeRadius, EventType.Repaint);
                }

                Gizmos.color = UnityEngine.Color.white;
                Gizmos.DrawWireCube(QuadrantSystem.GetQuadrantCenterFromPosition(character.transform.position), QuadrantSystem.QuadrandSize * Vector3.one);
            }
        }

        private UnityEngine.Color GetProbeGizmoColor(ColorProbeData probe)
        {
            UnityEngine.Color gizmoColor;

            switch(probe.Color)
            {
                case Color.Red:
                    gizmoColor = new UnityEngine.Color(1, 0, 0, probe.Amount);
                    break;
                case Color.Blue:
                    gizmoColor = new UnityEngine.Color(0, 0, 1, probe.Amount);
                    break;
                case Color.Yellow:
                    gizmoColor = new UnityEngine.Color(1, 1, 0, probe.Amount);
                    break;
                case Color.Green:
                    gizmoColor = new UnityEngine.Color(0, 1, 0, probe.Amount);
                    break;
                case Color.Cyan:
                    gizmoColor = new UnityEngine.Color(0, 1, 1, probe.Amount);
                    break;
                case Color.Magenta:
                    gizmoColor = new UnityEngine.Color(1, 0, 1, probe.Amount);
                    break;
                default:
                    gizmoColor = new UnityEngine.Color(0, 0, 0);
                    break;
            }

            return gizmoColor;
        }
    }
}
