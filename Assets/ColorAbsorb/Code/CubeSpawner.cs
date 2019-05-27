using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int amount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn(ColorAbsorber.amount));
    }

    private IEnumerator Spawn(int amount)
    {
        int count = 0;
        while(count < amount)
        {
            Quaternion randRot = Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            GameObject go = Instantiate(prefab, transform.position, randRot, this.transform);
            go.name = "Cube: " + count;
            count++;
            yield return new WaitForSeconds(ColorAbsorber.someTime);
        }
    }
}
