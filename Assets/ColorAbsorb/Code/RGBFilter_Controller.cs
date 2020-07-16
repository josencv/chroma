using UnityEngine;
using System.Collections;

public class RGBFilter_Controller : MonoBehaviour
{
    public static float someTime = 0.75f;
    public static int amount = 100;

    [SerializeField, Range(0, 1)] private int _red = 1;
    [SerializeField, Range(0, 1)] private int _green = 1;
    [SerializeField, Range(0, 1)] private int _blue = 1;

    [SerializeField] private Material[] mats;

    private void Start()
    {
        StartCoroutine(UpdateColor(amount));
    }

    private IEnumerator UpdateColor(int amount)
    {
        int count = 0;
        while(count < amount)
        {
            UpdateAllColors();
            count++;
            yield return new WaitForSeconds(someTime);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            UpdateAllColors();
        }
    }

    private void UpdateAllColors()
    {
        AbsorbableTest[] absorbers = FindObjectsOfType<AbsorbableTest>();
        mats = new Material[absorbers.Length];

        int i = 0;
        foreach(AbsorbableTest item in absorbers)
        {
            mats[i] = item.GetComponent<MeshRenderer>().material;
            i++;
        }

        _red = Random.Range(0, 2);
        _green = Random.Range(0, 2);
        _blue = Random.Range(0, 2);

        //Debug.Log($"Red {_red}, Green {_green}, Blue {_blue}");
        foreach(Material item in mats)
        {
            SetColor(item, _red, "_red");
            SetColor(item, _blue, "_blue");
            SetColor(item, _green, "_green");
        }
    }

    private void SetColor(Material mat, float value, string attr)
    {
        mat.SetFloat(attr, value);
    }
}
