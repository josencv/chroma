using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chroma.Infrastructure.Input;

public class SpawnCubesInCircle : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform player;

    [SerializeField] private float radialGrowthSpeed = 20f;
    private float radius = 5f;

    [SerializeField] private int cubeDensity = 6;
    [SerializeField] private int totalLevels = 60;
    private float angle = 360f;
    private float step;

    public MaterialDebug materialDebug;

    private void Start()
    {
        LogInstuction(GameInputButton.B, "Red");
        LogInstuction(GameInputButton.DPadUp, "Orange --Not Working");
        LogInstuction(GameInputButton.Y, "Yellow");
        LogInstuction(GameInputButton.A, "Green");
        LogInstuction(GameInputButton.L1, "Cyan");
        LogInstuction(GameInputButton.X, "Blue");
        LogInstuction(GameInputButton.R1, "Purple");
        StartCoroutine(SpawnCubes());
    }


    private IEnumerator SpawnCubes()
    {
        for(int i = 1; i <= totalLevels; i++)
        {
            Vector3 center = transform.position;
            step = angle / (cubeDensity * i);
            angle = 0;

            int count = 0;
            for(int j = 0; j < cubeDensity * i; j++)
            {
                count++;
                Vector3 pos = RandomCircle(center, 1.5f * i, angle);
                Quaternion rot =  Quaternion.FromToRotation(Vector3.up, center - pos);
                Instantiate(prefab, pos, rot, transform);
                angle += step;

                if(count > 100)
                {
                    count = 0;
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }


    private void Update()
    {
        //GameInputButtonState didClickR1 = InputManager.CurrentGameInput.GetButtonState(GameInputButton.R1);
        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.R2) == GameInputButtonState.Pressed)
        {
            radius += radialGrowthSpeed * Time.deltaTime;
        }
        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.L2) == GameInputButtonState.Pressed)
        {
            radius -= radialGrowthSpeed * Time.deltaTime;
        }

        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.B) == GameInputButtonState.Down)
        {

            materialDebug.Toggle_Red();
        }

        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.DPadUp) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_Orange();
        }

        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.Y) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_Yellow();
        }
        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.A) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_Green();
        }

        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.L1) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_LightBlue();
        }

        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.X) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_Blue();
        }
        
        if(InputManager.CurrentGameInput.GetButtonState(GameInputButton.R1) == GameInputButtonState.Down)
        {
            materialDebug.Toggle_Violet();
        }


        radius = Mathf.Clamp(radius, 0.0001f, 100f);

        Shader.SetGlobalFloat("_Range", radius);
        Shader.SetGlobalVector("_PlayerPos", player.position);
    }

    private void LogInstuction(GameInputButton button, string color)
    {
        Debug.Log($"Press <b>{button}</b> for <b><color={color}>{color}</color></b>");
    }

}
