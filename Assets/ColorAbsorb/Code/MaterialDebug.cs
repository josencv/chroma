using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDebug : MonoBehaviour
{

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material mat;
    [Space()]
    [Range(0, 360), SerializeField] private int RedRangeEnd = 10;
    [SerializeField] private Color RedRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool RedEnabled = true;
    [Range(0, 360), SerializeField] private int OrangeRangeEnd = 40;
    [SerializeField] private Color OrangeRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool OrangeEnabled = true;
    [Range(0, 360), SerializeField] private int YellowRangeEnd = 90;
    [SerializeField] private Color YellowRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool YellowEnabled = true;
    [Range(0, 360), SerializeField] private int GreenRangeEnd = 160;
    [SerializeField] private Color GreenRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool GreenEnabled = true;
    [Range(0, 360), SerializeField] private int LightBlueRangeEnd = 230;
    [SerializeField] private Color LightBlueRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool LighBlueEnabled = true;
    [Range(0, 360), SerializeField] private int BlueRangeEnd = 270;
    [SerializeField] private Color BlueRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool BlueEndabled = true;
    [Range(0, 360), SerializeField] private int VioletRangeEnd = 330;
    [SerializeField] private Color VioletRangeEndColor = new Color(1,1,1,1);
    [SerializeField] private bool VioletEnabled = true;

    #region Constants
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
    #endregion

    private void Start()
    {
        mat = meshRenderer.sharedMaterial;
    }

    private void OnValidate()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        SetAllColorsFromRange();

        SetRange();
    }

    private void SetRange()
    {
        OrangeRangeEnd = OrangeRangeEnd < RedRangeEnd ? RedRangeEnd : OrangeRangeEnd;
        YellowRangeEnd = YellowRangeEnd < OrangeRangeEnd ? OrangeRangeEnd : YellowRangeEnd;
        GreenRangeEnd = GreenRangeEnd < YellowRangeEnd ? YellowRangeEnd : GreenRangeEnd;
        LightBlueRangeEnd = LightBlueRangeEnd < GreenRangeEnd ? GreenRangeEnd : LightBlueRangeEnd;
        BlueRangeEnd = BlueRangeEnd < LightBlueRangeEnd ? LightBlueRangeEnd : BlueRangeEnd;
        VioletRangeEnd = VioletRangeEnd < BlueRangeEnd ? BlueRangeEnd : VioletRangeEnd;

        mat.SetFloat(RANGE_1_End, RedRangeEnd);
        mat.SetFloat(RANGE_2_End, OrangeRangeEnd);
        mat.SetFloat(RANGE_3_End, YellowRangeEnd);
        mat.SetFloat(RANGE_4_End, GreenRangeEnd);
        mat.SetFloat(RANGE_5_End, LightBlueRangeEnd);
        mat.SetFloat(RANGE_6_End, BlueRangeEnd);
    }

    private void SetRangeFromColor()
    {
        float H, S, V;
        Color.RGBToHSV(RedRangeEndColor, out H, out S, out V);
        RedRangeEnd = (int)(H * 360);
    }

    private void SetAllColorsFromRange()
    {
        SetColorFromRange(ref RedRangeEndColor, RedRangeEnd);
        SetColorFromRange(ref OrangeRangeEndColor, OrangeRangeEnd);
        SetColorFromRange(ref YellowRangeEndColor, YellowRangeEnd);
        SetColorFromRange(ref GreenRangeEndColor, GreenRangeEnd);
        SetColorFromRange(ref LightBlueRangeEndColor, LightBlueRangeEnd);
        SetColorFromRange(ref BlueRangeEndColor, BlueRangeEnd);
        SetColorFromRange(ref VioletRangeEndColor, VioletRangeEnd);
    }

    private void SetColorFromRange(ref Color c, int range)
    {
        c = Color.HSVToRGB(range / 360f, 1, 1);
    }

    private void Toggle_Color(string rangeName, ref bool enabled)
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(rangeName);
        val = val < 1 ? 1 : 0.0001f;
        enabled = val == 1;
        mat.SetFloat(rangeName, val);
    }

    #region Toggle Colors
    private void Toggle_Red()
    {
        Toggle_Color(RANGE_1, ref RedEnabled);
    }

    private void Toggle_Orange()
    {
        Toggle_Color(RANGE_2, ref OrangeEnabled);
    }

    private void Toggle_Yellow()
    {
        Toggle_Color(RANGE_3, ref YellowEnabled);
    }

    private void Toggle_Green()
    {
        Toggle_Color(RANGE_4, ref GreenEnabled);
    }

    private void Toggle_LightBlue()
    {
        Toggle_Color(RANGE_5, ref LighBlueEnabled);
    }

    private void Toggle_Blue()
    {
        Toggle_Color(RANGE_6, ref BlueEndabled);
    }

    private void Toggle_Violet()
    {
        Toggle_Color(RANGE_7, ref VioletEnabled);
    }
    #endregion

    #region Debug

    private void GetCurrentValues()
    {
        RedRangeEnd = (int)mat.GetFloat(RANGE_1_End);
        OrangeRangeEnd = (int)mat.GetFloat(RANGE_2_End);
        YellowRangeEnd = (int)mat.GetFloat(RANGE_3_End);
        GreenRangeEnd = (int)mat.GetFloat(RANGE_4_End);
        LightBlueRangeEnd = (int)mat.GetFloat(RANGE_5_End);
        BlueRangeEnd = (int)mat.GetFloat(RANGE_6_End);
        VioletRangeEnd = (int)mat.GetFloat(RANGE_7_End);
    }
    private void SetDefualts()
    {
        RedRangeEnd = 10;
        OrangeRangeEnd = 40;
        YellowRangeEnd = 90;
        GreenRangeEnd = 160;
        LightBlueRangeEnd = 230;
        BlueRangeEnd = 270;
        VioletRangeEnd = 330;

        mat.SetFloat(RANGE_1_End, RedRangeEnd);
        mat.SetFloat(RANGE_2_End, OrangeRangeEnd);
        mat.SetFloat(RANGE_3_End, YellowRangeEnd);
        mat.SetFloat(RANGE_4_End, GreenRangeEnd);
        mat.SetFloat(RANGE_5_End, LightBlueRangeEnd);
        mat.SetFloat(RANGE_6_End, BlueRangeEnd);

        SetAllColorsFromRange();
    }
    #endregion
 
    #region Custom Inspector
#if METHOD_BUTTON && UNITY_EDITOR
    [MethodButton("Toggle_Red",
        "Toggle_Orange",
        "Toggle_Yellow",
        "Toggle_Green",
        "Toggle_LightBlue",
        "Toggle_Blue",
        "Toggle_Violet",
        "GetCurrentValues",
        "SetDefualts")]
    [SerializeField] private bool editorFoldout;
#endif
    #endregion
}
