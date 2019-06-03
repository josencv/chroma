using UnityEngine;

public class MaterialDebug : MonoBehaviour
{
    #region Constants
    const string RED_1 = "_Range_1";
    const string ORANGE = "_Range_2";
    const string YELLOW = "_Range_3";
    const string GREEN = "_Range_4";
    const string LIGHTBLUE = "_Range_5";
    const string BLUE = "_Range_6";
    const string VIOLET = "_Range_7";

    const string RED_RANGE_END = "_Range_1_End";
    const string ORANGE_RANGE_END = "_Range_2_End";
    const string YELLOW_RANGE_END = "_Range_3_End";
    const string GREEN_RANGE_END = "_Range_4_End";
    const string LIGHT_BLUE_RANGE_END = "_Range_5_End";
    const string BLUE_RANGE_END = "_Range_6_End";
    const string VIOLET_RANGE_END = "_Range_7_End";
    #endregion

    #region Properties
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material mat;
    [Space()]
    [SerializeField] private Texture2D MainTex;

    [Header("Red"), Tooltip("Red goes from 0 to Red Range End, and Violet Range End to 360")]
    [SerializeField] private bool redEnabled = true;
    [Range(0, 360), SerializeField] private int redRangeEnd = 10;
    [SerializeField] private Color redRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float redSaturation = 1f;

    [Header("Orange"), Tooltip("Orange goes from Red Range End to Orange Range End")]
    [SerializeField] private bool orangeEnabled = true;
    [Range(0, 360), SerializeField] private int orangeRangeEnd = 40;
    [SerializeField] private Color orangeRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float orangeSaturation = 1f;

    [Header("Yellow"), Tooltip("Yellow goes from Orange Range End to Yellow Range End")]
    [SerializeField] private bool yellowEnabled = true;
    [Range(0, 360), SerializeField] private int yellowRangeEnd = 90;
    [SerializeField] private Color yellowRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float yellowSaturation = 1f;

    [Header("Green"), Tooltip("Green goes from Yellow Range End to Green Range End")]
    [SerializeField] private bool greenEnabled = true;
    [Range(0, 360), SerializeField] private int greenRangeEnd = 160;
    [SerializeField] private Color greenRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float greenSaturation = 1f;

    [Header("Light Blue"), Tooltip("Light Blue goes from Green Range End to Light Blue Range End")]
    [SerializeField] private bool lighBlueEnabled = true;
    [Range(0, 360), SerializeField] private int lightBlueRangeEnd = 230;
    [SerializeField] private Color lightBlueRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float lightBlueSaturation = 1f;

    [Header("Blue"), Tooltip("Blue goes from Light Blue Range End to Blue Range End")]
    [SerializeField] private bool blueEndabled = true;
    [Range(0, 360), SerializeField] private int blueRangeEnd = 270;
    [SerializeField] private Color blueRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float blueSaturation = 1f;

    [Header("Violet"), Tooltip("Violet goes from Blue Range End to Violet Range End")]
    [SerializeField] private bool violetEnabled = true;
    [Range(0, 360), SerializeField] private int violetRangeEnd = 330;
    [SerializeField] private Color violetRangeEndColor = new Color(1,1,1,1);
    [Range(0.0001f, 1f), SerializeField] private float violetSaturation = 1f;

    #endregion

    private void OnValidate()
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        mat.SetTexture("_MainTex", MainTex);

        SetAllSaturation();
        SetAllColorsFromRange();
        SetAllRanges();
    }

    #region Methods
    private void SetAllColorsFromRange()
    {
        SetColorFromRange(ref redRangeEndColor, redRangeEnd);
        SetColorFromRange(ref orangeRangeEndColor, orangeRangeEnd);
        SetColorFromRange(ref yellowRangeEndColor, yellowRangeEnd);
        SetColorFromRange(ref greenRangeEndColor, greenRangeEnd);
        SetColorFromRange(ref lightBlueRangeEndColor, lightBlueRangeEnd);
        SetColorFromRange(ref blueRangeEndColor, blueRangeEnd);
        SetColorFromRange(ref violetRangeEndColor, violetRangeEnd);
    }

    private void SetColorFromRange(ref Color c, int range)
    {
        c = Color.HSVToRGB(range / 360f, 1, 1);
    }

    private void SetAllRanges()
    {
        orangeRangeEnd = orangeRangeEnd < redRangeEnd ? redRangeEnd : orangeRangeEnd;
        yellowRangeEnd = yellowRangeEnd < orangeRangeEnd ? orangeRangeEnd : yellowRangeEnd;
        greenRangeEnd = greenRangeEnd < yellowRangeEnd ? yellowRangeEnd : greenRangeEnd;
        lightBlueRangeEnd = lightBlueRangeEnd < greenRangeEnd ? greenRangeEnd : lightBlueRangeEnd;
        blueRangeEnd = blueRangeEnd < lightBlueRangeEnd ? lightBlueRangeEnd : blueRangeEnd;
        violetRangeEnd = violetRangeEnd < blueRangeEnd ? blueRangeEnd : violetRangeEnd;

        mat.SetFloat(RED_RANGE_END, redRangeEnd);
        mat.SetFloat(ORANGE_RANGE_END, orangeRangeEnd);
        mat.SetFloat(YELLOW_RANGE_END, yellowRangeEnd);
        mat.SetFloat(GREEN_RANGE_END, greenRangeEnd);
        mat.SetFloat(LIGHT_BLUE_RANGE_END, lightBlueRangeEnd);
        mat.SetFloat(BLUE_RANGE_END, blueRangeEnd);
    }
    
    private void SetAllSaturation()
    {
        mat.SetFloat(RED_1, redSaturation);
        mat.SetFloat(ORANGE, orangeSaturation);
        mat.SetFloat(YELLOW, yellowSaturation);
        mat.SetFloat(GREEN, greenSaturation);
        mat.SetFloat(LIGHTBLUE, lightBlueSaturation);
        mat.SetFloat(BLUE, blueSaturation);
        mat.SetFloat(VIOLET, violetSaturation);
    }
    #endregion

    #region Toggle Colors Buttons
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

    private void Toggle_Red()
    {
        Toggle_Color(RED_1, ref redEnabled);
    }

    private void Toggle_Orange()
    {
        Toggle_Color(ORANGE, ref orangeEnabled);
    }

    private void Toggle_Yellow()
    {
        Toggle_Color(YELLOW, ref yellowEnabled);
    }

    private void Toggle_Green()
    {
        Toggle_Color(GREEN, ref greenEnabled);
    }

    private void Toggle_LightBlue()
    {
        Toggle_Color(LIGHTBLUE, ref lighBlueEnabled);
    }

    private void Toggle_Blue()
    {
        Toggle_Color(BLUE, ref blueEndabled);
    }

    private void Toggle_Violet()
    {
        Toggle_Color(VIOLET, ref violetEnabled);
    }
    #endregion

    #region Debug
    [ContextMenu("Set Defaults")]
    private void SetDefualts()
    {
        redRangeEnd = 10;
        orangeRangeEnd = 40;
        yellowRangeEnd = 90;
        greenRangeEnd = 160;
        lightBlueRangeEnd = 230;
        blueRangeEnd = 270;
        violetRangeEnd = 330;

        mat.SetFloat(RED_RANGE_END, redRangeEnd);
        mat.SetFloat(ORANGE_RANGE_END, orangeRangeEnd);
        mat.SetFloat(YELLOW_RANGE_END, yellowRangeEnd);
        mat.SetFloat(GREEN_RANGE_END, greenRangeEnd);
        mat.SetFloat(LIGHT_BLUE_RANGE_END, lightBlueRangeEnd);
        mat.SetFloat(BLUE_RANGE_END, blueRangeEnd);

        SetAllColorsFromRange();
    }

    private void GetCurrentValues()
    {
        redRangeEnd = (int)mat.GetFloat(RED_RANGE_END);
        orangeRangeEnd = (int)mat.GetFloat(ORANGE_RANGE_END);
        yellowRangeEnd = (int)mat.GetFloat(YELLOW_RANGE_END);
        greenRangeEnd = (int)mat.GetFloat(GREEN_RANGE_END);
        lightBlueRangeEnd = (int)mat.GetFloat(LIGHT_BLUE_RANGE_END);
        blueRangeEnd = (int)mat.GetFloat(BLUE_RANGE_END);
        violetRangeEnd = (int)mat.GetFloat(VIOLET_RANGE_END);
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
        "Toggle_Violet")]
    [SerializeField] private bool editorFoldout;
#endif
    #endregion
}
