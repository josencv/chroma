using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 

namespace MeshBrush.Examples
{
    public class RuntimeExample : MonoBehaviour
    {
        [SerializeField]
        MeshBrush meshbrushInstance;

        [SerializeField]
        Transform circleBrush;

        [SerializeField]
        Slider radiusSlider, scatteringSlider, densitySlider;

        Transform mainCamera;

        // Use this for initialization
        void Start()
        {
            mainCamera = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            // Link the slider values to the MeshBrush instance.
            meshbrushInstance.radius = radiusSlider.value;
            meshbrushInstance.scatteringRange = new Vector2(scatteringSlider.value, scatteringSlider.value);
            meshbrushInstance.densityRange = new Vector2(densitySlider.value, densitySlider.value);

            // Perform a raycast from the mouse position through the camera.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                // Position and orient the brush object accordingly.
                circleBrush.position = hit.point;
                circleBrush.forward = -hit.normal;
                circleBrush.localScale = new Vector3(meshbrushInstance.radius, meshbrushInstance.radius, 1.0f);

                // Detect user input for the various brush actions.
                if (Input.GetKey(meshbrushInstance.paintKey))
                {
                    meshbrushInstance.PaintMeshes(hit);
                }

                if (Input.GetKey(meshbrushInstance.deleteKey))
                {
                    meshbrushInstance.DeleteMeshes(hit);
                }

                if (Input.GetKey(meshbrushInstance.randomizeKey))
                {
                    meshbrushInstance.RandomizeMeshes(hit);
                }
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017