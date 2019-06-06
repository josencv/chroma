using UnityEngine;
using System.Collections;

namespace MeshBrush
{
    public class MeshBrushParent : MonoBehaviour
    {
        Transform[] paintedMeshes;
        MeshFilter[] meshFilters;

        Matrix4x4 localTransformationMatrix;
        Hashtable materialToMesh;

        MeshFilter currentMeshFilter;
        Renderer currentRenderer;

        Material[] materials;

        CombineUtility.MeshInstance instance;
        CombineUtility.MeshInstance[] instances;

        ArrayList objects;
        ArrayList elements;

        void Initialize()
        {
            paintedMeshes = GetComponentsInChildren<Transform>();
            meshFilters = GetComponentsInChildren<MeshFilter>();
        }

        /// <summary>
        /// Flags all painted meshes under this holder as static.
        /// </summary>
        public void FlagMeshesAsStatic()
        {
            Initialize();

            for (int i = paintedMeshes.Length - 1; i >= 0; i--)
            {
                paintedMeshes[i].gameObject.isStatic = true;
            }
        }

        /// <summary>
        /// Unflags all painted meshes under this holder as static (making them non-static).
        /// </summary>
        public void UnflagMeshesAsStatic()
        {
            Initialize();

            for (int i = paintedMeshes.Length - 1; i >= 0; i--)
            {
                paintedMeshes[i].gameObject.isStatic = false;
            }
        }

        /// <summary>
        /// Gets the total amount of meshes (<see cref="MeshFilter"/>) under this holder.
        /// </summary>
        public int GetMeshCount()
        {
            Initialize();

            return meshFilters.Length;
        }

        /// <summary>
        /// Gets the total amount of triangles under this holder.
        /// </summary>
        public int GetTrisCount()
        {
            Initialize();

            if (meshFilters.Length > 0)
            {
                int tris = 0;
                for (int i = meshFilters.Length - 1; i >= 0; i--)
                {
                    tris += meshFilters[i].sharedMesh.triangles.Length;
                }
                return tris / 3;
            }

            return 0;
        }

        public void DeleteAllMeshes()
        {
            DestroyImmediate(gameObject);
        }

        public void CombinePaintedMeshes(bool autoSelect, MeshFilter[] meshFilters)
        {
            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.LogError("MeshBrush: The meshFilters array you passed as an argument to the CombinePaintedMeshes function is empty or null... Combining action cancelled!");
                return;
            }

            localTransformationMatrix = transform.worldToLocalMatrix;
            materialToMesh = new Hashtable();

            int totalVertCount = 0;
            for (long i = 0; i < meshFilters.LongLength; i++)
            {
                currentMeshFilter = (MeshFilter)meshFilters[i];

                totalVertCount += currentMeshFilter.sharedMesh.vertexCount;

                if (totalVertCount > 64000)
                {
#if UNITY_EDITOR
                    if (UnityEditor.EditorUtility.DisplayDialog("Warning!", "You are trying to combine a group of meshes whose total vertex count exceeds Unity's built-in limit.\n\nThe process has been aborted to prevent the accidental deletion of all painted meshes and numerous disturbing error messages printed to the console.\n\nConsider splitting your meshes into smaller groups and combining them separately.\n\n=> You can do that for example based on the circle brush's area (press the combine meshes key in the scene view), or via multiple MeshBrush instances to form various painting sets and combine them individually; see the help section in the inspector for more detailed infos!", "Okay"))
                    {
                        return;
                    }
#endif
                    return;
                }
            }

            for (long i = 0; i < meshFilters.LongLength; i++)
            {
                currentMeshFilter = (MeshFilter)meshFilters[i];
                currentRenderer = meshFilters[i].GetComponent<Renderer>();

                instance = new CombineUtility.MeshInstance();
                instance.mesh = currentMeshFilter.sharedMesh;

                if (currentRenderer != null && currentRenderer.enabled && instance.mesh != null)
                {
                    instance.transform = localTransformationMatrix * currentMeshFilter.transform.localToWorldMatrix;

                    materials = currentRenderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++)
                    {
                        instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                        objects = (ArrayList)materialToMesh[materials[m]];
                        if (objects != null)
                        {
                            objects.Add(instance);
                        }
                        else
                        {
                            objects = new ArrayList();
                            objects.Add(instance);
                            materialToMesh.Add(materials[m], objects);
                        }
                    }

                    DestroyImmediate(currentRenderer.gameObject);
                }
            }

            foreach (DictionaryEntry de in materialToMesh)
            {
                elements = (ArrayList)de.Value;
                instances = (CombineUtility.MeshInstance[])elements.ToArray(typeof(CombineUtility.MeshInstance));

                var go = new GameObject("Combined mesh");

                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.AddComponent<SaveCombinedMesh>();
                go.GetComponent<Renderer>().material = (Material)de.Key;
                go.isStatic = true;

                currentMeshFilter = go.GetComponent<MeshFilter>();
                currentMeshFilter.mesh = CombineUtility.Combine(instances, false);

#if UNITY_EDITOR
                if (autoSelect)
                    UnityEditor.Selection.activeObject = go;
#endif
            }
            gameObject.isStatic = true;
        }
    }
}

// Copyright (C) Raphael Beck, 2017
