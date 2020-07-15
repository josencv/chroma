using Chroma.ColorSystem.Probes;
using Chroma.ColorSystem.Probes.Builder;
using Chroma.Utility;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbsorbableStatic))]
public class AbsorbableEditor : Editor
{
    private enum ActionType { ReplaceAsset = 0, Nothing = 1, CreateNewAsset = 2 }

    Absorbable absorbable;

    private void OnEnable()
    {
        absorbable = (Absorbable)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Build Probes"))
        {
            ActionType actionType = ActionType.CreateNewAsset;
            if (absorbable.ProbeDataRef.RuntimeKeyIsValid())
            {
                actionType = OpenAssetOverwriteDialog();
            }

            if (actionType == ActionType.Nothing)
            {
                return;
            }

            ColorProbeData probeData = GenerateProbeData();
            SaveAssetAndUpdateReference(probeData, actionType);

            GUIUtility.ExitGUI();
        }
    }

    private ColorProbeData GenerateProbeData()
    {
        Mesh mesh = absorbable.GetComponent<MeshFilter>().sharedMesh;
        ColorProbe[] probes;
        if (absorbable.TriangleColorCalculationMode == TriangleColorCalculationMode.FromTexture)
        {
            Material material = absorbable.GetComponent<Renderer>().sharedMaterial;
            probes = ColorProbesBuilder.GenerateProbes(mesh, material, absorbable.ClusterSize);
        }
        else
        {
            probes = ColorProbesBuilder.GenerateProbes(mesh, absorbable.TriangleManualColor, absorbable.ClusterSize);
        }

        ColorProbeData probeData = ScriptableObject.CreateInstance<ColorProbeData>();
        probeData.Probes = probes;
        return probeData;
    }

    private ActionType OpenAssetOverwriteDialog()
    {
        int option = EditorUtility.DisplayDialogComplex(
            "Overwrite data",
            "Do you want to replace the current probe data, or create a new asset?",
            "Replace",
            "Cancel",
            "Create new asset");

        return (ActionType)option;
    }

    // Note: candidate to be moved to a utility class
    private string GetNewAssetPath()
    {
        int counter = 0;
        string path = string.Format("Assets/Content/Generated/Probes/{0}{1}ProbeData.asset", absorbable.gameObject.name, "");
        string guid = AssetDatabase.AssetPathToGUID(path);

        while (!string.IsNullOrEmpty(guid))
        {
            counter++;
            path = string.Format("Assets/Content/Generated/Probes/{0}{1}ProbeData.asset", absorbable.gameObject.name, counter);
            guid = AssetDatabase.AssetPathToGUID(path);
        }

        return path;
    }

    private void SaveAssetAndUpdateReference(ColorProbeData probeData, ActionType actionType)
    {
        string path;
        if(actionType == ActionType.ReplaceAsset)
        {
            path = AssetDatabase.GUIDToAssetPath(absorbable.ProbeDataRef.AssetGUID);
        }
        else
        {
            path = GetNewAssetPath();
        }

        AssetDatabase.CreateAsset(probeData, path);
        AssetDatabase.SaveAssets();
        absorbable.ProbeDataRef = AddressablesUtil.MarkAssetAsAddressable(path);
    }
}
