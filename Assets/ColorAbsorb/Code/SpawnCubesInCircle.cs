using UnityEngine;
using System.Collections;
using Chroma.Infrastructure.Input;

namespace Chroma.Tests
{
    public class SpawnCubesInCircle : MonoBehaviour
    {
        #region Felids

        [Header("Scene Obejcts")]
        [SerializeField] private Transform player;
        [SerializeField] private MaterialDebug[] materialDebugs;

        [Header("Configure Spawned Cubes")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int levelDensity = 6;
        [SerializeField] private int totalLevels = 60;
        private float angle = 360f;

        [Header("Configure affected radius")]
        [SerializeField] private float radialGrowthSpeed = 20f;
        private float radius = 5f;
        
        #endregion

        #region Unity

        private void Start()
        {
            //materialDebugs = FindObjectsOfType<MaterialDebug>();
            LogInstuction(GameInputButton.B, "Red");
            LogInstuction(GameInputButton.DPadUp, "Orange");
            LogInstuction(GameInputButton.Y, "Yellow");
            LogInstuction(GameInputButton.A, "Green");
            LogInstuction(GameInputButton.L1, "Cyan");
            LogInstuction(GameInputButton.X, "Blue");
            LogInstuction(GameInputButton.R1, "Purple");
            StartCoroutine(SpawnCubes());
        }

        private void Update()
        {
            AdjustRadius();
            ToggleColors();
        }

        #endregion

        #region Spawning Cubes

        private IEnumerator SpawnCubes()
        {
            for(int i = 1; i <= totalLevels; i++)
            {
                Vector3 center = transform.position;
                float step = angle / (levelDensity * i);
                angle = 0;

                int count = 0;
                for(int j = 0; j < levelDensity * i; j++)
                {
                    count++;
                    Vector3 pos = GetPointOnCircle(center, 1.5f * i, angle);
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, center - pos);
                    Instantiate(prefab, pos, rot, transform);
                    angle += step;

                    if(count > 100)
                    {
                        count = 0;
                        yield return null;
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private Vector3 GetPointOnCircle(Vector3 center, float radius, float ang)
        {
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            return pos;
        }

        #endregion

        #region Color Absorb Control

        private void AdjustRadius()
        {
            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.R2) == GameInputButtonState.Pressed)
            {
                radius += radialGrowthSpeed * Time.deltaTime;
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.L2) == GameInputButtonState.Pressed)
            {
                radius -= radialGrowthSpeed * Time.deltaTime;
            }

            radius = Mathf.Clamp(radius, 0.0001f, 100f);

            Shader.SetGlobalFloat("_Range", radius);
            Shader.SetGlobalVector("_PlayerPos", player.position);
        }

        private void ToggleColors()
        {
            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.B) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Red();
                }
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.DPadUp) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Orange();
                }
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.Y) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Yellow();
                }
            }
            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.A) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Green();
                }
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.L1) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_LightBlue();
                }
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.X) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Blue();
                }
            }

            if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.R1) == GameInputButtonState.Down)
            {
                for(int i = 0; i < materialDebugs.Length; i++)
                {
                    materialDebugs[i].Toggle_Violet();
                }
            }
        }

        #endregion

        #region Logs

        private void LogInstuction(GameInputButton button, string color)
        {
            Debug.Log($"Press <b>{button}</b> for <b><color={color}>{color}</color></b>");
        }

        #endregion
    }
}
