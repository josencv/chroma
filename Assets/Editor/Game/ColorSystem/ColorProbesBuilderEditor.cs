using Chroma.ColorSystem.Probes.Builder;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorProbesBuilder))]
public class ColorProbesBuilderEditor : Editor
{
    ColorProbesBuilder builder;

    private void OnEnable()
    {
        builder = (ColorProbesBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Build Probes"))
        {
            builder.PlaceColorProbesObjects();
        }
    }
}
