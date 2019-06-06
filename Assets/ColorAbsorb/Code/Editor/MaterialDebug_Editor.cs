using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialDebug))]
public class MaterialDebug_Editor : Editor
{
    private MaterialDebug _materialDebug;

    SerializedProperty _meshRenderer;
    SerializedProperty _mat;
    SerializedProperty _mainTex;

    SerializedProperty _editorFoldout;
    SerializedProperty _redRangeEnd;
    SerializedProperty _redSaturation;
    SerializedProperty _orangeRangeEnd;
    SerializedProperty _orangeSaturation;
    SerializedProperty _yellowRangeEnd;
    SerializedProperty _yellowSaturation;
    SerializedProperty _greenRangeEnd;
    SerializedProperty _greenSaturation;
    SerializedProperty _lightBlueRangeEnd;
    SerializedProperty _lightBlueSaturation;
    SerializedProperty _blueRangeEnd;
    SerializedProperty _blueSaturation;
    SerializedProperty _violetRangeEnd;
    SerializedProperty _violetSaturation;
    SerializedProperty _width;
    SerializedProperty _smooth;

    private void OnEnable()
    {
        _materialDebug = (MaterialDebug)target;

        _meshRenderer = serializedObject.FindProperty("meshRenderer");
        _mat = serializedObject.FindProperty("mat");
        _mainTex = serializedObject.FindProperty("MainTex");

        _editorFoldout = serializedObject.FindProperty("editorFoldout");
        _redRangeEnd = serializedObject.FindProperty("redRangeEnd");
        _redSaturation = serializedObject.FindProperty("redSaturation");
        _orangeRangeEnd = serializedObject.FindProperty("orangeRangeEnd");
        _orangeSaturation = serializedObject.FindProperty("orangeSaturation");
        _yellowRangeEnd = serializedObject.FindProperty("yellowRangeEnd");
        _yellowSaturation = serializedObject.FindProperty("yellowSaturation");
        _greenRangeEnd = serializedObject.FindProperty("greenRangeEnd");
        _greenSaturation = serializedObject.FindProperty("greenSaturation");
        _lightBlueRangeEnd = serializedObject.FindProperty("lightBlueRangeEnd");
        _lightBlueSaturation = serializedObject.FindProperty("lightBlueSaturation");
        _blueRangeEnd = serializedObject.FindProperty("blueRangeEnd");
        _blueSaturation = serializedObject.FindProperty("blueSaturation");
        _violetRangeEnd = serializedObject.FindProperty("violetRangeEnd");
        _violetSaturation = serializedObject.FindProperty("violetSaturation");
        _width = serializedObject.FindProperty("width");
        _smooth = serializedObject.FindProperty("smooth");
    }


    public override void OnInspectorGUI()
    {
        var wholeStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
        wholeStyle.richText = true;
        wholeStyle.stretchWidth = true;
        var oldColor = GUI.backgroundColor;
        var imageStyle = new GUIStyle(GUI.skin.window) { alignment = TextAnchor.MiddleCenter };
        imageStyle.richText = true;

        imageStyle.fixedWidth = Screen.width - 50f; //600f * ratio;
        imageStyle.fixedHeight = 250f;
        imageStyle.stretchWidth = true;

        EditorGUILayout.BeginHorizontal(wholeStyle);
        EditorGUILayout.BeginVertical();
        GUILayout.Label(_materialDebug.shaderName);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(wholeStyle);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("<b>Image</b>", wholeStyle);
        GUI.backgroundColor = Color.black;
        EditorGUILayout.BeginVertical(wholeStyle);
        GUILayout.Label(_materialDebug.mat.mainTexture, imageStyle);
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(_mainTex, new GUIContent("Main Tex"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = oldColor;

        if(_materialDebug.shaderName.Equals("Shader Graphs/Color_Absorber_Smoothed"))
        {
            EditorGUILayout.PropertyField(_width, new GUIContent("Width"));
            EditorGUILayout.PropertyField(_smooth, new GUIContent("Smooth"));
        }

        DoRange("Red", wholeStyle, ref _redRangeEnd, ref _materialDebug.redRangeEndColor, ref _materialDebug.redEnabled, ref _redSaturation, _materialDebug.Toggle_Red);
        DoRange("Orange", wholeStyle, ref _orangeRangeEnd, ref _materialDebug.orangeRangeEndColor, ref _materialDebug.orangeEnabled, ref _orangeSaturation, _materialDebug.Toggle_Orange);
        DoRange("Yellow", wholeStyle, ref _yellowRangeEnd, ref _materialDebug.yellowRangeEndColor, ref _materialDebug.yellowEnabled, ref _yellowSaturation, _materialDebug.Toggle_Yellow);
        DoRange("Green", wholeStyle, ref _greenRangeEnd, ref _materialDebug.greenRangeEndColor, ref _materialDebug.greenEnabled, ref _greenSaturation, _materialDebug.Toggle_Green);
        DoRange("Light Blue", wholeStyle, ref _lightBlueRangeEnd, ref _materialDebug.lightBlueRangeEndColor, ref _materialDebug.lightBlueEnabled, ref _lightBlueSaturation, _materialDebug.Toggle_LightBlue);
        DoRange("Blue", wholeStyle, ref _blueRangeEnd, ref _materialDebug.blueRangeEndColor, ref _materialDebug.blueEnabled, ref _blueSaturation, _materialDebug.Toggle_Blue);
        DoRange("Violet", wholeStyle, ref _violetRangeEnd, ref _materialDebug.violetRangeEndColor, ref _materialDebug.violetEnabled, ref _violetSaturation, _materialDebug.Toggle_Violet);

        if(GUILayout.Button("Set Defualts"))
        {
            _materialDebug.SetDefualts();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void DoRange(string colorName, GUIStyle style, ref SerializedProperty rangeEnd, ref Color color, ref bool enabled, ref SerializedProperty saturation, Action action)
    {
        var oldColor = GUI.backgroundColor;
        EditorGUILayout.BeginHorizontal(style);
        {
            EditorGUILayout.BeginVertical(style);
            {
                EditorGUILayout.BeginVertical(style);
                {
                    GUI.color = color;
                    color = EditorGUILayout.ColorField($"{colorName} Color Range End", color);
                    EditorGUILayout.PropertyField(rangeEnd, new GUIContent($"{colorName} Hue Range End"), true);
                    GUI.color = Color.Lerp(Color.grey, color, saturation.floatValue);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    GUI.color = Color.white;
                    if(GUILayout.Button($"Toggle {colorName}", GUILayout.Width(200)))
                    {
                        action();
                    }
                    enabled = EditorGUILayout.ToggleLeft($"{colorName} Enabled", enabled);
                    GUI.color = Color.Lerp(Color.grey, color, saturation.floatValue);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginVertical(style);
                {             
                    GUI.color = Color.Lerp(Color.grey, color, saturation.floatValue);
                    EditorGUILayout.PropertyField(saturation, new GUIContent($"{colorName} Saturation"), true);
                    GUI.color = oldColor;
                }
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}