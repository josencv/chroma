using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDebug : MonoBehaviour
{

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material mat;

    const string RANGE_1 = "_Range_1";
    const string RANGE_2 = "_Range_2";
    const string RANGE_3 = "_Range_3";
    const string RANGE_4 = "_Range_4";
    const string RANGE_5 = "_Range_5";
    const string RANGE_6 = "_Range_6";
    const string RANGE_7 = "_Range_7";

    const string RANGE_1_End = "_Range_1_End";
    const string RANGE_2_End = "_Range_2_End";
    const string RANGE_3_End = "_Range_3_End";
    const string RANGE_4_End = "_Range_4_End";
    const string RANGE_5_End = "_Range_5_End";
    const string RANGE_6_End = "_Range_6_End";
    const string RANGE_7_End = "_Range_7_End";


    void Start()
    {
        mat = meshRenderer.sharedMaterial;
    }

    private void Toggle_Red()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_1);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_1, val);
    }

    private void Toggle_Orange()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_2);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_2, val);
    }

    private void Toggle_Yellow()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_3);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_3, val);
    }

    private void Toggle_Green()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_4);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_4, val);
    }

    private void Toggle_LightBlue()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_5);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_5, val);
    }

    private void Toggle_Blue()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_6);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_6, val);
    }

    private void Toggle_Violet()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(RANGE_7);
        val = val < 1 ? 1 : 0.0001f;
        mat.SetFloat(RANGE_7, val);
    }

#if METHOD_BUTTON && UNITY_EDITOR
    [MethodButton("Toggle_Red", "Toggle_Orange", "Toggle_Yellow", "Toggle_Green", "Toggle_LightBlue", "Toggle_Blue", "Toggle_Violet")]
    [SerializeField] private bool editorFoldout;
#endif
}
