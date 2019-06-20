using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCubesInCircle : MonoBehaviour
{
    public GameObject prefab;
    public Transform player;

    [Range(0.0001f, 100f)] public float radius = 20f;
    public float speed = 10f;


    public int numObjects = 10;
    public int numCircles = 20;
    float angle = 360;
    float step;

    void Start()
    {
        for(int i = 1; i <= numCircles; i++)
        {
            Vector3 center = transform.position;
            step = angle / (numObjects * i);
            angle = 0;

            for(int j = 0; j < numObjects * i; j++)
            {
                Vector3 pos = RandomCircle(center, 1.5f * i, angle);
                Quaternion rot = Quaternion.identity;// Quaternion.FromToRotation(Vector3.up, center - pos);
                Instantiate(prefab, pos, rot);
                angle += step;
            }
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
        if(Input.GetKey(KeyCode.R))
        {
            radius += speed * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.S))
        {
            radius -= speed * Time.deltaTime;
        }
        radius = Mathf.Clamp(radius, 0.0001f, 100f);

        Shader.SetGlobalFloat("_Range", radius);
        Shader.SetGlobalVector("_PlayerPos", player.position);
    }
}
