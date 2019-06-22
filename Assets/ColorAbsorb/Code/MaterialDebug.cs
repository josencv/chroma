using UnityEngine;

public class MaterialDebug : MonoBehaviour
{
    #region Constants
    const string RED = "_Range_1";
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
    [SerializeField] public MeshRenderer meshRenderer;
    [SerializeField] public Material mat;
    [Space()]
    [SerializeField] public Texture2D MainTex;

    [Header("Red"), Tooltip("Red goes from 0 to Red Range End, and Violet Range End to 360")]
    [SerializeField] public bool redEnabled = true;
    [Range(0, 360), SerializeField] public int redRangeEnd = 10;
    [SerializeField] public Color redRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float redSaturation = 1f;

    [Header("Orange"), Tooltip("Orange goes from Red Range End to Orange Range End")]
    [SerializeField] public bool orangeEnabled = true;
    [Range(0, 360), SerializeField] public int orangeRangeEnd = 40;
    [SerializeField] public Color orangeRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float orangeSaturation = 1f;

    [Header("Yellow"), Tooltip("Yellow goes from Orange Range End to Yellow Range End")]
    [SerializeField] public bool yellowEnabled = true;
    [Range(0, 360), SerializeField] public int yellowRangeEnd = 90;
    [SerializeField] public Color yellowRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float yellowSaturation = 1f;

    [Header("Green"), Tooltip("Green goes from Yellow Range End to Green Range End")]
    [SerializeField] public bool greenEnabled = true;
    [Range(0, 360), SerializeField] public int greenRangeEnd = 160;
    [SerializeField] public Color greenRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float greenSaturation = 1f;

    [Header("Light Blue"), Tooltip("Light Blue goes from Green Range End to Light Blue Range End")]
    [SerializeField] public bool lightBlueEnabled = true;
    [Range(0, 360), SerializeField] public int lightBlueRangeEnd = 230;
    [SerializeField] public Color lightBlueRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float lightBlueSaturation = 1f;

    [Header("Blue"), Tooltip("Blue goes from Light Blue Range End to Blue Range End")]
    [SerializeField] public bool blueEnabled = true;
    [Range(0, 360), SerializeField] public int blueRangeEnd = 270;
    [SerializeField] public Color blueRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float blueSaturation = 1f;

    [Header("Violet"), Tooltip("Violet goes from Blue Range End to Violet Range End")]
    [SerializeField] public bool violetEnabled = true;
    [Range(0, 360), SerializeField] public int violetRangeEnd = 330;
    [SerializeField] public Color violetRangeEndColor = new Color(1, 1, 1, 1);
    [Range(0f, 1f), SerializeField] public float violetSaturation = 1f;

    //[Range(0F, 1f), SerializeField] public float width = 0.061f;
    //[Range(0F, 1f), SerializeField] public float smooth = 0.13f;

    public string shaderName = string.Empty;

    #endregion

    public void OnValidate()
    {
            meshRenderer = GetComponent<MeshRenderer>();
        if(!mat)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            mat = meshRenderer.sharedMaterial;
        }

        if(shaderName.Equals(string.Empty))
        {
            shaderName = mat.shader.name;
        }

        mat.SetTexture("_MainTex", MainTex);

        //mat.SetFloat("_MinFade", width);
        //mat.SetFloat("_MaxFade", smooth);

        SetAllSaturation();
        SetAllColorsFromRange();
        SetAllRanges();
    }

    #region Methods
    public void SetAllColorsFromRange()
    {
        SetColorFromRange(ref redRangeEndColor, redRangeEnd);
        SetColorFromRange(ref orangeRangeEndColor, orangeRangeEnd);
        SetColorFromRange(ref yellowRangeEndColor, yellowRangeEnd);
        SetColorFromRange(ref greenRangeEndColor, greenRangeEnd);
        SetColorFromRange(ref lightBlueRangeEndColor, lightBlueRangeEnd);
        SetColorFromRange(ref blueRangeEndColor, blueRangeEnd);
        SetColorFromRange(ref violetRangeEndColor, violetRangeEnd);
    }

    public void SetColorFromRange(ref Color c, int range)
    {
        c = Color.HSVToRGB(range / 360f, 1, 1);
    }

    public void SetAllRanges()
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
        mat.SetFloat(VIOLET_RANGE_END, violetRangeEnd);
    }

    public void SetAllSaturation()
    {
        mat.SetFloat(RED, redSaturation);
        mat.SetFloat(ORANGE, orangeSaturation);
        mat.SetFloat(YELLOW, yellowSaturation);
        mat.SetFloat(GREEN, greenSaturation);
        mat.SetFloat(LIGHTBLUE, lightBlueSaturation);
        mat.SetFloat(BLUE, blueSaturation);
        mat.SetFloat(VIOLET, violetSaturation);

        redEnabled = redSaturation == 1;
        orangeEnabled = orangeSaturation == 1;
        yellowEnabled = yellowSaturation == 1;
        greenEnabled = greenSaturation == 1;
        lightBlueEnabled = lightBlueSaturation == 1;
        blueEnabled = blueSaturation == 1;
        violetEnabled = violetSaturation == 1;
    }
    #endregion

    #region Toggle Colors Buttons
    public void Toggle_Color(string rangeName, ref bool enabled, ref float saturation)
    {
        if(!mat)
        {
            mat = meshRenderer.sharedMaterial;
        }

        float val = mat.GetFloat(rangeName);
        val = val < 1 ? 1 : 0.0f;
        enabled = val == 1;
        mat.SetFloat(rangeName, val);
        saturation = val;
    }

    public void Toggle_Red()
    {
        Toggle_Color(RED, ref redEnabled, ref redSaturation);
    }

    public void Toggle_Orange()
    {
        Toggle_Color(ORANGE, ref orangeEnabled, ref orangeSaturation);
    }

    public void Toggle_Yellow()
    {
        Toggle_Color(YELLOW, ref yellowEnabled, ref yellowSaturation);
    }

    public void Toggle_Green()
    {
        Toggle_Color(GREEN, ref greenEnabled, ref greenSaturation);
    }

    public void Toggle_LightBlue()
    {
        Toggle_Color(LIGHTBLUE, ref lightBlueEnabled, ref lightBlueSaturation);
    }

    public void Toggle_Blue()
    {
        Toggle_Color(BLUE, ref blueEnabled, ref blueSaturation);
    }

    public void Toggle_Violet()
    {
        Toggle_Color(VIOLET, ref violetEnabled, ref violetSaturation);
    }
    #endregion

    #region Debug
    [ContextMenu("Set Defaults")]
    public void SetDefaults()
    {
        redRangeEnd = 2;
        orangeRangeEnd = 30;
        yellowRangeEnd = 75;
        greenRangeEnd = 150;
        lightBlueRangeEnd = 205;
        blueRangeEnd = 263;
        violetRangeEnd = 330;

        mat.SetFloat(RED_RANGE_END, redRangeEnd);
        mat.SetFloat(ORANGE_RANGE_END, orangeRangeEnd);
        mat.SetFloat(YELLOW_RANGE_END, yellowRangeEnd);
        mat.SetFloat(GREEN_RANGE_END, greenRangeEnd);
        mat.SetFloat(LIGHT_BLUE_RANGE_END, lightBlueRangeEnd);
        mat.SetFloat(BLUE_RANGE_END, blueRangeEnd);
        mat.SetFloat(VIOLET_RANGE_END, violetRangeEnd);

        mat.SetFloat(RED, 1);
        mat.SetFloat(ORANGE, 1);
        mat.SetFloat(YELLOW, 1);
        mat.SetFloat(GREEN, 1);
        mat.SetFloat(LIGHTBLUE, 1);
        mat.SetFloat(BLUE, 1);
        mat.SetFloat(VIOLET, 1);

        redEnabled = orangeEnabled = yellowEnabled = greenEnabled = lightBlueEnabled = blueEnabled = violetEnabled = true;
        //width = 0.061f;
        //smooth = 0.231f;


        //mat.SetFloat("_MinFade", width);
        //mat.SetFloat("_MaxFade", smooth);

        SetAllColorsFromRange();
    }

    public void GetCurrentValues()
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
}
