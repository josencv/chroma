using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDebug : MonoBehaviour
{

    [SerializeField] private MeshRenderer meshRenderer;
    Material mat;

    const string RANGE_1 = "_Range_1";
    const string RANGE_2 = "_Range_2";
    const string RANGE_3 = "_Range_3";
    const string RANGE_4 = "_Range_4";
    const string RANGE_5 = "_Range_5";
    const string RANGE_6 = "_Range_6";
    const string RANGE_7 = "_Range_7";

    void Start()
    {
        mat = meshRenderer.sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void Toggle_Range_1()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_1);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_1, val);
    }

    private void Toggle_Range_2()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_2);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_2, val);
    }

    private void Toggle_Range_3()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_3);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_3, val);
    }

    private void Toggle_Range_4()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_4);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_4, val);
    }

    private void Toggle_Range_5()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_5);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_5, val);
    }

    private void Toggle_Range_6()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_6);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_6, val);
    }

    private void Toggle_Range_7()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_7);
        val = val < 1 ? 1 : 0;
        mat.SetFloat(RANGE_7, val);
    }

#if METHOD_BUTTON && UNITY_EDITOR
    [MethodButton("Toggle_Range_1", "Toggle_Range_2", "Toggle_Range_3", "Toggle_Range_4", "Toggle_Range_5", "Toggle_Range_6", "Toggle_Range_7")]
    [SerializeField] private bool editorFoldout;
#endif
}
