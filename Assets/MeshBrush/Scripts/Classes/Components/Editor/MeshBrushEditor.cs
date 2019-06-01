using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace MeshBrush
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(MeshBrush))]
    public class MeshBrushEditor : Editor
    {
        MeshBrush meshBrush;
        MeshBrushParent meshBrushParent;

        Event currentEvent;

        #region Serialized properties

        SerializedProperty active;
        SerializedProperty groupName;
        SerializedProperty globalPaintingMode;
        SerializedProperty showGlobalPaintingLayersInspector;
        SerializedProperty collapsed;
        SerializedProperty layerMask;
        SerializedProperty stats;
        SerializedProperty lockSceneView;
        SerializedProperty classicUI;
        SerializedProperty previewIconSize;
        SerializedProperty meshes;
        SerializedProperty deactivatedMeshes;
        SerializedProperty radius;
        SerializedProperty color;
        SerializedProperty quantityRange;
        SerializedProperty useDensity;
        SerializedProperty densityRange;
        SerializedProperty delay;
        SerializedProperty offsetRange;
        SerializedProperty slopeInfluenceRange;
        SerializedProperty useSlopeFilter;
        SerializedProperty angleThresholdRange;
        SerializedProperty inverseSlopeFilter;
        SerializedProperty manualReferenceVectorSampling;
        SerializedProperty showReferenceVectorInSceneView;
        SerializedProperty slopeReferenceVector;
        SerializedProperty slopeReferenceVectorSampleLocation;
        SerializedProperty yAxisTangent;
        SerializedProperty strokeAlignment;
        SerializedProperty lastPaintLocation;
        SerializedProperty brushStrokeDirection;
        SerializedProperty scatteringRange;
        SerializedProperty useOverlapFilter;
        SerializedProperty minimumAbsoluteDistanceRange;
        SerializedProperty uniformRandomScale;
        SerializedProperty uniformAdditiveScale;
        SerializedProperty randomScaleRange;
        SerializedProperty randomScaleRangeX;
        SerializedProperty randomScaleRangeY;
        SerializedProperty randomScaleRangeZ;
        SerializedProperty randomScaleCurve;
        SerializedProperty randomScaleCurveVariation;
        SerializedProperty additiveScaleRange;
        SerializedProperty additiveScaleNonUniform;
        SerializedProperty randomRotationRangeX;
        SerializedProperty randomRotationRangeY;
        SerializedProperty randomRotationRangeZ;
        SerializedProperty positionBrushRandomizer;
        SerializedProperty rotationBrushRandomizer;
        SerializedProperty scaleBrushRandomizer;
        SerializedProperty autoIgnoreRaycast;
        SerializedProperty autoStatic;
        SerializedProperty autoSelectOnCombine;

        #region Key bindings

        SerializedProperty paintKey;
        SerializedProperty deleteKey;
        SerializedProperty combineKey;
        SerializedProperty randomizeKey;
        SerializedProperty increaseRadiusKey;
        SerializedProperty decreaseRadiusKey;

        #endregion

        #region Foldouts

        SerializedProperty helpFoldout;
        SerializedProperty helpTemplatesFoldout;
        SerializedProperty helpGeneralUsageFoldout;
        SerializedProperty helpOptimizationFoldout;
        SerializedProperty meshesFoldout;
        SerializedProperty templatesFoldout;
        SerializedProperty keyBindingsFoldout;
        SerializedProperty brushFoldout;
        SerializedProperty slopesFoldout;
        SerializedProperty randomizersFoldout;
        SerializedProperty overlapFilterFoldout;
        SerializedProperty additiveScaleFoldout;
        SerializedProperty optimizationFoldout;

        #endregion

        #region Range limits

        SerializedProperty maxQuantityLimit;
        SerializedProperty maxDelayLimit;
        SerializedProperty maxDensityLimit;
        SerializedProperty minOffsetLimit;
        SerializedProperty maxOffsetLimit;
        SerializedProperty maxMinimumAbsoluteDistanceLimit;
        SerializedProperty maxAdditiveScaleLimit;
        SerializedProperty maxRandomScaleLimit;

        #endregion

        #endregion

        static List<string> favouriteTemplates = new List<string>(15);

        static string FavouritesFilePath
        {
            get
            {
                return Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("MeshBrushEditor")[0])) + "/FavouriteTemplates.xml";
            }
        }

        Action activeBrushMode;

        // Default brush mode should be paint.
        BrushMode brushMode = BrushMode.Paint;

        Vector2 templatesScroll = Vector2.zero; // 2D Vectors used for scrollbars.
        Vector2 layersScroll = Vector2.zero;
        Vector2 setScroll = Vector2.zero;

        // And here comes the raycasting part...
        Ray ray;          // This is the screen ray that shoots out of the scene view's camera when we press the paint button...
        RaycastHit hit;   // And this is its raycasthit.

        int globalPaintLayerMask = 1;
        float radiusModAcceleration = 1.0f;

        int totalPaintedMeshes;
        int totalPaintedTris;

        int currentObjectPickerIndex;

        static readonly string[] layerNames = new string[32];
        static readonly Color disabledMeshThumbnailColor = Color.red;

        #region MenuItem functions

        // Define a custom menu entry for the Unity toolbar above.
        [MenuItem("GameObject/MeshBrush/Paint meshes on selected GameObject", priority = 20)]
        static void CreateMeshBrushInstance() // This function gets called every time we click the above defined menu entry (since it is being defined exactly below the [MenuItem()] statement).
        {
            // Check if there is a GameObject selected.
            if (Selection.activeGameObject != null)
            {
                // Check if the selected GameObject has a collider on it (without it, where would we paint our meshes on?) :-|
                if (Selection.activeGameObject.GetComponent<Collider>())
                {
                    Selection.activeGameObject.AddComponent<MeshBrush>();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("GameObject has no collider component", "The GameObject on which you want to paint meshes doesn't have a collider...\nOn top of what did you expect to paint meshes? :)\n\n" +
                        "Do you want me to put a collider on it for you (it'll be a mesh collider)?", "Yes please!", "No thanks"))
                    {
                        Selection.activeGameObject.AddComponent<MeshCollider>();
                        Selection.activeGameObject.AddComponent<MeshBrush>();
                    }
                    else return;
                }
            }
            else EditorUtility.DisplayDialog("No GameObject selected", "No GameObject selected man... that's not cool bro D: what did you expect? To paint your meshes onto nothingness? :DDDDD", "Uhmmm...");
        }

        // This initializes a global painting MeshBrush instance.
        [MenuItem("GameObject/MeshBrush/Global painting", priority = 20)]
        static void CreateGlobalPaintingInstance()
        {
            var globalPaintingObj = new GameObject("MeshBrush Global Painting");

            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            globalPaintingObj.transform.position = sceneCam.transform.position + sceneCam.transform.forward * 1.5f;
            Selection.activeGameObject = globalPaintingObj;

            var meshBrush = new SerializedObject(globalPaintingObj.AddComponent<MeshBrush>());

            meshBrush.FindProperty("globalPaintingMode").boolValue = true;
            meshBrush.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        void OnEnable()
        {
            // Assign the reference to the script we are inspecting.
            meshBrush = (MeshBrush)target;

            GatherSerializedProperties();

            // Initialize the brushmode delegate.
            activeBrushMode = BrushMode_MeshPaint;

            if (meshBrush.CachedCollider == null && !globalPaintingMode.boolValue)
            {
                Debug.LogWarning("MeshBrush: This GameObject has no collider attached to it... MeshBrush needs a collider in order to work properly though; fix please! (see inspector for further details)");
            }

            // Initialize and validate the global painting layers array.
            if (layerMask.arraySize != 32)
            {
                layerMask.ClearArray();
                layerMask.arraySize = 32;

                for (int i = 0; i < layerMask.arraySize; i++)
                {
                    var arrayElement = layerMask.GetArrayElementAtIndex(i);
                    arrayElement.boolValue = true;
                }
            }

            UpdateLayerNames();
            UpdatePaintLayerMask();

            if (!File.Exists(FavouritesFilePath))
            {
                FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
            }

            FavouriteTemplatesUtility.LoadFavouriteTemplates(FavouritesFilePath, favouriteTemplates);

            meshBrushParent = meshBrush.HolderObj.GetComponent<MeshBrushParent>();

            UpdateTrisCounter();

            meshBrush.GatherPaintedMeshes();

            Undo.undoRedoPerformed += UpdateTrisCounter;

            meshBrush.EditRangeLimitsWizardOpened += OpenRangeLimitsWizard;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= UpdateTrisCounter;

            meshBrush.EditRangeLimitsWizardOpened -= OpenRangeLimitsWizard;
        }

        void GatherSerializedProperties()
        {
            active = serializedObject.FindProperty("active");
            groupName = serializedObject.FindProperty("groupName");
            showGlobalPaintingLayersInspector = serializedObject.FindProperty("showGlobalPaintingLayersInspector");
            globalPaintingMode = serializedObject.FindProperty("globalPaintingMode");
            collapsed = serializedObject.FindProperty("collapsed");
            layerMask = serializedObject.FindProperty("layerMask");
            stats = serializedObject.FindProperty("stats");
            lockSceneView = serializedObject.FindProperty("lockSceneView");
            classicUI = serializedObject.FindProperty("classicUI");
            previewIconSize = serializedObject.FindProperty("previewIconSize");
            meshes = serializedObject.FindProperty("meshes");
            deactivatedMeshes = serializedObject.FindProperty("deactivatedMeshes");
            radius = serializedObject.FindProperty("radius");
            color = serializedObject.FindProperty("color");
            useDensity = serializedObject.FindProperty("useDensity");
            densityRange = serializedObject.FindProperty("densityRange");
            quantityRange = serializedObject.FindProperty("quantityRange");
            delay = serializedObject.FindProperty("delay");
            offsetRange = serializedObject.FindProperty("offsetRange");
            slopeInfluenceRange = serializedObject.FindProperty("slopeInfluenceRange");
            useSlopeFilter = serializedObject.FindProperty("useSlopeFilter");
            angleThresholdRange = serializedObject.FindProperty("angleThresholdRange");
            inverseSlopeFilter = serializedObject.FindProperty("inverseSlopeFilter");
            manualReferenceVectorSampling = serializedObject.FindProperty("manualReferenceVectorSampling");
            showReferenceVectorInSceneView = serializedObject.FindProperty("showReferenceVectorInSceneView");
            slopeReferenceVector = serializedObject.FindProperty("slopeReferenceVector");
            slopeReferenceVectorSampleLocation = serializedObject.FindProperty("slopeReferenceVectorSampleLocation");
            yAxisTangent = serializedObject.FindProperty("yAxisTangent");
            strokeAlignment = serializedObject.FindProperty("strokeAlignment");
            lastPaintLocation = serializedObject.FindProperty("lastPaintLocation");
            brushStrokeDirection = serializedObject.FindProperty("brushStrokeDirection");
            autoIgnoreRaycast = serializedObject.FindProperty("autoIgnoreRaycast");
            scatteringRange = serializedObject.FindProperty("scatteringRange");
            useOverlapFilter = serializedObject.FindProperty("useOverlapFilter");
            minimumAbsoluteDistanceRange = serializedObject.FindProperty("minimumAbsoluteDistanceRange");
            uniformRandomScale = serializedObject.FindProperty("uniformRandomScale");
            uniformAdditiveScale = serializedObject.FindProperty("uniformAdditiveScale");
            randomScaleRange = serializedObject.FindProperty("randomScaleRange");
            randomScaleRangeX = serializedObject.FindProperty("randomScaleRangeX");
            randomScaleRangeY = serializedObject.FindProperty("randomScaleRangeY");
            randomScaleRangeZ = serializedObject.FindProperty("randomScaleRangeZ");
            randomScaleCurve = serializedObject.FindProperty("randomScaleCurve");
            randomScaleCurveVariation = serializedObject.FindProperty("randomScaleCurveVariation");
            additiveScaleRange = serializedObject.FindProperty("additiveScaleRange");
            additiveScaleNonUniform = serializedObject.FindProperty("additiveScaleNonUniform");
            randomRotationRangeX = serializedObject.FindProperty("randomRotationRangeX");
            randomRotationRangeY = serializedObject.FindProperty("randomRotationRangeY");
            randomRotationRangeZ = serializedObject.FindProperty("randomRotationRangeZ");
            autoStatic = serializedObject.FindProperty("autoStatic");
            autoSelectOnCombine = serializedObject.FindProperty("autoSelectOnCombine");

            positionBrushRandomizer = serializedObject.FindProperty("positionBrushRandomizer");
            rotationBrushRandomizer = serializedObject.FindProperty("rotationBrushRandomizer");
            scaleBrushRandomizer = serializedObject.FindProperty("scaleBrushRandomizer");

            helpFoldout = serializedObject.FindProperty("helpFoldout");
            helpTemplatesFoldout = serializedObject.FindProperty("helpTemplatesFoldout");
            helpGeneralUsageFoldout = serializedObject.FindProperty("helpGeneralUsageFoldout");
            helpOptimizationFoldout = serializedObject.FindProperty("helpOptimizationFoldout");
            meshesFoldout = serializedObject.FindProperty("meshesFoldout");
            templatesFoldout = serializedObject.FindProperty("templatesFoldout");
            keyBindingsFoldout = serializedObject.FindProperty("keyBindingsFoldout");
            brushFoldout = serializedObject.FindProperty("brushFoldout");
            slopesFoldout = serializedObject.FindProperty("slopesFoldout");
            randomizersFoldout = serializedObject.FindProperty("randomizersFoldout");
            overlapFilterFoldout = serializedObject.FindProperty("overlapFilterFoldout");
            additiveScaleFoldout = serializedObject.FindProperty("additiveScaleFoldout");
            optimizationFoldout = serializedObject.FindProperty("optimizationFoldout");

            paintKey = serializedObject.FindProperty("paintKey");
            deleteKey = serializedObject.FindProperty("deleteKey");
            combineKey = serializedObject.FindProperty("combineKey");
            randomizeKey = serializedObject.FindProperty("randomizeKey");
            increaseRadiusKey = serializedObject.FindProperty("increaseRadiusKey");
            decreaseRadiusKey = serializedObject.FindProperty("decreaseRadiusKey");

            maxQuantityLimit = serializedObject.FindProperty("maxQuantityLimit");
            maxDelayLimit = serializedObject.FindProperty("maxDelayLimit");
            maxDensityLimit = serializedObject.FindProperty("maxDensityLimit");
            minOffsetLimit = serializedObject.FindProperty("minOffsetLimit");
            maxOffsetLimit = serializedObject.FindProperty("maxOffsetLimit");
            maxMinimumAbsoluteDistanceLimit = serializedObject.FindProperty("maxMinimumAbsoluteDistanceLimit");
            maxAdditiveScaleLimit = serializedObject.FindProperty("maxAdditiveScaleLimit");
            maxRandomScaleLimit = serializedObject.FindProperty("maxRandomScaleLimit");
        }

        void UpdatePaintLayerMask()
        {
            globalPaintLayerMask = 1;
            for (int i = 0; i < layerMask.arraySize; i++)
            {
                var arrayElement = layerMask.GetArrayElementAtIndex(i);
                if (arrayElement.boolValue == true)
                {
                    globalPaintLayerMask |= 1 << i;
                }
            }
        }

        void UpdateLayerNames()
        {
            for (int i = 0; i < layerNames.Length; i++)
            {
                layerNames[i] = string.IsNullOrEmpty(LayerMask.LayerToName(i)) ? "Layer " + i : LayerMask.LayerToName(i);
            }
        }

        void OpenRangeLimitsWizard()
        {
            var wizard = ScriptableWizard.DisplayWizard<EditRangeLimitsWizard>("Edit range limits", "Done", "Restore default limits");
            wizard.Initialize(meshBrush);
        }

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Group name: ", GUILayout.Width(78f));
                EditorGUILayout.PropertyField(groupName, GUIContent.none);

                meshBrush.HolderObj.name = groupName.stringValue;
            }
            EditorGUILayout.EndHorizontal();

            // MAIN TOGGLE (this one can entirely turn the meshbrush on and off)
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(active, GUIContent.none, GUILayout.Width(15f));
                EditorGUILayout.LabelField(new GUIContent("Enabled", "Enables/disables this MeshBrush group."), GUILayout.Width(52f), GUILayout.ExpandWidth(false));

                EditorGUILayout.PropertyField(collapsed, GUIContent.none, GUILayout.Width(15f));
                EditorGUILayout.LabelField(new GUIContent("Collapsed", "Collapses this MeshBrush component's inspector."), GUILayout.Width(62.75f), GUILayout.ExpandWidth(false));

                if (lockSceneView.boolValue)
                {
                    GUI.color = Color.yellow;
                }

                EditorGUILayout.PropertyField(lockSceneView, GUIContent.none, GUILayout.Width(15f));
                EditorGUILayout.LabelField(new GUIContent("Lock scene view", "Avoids losing focus of the scene view or accidentally deselect this GameObject when painting and clicking around on other objects in the scene.\n\nThis is particularly useful if you're painting with your mouse buttons as paint keys or even tablets."), GUILayout.Width(97f), GUILayout.ExpandWidth(false));

                if (lockSceneView.boolValue)
                {
                    GUI.color = Color.white;
                }

                // Triangle counter label toggle
                EditorGUILayout.PropertyField(stats, GUIContent.none, GUILayout.Width(15f));
                EditorGUILayout.LabelField(new GUIContent("Stats", "Displays a statistics label at the top of the MeshBrush inspector.\n\nThe displayed numbers refer to the total amount of meshes and triangles that have been painted with this MeshBrush instance."), GUILayout.Width(35f), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();
            
            if (!collapsed.boolValue)
            {
                MainInspectorSettings();

                GlobalPaintingModeInspector();

                TrisCounterInspector();

                UnlinkedMeshBrushInstanceInspector();

                MissingColliderInspector();

                HelpSectionInspector();

                GUI.enabled = active.boolValue;

                TemplatesInspector();

                MeshesInspector();

                KeyBindingsInspector();

                BrushSettingsInspector();

                SlopeSettingsInspector();

                RandomizersInspector();

                OverlapFilterInspector();

                AdditiveScaleInspector();

                OptimizationInspector();
            }

            EditorGUILayout.Space();

            // Repaint the scene view whenever the inspector's gui is changed in some way. 
            // This avoids weird disturbing snaps of the reference slope vector and the circle 
            // brush GUI handle inside the scene view when we return to it after changing some settings in the inspector.
            if (GUI.changed)
            {
                SceneView.RepaintAll();
                UpdatePaintLayerMask();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void MainInspectorSettings()
        {
            EditorGUILayout.BeginHorizontal();
            {
                
                
            }
            EditorGUILayout.EndHorizontal();
        }

        void GlobalPaintingModeInspector()
        {
            showGlobalPaintingLayersInspector.boolValue = EditorGUILayout.Foldout(showGlobalPaintingLayersInspector.boolValue, "Layers");

            if (globalPaintingMode.boolValue && showGlobalPaintingLayersInspector.boolValue)
            {
                // Global painting interface:
                EditorGUILayout.BeginVertical("Box");
                {
                    // Title
                    GUI.color = new Color(3.0f, 1.0f, 0.0f, 1.0f);
                    EditorGUILayout.LabelField("MeshBrush - Global Painting Mode", EditorStyles.boldLabel);
                    GUI.color = Color.white;

                    EditorGUILayout.LabelField(new GUIContent("Layer based painting", "You have the control over where MeshBrush is allowed to paint your meshes and where not.\nMeshBrush will only paint onto objects in the scene whose layers are enabled here inside this layer selection."));

                    // Scrollbar for the layer selection
                    layersScroll = EditorGUILayout.BeginScrollView(layersScroll, false, false, GUILayout.Height(166f));
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                // First column of layers (these are the built-in standard Unity layers).
                                for (int i = 0; i < 8; i++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        GUI.enabled = i != 2;
                                        var layer = layerMask.GetArrayElementAtIndex(i);
                                        EditorGUILayout.PropertyField(layer, GUIContent.none, GUILayout.Width(15f));
                                        EditorGUILayout.LabelField(LayerMask.LayerToName(i), GUILayout.Width(90f));
                                        GUI.enabled = true;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();

                            // The next 3 vertical groups represent the second, third and fourth column of the layer selection.
                            EditorGUILayout.BeginVertical();
                            {
                                for (int i = 8; i < 16; i++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        var layer = layerMask.GetArrayElementAtIndex(i);
                                        EditorGUILayout.PropertyField(layer, GUIContent.none, GUILayout.Width(15f));
                                        GUI.color = string.IsNullOrEmpty(LayerMask.LayerToName(i)) ? new Color(0.65f, 0.65f, 0.65f, 1.0f) : Color.white;
                                        EditorGUILayout.LabelField(layerNames[i], GUILayout.Width(90f));
                                        GUI.color = Color.white;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            {
                                for (int i = 16; i < 24; i++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        var layer = layerMask.GetArrayElementAtIndex(i);
                                        EditorGUILayout.PropertyField(layer, GUIContent.none, GUILayout.Width(15f));
                                        GUI.color = string.IsNullOrEmpty(LayerMask.LayerToName(i)) ? new Color(0.65f, 0.65f, 0.65f, 1.0f) : Color.white;
                                        EditorGUILayout.LabelField(layerNames[i], GUILayout.Width(90f));
                                        GUI.color = Color.white;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            {
                                for (int i = 24; i < 32; i++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        var layer = layerMask.GetArrayElementAtIndex(i);
                                        EditorGUILayout.PropertyField(layer, GUIContent.none, GUILayout.Width(15f));
                                        GUI.color = string.IsNullOrEmpty(LayerMask.LayerToName(i)) ? new Color(0.65f, 0.65f, 0.65f, 1.0f) : Color.white;
                                        EditorGUILayout.LabelField(layerNames[i], GUILayout.Width(90f));
                                        GUI.color = Color.white;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();

                    // Buttons to automatically select/deselect all layers.
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("All", GUILayout.Width(55f), GUILayout.Height(20f)))
                        {
                            for (int i = 0; i < layerMask.arraySize; i++)
                            {
                                layerMask.GetArrayElementAtIndex(i).boolValue = true;
                            }
                        }

                        if (GUILayout.Button("None", GUILayout.Width(55f), GUILayout.Height(20f)))
                        {
                            for (int i = 0; i < layerMask.arraySize; i++)
                            {
                                layerMask.GetArrayElementAtIndex(i).boolValue = false;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        void UnlinkedMeshBrushInstanceInspector()
        {
            if (Selection.instanceIDs.Length != 1 || Selection.activeInstanceID != meshBrush.gameObject.GetInstanceID())
            {
                GUI.color = Color.red;
                if (GUILayout.Button(new GUIContent("MeshBrush instance unlinked - Click here to fix!", "This MeshBrush instance is still active due to the engaged inspector lock...\nThe GameObject to which this component belongs is currently deselected, that's why your brush isn't appearing in your scene view and why you can't paint meshes right now.\nClick this button to automatically select the GameObject to which this MeshBrush instance belongs, and start painting again."), GUILayout.Height(25f)))
                {
                    Selection.activeGameObject = meshBrush.gameObject;
                }
                GUI.color = Color.white;
            }
        }

        void MissingColliderInspector()
        {
            if (meshBrush.CachedCollider == null && !globalPaintingMode.boolValue)
            {
                EditorGUILayout.HelpBox("This GameObject has no collider attached to it.\nMeshBrush needs a collider in order to work properly! Do you want to add a collider now?", MessageType.Error);

                if (GUILayout.Button("Yes, add a MeshCollider now please", GUILayout.Height(27f)))
                {
                    meshBrush.gameObject.AddComponent<MeshCollider>();
                }
                if (GUILayout.Button("Yes, add a BoxCollider now please", GUILayout.Height(27f)))
                {
                    meshBrush.gameObject.AddComponent<BoxCollider>();
                }
                if (GUILayout.Button("Yes, add a SphereCollider now please", GUILayout.Height(27f)))
                {
                    meshBrush.gameObject.AddComponent<SphereCollider>();
                }
                if (GUILayout.Button("Yes, add a CapsuleCollider now please", GUILayout.Height(27f)))
                {
                    meshBrush.gameObject.AddComponent<CapsuleCollider>();
                }
                globalPaintingMode.boolValue |= GUILayout.Button("No, switch to global painting mode now please", GUILayout.Height(27f));
                EditorGUILayout.Space();

                GUI.enabled = false;
            }
        }

        void HelpSectionInspector()
        {
            // Foldout menu for the help section.
            helpFoldout.boolValue = EditorGUILayout.Foldout(helpFoldout.boolValue, "Help");

            // The help foldout menu in the inspector.
            if (helpFoldout.boolValue)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.HelpBox("Paint meshes onto your GameObject's surface.\n_______\n\nKeyboard shortcuts:\n\nPaint meshes:\tpress or hold    " + meshBrush.paintKey + "\nDelete meshes:\tpress or hold    " + meshBrush.deleteKey + "\nCombine meshes:\tpress or hold    " + meshBrush.combineKey + "\nRandomize:\tpress or hold    " + meshBrush.randomizeKey + "\nIncrease radius:\tpress or hold    " + meshBrush.increaseRadiusKey + "\nDecrease radius:\tpress or hold    " + meshBrush.decreaseRadiusKey + "\n_______\n", MessageType.None);

                helpGeneralUsageFoldout.boolValue = EditorGUILayout.Foldout(helpGeneralUsageFoldout.boolValue, "General usage");
                if (helpGeneralUsageFoldout.boolValue)
                {
                    EditorGUILayout.HelpBox("Assign one or more prefab objects to the set of meshes to paint below and press " + meshBrush.paintKey + " while hovering your mouse above your GameObject to start painting meshes. Press " + meshBrush.deleteKey + " to delete painted meshes." +
                    "\n\nMake sure that the local Y-axis of each prefab mesh is the one pointing away from the surface on which you are painting (to avoid weird rotation errors).\n\n" +
                    "Check the documentation text file that comes with MeshBrush (or the YouTube tutorials) to find out more about the individual brush parameters (but most of them should be quite self explainatory, or at least supplied with a tooltip text label after hovering your mouse over them for a couple of seconds).\n\nFeel free to add multiple MeshBrush script instances to one GameObject for multiple mesh painting sets, with defineable group names and parameters for each of them;\n" +
                    "MeshBrush will then randomly cycle through all of your MeshBrush instances and paint your meshes within the corresponding circle brush based on the corresponding parameters for that set.", MessageType.None);
                }

                helpTemplatesFoldout.boolValue = EditorGUILayout.Foldout(helpTemplatesFoldout.boolValue, "Templates");
                if (helpTemplatesFoldout.boolValue)
                {
                    EditorGUILayout.HelpBox("In the templates foldout menu you can save your favourite brush settings and setups to MeshBrush template files for later reusage.\n\nJust press the save button and name your file and path.\nTo load a template file, press the load button and load up your template file from disk; it's as simple as that.\n\nMeshBrush template files are xml formatted text files, so if you want, you can also open them up with notepad (or some other text editor) and change the settings from there.", MessageType.None);
                }

                helpOptimizationFoldout.boolValue = EditorGUILayout.Foldout(helpOptimizationFoldout.boolValue, "Optimization");
                if (helpOptimizationFoldout.boolValue)
                {
                    EditorGUILayout.HelpBox("You can press 'Flag/Unflag all painted meshes as static' to mark/unmark as static all the meshes you've painted so far.\nFlagging painted meshes as static will improve performance overhead thanks to Unity's built-in static batching functionality, " +
                    "as long as the meshes obviously don't move (and as long as they share the same material).\nSo don't flag meshes as static if you have fancy looking animations on your prefab meshes (like, for instance, swaying animations for vegetation or similar properties that make the mesh move, rotate or scale in any way).\n_______\n\n" +
                    "Once you're done painting you can combine your meshes either with the 'Combine all painted meshes button' or by pressing " + meshBrush.combineKey + " (this will combine all the meshes inside the brush area).\nCheck out the documentation for further information.", MessageType.None);
                }

                EditorGUI.indentLevel = 0;
            }
        }

        void TrisCounterInspector()
        {
            if (stats.boolValue)
            {
                EditorGUILayout.BeginVertical("Box");
                {
                    GUI.color = new Color(3f, 1f, 0f, 1f);
                    EditorGUILayout.LabelField("Meshes: " + totalPaintedMeshes, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Triangles: " + totalPaintedTris, EditorStyles.boldLabel);
                    GUI.color = Color.white;
                }
                EditorGUILayout.EndVertical();
            }
        }

        void TemplatesInspector()
        {
            templatesFoldout.boolValue = EditorGUILayout.Foldout(templatesFoldout.boolValue, "Templates");

            if (templatesFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical(GUILayout.MaxWidth(57f));
                    {
                        if (GUILayout.Button(new GUIContent(EditorIcons.SaveTemplateIcon, "Save/export these current MeshBrush settings to a template file."), GUILayout.Height(55f), GUILayout.Width(55f)))
                        {
                            var templateFilePath = EditorUtility.SaveFilePanelInProject("Select template file storage path", "<insert template name here>", "xml", "A MeshBrush template contains all the relevant settings of a MeshBrush instance.\nIt can be saved from and loaded into a MeshBrush instance and makes swapping settings setups in/out easy.");
                            if (!string.IsNullOrEmpty(templateFilePath))
                            {
                                meshBrush.SaveTemplate(templateFilePath);
                            }
                        }

                        if (GUILayout.Button(new GUIContent(EditorIcons.LoadTemplateIcon, "Load up a template file."), GUILayout.Height(55f), GUILayout.Width(55f)))
                        {
                            Undo.RegisterCompleteObjectUndo(meshBrush, "load MeshBrush template");

                            var templateFilePath = EditorUtility.OpenFilePanel("Select template file to load", "/Assets", "xml");
                            if (!string.IsNullOrEmpty(templateFilePath))
                            {
                                meshBrush.LoadTemplate(templateFilePath);
                            }
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(-3);

                    GUILayout.BeginVertical("Box", GUILayout.MaxHeight(110f));
                    {
                        GUILayout.Space(-1);
                        templatesScroll = EditorGUILayout.BeginScrollView(templatesScroll, false, false);
                        {
                            if (favouriteTemplates.Count < 1)
                                EditorGUILayout.LabelField("Favourites list is empty", EditorStyles.wordWrappedLabel);

                            for (int i = 0; i < favouriteTemplates.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button(Path.GetFileNameWithoutExtension(favouriteTemplates[i])))
                                    {
                                        if (File.Exists(favouriteTemplates[i]))
                                        {
                                            Undo.RegisterCompleteObjectUndo(meshBrush, "load favourite MeshBrush template");
                                            meshBrush.LoadTemplate(favouriteTemplates[i]);
                                        }
                                        else
                                        {
                                            EditorUtility.DisplayDialog("Failed to load template!", "The selected template file couldn't be loaded.\n\nIt's probably been renamed, deleted or moved elsewhere.\n\nThe corresponding entry in the list of favourite templates will be removed.", "Okay");

                                            favouriteTemplates.RemoveAt(i);
                                            FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                        }
                                    }

                                    if (GUILayout.Button(new GUIContent("...", "Reassign this template"), GUILayout.Width(27f)))
                                    {
                                        string oldPath = favouriteTemplates[i];
                                        favouriteTemplates[i] = EditorUtility.OpenFilePanel("Reassignment - Select MeshBrush Template", "Assets/MeshBrush/Saved Templates", "xml");

                                        // Revert back to the previous template in case the user cancels the reassignment.
                                        if (string.IsNullOrEmpty(favouriteTemplates[i]))
                                        {
                                            favouriteTemplates[i] = oldPath;
                                        }

                                        FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                    }

                                    if (GUILayout.Button(new GUIContent("-", "Removes this template from the list."), GUILayout.Width(27f)))
                                    {
                                        favouriteTemplates.RemoveAt(i);
                                        FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            EditorGUILayout.BeginHorizontal();
                            {
                                // The following block of code is responsible for the favourites list's drag 'n' drop add button.
                                currentEvent = Event.current;

                                Rect dropAreaAdd = GUILayoutUtility.GetRect(30f, 22f, GUILayout.Width(30f), GUILayout.Height(22f), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                                dropAreaAdd.x += 3; dropAreaAdd.y += 3;

                                GUI.Box(dropAreaAdd, new GUIContent(EditorIcons.AddIcon, "Click here to add a MeshBrush template file to your favourites list, or drag and drop multiple template files onto this button."));

                                switch (currentEvent.type)
                                {
                                    case EventType.MouseDown:
                                        if (dropAreaAdd.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                                        {
                                            string path = EditorUtility.OpenFilePanel("Select favourite MeshBrush Template", "Assets/MeshBrush/Templates", "xml");
                                            if (string.CompareOrdinal(path, string.Empty) != 0)
                                            {
                                                favouriteTemplates.Add(path);
                                                FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);

                                                Repaint();
                                            }
                                        }
                                        break;

                                    case EventType.DragUpdated:
                                    case EventType.DragPerform:

                                        if (!dropAreaAdd.Contains(currentEvent.mousePosition))
                                            break;

                                        DragAndDrop.visualMode = DragAndDrop.objectReferences.Length == 1 && string.CompareOrdinal(Path.GetExtension(AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0].GetInstanceID())), ".xml") != 0 ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Copy;

                                        if (currentEvent.type == EventType.DragPerform)
                                        {
                                            DragAndDrop.AcceptDrag();

                                            //Undo.RecordObject(_mb, "drag 'n' drop action (MeshBrush)");
                                            for (int ii = 0; ii < DragAndDrop.objectReferences.Length; ii++)
                                            {
                                                string path = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[ii].GetInstanceID());
                                                if (string.CompareOrdinal(Path.GetExtension(path), ".xml") == 0)
                                                {
                                                    favouriteTemplates.Add(path);
                                                    FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                                }
                                            }

                                            Repaint();
                                            GC.Collect();
                                        }
                                        break;
                                }
                                GUILayout.Space(4f);

                                GUI.enabled = favouriteTemplates.Count > 0;
                                if (GUILayout.Button(new GUIContent("-", "Removes the bottom-most template from the list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                                {
                                    if (favouriteTemplates.Count > 0)
                                    {
                                        favouriteTemplates.RemoveAt(favouriteTemplates.Count - 1);
                                        FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                    }
                                }
                                GUI.enabled = active.boolValue;

                                GUI.enabled = favouriteTemplates.Count > 0;
                                if (GUILayout.Button(new GUIContent("C", "Clears the entire favourites list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                                {
                                    favouriteTemplates.Clear();
                                    FavouriteTemplatesUtility.SaveFavouriteTemplates(favouriteTemplates, FavouritesFilePath);
                                }
                                GUI.enabled = active.boolValue;
                            }
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(5f);
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.5f);
            }
        }

        void MeshesInspector()
        {
            meshesFoldout.boolValue = EditorGUILayout.Foldout(meshesFoldout.boolValue, "Meshes");

            if (meshesFoldout.boolValue)
            {
                if (classicUI.boolValue)
                {
                    ClassicSetOfMeshesToPaintInspector();
                }
                else
                {
                    ModernSetOfMeshesToPaintInspector();
                }

                GUI.color = brushMode == BrushMode.PrecisionPlacement ? Color.yellow : Color.white;

                if (GUILayout.Button(new GUIContent("Precision placement mode", "Enters the precision placement mode, which allows you to place a single mesh from your set of meshes to paint into the scene with maximum precision.\nYou can cycle through the meshes in your set with the N and B keys; N selects the next mesh in your set, whereas B cycles backwards.\n\nThe placement of the mesh is divided into 3 steps, each concluded by pressing the paint button:\n\n1)\tScale (drag the mouse to adjust...)\n2)\tRotation\n3)\tVertical offset\n\nThe third press of the paint button terminates the placement and prepares the next one."), GUILayout.Height(38f)))
                {
                    brushMode = BrushMode.PrecisionPlacement;
                }

                GUI.color = Color.white;
            }
        }

        void ClassicSetOfMeshesToPaintInspector()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                setScroll = EditorGUILayout.BeginScrollView(setScroll, false, false, GUILayout.ExpandWidth(false), GUILayout.Height(193f));
                {
                    for (int i = 0; i < meshes.arraySize; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = active.boolValue && meshes.arraySize > 1;
                            if (GUILayout.Button(new GUIContent("-", "Removes this entry from the list."), GUILayout.Width(30f), GUILayout.Height(16.5f)))
                            {
                                RemoveMeshToPaint(i);
                                continue;
                            }
                            GUI.enabled = active.boolValue;

                            EditorGUILayout.ObjectField(meshes.GetArrayElementAtIndex(i), typeof(GameObject), GUIContent.none, GUILayout.Height(16.35f));
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(2.75f);
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("+", "Adds an entry to the list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        AddMeshToPaint(null);
                    }

                    GUI.enabled = active.boolValue && meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("-", "Removes the bottom row from the list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 1)
                        {
                            RemoveMeshToPaint(meshes.arraySize - 1);
                        }
                    }
                    GUI.enabled = active.boolValue;

                    GUI.enabled = active.boolValue && meshes.arraySize > 0;
                    if (GUILayout.Button(new GUIContent("X", "Clears all values from all rows in the list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 0)
                        {
                            NullifySetOfMeshesToPaint();
                        }
                    }
                    GUI.enabled = active.boolValue;

                    GUI.enabled = active.boolValue && meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("C", "Clears the list (deleting all rows at once)."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 1)
                        {
                            ClearSetOfMeshesToPaint();
                        }
                    }
                    GUI.enabled = active.boolValue;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Switch to modern UI", GUILayout.Height(22f)))
                    {
                        classicUI.boolValue = false;
                    }
                    GUI.enabled = meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("Clean unused fields", "Clean up the set of meshes to paint by removing all its empty (null) fields."), GUILayout.Height(22f)))
                    {
                        meshBrush.CleanSetOfMeshesToPaint();
                    }
                    GUI.enabled = active.boolValue;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.5f);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
        }

        void ModernSetOfMeshesToPaintInspector()
        {
            EditorGUILayout.BeginVertical("Box");
            {
                setScroll = EditorGUILayout.BeginScrollView(setScroll, false, false, GUILayout.Height(193f));
                {
                    int maxIconsPerRow = (int)(EditorGUIUtility.currentViewWidth / (previewIconSize.floatValue + 11f));

                    EditorGUILayout.BeginHorizontal();
                    {
                        int rowIconsCount = 0;
                        for (int i = 0; i < meshes.arraySize; i++)
                        {
                            if (rowIconsCount == maxIconsPerRow)
                            {
                                EditorGUILayout.EndHorizontal();
                                GUILayout.Space(6f);
                                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

                                rowIconsCount = 0;
                            }

                            rowIconsCount++;

                            currentEvent = Event.current;

                            List<int> ignoredMeshIds = new List<int>(deactivatedMeshes.arraySize);

                            for (int ii = deactivatedMeshes.arraySize - 1; ii >= 0; ii--)
                            {
                                var m = deactivatedMeshes.GetArrayElementAtIndex(ii);
                                if (m != null && m.objectReferenceValue != null)
                                    ignoredMeshIds.Add(m.objectReferenceInstanceIDValue);
                            }

                            Rect dropArea = GUILayoutUtility.GetRect(previewIconSize.floatValue, previewIconSize.floatValue, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                            dropArea.x += 3; dropArea.y += 3;

                            SerializedProperty meshToPaint = meshes.GetArrayElementAtIndex(i);
                            
                            var lastColor = GUI.color;
                            if (ignoredMeshIds.Contains(meshToPaint.objectReferenceInstanceIDValue))
                            {
                                GUI.color = disabledMeshThumbnailColor;
                            }
                            GUI.Box(dropArea, new GUIContent(meshToPaint.objectReferenceValue != null ? AssetPreview.GetAssetPreview(meshToPaint.objectReferenceValue) : EditorIcons.NullIcon, meshToPaint.objectReferenceValue != null ? meshToPaint.objectReferenceValue.name : "Unassigned field; either assign something to it or remove it!"));
                            GUI.color = lastColor;
                            
                            if (dropArea.Contains(currentEvent.mousePosition))
                            {
                                switch (currentEvent.type)
                                {
                                    case EventType.MouseDown:

                                        switch (currentEvent.button)
                                        {
                                            // Left mouse button for object selection.
                                            case 0:
                                                if (currentEvent.control)
                                                {
                                                    bool found = false;
                                                    for (int ii = deactivatedMeshes.arraySize - 1; ii >= 0; ii--)
                                                    {
                                                        var m = deactivatedMeshes.GetArrayElementAtIndex(ii);
                                                        if (m.objectReferenceInstanceIDValue == meshToPaint.objectReferenceInstanceIDValue)
                                                        {
                                                            found = true;
                                                            deactivatedMeshes.DeleteArrayElementAtIndex(ii);
                                                            deactivatedMeshes.DeleteArrayElementAtIndex(ii);
                                                            break;
                                                        }
                                                    }
                                                    if (!found)
                                                    {
                                                        int tail = deactivatedMeshes.arraySize++;
                                                        deactivatedMeshes.GetArrayElementAtIndex(tail).objectReferenceValue = meshToPaint.objectReferenceValue;
                                                    }
                                                    break;
                                                }
                                                // Ping the object in the project panel in case it 
                                                // isn't null (otherwise show the object picker dialog).
                                                if (meshToPaint.objectReferenceValue != null)
                                                {
                                                    EditorGUIUtility.PingObject(meshToPaint.objectReferenceValue);
                                                }
                                                else
                                                {
                                                    EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, string.Empty, 0);
                                                    currentObjectPickerIndex = i;
                                                }
                                                break;
                                            // Right mouse button for entry deletion.
                                            case 1:
                                                RemoveMeshToPaint(i);
                                                break;
                                            // Press the mouse wheel on an object entry to nullify it.
                                            case 2:
                                                NullifyMeshToPaint(i);
                                                break;
                                        }
                                        break;

                                    // Drag and drop behaviour:
                                    case EventType.DragUpdated:
                                    case EventType.DragPerform:

                                        if (DragAndDrop.objectReferences.Length > 1)
                                        {
                                            break;
                                        }

                                        DragAndDrop.visualMode = DragAndDrop.objectReferences[0] is GameObject ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;

                                        if (currentEvent.type == EventType.DragPerform)
                                        {
                                            DragAndDrop.AcceptDrag();

                                            if (DragAndDrop.objectReferences[0] is GameObject)
                                            {
                                                meshToPaint.objectReferenceValue = DragAndDrop.objectReferences[0];
                                            }
                                        }
                                        break;
                                }
                            }
                            GUILayout.Space(6f);

                            if (string.CompareOrdinal(Event.current.commandName, "ObjectSelectorUpdated") == 0)
                            {
                                meshes.GetArrayElementAtIndex(currentObjectPickerIndex).objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                {
                    currentEvent = Event.current;

                    Rect dropArea = GUILayoutUtility.GetRect(30f, 22f, GUILayout.Width(30f), GUILayout.Height(22f), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    dropArea.x += 3; dropArea.y += 3;

                    GUI.Box(dropArea, new GUIContent(EditorIcons.AddIcon, "Click here to add an empty field to the set, or drag one or more meshes here to add them to the set.\n\nWith your mouse over one of the fields, the following controls apply:\n\nLeft mouse button = ping object in project panel (or reassign object in case of a null field)\n\nMiddle mouse button = clear field (make it null)\n\nRight mouse button = delete field (removing it from the set entirely)"));
                    if (dropArea.Contains(currentEvent.mousePosition))
                    {
                        switch (currentEvent.type)
                        {
                            case EventType.MouseDown:

                                if (currentEvent.button == 0)
                                {
                                    AddMeshToPaint(null);
                                }
                                break;

                            case EventType.DragUpdated:
                            case EventType.DragPerform:

                                DragAndDrop.visualMode = DragAndDrop.objectReferences.Length == 1 && !(DragAndDrop.objectReferences[0] is GameObject) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Copy;

                                if (currentEvent.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();

                                    for (int ii = 0; ii < DragAndDrop.objectReferences.Length; ii++)
                                    {
                                        if (DragAndDrop.objectReferences[ii] is GameObject)
                                        {
                                            AddMeshToPaint(DragAndDrop.objectReferences[ii]);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    GUILayout.Space(3f);

                    GUI.enabled = active.boolValue && meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("-", "Removes the last field from the list."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 1)
                        {
                            RemoveMeshToPaint(meshes.arraySize - 1);
                        }
                    }
                    GUI.enabled = active.boolValue;

                    GUI.enabled = active.boolValue && meshes.arraySize > 0;
                    if (GUILayout.Button(new GUIContent("X", "Clears all values from all fields in the set of meshes to paint."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 0)
                        {
                            NullifySetOfMeshesToPaint();
                        }
                    }
                    GUI.enabled = active.boolValue;

                    GUI.enabled = active.boolValue && meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("C", "Clears the set of meshes to paint (deleting all fields at once)."), GUILayout.Width(30f), GUILayout.Height(22f)))
                    {
                        if (meshes.arraySize > 1)
                        {
                            ClearSetOfMeshesToPaint();
                        }
                    }
                    GUI.enabled = active.boolValue;

                    EditorGUIUtility.fieldWidth = 40f;
                    EditorGUILayout.LabelField("Icon size: ", GUILayout.Width(58f));
                    EditorGUILayout.Slider(previewIconSize, 40f, 80f, GUIContent.none, GUILayout.Width(90f), GUILayout.ExpandWidth(true));
                    EditorGUIUtility.fieldWidth = 0;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4f);

                EditorGUILayout.BeginHorizontal();
                {
                    classicUI.boolValue |= GUILayout.Button("Switch to classic UI", GUILayout.Height(22f));
                    GUI.enabled = meshes.arraySize > 1;
                    if (GUILayout.Button(new GUIContent("Clean unused fields", "Clean up the set of meshes to paint by removing all its empty (null) fields."), GUILayout.Height(22f)))
                    {
                        meshBrush.CleanSetOfMeshesToPaint();
                    }
                    GUI.enabled = active.boolValue;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(1.5f);
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(2);
        }

        void KeyBindingsInspector()
        {
            keyBindingsFoldout.boolValue = EditorGUILayout.Foldout(keyBindingsFoldout.boolValue, "Key bindings");

            if (keyBindingsFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Paint");
                    EditorGUILayout.PropertyField(paintKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Delete");
                    EditorGUILayout.PropertyField(deleteKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Combine meshes");
                    EditorGUILayout.PropertyField(combineKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Randomize meshes");
                    EditorGUILayout.PropertyField(randomizeKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Increase radius");
                    EditorGUILayout.PropertyField(increaseRadiusKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Decrease radius");
                    EditorGUILayout.PropertyField(decreaseRadiusKey, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Reset to default keys"))
                {
                    Undo.RegisterCompleteObjectUndo(meshBrush, "reset to default keys (MeshBrush)");
                    meshBrush.ResetKeyBindings();
                }
            }
        }

        void BrushSettingsInspector()
        {
            brushFoldout.boolValue = EditorGUILayout.Foldout(brushFoldout.boolValue, "Brush");

            if (brushFoldout.boolValue)
            {
                // Color picker for our circle brush.
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(new GUIContent("Color:", "Color of the circle brush."), GUILayout.Width(50f));
                    EditorGUILayout.PropertyField(color, GUIContent.none);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUIUtility.labelWidth = 75f;
                    {
                        EditorGUILayout.PropertyField(radius, new GUIContent("Radius [m]:", "Radius of the circle brush."));
                        GUILayout.Space(10f);
                    }
                    EditorGUIUtility.labelWidth = 0;

                    EditorGUILayout.PropertyField(useDensity, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Constant mesh density", "Define how many meshes per square meter should be painted, instead of a fixed number of meshes to paint per stroke.\n\nThis replaces the traditional number of meshes to paint value by keeping a constant mesh density throughout variable brush sizes.\n\nFor example:\nA mesh density value of 5 meshes/m\u00B2 with a 1 m\u00B2 brush would result in painting exactly five meshes. If we were now to increase the brush size to an area of 3 m\u00B2 (meaning a radius of 0.977 m, since the area of a circle is Pi times its radius squared), we would thus end up painting a total of\n5 meshes/m\u00B2 * 3 m\u00B2 = 15 meshes. Fifteen painted meshes in this case.\n\nAs you can see, this option lets you keep a constant mesh density even when painting with different brush sizes."), GUILayout.Width(140f));
                }
                EditorGUILayout.EndHorizontal();

                if (useDensity.boolValue)
                {
                    EditorGUILayout.LabelField("Mesh density [meshes/m\u00B2]:", GUILayout.Width(165f));
                    DrawMinMaxSlider(densityRange, 0.1f, maxDensityLimit.floatValue, 3);
                }
                else
                {
                    EditorGUILayout.LabelField("Quantity [meshes/stroke]:", GUILayout.Width(165f));
                    DrawMinMaxSlider(quantityRange, 1, maxQuantityLimit.intValue);
                }

                // Slider for the offset amount.
                EditorGUILayout.LabelField(new GUIContent("Offset amount [cm]:", "Offsets all the painted meshes away from their underlying surface.\n\nThis is useful if your meshes are stuck inside your GameObject's geometry, or floating above it.\nGenerally, if you place your pivot points carefully, you won't need this."));
                DrawMinMaxSlider(offsetRange, minOffsetLimit.floatValue, maxOffsetLimit.floatValue);

                GUI.enabled &= (quantityRange.vector2Value.y > 1 || useDensity.boolValue);

                // Slider for the scattering.
                EditorGUILayout.LabelField(new GUIContent("Scattering [%]:", "Percentage of how much the meshes are scattered away from the center of the circle brush.\n\n(Default is 60%)"));
                DrawMinMaxSlider(scatteringRange, 0f, 100f, 2);

                GUI.enabled = active.boolValue;

                // Slider for the delay between paint strokes.
                EditorGUILayout.Slider(delay, 0.03f, maxDelayLimit.floatValue, new GUIContent("Delay [s]:", "If you press and hold down the paint button, this value will define the delay (in seconds) between paint strokes; thus, the higher you set this value, the slower you'll be painting meshes."));

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(yAxisTangent, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Y-Axis tangent to surface", "As you decrease the slope influence value, you can choose whether you want your painted meshes to be kept upright along the Y-Axis, or tangent to their underlying surface.\n\nKeep this off when aligning meshes with the brush stroke direction!"), GUILayout.Width(155f));

                    EditorGUILayout.PropertyField(strokeAlignment, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Align with stroke", "Aligns all painted meshes with the direction of the brush stroke.\n\nThis only works if there is no random rotation set up (since the rotation randomizers are calculated AFTER the original placement of the meshes)."), GUILayout.Width(110f));

                    GUI.enabled &= strokeAlignment.boolValue;
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled = strokeAlignment.boolValue && lastPaintLocation.vector3Value != Vector3.zero && brushStrokeDirection.vector3Value != Vector3.zero;

                if (GUILayout.Button(new GUIContent("Reset brush stroke direction", "Resets the currently considered brush stroke direction."), GUILayout.Height(27f)))
                {
                    lastPaintLocation.vector3Value = Vector3.zero;
                    brushStrokeDirection.vector3Value = Vector3.zero;
                }

                GUI.enabled = active.boolValue;
            }
        }

        void SlopeSettingsInspector()
        {
            slopesFoldout.boolValue = EditorGUILayout.Foldout(slopesFoldout.boolValue, "Slope settings");

            if (slopesFoldout.boolValue)
            {
                EditorGUILayout.LabelField(new GUIContent("Slope influence [%]:", "Defines how much influence slopes have on the rotation of the painted meshes.\n\nA value of 100% for example would adapt the painted meshes to be perfectly perpendicular to the surface beneath them, whereas a value of 0% would keep them upright at all times."));
                DrawMinMaxSlider(slopeInfluenceRange, 0f, 100f, 2);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(useSlopeFilter, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField("Use slope filter");
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled &= useSlopeFilter.boolValue;

                EditorGUILayout.LabelField(new GUIContent("Angle threshold [°]:", "Avoids the placement of meshes on slopes and hills whose angles exceed this value.\nA low value of 20° for example would restrain the painting of meshes onto very flat surfaces, while the maximum value of 180° would deactivate the slope filter completely."));

                DrawMinMaxSlider(angleThresholdRange, 1.0f, 180.0f, 3);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(inverseSlopeFilter, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Inverse slope filter", "Inverts the slope filter's functionality; low values of the filter would therefore focus the placement of meshes onto slopes instead of avoiding it."), GUILayout.Width(110f));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(manualReferenceVectorSampling, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Manual reference vector sampling", "You can choose to manually sample a reference slope vector, whose direction will then be used by the slope filter instead of the world's Y-Up axis, to further help you paint meshes with the slope filter applied onto arbitrary geometry (like for instance painting onto huge round planet-meshes, concave topologies like caves etc...).\n\nTo sample one, enter the reference vector sampling mode by clicking the 'Sample reference vector' button below."), GUILayout.Width(200f));
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled &= manualReferenceVectorSampling.boolValue != false;

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(showReferenceVectorInSceneView, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField("Show sampled vector", GUILayout.Width(130f));

                    GUI.color = brushMode == BrushMode.SampleReferenceVector ? Color.yellow : Color.white;
                    if (GUILayout.Button(new GUIContent("Sample reference vector", "Activates the reference vector sampling mode, which allows you to pick a normal vector of your mesh to use as a reference by the slope filter.\n\nTo sample from more than one reference normal vector, keep the shift key pressed whilst sampling: the averaged vector of all samples will be selected.\n\nPress " + meshBrush.paintKey + " to sample a vector.\nPress Esc to cancel the sampling and return to the regular mesh painting mode.\n(Deselecting and reselecting this object will also exit the sampling mode)"), GUILayout.Height(27f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                    {
                        brushMode = BrushMode.SampleReferenceVector;
                    }
                    GUI.color = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled = active.boolValue;

                if (GUILayout.Button("Reset all slope settings", GUILayout.Height(27f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(meshBrush, "reset all slope settings (MeshBrush)");
                    meshBrush.ResetSlopeSettings();
                }
            }
        }

        void RandomizersInspector()
        {
            randomizersFoldout.boolValue = EditorGUILayout.Foldout(randomizersFoldout.boolValue, new GUIContent("Randomizers", "Give your painted meshes a unique look in your scene by randomizing some of their properties, like for instance rotation, scale, etc..."));

            if (randomizersFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(uniformRandomScale, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Scale uniformly", "Applies the scale uniformly along all three XYZ axes."), GUILayout.Width(100f));
                }
                EditorGUILayout.EndHorizontal();

                if (uniformRandomScale.boolValue)
                {
                    EditorGUILayout.LabelField(new GUIContent("Random scale [XYZ]:", "Sets the scale of the painted meshes to a random value chosen from the specified range.\nThe scale will be applied uniformly across all three axes; X, Y and Z."), GUILayout.Width(140f));
                    DrawMinMaxSlider(randomScaleRange, 0.0f, maxRandomScaleLimit.floatValue);
                }
                else
                {
                    EditorGUILayout.LabelField(new GUIContent("Random scale [X]:", "Randomly scales the painted meshes along their X axis between these two minimum/maximum scale values."), GUILayout.Width(115f));
                    DrawMinMaxSlider(randomScaleRangeX, 0.01f, maxRandomScaleLimit.floatValue);
                    EditorGUILayout.LabelField(new GUIContent("Random scale [Y]:", "Randomly scales the painted meshes along their Y axis between these two minimum/maximum scale values."), GUILayout.Width(115f));
                    DrawMinMaxSlider(randomScaleRangeY, 0.01f, maxRandomScaleLimit.floatValue);
                    EditorGUILayout.LabelField(new GUIContent("Random scale [Z]:", "Randomly scales the painted meshes along their Z axis between these two minimum/maximum scale values."), GUILayout.Width(115f));
                    DrawMinMaxSlider(randomScaleRangeZ, 0.01f, maxRandomScaleLimit.floatValue);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUIUtility.fieldWidth = 3.0f;
                    EditorGUIUtility.labelWidth = 130.0f;
                    EditorGUILayout.CurveField(randomScaleCurve, color.colorValue, new Rect(0.0f, 0.0f, 1.0f, maxRandomScaleLimit.floatValue), new GUIContent("Random scale curve:", "This is a curve that controls the scale of the painted meshes based on their distance to the brush circle's center.\n\nTime=0 (left side of curve) translates to the circle brush center;\n\nTime=1 (right side of curve) is the outer edge of the circle brush.\n\nThe scale value of the painted meshes will be multiplied by the established curve value, meaning that if the curve value is 1, then the scale will be unaffected by the curve.\n\nThis can be useful if you want to achieve an effect like for example shrinking the painted meshes the closer they get to the brush circle's edge."));

                    EditorGUIUtility.labelWidth = 90.0f;
                    EditorGUILayout.PropertyField(randomScaleCurveVariation, new GUIContent("Variation [±]:", "The allowed error margin for the scale curve value (a random value will be picked between -variation and +variation and added to the evaluated curve value).\n\nIt's best to keep this value low (between 0 and maybe 0.2). Setting this value too high would make the scale curve meaningless in the first place, since it would randomize the scale of the painted meshes in a way such that the effects of the curve driven scale aren't even visible anymore."));

                    EditorGUIUtility.labelWidth = EditorGUIUtility.fieldWidth = 0;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(new GUIContent("Random rotation amount [%]:", "Applies a random rotation around the local X, Y and Z-axis of the painted meshes."));

                EditorGUILayout.LabelField(new GUIContent("X"));
                DrawMinMaxSlider(randomRotationRangeX, 0.0f, 100.0f, 2);
                EditorGUILayout.LabelField(new GUIContent("Y"));
                DrawMinMaxSlider(randomRotationRangeY, 0.0f, 100.0f, 2);
                EditorGUILayout.LabelField(new GUIContent("Z"));
                DrawMinMaxSlider(randomRotationRangeZ, 0.0f, 100.0f, 2);

                EditorGUILayout.LabelField("Randomizer brush:");

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(positionBrushRandomizer, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Position", "Should the randomizer brush stroke affect the position of the painted meshes? If yes, the randomization will occur based on the established scattering percentage range values."), GUILayout.Width(57f));

                    EditorGUILayout.PropertyField(rotationBrushRandomizer, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Rotation", "Should the randomizer brush stroke affect the rotation of the painted meshes around their local-y axis?"), GUILayout.Width(60f));

                    EditorGUILayout.PropertyField(scaleBrushRandomizer, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Scale", "Should the randomizer brush stroke affect the scale of the painted meshes? If yes, the scale will be recalculated based on the defined randomization settings as well as the curve based multiplier."), GUILayout.Width(37f));
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button(new GUIContent("Reset all randomizers", "Resets all the randomization parameters back to zero."), GUILayout.Height(27f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(meshBrush, "reset all randomizers (MeshBrush)");
                    meshBrush.ResetRandomizers();
                }
            }
        }

        void OverlapFilterInspector()
        {
            overlapFilterFoldout.boolValue = EditorGUILayout.Foldout(overlapFilterFoldout.boolValue, new GUIContent("Overlap filter", "You can teach your painted meshes not to overlap each other with this useful filter."));

            if (overlapFilterFoldout.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(useOverlapFilter, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent(" Use overlap filter", "Activates or deactivates the overlap filter.\n\nWarning:\nThe overlap filter can be a pretty performance heavy feature, whose operations may slow down your PC when used in large groups of meshes.\n\nTo reduce the overhead whilst painting, it is recommended to only activate this feature when it is really strictly needed and in small groups (it's way better to have many small MeshBrush groups instead of one huge container group with all painted meshes under it)."), GUILayout.Width(111.5f));
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled = useOverlapFilter.boolValue;

                EditorGUILayout.LabelField(new GUIContent("Min. absolute distance [m]:", "The minimum absolute distance (in meters) between painted meshes."), GUILayout.Width(163f));

                DrawMinMaxSlider(minimumAbsoluteDistanceRange, 0.0f, maxMinimumAbsoluteDistanceLimit.floatValue, 4);

                if (GUILayout.Button(new GUIContent("Reset overlap filter settings", "Resets all overlap filter settings back to their default values."), GUILayout.Height(27f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(meshBrush, "reset overlap filter settings (MeshBrush)");
                    meshBrush.ResetOverlapFilterSettings();
                }
            }

            GUI.enabled = active.boolValue;
        }

        void AdditiveScaleInspector()
        {
            additiveScaleFoldout.boolValue = EditorGUILayout.Foldout(additiveScaleFoldout.boolValue, new GUIContent("Apply additive scale", "Applies a constant, fixed amount of 'additive' scale after the meshes have been placed."));

            if (additiveScaleFoldout.boolValue)
            {
                EditorGUILayout.PropertyField(uniformAdditiveScale, new GUIContent("Scale uniformly", "Applies the scale uniformly along all three XYZ axes."));

                if (uniformAdditiveScale.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Add to scale:", GUILayout.Width(75f));
                    DrawMinMaxSlider(additiveScaleRange, 0.0f, maxAdditiveScaleLimit.floatValue);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.PropertyField(additiveScaleNonUniform, new GUIContent("Add to scale:"));
                }

                if (GUILayout.Button("Reset additive scale", GUILayout.Height(27f), GUILayout.Width(150f), GUILayout.ExpandWidth(true)))
                {
                    meshBrush.ResetAdditiveScale();
                }
            }
        }

        void OptimizationInspector()
        {
            optimizationFoldout.boolValue = EditorGUILayout.Foldout(optimizationFoldout.boolValue, "Optimize");

            if (optimizationFoldout.boolValue)
            {
                // Create 2 buttons for quickly flagging/unflagging all painted meshes as static...
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("Flag all painted\nmeshes as static", "Flags all the meshes you've painted so far as static in the editor.\nCheck out the Unity documentation about drawcall batching if you don't know what this is good for."), GUILayout.Height(50f), GUILayout.ExpandWidth(true)) && meshBrushParent)
                        meshBrushParent.FlagMeshesAsStatic();
                    if (GUILayout.Button("Unflag all painted\nmeshes as static", GUILayout.Height(50f), GUILayout.ExpandWidth(true)) && meshBrushParent)
                        meshBrushParent.UnflagMeshesAsStatic();
                }
                EditorGUILayout.EndHorizontal();

                // ...and 2 other buttons for combining and deleting.
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("Combine all\npainted meshes", "Once you're done painting meshes, you can click here to combine them all. This will combine all the meshes you've painted into one single mesh (one per material).\n\nVery useful for performance optimization.\nCannot be undone."), GUILayout.Height(50f), GUILayout.ExpandWidth(true)))
                    {
                        if (meshBrush.HolderObj != null)
                        {
                            meshBrushParent.CombinePaintedMeshes(autoSelectOnCombine.boolValue, meshBrush.HolderObj.GetComponentsInChildren<MeshFilter>());
                            UpdateTrisCounter();
                        }
                    }

                    //...and one to delete all the meshes we painted on this GameObject so far.
                    if (GUILayout.Button(new GUIContent("Delete all\npainted meshes", "Are you sure? This will delete all the meshes you've painted onto this GameObject's surface so far (except already combined meshes)."), GUILayout.Height(50f), GUILayout.ExpandWidth(true)) && meshBrushParent)
                    {
                        Undo.DestroyObjectImmediate(meshBrush.HolderObj.gameObject);
                        meshBrush.GatherPaintedMeshes();
                        UpdateTrisCounter();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(autoIgnoreRaycast, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Auto-ignore raycast", "Should all painted meshes be automatically set to the Ignore Raycast layer?\n\nThis can be useful if you want to avoid the meshes to stack up on each other when painting in global painting mode on the same spot over and over."), GUILayout.Width(155f));

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(autoSelectOnCombine, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField(new GUIContent("Auto-select combined mesh", "Automatically select the combined mesh GameObject after combining the meshes inside the brush area."), GUILayout.Width(180f));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(autoStatic, GUIContent.none, GUILayout.Width(15f));
                    EditorGUILayout.LabelField("Auto-flag painted meshes as static", GUILayout.Width(210f));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws a minimum-maximum range slider with automatic float fields 
        /// to control the left and right range values precisely and directly in the inspector.
        /// </summary>
        /// <param name="rangeVector2">The SerializedProperty of the Vector2 holding the range values</param>
        /// <param name="minLimit">Minimum range limit.</param>
        /// <param name="maxLimit">Maximum range limit.</param>
        /// <param name="fractionalDigits">If this is greater than 0, round the slider values to this amount of fractional digits after the point.<para> </para>Negative values (including zero) will be ignored, thus using the standard full amount of fractional digits.</param>
        void DrawMinMaxSlider(SerializedProperty rangeVector2, float minLimit, float maxLimit, int fractionalDigits = -1)
        {
            float min = rangeVector2.vector2Value.x;
            float max = rangeVector2.vector2Value.y;

            EditorGUILayout.BeginHorizontal();
            {
                min = EditorGUILayout.FloatField(GUIContent.none, min, GUILayout.Width(50.0f));
                if (min >= max) min = max;
                EditorGUILayout.MinMaxSlider(ref min, ref max, minLimit, maxLimit);
                max = EditorGUILayout.FloatField(GUIContent.none, max, GUILayout.Width(50.0f));
                if (max <= min) max = min;
            }
            EditorGUILayout.EndHorizontal();

            if (fractionalDigits > 0)
            {
                min = (float)Math.Round(min, fractionalDigits);
                max = (float)Math.Round(max, fractionalDigits);
            }

            rangeVector2.vector2Value = new Vector2(min, max);
        }

        /// <summary>
        /// Draws a minimum-maximum range slider with automatic int fields 
        /// to control the left and right range values precisely and directly in the inspector.<para> </para>
        /// This slider will snap to round integer values (no floating point).
        /// </summary>
        /// <param name="rangeVector2">The SerializedProperty of the Vector2 holding the range values.</param>
        /// <param name="minLimit">Minimum range limit.</param>
        /// <param name="maxLimit">Maximum range limit.</param>
        void DrawMinMaxSlider(SerializedProperty rangeVector2, int minLimit, int maxLimit)
        {
            float min = rangeVector2.vector2Value.x;
            float max = rangeVector2.vector2Value.y;

            EditorGUILayout.BeginHorizontal();
            {
                min = EditorGUILayout.FloatField(GUIContent.none, min, GUILayout.Width(50.0f));
                if (min >= max) min = max;
                EditorGUILayout.MinMaxSlider(ref min, ref max, minLimit, maxLimit);
                max = EditorGUILayout.FloatField(GUIContent.none, max, GUILayout.Width(50.0f));
                if (max <= min) max = min;
            }
            EditorGUILayout.EndHorizontal();

            rangeVector2.vector2Value = new Vector2(Mathf.Floor(min), Mathf.Floor(max));
        }

        void UpdateTrisCounter()
        {
            if (meshBrushParent == null)
            {
                OnEnable();
            }

            totalPaintedMeshes = meshBrushParent.GetMeshCount();
            totalPaintedTris = meshBrushParent.GetTrisCount();
        }

        #endregion

        #region Set of meshes to paint operations

        void NullifySetOfMeshesToPaint()
        {
            for (int i = 0; i < meshes.arraySize; i++)
            {
                meshes.GetArrayElementAtIndex(i).objectReferenceValue = null;
            }
        }

        void ClearSetOfMeshesToPaint()
        {
            meshes.ClearArray();
            meshes.arraySize = 0;
            AddMeshToPaint(null);
        }

        void AddMeshToPaint(UnityEngine.Object meshToPaint)
        {
            meshes.arraySize++;
            meshes.GetArrayElementAtIndex(meshes.arraySize - 1).objectReferenceValue = meshToPaint;
        }

        void NullifyMeshToPaint(int index)
        {
            meshes.GetArrayElementAtIndex(index).objectReferenceValue = null;
        }

        void RemoveMeshToPaint(int index)
        {
            if (meshes.arraySize == 1)
                return;

            var meshToPaint = meshes.GetArrayElementAtIndex(index).objectReferenceValue;
            if (meshToPaint != null)
            {
                meshes.DeleteArrayElementAtIndex(index);
            }
            meshes.DeleteArrayElementAtIndex(index);
        }

        #endregion

        void OnSceneGUI()
        {
            // Only enable the meshbrush when the user sets the 
            // specific instance to enabled (through the toggle in the inspector).
            if (active.boolValue)
            {
                currentEvent = Event.current;

                if (lockSceneView.boolValue)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                }

                if (!globalPaintingMode.boolValue && meshBrush.CachedCollider == null)
                {
                    return;
                }

                Handles.color = color.colorValue;

                activeBrushMode();

                switch (brushMode)
                {
                    default:
                        activeBrushMode = BrushMode_MeshPaint;
                        break;
                    case BrushMode.PrecisionPlacement:
                        activeBrushMode = BrushMode_PrecisionPlacement;
                        break;
                    case BrushMode.SampleReferenceVector:
                        activeBrushMode = BrushMode_SampleReferenceVector;
                        break;
                }

                switch (currentEvent.type)
                {
                    case EventType.KeyDown:
                        if (currentEvent.keyCode == (KeyCode) increaseRadiusKey.intValue)
                        {
                            radiusModAcceleration += 5.25f * Time.deltaTime;
                            radius.floatValue += 0.35f * radiusModAcceleration * Time.deltaTime;
                            currentEvent.Use();
                            serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                        else if (currentEvent.keyCode == (KeyCode) decreaseRadiusKey.intValue && radius.floatValue > float.Epsilon)
                        {
                            radiusModAcceleration += 5.25f * Time.deltaTime;
                            radius.floatValue -= 0.35f * radiusModAcceleration * Time.deltaTime;
                            currentEvent.Use();
                            serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                        break;
                    case EventType.KeyUp:
                        if (currentEvent.keyCode == (KeyCode) increaseRadiusKey.intValue || currentEvent.keyCode == (KeyCode) decreaseRadiusKey.intValue)
                        {
                            radiusModAcceleration = 1.0f;
                        }
                        break;
                }

                // Draw the custom sampled reference slope vector in the scene view (given that the user wants it to appear and he is actually using the slope filter at all)...
                if (showReferenceVectorInSceneView.boolValue && manualReferenceVectorSampling.boolValue && useSlopeFilter.boolValue)
                {
                    Handles.ArrowHandleCap(0, slopeReferenceVectorSampleLocation.vector3Value, Quaternion.LookRotation(slopeReferenceVector.vector3Value), 0.9f, EventType.Repaint);
                }
            }
        }

        #region Brush mode functions

        void BrushMode_MeshPaint() // This method represents the MeshPaint mode for the brush. This is the default brush mode.
        {
            currentEvent = Event.current;

            // Only cast rays if we have our object selected (for the sake of performance).
            if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.transform == meshBrush.CachedTransform)
            {
                // Shoot the ray through the 2D mouse position on the scene view window.
                ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                if (globalPaintingMode.boolValue ? Physics.Raycast(ray, out hit, Mathf.Infinity, globalPaintLayerMask) : meshBrush.CachedCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    // Filter out the unselected layers (for global painting mode).
                    if (globalPaintingMode.boolValue && !layerMask.GetArrayElementAtIndex(hit.transform.gameObject.layer).boolValue)
                    {
                        return;
                    }

                    // Constantly update scene view at this point 
                    // (to avoid the circle handle jumping around as we click in and out of the scene view).
                    SceneView.RepaintAll();

                    // Thanks to the RepaintAll() function above, the circle handle that we draw here gets updated at all times inside our scene view.
                    Handles.DrawWireDisc(hit.point, hit.normal, radius.floatValue);

                    // If a paint stroke is possible (depends on the delay defined in the inspector), 
                    // call the corresponding function when the user presses the paint, delete, randomize or combine button. 
                    switch (currentEvent.type)
                    {
                        case EventType.MouseDown:
                        case EventType.MouseDrag:

                            if (meshBrush.paintKey == KeyCode.Mouse0 && currentEvent.button == 0 && currentEvent.modifiers == EventModifiers.None)
                                meshBrush.PaintMeshes(hit);
                            else if (meshBrush.deleteKey == KeyCode.Mouse0 && currentEvent.button == 0 && currentEvent.modifiers == EventModifiers.None)
                                meshBrush.DeleteMeshes(hit);
                            else if (meshBrush.randomizeKey == KeyCode.Mouse0 && currentEvent.button == 0)
                                meshBrush.RandomizeMeshes(hit);
                            else if (currentEvent.type != EventType.MouseDrag && meshBrush.combineKey == KeyCode.Mouse0 && currentEvent.button == 0)
                                meshBrush.CombineMeshes(hit);

                            UpdateTrisCounter();
                            Repaint();

                            break;
                        case EventType.KeyDown:

                            if (currentEvent.keyCode == meshBrush.paintKey)
                            {
                                meshBrush.PaintMeshes(hit);
                                currentEvent.Use();
                            }
                            else if (currentEvent.keyCode == meshBrush.deleteKey)
                            {
                                meshBrush.DeleteMeshes(hit);
                                currentEvent.Use();
                            }
                            else if (currentEvent.keyCode == meshBrush.combineKey)
                            {
                                meshBrush.CombineMeshes(hit);
                                currentEvent.Use();
                            }
                            else if (currentEvent.keyCode == meshBrush.randomizeKey)
                            {
                                meshBrush.RandomizeMeshes(hit);
                                currentEvent.Use();
                            }

                            UpdateTrisCounter();
                            Repaint();

                            break;
                    }
                }
            }
        }

        #region Precision placement mode relevant variables

        int _i = 0; int _phase = 0;
        Vector2 lastMousePos; GameObject previewObj;

        #endregion

        void BrushMode_PrecisionPlacement() // Mode for the precise placement of single meshes.
        {
            currentEvent = Event.current;
            for (int i = 0; i < meshes.arraySize; i++)
            {
                var arrayElement = meshes.GetArrayElementAtIndex(i);

                // Here the user can choose to cancel the precision placement mode and return to 
                // the regular painting mode by pressing escape. The same happens (in conjuction
                // with an error dialog) if the user forgets to assign one or more fields in the set of meshes to paint.
                if (arrayElement.objectReferenceValue == null || (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape))
                {
                    if (arrayElement.objectReferenceValue == null)
                        EditorUtility.DisplayDialog("Warning!", "One or more fields in the set of meshes to paint is empty. Please assign something to all fields before painting.", "Okay");

                    if (previewObj != null)
                    {
                        DestroyImmediate(previewObj);
                        previewObj = null;
                    }
                    _phase = 0;
                    lastMousePos = Vector2.zero;
                    brushMode = BrushMode.Paint;

                    Repaint();
                    return;
                }
            }

            var selectedArrayElement = meshes.GetArrayElementAtIndex(_i);
            var selectedArrayElementGameObject = (GameObject)selectedArrayElement.objectReferenceValue;
            var selectedArrayElementGameObjectName = selectedArrayElementGameObject.name;

            if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.transform == meshBrush.CachedTransform)
            {
                int layer = selectedArrayElementGameObject.layer;
                ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

                if (globalPaintingMode.boolValue ? Physics.Raycast(ray, out hit, Mathf.Infinity, globalPaintLayerMask) : meshBrush.CachedCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    SceneView.RepaintAll();

                    // Initiate the preview mesh object...
                    if (previewObj == null)
                    {
                        previewObj = (GameObject)PrefabUtility.InstantiatePrefab(selectedArrayElement.objectReferenceValue);
                        previewObj.transform.position = hit.point;
                        previewObj.transform.rotation = Quaternion.identity;

                        previewObj.name = "Preview";
                        previewObj.layer = 2;
                        previewObj.transform.parent = meshBrush.HolderObj;

                        previewObj.isStatic |= autoStatic.boolValue;
                    }

                    // Cycle through the set of meshes to paint 
                    // and select a GameObject to place with the left and right arrow keys.
                    if (meshes.arraySize > 1)
                    {
                        if (currentEvent.type == EventType.KeyDown && _phase < 1)
                            switch (currentEvent.keyCode)
                            {
                                case KeyCode.B:
                                    _i--;
                                    if (previewObj != null)
                                    {
                                        DestroyImmediate(previewObj);
                                        previewObj = null;
                                    }
                                    break;
                                case KeyCode.N:
                                    _i++;
                                    if (previewObj != null)
                                    {
                                        DestroyImmediate(previewObj);
                                        previewObj = null;
                                    }
                                    break;
                            }

                        if (_i < 0) _i = meshes.arraySize - 1;
                        if (_i >= meshes.arraySize) _i = 0;
                    }
                    else _i = 0;
                    _i = Mathf.Clamp(_i, 0, meshes.arraySize - 1);
                }

                switch (_phase)
                {
                    case 0:
                        // Choose a precise location for the mesh inside the scene view.
                        if (previewObj != null)
                        {
                            previewObj.transform.position = hit.point;
                            previewObj.transform.up = Vector3.Lerp(Vector3.up, hit.normal, Mathf.Lerp(slopeInfluenceRange.vector2Value.x, slopeInfluenceRange.vector2Value.y, 0.5f) * 0.01f);
                            Handles.Label(hit.point + Vector3.right + Vector3.up, "Currently selected: " + selectedArrayElementGameObjectName + "\nSelect next: [N]  /  Select previous: [B]\nConfirm location: [" + meshBrush.paintKey + "]  /  Cancel placement: [ESC]", EditorStyles.helpBox);
                        }

                        // Confirm placement location and go to the next phase.
                        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == meshBrush.paintKey || (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && meshBrush.paintKey == KeyCode.Mouse0))
                        {
                            lastMousePos = currentEvent.mousePosition;
                            _phase = 1;
                        }

                        break;
                    case 1:
                        // Adjust the scale by dragging around the mouse.
                        if (previewObj != null)
                        {
                            Vector2 delta = lastMousePos - currentEvent.mousePosition;
                            if (delta != Vector2.zero) previewObj.transform.localScale = Vector3.one * delta.magnitude * 0.01f;
                            Handles.Label(hit.point + Vector3.right + Vector3.up, "Currently selected: " + selectedArrayElementGameObjectName + "\nAdjust the scale by dragging the mouse away from the center\nConfirm scale: [" + meshBrush.paintKey + "]  /  Cancel placement: [ESC]", EditorStyles.helpBox);
                        }

                        // Confirm the adjusted scale and move over to the rotation phase.
                        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == meshBrush.paintKey || (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && meshBrush.paintKey == KeyCode.Mouse0))
                        {
                            lastMousePos = currentEvent.mousePosition;
                            _phase = 2;
                        }

                        break;
                    case 2:
                        // Adjust the rotation (along the Y axis) by dragging aroung the mouse. 
                        if (previewObj != null)
                        {
                            float yRot = Event.current.delta.magnitude;
                            if (Event.current.delta.x > 0) yRot *= -1;

                            if (yRot < -360.0f) yRot += 360.0f;
                            if (yRot > 360.0f) yRot -= 360.0f;

                            if (Mathf.Abs(yRot) > 0.0f)
                                previewObj.transform.Rotate(new Vector3(0.0f, yRot, 0.0f), Space.Self);

                            Handles.Label(hit.point + Vector3.right + Vector3.up, "Currently selected: " + selectedArrayElementGameObjectName + "\nAdjust the rotation along the Y-Axis by dragging your mouse horizontally\nConfirm rotation: [" + meshBrush.paintKey + "]  /  Cancel placement: [ESC]", EditorStyles.helpBox);
                        }

                        // Confirm the adjusted rotation and move to the last phase: vertical offset.
                        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == meshBrush.paintKey || (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && meshBrush.paintKey == KeyCode.Mouse0))
                        {
                            lastMousePos = currentEvent.mousePosition;
                            _phase = 3;
                        }

                        break;
                    case 3:
                        // In this step of the placement we adjust the vertical offset along the local Y axis.
                        if (previewObj != null)
                        {
                            float yPos = currentEvent.delta.magnitude;
                            if (currentEvent.delta.y > 0) yPos *= -1;

                            if (Mathf.Abs(yPos) > 0.0f)
                                previewObj.transform.Translate(new Vector3(0.0f, yPos * 0.005f, 0.0f), Space.Self);

                            Handles.Label(hit.point + Vector3.right + Vector3.up, "Currently selected: " + selectedArrayElementGameObjectName + "\nAdjust the offset along the Y-axis by dragging your mouse vertically\nConfirm offset: [" + meshBrush.paintKey + "]  /  Cancel placement: [ESC]\nPlace another mesh: hold down [Shift] when confirming", EditorStyles.helpBox);
                        }

                        // Final placement confirmation
                        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == meshBrush.paintKey || (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && meshBrush.paintKey == KeyCode.Mouse0))
                        {
                            GameObject finalObj = PrefabUtility.InstantiatePrefab(selectedArrayElement.objectReferenceValue) as GameObject;

                            finalObj.transform.position = previewObj.transform.position;
                            finalObj.transform.rotation = previewObj.transform.rotation;

                            finalObj.transform.localScale = previewObj.transform.localScale;
                            finalObj.name = selectedArrayElementGameObjectName;
                            finalObj.layer = layer;
                            finalObj.transform.parent = meshBrush.HolderObj;

                            finalObj.isStatic |= autoStatic.boolValue;

                            if (previewObj != null)
                            {
                                DestroyImmediate(previewObj);
                            }

                            // Allow the placement to be undone.
                            Undo.RegisterCreatedObjectUndo(finalObj, "precision placement (MeshBrush)");

                            _phase = 0;
                            previewObj = null;
                            lastMousePos = Vector2.zero;

                            UpdateTrisCounter();
                            Repaint();

                            // Automatically jump back to the paint brush mode.
                            brushMode = currentEvent.shift ? BrushMode.PrecisionPlacement : BrushMode.Paint;
                        }
                        break;
                }
            }
        }

        #region Reference vector sampling variables

        readonly List<Vector3> referenceVectors = new List<Vector3>(7);
        float nextReferenceVectorsFlicker;
        bool referenceVectorsFlickerState;

        #endregion

        // This one represents the vector sampling mode. 
        // This brushmode allows the user to sample a custom defined slope reference 
        // vector used by the slope filter... Check out the tutorial to find out what this does in case you're confused.
        void BrushMode_SampleReferenceVector()
        {
            if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.transform == meshBrush.CachedTransform)
            {
                ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                if (globalPaintingMode.boolValue ? Physics.Raycast(ray, out hit, Mathf.Infinity, globalPaintLayerMask) : meshBrush.CachedCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    SceneView.RepaintAll();
                    // Sample the reference vector for the slope filter.
                    if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == meshBrush.paintKey || (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && meshBrush.paintKey == KeyCode.Mouse0))
                    {
                        // Averaged reference vector sampling (holding shift key).
                        if (currentEvent.shift)
                        {
                            referenceVectors.Add(hit.normal);
                        }
                        else
                        {
                            meshBrush.SampleReferenceVector(referenceVectors.Count > 0 ? GetAveragedVector(referenceVectors) : hit.normal, hit.point);
                            referenceVectors.Clear();

                            // Jump back to the meshpaint mode automatically.
                            brushMode = BrushMode.Paint;
                        }
                    }

                    // Allow the action to be undone.
                    Undo.RecordObject(meshBrush, "reference vector sampling (MeshBrush)");
                    if (referenceVectors.Count > 0)
                    {
                        if (nextReferenceVectorsFlicker < Time.realtimeSinceStartup)
                        {
                            nextReferenceVectorsFlicker = Time.realtimeSinceStartup + 0.2f;
                            referenceVectorsFlickerState = !referenceVectorsFlickerState;
                        }

                        if (referenceVectorsFlickerState)
                        {
                            for (int i = referenceVectors.Count - 1; i >= 0; i--)
                            {
                                Handles.ArrowHandleCap(0, hit.point, Quaternion.LookRotation(referenceVectors[i]), 0.4f, EventType.Repaint);
                            }
                        }

                        Handles.ArrowHandleCap(0, hit.point, Quaternion.LookRotation(GetAveragedVector(referenceVectors)), 0.9f, EventType.Repaint);
                    }
                    else
                    {
                        Handles.ArrowHandleCap(0, hit.point, Quaternion.LookRotation(hit.normal), 0.9f, EventType.Repaint);
                    }

                    // Cancel the sampling mode by pressing the escape button.
                    if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape)
                    {
                        referenceVectors.Clear();
                        brushMode = BrushMode.Paint;
                        Repaint();
                    }
                }
            }
        }

        #endregion

        Vector3 GetAveragedVector(List<Vector3> inputVectors)
        {
            if (inputVectors == null || inputVectors.Count == 0)
            {
                Debug.LogError("MeshBrush: The passed input vectors list is null or empty... couldn't return averaged vector.");
                return default(Vector3);
            }

            Vector3 averagedVector = Vector3.zero;
            for (int i = inputVectors.Count - 1; i >= 0; i--)
            {
                averagedVector += inputVectors[i];
            }
            averagedVector /= inputVectors.Count;

            return averagedVector.normalized;
        }
    }
}

// Copyright (C) Raphael Beck | Glitched Polygons, 2019
