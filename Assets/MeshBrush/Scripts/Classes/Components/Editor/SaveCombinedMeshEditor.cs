using UnityEngine;
using UnityEditor;

namespace MeshBrush
{
    [CustomEditor(typeof(SaveCombinedMesh))]
    public class SaveCombinedMeshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var saveCombinedMesh = (SaveCombinedMesh)target;
            var meshFilter = saveCombinedMesh.GetComponent<MeshFilter>();

            EditorGUILayout.Space();

            if (!meshFilter && Event.current.type == EventType.Repaint)
            {
                DestroyImmediate(saveCombinedMesh);
                return;
            }

            if (string.CompareOrdinal(AssetDatabase.GetAssetPath(meshFilter.sharedMesh), string.Empty) != 0 && Event.current.type == EventType.Repaint)
            {
                DestroyImmediate(saveCombinedMesh);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("  Save combined mesh to disk", EditorIcons.SaveMeshIcon, "When you combine painted meshes via the MeshBrush inspector, the MeshFilter of the combined GameObject has its mesh data stored inside the current scene's root (which means it cannot be accessed, making it thus impossible to store it in a prefab).\n\nIf you intend to use the combined mesh later on (e.g. for a prefab), you have to save the mesh data to disk first.\n\nWhen you combine a set of meshes with MeshBrush, this script is added automatically to the new combined mesh GameObject.\n\nOnce you saved the mesh asset to disk, this script is no longer needed and deletes itself. It's not needed at runtime either, so it will destroy itself on Start too... "), GUILayout.Height(75f), GUILayout.Width(260f)))
                {
                    var meshStoragePath = EditorUtility.SaveFilePanelInProject("Select mesh data storage path", "Combined Mesh", "asset", "By saving your combined mesh to disk instead of leaving it stored in the scene's root you gain access to its MeshFilter data. This allows you for example to make a prefab out of the combined mesh for later usage without losing the mesh references.");
                    if (meshStoragePath.Length != 0)
                    {
                        AssetDatabase.CreateAsset(meshFilter.sharedMesh, meshStoragePath);
                    }
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}

// Copyright (C) Raphael Beck, 2017