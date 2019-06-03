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
	}


	public override void OnInspectorGUI()
	{
        var wholeStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter };
        wholeStyle.richText = true;
        wholeStyle.stretchWidth = true;
        var oldColor = GUI.backgroundColor;
        var imageStyle = new GUIStyle(GUI.skin.window) { alignment = TextAnchor.MiddleCenter };
        imageStyle.richText = true;
        imageStyle.stretchWidth = true;
        imageStyle.stretchHeight = true;

        EditorGUILayout.BeginHorizontal(wholeStyle);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("<b>Image</b>", wholeStyle);
        EditorGUILayout.BeginVertical(wholeStyle);
        GUI.backgroundColor = Color.black;
        GUILayout.Label(_materialDebug.mat.mainTexture, imageStyle);
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndVertical();
        EditorGUILayout.PropertyField(_mainTex, new GUIContent("Main Tex"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = oldColor;

        DoRange("Red", wholeStyle, ref _redRangeEnd, ref _materialDebug.redRangeEndColor, ref _materialDebug.redEnabled, ref _redSaturation, _materialDebug.Toggle_Red);
        DoRange("Orange", wholeStyle, ref _orangeRangeEnd, ref _materialDebug.orangeRangeEndColor, ref _materialDebug.orangeEnabled, ref _orangeSaturation, _materialDebug.Toggle_Orange);
        DoRange("Yellow", wholeStyle, ref _yellowRangeEnd, ref _materialDebug.yellowRangeEndColor, ref _materialDebug.yellowEnabled, ref _yellowSaturation, _materialDebug.Toggle_Yellow);
        DoRange("Green", wholeStyle, ref _greenRangeEnd, ref _materialDebug.greenRangeEndColor, ref _materialDebug.greenEnabled, ref _greenSaturation, _materialDebug.Toggle_Green);
        DoRange("Light Blue", wholeStyle, ref _lightBlueRangeEnd, ref _materialDebug.lightBlueRangeEndColor, ref _materialDebug.lightBlueEnabled, ref _lightBlueSaturation, _materialDebug.Toggle_LightBlue);
        DoRange("Blue", wholeStyle, ref _blueRangeEnd, ref _materialDebug.blueRangeEndColor, ref _materialDebug.blueEnabled, ref _blueSaturation, _materialDebug.Toggle_Blue);
        DoRange("Violet", wholeStyle, ref _violetRangeEnd, ref _materialDebug.violetRangeEndColor, ref _materialDebug.violetEnabled, ref _violetSaturation, _materialDebug.Toggle_Violet);

		// Buttons
		if(GUILayout.Button("Set Defualts"))
		{
			_materialDebug.SetDefualts();
		}

		serializedObject.ApplyModifiedProperties();
	}

    public void DoRange(string colorName, GUIStyle style, ref SerializedProperty rangeEnd, ref Color color, ref bool enabled, ref SerializedProperty saturation, Action action)
    {
        EditorGUILayout.BeginHorizontal(style);
        EditorGUILayout.BeginVertical(style);

        color = EditorGUILayout.ColorField($"{colorName} Color Range End", color);
        EditorGUILayout.PropertyField(rangeEnd, new GUIContent($"{colorName} Hue Range End"), true);

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button($"Toggle {colorName}"))
        {
            action();
        }
        enabled = GUILayout.Toggle(enabled, $"{colorName} Enabled");
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.PropertyField(saturation, new GUIContent($"{colorName} Saturation"), true);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
