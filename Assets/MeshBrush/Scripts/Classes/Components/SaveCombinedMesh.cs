using UnityEngine;

namespace MeshBrush
{
    // This class is useful if you want to save your
    // combined meshes to disk for prefabbing or for
    // later usage in other scenes without breaking the reference.
    public class SaveCombinedMesh : MonoBehaviour
    {
        // When you combine painted meshes via the MeshBrush inspector, the MeshFilter's mesh is stored inside
        // the current scene's root (which means it cannot be accessed, making it thus impossible to store the mesh in a prefab).
        // If you intend to use the combined mesh later on (e.g. via a prefab), you have to store the mesh data to disk first.
        // You can do so through this script's inspector. When you combine a mesh, this script is added automatically to the
        // combined mesh GameObject. Once you saved the mesh asset to disk, the script is no longer needed and deletes itself.
        // It's not needed at runtime either, so it will destroy itself on Start too... 
        void Start()
        {
            Destroy(this);
        }
    }
}

// Copyright (C) Raphael Beck, 2015