using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Random = UnityEngine.Random;

using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MeshBrush
{
    /// <summary>
    /// MeshBrush component that provides functionality for painting, deleting and combining meshes in your scenes.
    /// </summary>
    public class MeshBrush : MonoBehaviour
    {
        /// <summary>
        /// The current version of MeshBrush.
        /// </summary>
        public const float version = 2.0f;

        #region MeshBrush instance settings

        /// <summary>
        /// Activates or deactivates this MeshBrush instance.
        /// </summary>
        public bool active = true;

        /// <summary>
        /// The name for this MeshBrush group (and also its holder object).
        /// </summary>
        public string groupName = "<group name>";

        /// <summary>
        /// Whether the layers section should be shown or collapsed inside a Global Painting Mode inspector.
        /// </summary>
        public bool showGlobalPaintingLayersInspector = true;

        /// <summary>
        /// Global painting layer mask.
        /// </summary>
        public bool[] layerMask = new bool[32]
        {
            true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true
        };

        /// <summary>
        /// The brush radius.
        /// </summary>
        public float radius = 0.3f;

        /// <summary>
        /// The color of the brush helper handle.
        /// </summary>
        public Color color = Color.white;

        /// <summary>
        /// The range within which to choose the amount of meshes to paint.
        /// </summary>
        public Vector2 quantityRange = Vector2.one;

        /// <summary>
        /// Keep a constant mesh density inside the circle brush area with this option.
        /// </summary>
        public bool useDensity;

        /// <summary>
        /// The range within which to choose the mesh density value in meshes/m².
        /// </summary>
        public Vector2 densityRange = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// The delay between paint strokes when holding down your paint button.
        /// </summary>
        public float delay = 0.25f;

        /// <summary>
        /// A float variable for the minimum vertical offset of the mesh we are going to paint.<para> </para> 
        /// You probably won't ever need this if you place the pivot of your meshes nicely, but you never know.
        /// </summary>
        public Vector2 offsetRange;

        /// <summary>
        /// The range within which to choose the float value for the influence 
        /// that the underlying surface normal should have on the painted meshes.
        /// </summary>
        public Vector2 slopeInfluenceRange = new Vector2(95f, 100f);

        /// <summary>
        /// Activate/deactivate the slope filter.
        /// </summary>
        public bool useSlopeFilter;

        /// <summary>
        /// The range within which to choose the float value for the slope filter's angle threshold value
        /// (adjust this to avoid having meshes painted on steep surfaces such as cliffs and hills).
        /// </summary>
        public Vector2 angleThresholdRange = new Vector2(25f, 30f);

        /// <summary>
        /// Invert the slope filter functionality with ease.
        /// </summary>
        public bool inverseSlopeFilter;

        /// <summary>
        /// The sampled reference slope vector.
        /// </summary>
        public Vector3 slopeReferenceVector = Vector3.up;

        /// <summary>
        /// The point in space where we sampled our reference slope vector.
        /// </summary>
        public Vector3 slopeReferenceVectorSampleLocation = Vector3.zero;

        /// <summary>
        /// Determines if the local Y-Axis of painted meshes should be kept tangent to their underlying surface or not.
        /// </summary>
        public bool yAxisTangent;

        /// <summary>
        /// Aligns the painted meshes with the direction of the brush stroke 
        /// (only works if there is no rotation randomizer set up).
        /// </summary>
        public bool strokeAlignment;

        /// <summary>
        /// Should all painted meshes be automatically set to the Ignore Raycast layer?<para> </para>
        /// This is useful if you want to avoid the meshes to stack up on each other when painting on the same spot over and over.
        /// </summary>
        public bool autoIgnoreRaycast;

        /// <summary>
        /// The range within which to choose the percentage of scattering applied to the painted meshes 
        /// (how much they spread out from the center of the circle brush).
        /// </summary>
        public Vector2 scatteringRange = new Vector2(70f, 80f);

        /// <summary>
        /// Activate/deactivate the overlap filter.
        /// </summary>
        public bool useOverlapFilter;

        /// <summary>
        /// Random range within which to choose the minimum absolute distance value for the overlap filter.
        /// </summary>
        public Vector2 minimumAbsoluteDistanceRange = new Vector2(0.25f, 0.5f);

        /// <summary>
        /// Should the random scale value be applied uniformly across all axes?
        /// </summary>
        public bool uniformRandomScale = true;

        /// <summary>
        /// Should the additive scale value be applied uniformly across all axes (X, Y and Z)?
        /// </summary>
        public bool uniformAdditiveScale = true;

        /// <summary>
        /// The range within which to choose the uniform random scale value.
        /// </summary>
        public Vector2 randomScaleRange = Vector2.one;

        /// <summary>
        /// The range within which to choose the X value for the non-uniform random scale.
        /// </summary>
        public Vector2 randomScaleRangeX = Vector2.one;

        /// <summary>
        /// The range within which to choose the Y value for the non-uniform random scale.
        /// </summary>
        public Vector2 randomScaleRangeY = Vector2.one;

        /// <summary>
        /// The range within which to choose the Z value for the non-uniform random scale.
        /// </summary>
        public Vector2 randomScaleRangeZ = Vector2.one;

        /// <summary>
        /// The range within which to choose the additive scale (this value will be added uniformly to all axes after the paint stroke).
        /// </summary>
        public Vector2 additiveScaleRange = Vector2.zero;

        /// <summary>
        /// Vector3 variable for the non-uniform additive scale.
        /// </summary>
        public Vector3 additiveScaleNonUniform = Vector3.zero;

        /// <summary>
        /// The random scale curve.
        /// </summary>
        public AnimationCurve randomScaleCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// The random scale curve ± variation.
        /// </summary>
        public float randomScaleCurveVariation;

        /// <summary>
        /// The range within which to choose the randomization percentage around the local-x axis of the painted meshes.<para> </para>
        /// 0% means no randomization at all and 100% means full randomization.
        /// </summary>
        public Vector2 randomRotationRangeX = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// The range within which to choose the randomization percentage around the local-y axis of the painted meshes.<para> </para>
        /// 0% means no randomization at all and 100% means full randomization.
        /// </summary>
        public Vector2 randomRotationRangeY = new Vector2(0.0f, 5.0f);

        /// <summary>
        /// The range within which to choose the randomization percentage around the local-z axis of the painted meshes.<para> </para>
        /// 0% means no randomization at all and 100% means full randomization.
        /// </summary>
        public Vector2 randomRotationRangeZ = new Vector2(0.0f, 0.0f);

        #region Randomizer brush settings

        public bool positionBrushRandomizer;
        public bool rotationBrushRandomizer = true;
        public bool scaleBrushRandomizer = true;

        #endregion

        #endregion

        #region Key bindings

        public KeyCode paintKey = KeyCode.P;
        public KeyCode deleteKey = KeyCode.L;
        public KeyCode combineKey = KeyCode.K;
        public KeyCode randomizeKey = KeyCode.J;
        public KeyCode increaseRadiusKey = KeyCode.O;
        public KeyCode decreaseRadiusKey = KeyCode.I;

        #endregion

        #region Range limits

        [SerializeField]
        int maxQuantityLimit = 100;

        [SerializeField]
        float maxDelayLimit = 1.0f;

        [SerializeField]
        float maxDensityLimit = 10.0f;

        [SerializeField]
        float minOffsetLimit = -50.0f;

        [SerializeField]
        float maxOffsetLimit = 50.0f;

        [SerializeField]
        float maxMinimumAbsoluteDistanceLimit = 3.0f;

        [SerializeField]
        float maxAdditiveScaleLimit = 3.0f;

        [SerializeField]
        float maxRandomScaleLimit = 3.0f;

        #endregion

        #region Inspector foldouts

        public bool helpFoldout;
        public bool helpTemplatesFoldout;
        public bool helpGeneralUsageFoldout;
        public bool helpOptimizationFoldout;
        public bool meshesFoldout = true;
        public bool templatesFoldout = true;
        public bool keyBindingsFoldout;
        public bool brushFoldout = true;
        public bool slopesFoldout = true;
        public bool randomizersFoldout = true;
        public bool overlapFilterFoldout = true;
        public bool additiveScaleFoldout = true;
        public bool optimizationFoldout = true;

        #endregion

        #region Editor settings

        /// <summary>
        /// Global painting mode state.<para> </para> 
        /// The user should have no control over this variable, because it gets 
        /// set up automatically when a MeshBrush component is added via the MeshBrush 
        /// menu item under GameObject, or a global painting instance is created (also through that menu)..
        /// </summary>
        [SerializeField]
        bool globalPaintingMode;

        /// <summary>
        /// Should the MeshBrush component be in a collapsed state? 
        /// </summary>
        public bool collapsed;

        /// <summary>
        /// Activates or deactivates the stats label in the inspector.
        /// </summary>
        public bool stats;

        /// <summary>
        /// Avoids losing focus of the scene view window.
        /// </summary>
        public bool lockSceneView;

        /// <summary>
        /// Toggle for the classic set of meshes to paint UI.
        /// </summary>
        public bool classicUI;

        /// <summary>
        /// Size of the icon boxes that appear in the modern set of meshes to paint UI.
        /// </summary>
        public float previewIconSize = 60.0f;

        /// <summary>
        /// Manually sample the reference slope vector.
        /// </summary>
        public bool manualReferenceVectorSampling;

        /// <summary>
        /// Show/hide the reference gui vector in the scene view.
        /// </summary>
        public bool showReferenceVectorInSceneView = true;

        /// <summary>
        /// Automatically flag all painted meshes as static.
        /// </summary>
        public bool autoStatic;

        /// <summary>
        /// Automatically select the combined mesh after pressing the button in the Optimize foldout?
        /// </summary>
        public bool autoSelectOnCombine = true;

        #endregion

        #region Cached components

        Transform cachedTransform;
        /// <summary>
        /// The cached <see cref="Transform"/> component.
        /// </summary>
        public Transform CachedTransform
        {
            get
            {
                if (cachedTransform == null)
                {
                    cachedTransform = transform;
                }
                return cachedTransform;
            }
        }

        Collider cachedCollider;
        /// <summary>
        /// The cached <see cref="Collider"/> component. 
        /// </summary>
        public Collider CachedCollider
        {
            get
            {
                if (cachedCollider == null)
                {
                    cachedCollider = GetComponent<Collider>();
                }
                return cachedCollider;
            }
        }

        #endregion

        GameObject brush;
        /// <summary>
        /// This is the invisible brush <see cref="GameObject"/> that wanders around when painting multiple meshes at once.
        /// </summary>
        public GameObject Brush
        {
            get
            {
                if (brush == null)
                {
                    CheckBrush();
                }

                return brush;
            }
        }

        Transform brushTransform;
        /// <summary>
        /// This is the invisible brush <see cref="Transform"/> that wanders around when painting multiple meshes at once.
        /// </summary>
        public Transform BrushTransform
        {
            get
            {
                if (brushTransform == null)
                {
                    CheckBrush();
                }

                return brushTransform;
            }
        }

        Transform holderObj;
        /// <summary>
        /// The MeshBrush instance's holder <see cref="Transform"/>;
        /// this is the root which all painted meshes will be parented to.
        /// </summary>
        public Transform HolderObj
        {
            get
            {
                if (holderObj == null)
                {
                    CheckHolder();
                }

                return holderObj;
            }
        }

        const string minString = "min";
        const string maxString = "max";

        const string trueString = "true";
        const string falseString = "false";

        const string enabledString = "enabled";

        /// <summary>
        /// The last brush location where meshes have been painted.
        /// </summary>
        public Vector3 lastPaintLocation;

        /// <summary>
        /// The current direction of the paint stroke.
        /// </summary>
        public Vector3 brushStrokeDirection;

        /// <summary>
        /// This is the set of meshes to paint that can be edited inside the inspector.
        /// </summary>
        // If you really need to modify the set of meshes to paint at runtime, just make this list public. 
        // But be careful when editing it whilst painting or doing other stuff. 
        [SerializeField]
        List<GameObject> meshes = new List<GameObject>(5) { null };

        /// <summary>
        /// A sub-set of <see cref="meshes"/> that should not be included in the next paint stroke.
        /// </summary>
        [SerializeField]
        List<GameObject> deactivatedMeshes = new List<GameObject>(2);

        /// <summary>
        /// The painted meshes' <see cref="Transform"/> components.
        /// </summary>
        List<Transform> paintedMeshes = new List<Transform>(200);

        /// <summary>
        /// This list contains the <see cref="Transform"/> components that have been gathered via the <see cref="GatherMeshesInsideBrushArea(RaycastHit)"/> method.
        /// </summary>
        List<Transform> paintedMeshesInsideBrushArea = new List<Transform>(50);

        float nextFeasibleStrokeTime;

#if UNITY_EDITOR

        public event Action EditRangeLimitsWizardOpened;

        [ContextMenu("Edit Range Limits")]
        void OpenRangeLimitsWizard()
        {
            if (EditRangeLimitsWizardOpened != null)
            {
                EditRangeLimitsWizardOpened.Invoke();
            }
        }

#endif

        /// <summary>
        /// Validates the MeshBrush instance settings. 
        /// Always call this method after changing some settings externally (e.g. at runtime from other scripts)!
        /// </summary>
        public void OnValidate()
        {
            // Avoid having unassigned keys in MeshBrush; 
            // Reset to the default value in case the user tries to set the button to "None".
            ValidateKeyBindings();

            // Prevent illegal range limits for the brush settings.
            ValidateRangeLimits();

            // Never allow an empty set of meshes to paint.
            if (meshes.Count == 0)
            {
                meshes.Add(null);
            }

            if (layerMask.Length != 32)
            {
                layerMask = new bool[32];

                for (int i = layerMask.Length - 1; i >= 0; i--)
                {
                    layerMask[i] = true;
                }
            }

            // Always leave the Unity built-in Ignore Raycast layer disabled.
            if (layerMask[2] == true)
            {
                layerMask[2] = false;
            }

            if (radius < 0.01f) radius = 0.01f;
            radius = (float)Math.Round(radius, 3);

            VectorClampingUtility.ClampVector(ref quantityRange, 1, maxQuantityLimit, 1, maxQuantityLimit);
            VectorClampingUtility.ClampVector(ref densityRange, 0.1f, maxDensityLimit, 0.1f, maxDensityLimit);

            delay = Mathf.Clamp(delay, 0.03f, maxDelayLimit);

            randomScaleCurveVariation = Mathf.Clamp(randomScaleCurveVariation, 0.0f, 3.0f);

            VectorClampingUtility.ClampVector(ref offsetRange, minOffsetLimit, maxOffsetLimit, minOffsetLimit, maxOffsetLimit);

            VectorClampingUtility.ClampVector(ref scatteringRange, 0.0f, 100.0f, 0.0f, 100.0f);

            VectorClampingUtility.ClampVector(ref slopeInfluenceRange, 0.0f, 100.0f, 0.0f, 100.0f);
            VectorClampingUtility.ClampVector(ref angleThresholdRange, 1.0f, 180.0f, 1.0f, 180.0f);

            VectorClampingUtility.ClampVector(ref minimumAbsoluteDistanceRange, 0.0f, maxMinimumAbsoluteDistanceLimit, 0.0f, maxMinimumAbsoluteDistanceLimit);

            VectorClampingUtility.ClampVector(ref randomScaleRange, 0.01f, maxRandomScaleLimit, 0.0f, maxRandomScaleLimit);
            VectorClampingUtility.ClampVector(ref randomScaleRangeX, 0.01f, maxRandomScaleLimit, 0.0f, maxRandomScaleLimit);
            VectorClampingUtility.ClampVector(ref randomScaleRangeY, 0.01f, maxRandomScaleLimit, 0.0f, maxRandomScaleLimit);
            VectorClampingUtility.ClampVector(ref randomScaleRangeZ, 0.01f, maxRandomScaleLimit, 0.0f, maxRandomScaleLimit);
            VectorClampingUtility.ClampVector(ref randomRotationRangeY, 0.0f, 100.0f, 0.0f, 100.0f);

            VectorClampingUtility.ClampVector(ref additiveScaleRange, -0.9f, maxAdditiveScaleLimit, -0.9f, maxAdditiveScaleLimit);
            VectorClampingUtility.ClampVector(ref additiveScaleNonUniform, -0.9f, maxAdditiveScaleLimit, -0.9f, maxAdditiveScaleLimit, -0.9f, maxAdditiveScaleLimit);
        }

        void ValidateRangeLimits()
        {
            maxQuantityLimit = Mathf.Clamp(maxQuantityLimit, 1, 1000);
            maxDensityLimit = Mathf.Clamp(maxDensityLimit, 1.0f, 1000.0f);
            maxDelayLimit = Mathf.Clamp(maxDelayLimit, 1.0f, 10.0f);
            minOffsetLimit = Mathf.Clamp(minOffsetLimit, -1000.0f, -1.0f);
            maxOffsetLimit = Mathf.Clamp(maxOffsetLimit, 1.0f, 1000.0f);
            maxMinimumAbsoluteDistanceLimit = Mathf.Clamp(maxMinimumAbsoluteDistanceLimit, 3.0f, 1000.0f);
            maxAdditiveScaleLimit = Mathf.Clamp(maxAdditiveScaleLimit, 3.0f, 1000.0f);
            maxRandomScaleLimit = Mathf.Clamp(maxRandomScaleLimit, 3.0f, 1000.0f);
        }

        void ValidateKeyBindings()
        {
            if (paintKey == KeyCode.None)
            {
                paintKey = KeyCode.P;
            }

            if (deleteKey == KeyCode.None)
            {
                deleteKey = KeyCode.L;
            }

            if (randomizeKey == KeyCode.None)
            {
                randomizeKey = KeyCode.J;
            }

            if (combineKey == KeyCode.None)
            {
                combineKey = KeyCode.K;
            }

            if (increaseRadiusKey == KeyCode.None)
            {
                increaseRadiusKey = KeyCode.O;
            }

            if (decreaseRadiusKey == KeyCode.None)
            {
                decreaseRadiusKey = KeyCode.I;
            }
        }

        public void GatherPaintedMeshes()
        {
            paintedMeshes = HolderObj.GetComponentsInChildren<Transform>().ToList();
        }

        /// <summary>
        /// Cleans up the set of meshes to paint by removing unused null fields.
        /// </summary>
        public void CleanSetOfMeshesToPaint()
        {
            if (meshes.Count <= 1)
            {
                return;
            }

            for (int i = meshes.Count - 1; i >= 0; i--)
            {
                if (meshes[i] == null)
                {
                    meshes.RemoveAt(i);
                }
            }

            // Never allow an empty set of meshes to paint.
            if (meshes.Count == 0)
            {
                meshes.Add(null);
            }
        }

        void GatherMeshesInsideBrushArea(RaycastHit brushLocation)
        {
            // Recycle the list of MeshFilters inside of the brush area.
            paintedMeshesInsideBrushArea.Clear();

            // Only add the meshes to the list that are inside the brush area.
            // An object is considered inside of our circle brush area 
            // if the distance between its transform and the current location 
            // of the circle brush's center point is smaller than the circle's radius.
            foreach (Transform paintedMesh in paintedMeshes)
            {
                if (paintedMesh != null && paintedMesh != BrushTransform && paintedMesh != HolderObj)
                {
                    if (Vector3.Distance(brushLocation.point, paintedMesh.position) < radius)
                    {
                        paintedMeshesInsideBrushArea.Add(paintedMesh);
                    }
                }
            }
        }

        /// <summary>
        /// Paint meshes inside the brush area.
        /// </summary>
        public void PaintMeshes(RaycastHit brushLocation)
        {
            // Respect the specified delay between paint strokes.
            if (nextFeasibleStrokeTime >= Time.realtimeSinceStartup)
            {
                return;
            }
            nextFeasibleStrokeTime = Time.realtimeSinceStartup + delay;

            // For the creation of multiple meshes at once we need a temporary brush gameobject, 
            // which will wander around our circle brush's area to shoot rays and adapt the meshes.
            // If there is no available brush, create one!
            CheckBrush();

            // Calculate the direction of the brush stroke.
            brushStrokeDirection = brushLocation.point - lastPaintLocation;

            int numberOfMeshesToPaint = useDensity ? (int)((radius * radius * Mathf.PI) * (Random.Range(densityRange.x, densityRange.y))) : (int)Random.Range(quantityRange.x, quantityRange.y + 1);
            if (numberOfMeshesToPaint <= 0) numberOfMeshesToPaint = 1;

            if (useOverlapFilter)
            {
                GatherMeshesInsideBrushArea(brushLocation);
            }

            bool nullMeshWarning = false;
            for (int i = numberOfMeshesToPaint; i > 0; i--)
            {
                // Calcuate how far away from the center of the brush the meshes should be allowed to end up.
                float inset = (radius * 0.01f * Random.Range(scatteringRange.x, scatteringRange.y));

                // Position the brush object slightly away from our raycasthit and rotate it correctly.
                brushTransform.position = brushLocation.point + (brushLocation.normal * 0.5f);
                brushTransform.rotation = Quaternion.LookRotation(brushLocation.normal);
                brushTransform.up = brushTransform.forward;

                // Afterwards, translate it inside the brush's circle area based on the scattering percentage defined by the user.
                if (numberOfMeshesToPaint > 1)
                {
                    brushTransform.Translate(Random.Range(-Random.insideUnitCircle.x * inset, Random.insideUnitCircle.x * inset), 0.0f, Random.Range(-Random.insideUnitCircle.y * inset, Random.insideUnitCircle.y * inset), Space.Self);
                }

                // Perform the final raycast from the brush object's location to our gameobject's surface. 
                // I'm giving this a limit of 2.5m to avoid meshes being painted behind hills and walls when the brush's radius is big.
                RaycastHit tempHit;
                if (globalPaintingMode ? Physics.Raycast(new Ray(brushTransform.position, -brushLocation.normal), out tempHit, 2.5f) : CachedCollider.Raycast(new Ray(brushTransform.position, -brushLocation.normal), out tempHit, 2.5f))
                {
                    // Calculate the slope angle based on the angle between the world's upvector (or a manually sampled reference vector) and the normal vector of our hit.
                    var slopeAngle = useSlopeFilter ? Vector3.Angle(tempHit.normal, manualReferenceVectorSampling ? slopeReferenceVector : Vector3.up) : inverseSlopeFilter ? 180.0f : 0.0f;

                    // And if all conditions are met, paint our meshes according to the user's parameters.
                    if (inverseSlopeFilter ? slopeAngle > Random.Range(angleThresholdRange.x, angleThresholdRange.y) : slopeAngle < Random.Range(angleThresholdRange.x, angleThresholdRange.y))
                    {
                        // Apply the overlap filter.
                        if (useOverlapFilter)
                        {
                            if (CheckOverlap(tempHit.point))
                                continue;
                        }

                        // This is the creation of the mesh. 
                        GameObject paintedMesh = null;
#if UNITY_EDITOR
                        // Maintain any eventual prefab connections inside the editor.
                        var set = meshes.Except(deactivatedMeshes).ToList();
                        if (set.Count <= 0)
                        {
                            Debug.LogWarning(string.Format("MeshBrush (group \"{0}\") - You disabled all meshes inside the set of meshes to paint. The paint stroke will be empty!", groupName));
                            continue;
                        }
                        paintedMesh = (GameObject)PrefabUtility.InstantiatePrefab(set[Random.Range(0, set.Count)]);
#else
                        paintedMesh = Instantiate(meshes[Random.Range(0, meshes.Count)]);
#endif
                        if (paintedMesh == null)
                        {
                            if (!nullMeshWarning)
                            {
                                nullMeshWarning = true;
                                Debug.LogError("MeshBrush: one or more fields in the set of meshes to paint is null. Please assign all fields before painting (or remove empty ones).");
                            }

                            continue;
                        }

                        if (autoIgnoreRaycast)
                        {
                            paintedMesh.layer = 2;
                        }

                        // Cache the Transform component of the painted mesh.
                        var paintedMeshTransform = paintedMesh.transform;

                        // Orient it according to its underlying surface.
                        OrientPaintedMesh(paintedMeshTransform, tempHit);

                        // Apply mesh offset along local-y axis.
                        if (Mathf.Abs(offsetRange.x) > float.Epsilon || Mathf.Abs(offsetRange.y) > float.Epsilon)
                        {
                            MeshTransformationUtility.ApplyMeshOffset(paintedMeshTransform, Random.Range(offsetRange.x, offsetRange.y), brushLocation.normal);
                        }

                        // Apply the random scale.
                        if (uniformRandomScale)
                        {
                            if (Mathf.Abs(randomScaleRange.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRange.y - 1.0f) > float.Epsilon)
                            {
                                MeshTransformationUtility.ApplyRandomScale(paintedMeshTransform, randomScaleRange);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(randomScaleRangeX.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeX.y - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeY.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeY.y - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeZ.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeZ.y - 1.0f) > float.Epsilon)
                            {
                                MeshTransformationUtility.ApplyRandomScale(paintedMeshTransform, randomScaleRangeX, randomScaleRangeY, randomScaleRangeZ);
                            }
                        }

                        // Apply the scale curve.
                        paintedMeshTransform.localScale *= Mathf.Abs(randomScaleCurve.Evaluate(Vector3.Distance(paintedMeshTransform.position, brushLocation.point) / radius) + Random.Range(-randomScaleCurveVariation, randomScaleCurveVariation));

                        // Constant, additive scale (adds up to the total scale after everything else).
                        if (uniformAdditiveScale)
                        {
                            if (Mathf.Abs(additiveScaleRange.x) > float.Epsilon || Mathf.Abs(additiveScaleRange.y) > float.Epsilon)
                            {
                                MeshTransformationUtility.AddConstantScale(paintedMeshTransform, additiveScaleRange);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(additiveScaleNonUniform.x) > float.Epsilon || Mathf.Abs(additiveScaleNonUniform.y) > float.Epsilon || Mathf.Abs(additiveScaleNonUniform.z) > float.Epsilon)
                            {
                                MeshTransformationUtility.AddConstantScale(paintedMeshTransform, additiveScaleNonUniform.x, additiveScaleNonUniform.y, additiveScaleNonUniform.z);
                            }
                        }

                        // Apply random rotation around local-y axis.
                        if (randomRotationRangeX.x > 0.0f || randomRotationRangeX.y > 0.0f || randomRotationRangeY.x > 0.0f || randomRotationRangeY.y > 0.0f || randomRotationRangeZ.x > 0.0f || randomRotationRangeZ.y > 0.0f)
                        {
                            MeshTransformationUtility.ApplyRandomRotation(
                                paintedMeshTransform, 
                                Random.Range(randomRotationRangeX.x, randomRotationRangeX.y), 
                                Random.Range(randomRotationRangeY.x, randomRotationRangeY.y), 
                                Random.Range(randomRotationRangeZ.x, randomRotationRangeZ.y)
                            );
                        }

                        // Set the instantiated object as a parent of the holder GameObject.
                        paintedMeshTransform.parent = HolderObj;

                        // Automatically flag the painted mesh as static 
                        // (if the user has the auto static option checked in the inspector).
                        paintedMesh.isStatic |= autoStatic;

                        paintedMeshes.Add(paintedMeshTransform);
#if UNITY_EDITOR
                        // Allow the undo operation for the creation of meshes inside the editor.
                        Undo.RegisterCreatedObjectUndo(paintedMesh, paintedMesh.name);
#endif
                    }
                }
            }

            // Update the last paint location (used for brush stroke alignment).
            lastPaintLocation = brushLocation.point;
        }

        /// <summary>
        /// Randomize all painted meshes inside the brush area.
        /// </summary>
        public void RandomizeMeshes(RaycastHit brushLocation)
        {
            // Respect the specified delay between paint strokes.
            if (nextFeasibleStrokeTime >= Time.realtimeSinceStartup)
            {
                return;
            }
            nextFeasibleStrokeTime = Time.realtimeSinceStartup + delay;

            GatherMeshesInsideBrushArea(brushLocation);

            for (int i = paintedMeshesInsideBrushArea.Count - 1; i >= 0; i--)
            {
                var transformToRandomize = paintedMeshesInsideBrushArea[i];
                if (transformToRandomize != null)
                {
#if UNITY_EDITOR
                    Undo.RecordObject(transformToRandomize, "randomize meshes (MeshBrush)");
#endif
                    if (positionBrushRandomizer)
                    {
                        // Calcuate how far away from the center of the brush the meshes should be allowed to end up.
                        var inset = (radius * 0.01f * Random.Range(scatteringRange.x, scatteringRange.y));

                        // Position the brush object slightly away from our raycasthit and rotate it correctly.
                        brushTransform.position = brushLocation.point + (brushLocation.normal * 0.5f);
                        brushTransform.rotation = Quaternion.LookRotation(brushLocation.normal);
                        brushTransform.up = brushTransform.forward;

                        // Afterwards, translate it inside the brush's circle area based on the scattering percentage defined by the user.
                        brushTransform.Translate(Random.Range(-Random.insideUnitCircle.x * inset, Random.insideUnitCircle.x * inset), 0.0f, Random.Range(-Random.insideUnitCircle.y * inset, Random.insideUnitCircle.y * inset), Space.Self);

                        RaycastHit tempHit;
                        if (globalPaintingMode ? Physics.Raycast(new Ray(brushTransform.position, -brushLocation.normal), out tempHit, 2.5f) : CachedCollider.Raycast(new Ray(brushTransform.position, -brushLocation.normal), out tempHit, 2.5f))
                        {
                            OrientPaintedMesh(transformToRandomize, tempHit);
                        }

                        // Apply mesh offset along local-y axis.
                        if (Mathf.Abs(offsetRange.x) > float.Epsilon || Mathf.Abs(offsetRange.y) > float.Epsilon)
                        {
                            MeshTransformationUtility.ApplyMeshOffset(transformToRandomize, Random.Range(offsetRange.x, offsetRange.y), brushLocation.normal);
                        }
                    }

                    if (rotationBrushRandomizer)
                    {
                        // Apply random rotation around local-y axis.
                        if (randomRotationRangeX.x > 0.0f || randomRotationRangeX.y > 0.0f || randomRotationRangeY.x > 0.0f || randomRotationRangeY.y > 0.0f || randomRotationRangeZ.x > 0.0f || randomRotationRangeZ.y > 0.0f)
                        {
                            MeshTransformationUtility.ApplyRandomRotation(
                                transformToRandomize,
                                Random.Range(randomRotationRangeX.x, randomRotationRangeX.y),
                                Random.Range(randomRotationRangeY.x, randomRotationRangeY.y),
                                Random.Range(randomRotationRangeZ.x, randomRotationRangeZ.y)
                            );
                        }
                    }

                    if (scaleBrushRandomizer)
                    {
                        // Apply the random scale.
                        if (uniformRandomScale)
                        {
                            if (Mathf.Abs(randomScaleRange.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRange.y - 1.0f) > float.Epsilon)
                            {
                                MeshTransformationUtility.ApplyRandomScale(transformToRandomize, randomScaleRange);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(randomScaleRangeX.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeX.y - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeY.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeY.y - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeZ.x - 1.0f) > float.Epsilon || Mathf.Abs(randomScaleRangeZ.y - 1.0f) > float.Epsilon)
                            {
                                MeshTransformationUtility.ApplyRandomScale(transformToRandomize, randomScaleRangeX, randomScaleRangeY, randomScaleRangeZ);
                            }
                        }

                        // Apply the scale curve.
                        transformToRandomize.localScale *= Mathf.Abs(randomScaleCurve.Evaluate(Vector3.Distance(transformToRandomize.position, brushLocation.point) / radius) + Random.Range(-randomScaleCurveVariation, randomScaleCurveVariation));
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all painted meshes (except already combined ones) inside the specified brush circle area.
        /// </summary>
        public void DeleteMeshes(RaycastHit brushLocation)
        {
            // Respect the specified delay between paint strokes.
            if (nextFeasibleStrokeTime >= Time.realtimeSinceStartup)
            {
                return;
            }
            nextFeasibleStrokeTime = Time.realtimeSinceStartup + delay;

            GatherMeshesInsideBrushArea(brushLocation);

            for (int i = paintedMeshesInsideBrushArea.Count - 1; i >= 0; i--)
            {
                paintedMeshes.Remove(paintedMeshesInsideBrushArea[i]);

                var objectToDestroy = paintedMeshesInsideBrushArea[i].gameObject;
                if (objectToDestroy.transform.parent == HolderObj.transform)
                {
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(objectToDestroy);
#else
                    Destroy(objectToDestroy);
#endif
                }
            }
        }

        /// <summary>
        /// Combines all meshes inside the specified brush circle area.
        /// </summary>
        public void CombineMeshes(RaycastHit brushLocation)
        {
            // Respect the specified delay between paint strokes.
            if (nextFeasibleStrokeTime >= Time.realtimeSinceStartup)
            {
                return;
            }
            nextFeasibleStrokeTime = Time.realtimeSinceStartup + delay;

            GatherMeshesInsideBrushArea(brushLocation);

            if (paintedMeshesInsideBrushArea.Count > 0)
            {
                HolderObj.GetComponent<MeshBrushParent>().CombinePaintedMeshes(autoSelectOnCombine, paintedMeshesInsideBrushArea.Select(mesh => mesh.GetComponent<MeshFilter>()).ToArray());
            }

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
        }

        /// <summary>
        /// Samples the slope filter's reference vector at the specified location.
        /// </summary>
        public void SampleReferenceVector(Vector3 referenceVector, Vector3 sampleLocation)
        {
            slopeReferenceVector = referenceVector;
            slopeReferenceVectorSampleLocation = sampleLocation;
        }

        /// <summary>
        /// Orients a painted mesh to the specified target location (adjusting position and rotation accordingly).
        /// </summary>
        /// <param name="targetTransform">The <see cref="Transform"/> of the painted mesh to orient.</param>
        /// <param name="targetLocation">The underlying surface to which you want to orient your painted mesh.</param>
        void OrientPaintedMesh(Transform targetTransform, RaycastHit targetLocation)
        {
            targetTransform.position = targetLocation.point;
            targetTransform.rotation = Quaternion.LookRotation(targetLocation.normal);

            // Align the painted mesh's up vector to the corresponding direction (defined by the user).
            Vector3 newUp = Vector3.Lerp(yAxisTangent ? targetTransform.up : Vector3.up, targetTransform.forward, Random.Range(slopeInfluenceRange.x, slopeInfluenceRange.y) * 0.01f);

            // Brush stroke alignment.
            Vector3 newFwd = strokeAlignment && (brushStrokeDirection != Vector3.zero && lastPaintLocation != Vector3.zero) ? brushStrokeDirection : targetTransform.forward;

            // We need to orthonormalize the two vectors before applying the rotation.
            Vector3.OrthoNormalize(ref newUp, ref newFwd);

            // Apply the mesh rotation accordingly.
            targetTransform.rotation = Quaternion.LookRotation(newFwd, newUp);
        }

        bool CheckOverlap(Vector3 objPos)
        {
            if (paintedMeshes == null || paintedMeshes.Count < 1)
            {
                return false;
            }

            foreach (Transform otherPaintedMesh in paintedMeshes)
            {
                if (otherPaintedMesh != null && otherPaintedMesh != BrushTransform && otherPaintedMesh != HolderObj && Vector3.Distance(objPos, otherPaintedMesh.position) < Random.Range(minimumAbsoluteDistanceRange.x, minimumAbsoluteDistanceRange.y))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the MeshBrush component has a linked child holder object or not 
        /// (and creates one if it doesn't).<para> </para>
        /// The holder GameObject acts as a root for all painted meshes.
        /// </summary>
        void CheckHolder()
        {
            var holders = GetComponentsInChildren<MeshBrushParent>();
            if (holders.Length > 0)
            {
                holderObj = null;

                for (int i = 0; i < holders.Length; i++)
                {
                    if (holders[i] != null && string.CompareOrdinal(holders[i].name, groupName) == 0)
                    {
                        holderObj = holders[i].transform;
                    }
                }

                if (holderObj == null)
                {
                    CreateHolder();
                }
            }
            else CreateHolder();
        }

        /// <summary>
        /// Checks if there is already a linked brush object and
        /// creates one if there isn't.<para> </para>
        /// The brush is needed for multiple mesh painting.
        /// </summary>
        void CheckBrush()
        {
            CheckHolder();
            brushTransform = holderObj.Find("Brush");
            if (brushTransform == null)
            {
                CreateBrush();
            }
        }

        void CreateHolder()
        {
            var newHolder = new GameObject(groupName);

            newHolder.AddComponent<MeshBrushParent>();
            newHolder.transform.rotation = CachedTransform.rotation;
            newHolder.transform.parent = CachedTransform;
            newHolder.transform.localPosition = Vector3.zero;

            holderObj = newHolder.transform;
        }

        void CreateBrush()
        {
            brush = new GameObject("Brush");
            brushTransform = brush.transform;
            brushTransform.position = CachedTransform.position;
            brushTransform.parent = holderObj;
        }

        public void ResetKeyBindings()
        {
            paintKey = KeyCode.P;
            deleteKey = KeyCode.L;
            combineKey = KeyCode.K;
            randomizeKey = KeyCode.J;
            increaseRadiusKey = KeyCode.O;
            decreaseRadiusKey = KeyCode.I;
        }

        public void ResetSlopeSettings()
        {
            slopeInfluenceRange = new Vector2(95f, 100f);
            angleThresholdRange = new Vector2(25f, 30f);
            useSlopeFilter = false;
            inverseSlopeFilter = false;
            manualReferenceVectorSampling = false;
            showReferenceVectorInSceneView = true;
        }

        public void ResetRandomizers()
        {
            randomScaleRange = Vector2.one;
            randomScaleRangeX = randomScaleRangeY = randomScaleRangeZ = Vector2.one;

            randomScaleCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
            randomScaleCurveVariation = 0.0f;

            randomRotationRangeY = new Vector2(0.0f, 5.0f);
            randomRotationRangeX = randomRotationRangeZ = Vector2.zero;

            positionBrushRandomizer = false;
            rotationBrushRandomizer = true;
            scaleBrushRandomizer = true;
        }

        public void ResetAdditiveScale()
        {
            uniformRandomScale = true;
            additiveScaleRange = Vector2.zero;
            additiveScaleNonUniform = Vector3.zero;
        }

        public void ResetOverlapFilterSettings()
        {
            useOverlapFilter = false;
            minimumAbsoluteDistanceRange = new Vector2(0.25f, 0.5f);
        }

        /// <summary>
        /// Saves a MeshBrush instance out to a template file. 
        /// </summary>
        /// <param name="filePath">Where should the template file be stored? Please provide the full path (including the .xml extension).</param>
        /// <returns>The template's <see cref="XDocument"/> after saving, to allow for further editing and updating.</returns>
        public XDocument SaveTemplate(string filePath)
        {
            var templateDocument = new XDocument(
                new XElement("meshBrushTemplate", new XAttribute("version", version),
                    new XElement("instance",
                        new XElement("active", active),
                        new XElement("name", groupName),
                        new XElement("stats", stats),
                        new XElement("lockSceneView", lockSceneView)),
                    new XElement("meshes",
                        new XElement("ui",
                            new XElement("style", classicUI ? "classic" : "modern"),
                            new XElement("iconSize", previewIconSize))
#if UNITY_EDITOR
                      , new XElement("assets",
                            meshes.Select(mesh => new XElement("asset",
                                new XElement("guid", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mesh))),
                                new XElement("path", mesh != null ? AssetDatabase.GetAssetPath(mesh) : "null"))))
#endif 
                        ),
                    new XElement("keyBindings",
                        new XElement("paint", paintKey),
                        new XElement("delete", deleteKey),
                        new XElement("combine", combineKey),
                        new XElement("randomize", randomizeKey),
                        new XElement("increaseRadius", increaseRadiusKey),
                        new XElement("decreaseRadius", decreaseRadiusKey)),
                    new XElement("brush",
                        new XElement("radius", radius),
                        new XElement("color",
                            new XElement("r", color.r),
                            new XElement("g", color.g),
                            new XElement("b", color.b),
                            new XElement("a", color.a)),
                        new XElement("quantity",
                            new XElement(minString, quantityRange.x),
                            new XElement(maxString, quantityRange.y)),
                        new XElement("useDensity", useDensity),
                        new XElement("density",
                            new XElement(minString, densityRange.x),
                            new XElement(maxString, densityRange.y)),
                        new XElement("offset",
                            new XElement(minString, offsetRange.x),
                            new XElement(maxString, offsetRange.y)),
                        new XElement("scattering",
                            new XElement(minString, scatteringRange.x),
                            new XElement(maxString, scatteringRange.y)),
                        new XElement("delay", delay),
                        new XElement("yAxisTangent", yAxisTangent),
                        new XElement("strokeAlignment", strokeAlignment)),
                    new XElement("slopes",
                        new XElement("slopeInfluence",
                            new XElement(minString, slopeInfluenceRange.x),
                            new XElement(maxString, slopeInfluenceRange.y)),
                        new XElement("slopeFilter",
                            new XElement(enabledString, useSlopeFilter),
                            new XElement("inverse", inverseSlopeFilter),
                            new XElement("angleThreshold",
                                new XElement(minString, angleThresholdRange.x),
                                new XElement(maxString, angleThresholdRange.y)),
                            new XElement("manualReferenceVectorSampling", manualReferenceVectorSampling),
                            new XElement("showReferenceVectorInSceneView", showReferenceVectorInSceneView),
                            new XElement("referenceVector",
                                new XElement("x", slopeReferenceVector.x),
                                new XElement("y", slopeReferenceVector.y),
                                new XElement("z", slopeReferenceVector.z)),
                            new XElement("referenceVectorSampleLocation",
                                new XElement("x", slopeReferenceVectorSampleLocation.x),
                                new XElement("y", slopeReferenceVectorSampleLocation.y),
                                new XElement("z", slopeReferenceVectorSampleLocation.z)))),
                    new XElement("randomizers",
                        new XElement("scale",
                            new XElement("scaleUniformly", uniformRandomScale),
                            new XElement("uniform",
                                new XElement(minString, randomScaleRange.x),
                                new XElement(maxString, randomScaleRange.y)),
                            new XElement("nonUniform",
                                new XElement("x",
                                    new XElement(minString, randomScaleRangeX.x),
                                    new XElement(maxString, randomScaleRangeX.y)),
                                new XElement("y",
                                    new XElement(minString, randomScaleRangeY.x),
                                    new XElement(maxString, randomScaleRangeY.y)),
                                new XElement("z",
                                    new XElement(minString, randomScaleRangeZ.x),
                                    new XElement(maxString, randomScaleRangeZ.y))),
                            new XElement("curve",
                                new XElement("variation", randomScaleCurveVariation),
                                new XElement("keys",
                                    randomScaleCurve.keys.Select(key => new XElement("key",
                                        new XElement("time", key.time),
                                        new XElement("value", key.value),
                                        new XElement("inTangent", key.inTangent),
                                        new XElement("outTangent", key.outTangent)))))),
                        new XElement("rotation",
                            new XElement("x", 
                                new XElement(minString, randomRotationRangeX.x), 
                                new XElement(maxString, randomRotationRangeX.y)),
                            new XElement("y", 
                                new XElement(minString, randomRotationRangeY.x), 
                                new XElement(maxString, randomRotationRangeY.y)),
                            new XElement("z", 
                                new XElement(minString, randomRotationRangeZ.x), 
                                new XElement(maxString, randomRotationRangeZ.y))),
                        new XElement("randomizerBrush",
                            new XElement("position", positionBrushRandomizer),
                            new XElement("rotation", rotationBrushRandomizer),
                            new XElement("scale", scaleBrushRandomizer))),
                    new XElement("overlapFilter",
                        new XElement(enabledString, useOverlapFilter),
                        new XElement("minimumAbsoluteDistance",
                            new XElement(minString, minimumAbsoluteDistanceRange.x),
                            new XElement(maxString, minimumAbsoluteDistanceRange.y))),
                    new XElement("additiveScale",
                        new XElement("scaleUniformly", uniformAdditiveScale),
                        new XElement("uniform",
                            new XElement(minString, additiveScaleRange.x),
                            new XElement(maxString, additiveScaleRange.y)),
                        new XElement("nonUniform",
                            new XElement("x", additiveScaleNonUniform.x),
                            new XElement("y", additiveScaleNonUniform.y),
                            new XElement("z", additiveScaleNonUniform.z))),
                    new XElement("optimization",
                        new XElement("autoIgnoreRaycast", autoIgnoreRaycast),
                        new XElement("autoSelectOnCombine", autoSelectOnCombine),
                        new XElement("autoStatic", autoStatic)),
                    new XElement("rangeLimits",
                        new XElement("quantity",
                            new XElement(maxString, maxQuantityLimit)),
                        new XElement("density",
                            new XElement(maxString, maxDensityLimit)),
                        new XElement("offset",
                            new XElement(minString, minOffsetLimit),
                            new XElement(maxString, maxOffsetLimit)),
                        new XElement("delay",
                            new XElement(maxString, maxDelayLimit)),
                        new XElement("minimumAbsoluteDistance",
                            new XElement(maxString, maxMinimumAbsoluteDistanceLimit)),
                        new XElement("randomScale",
                            new XElement(maxString, maxRandomScaleLimit)),
                        new XElement("additiveScale",
                            new XElement(maxString, maxAdditiveScaleLimit))),
                    new XElement("inspectorFoldouts",
                        new XElement("help", helpFoldout),
                        new XElement("templatesHelp", helpTemplatesFoldout),
                        new XElement("generalUsageHelp", helpGeneralUsageFoldout),
                        new XElement("optimizationHelp", helpOptimizationFoldout),
                        new XElement("meshes", meshesFoldout),
                        new XElement("templates", templatesFoldout),
                        new XElement("keyBindings", keyBindingsFoldout),
                        new XElement("brush", brushFoldout),
                        new XElement("slopes", slopesFoldout),
                        new XElement("randomizers", randomizersFoldout),
                        new XElement("overlapFilter", overlapFilterFoldout),
                        new XElement("additiveScale", additiveScaleFoldout),
                        new XElement("optimization", optimizationFoldout)),
                    new XElement("globalPaintingMode",
                        new XElement(enabledString, globalPaintingMode),
                        new XElement("layerMask", layerMask.Select((layer, index) => new XElement("layer", new XAttribute("index", index), layer))))
            ));

            templateDocument.Save(filePath);

#if UNITY_EDITOR

            // Refresh Unity's AssetDatabase.
            // If we don't do this, the newly created xml file
            // might not immediately show up in the project panel.
            AssetDatabase.Refresh();
#endif
            return templateDocument;
        }

        /// <summary>
        /// Loads a MeshBrush template into a MeshBrush instance.
        /// </summary>
        /// <param name="filePath">The template file to feed into this MeshBrush instance (full path including .xml extension).</param>
        /// <returns>True if the loading procedure was successful; false if it failed in some way.</returns>
        public bool LoadTemplate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogError("MeshBrush: the specified template file path is invalid or does not exist! Cancelling loading procedure...");
                return false;
            }

            var templateDocument = XDocument.Load(filePath);

            if (templateDocument == null || templateDocument.Root == null)
            {
                Debug.LogError("MeshBrush: the specified template file couldn't be loaded.");
                return false;
            }

            float templateVersion = version;
            if (!float.TryParse(templateDocument.Root.FirstAttribute.Value, out templateVersion))
            {
                Debug.LogWarning("MeshBrush: The template you just loaded doesn't seem to contain a MeshBrush version number. Loading procedure might yield unpredictable results in cross-version situations. File path: "+ filePath);
            }

            foreach (var element in templateDocument.Root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "instance":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "active":
                                    active = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "name":
                                    groupName = subElement.Value;
                                    break;
                                case "stats":
                                    stats = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "lockSceneView":
                                    lockSceneView = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                            }
                        }
                        break;
                    case "meshes":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "ui":
                                    classicUI = string.CompareOrdinal(subElement.Element("style").Value, "classic") == 0;
                                    previewIconSize = float.Parse(subElement.Element("iconSize").Value);
                                    break;
#if UNITY_EDITOR
                                case "assets":
                                    var assets = subElement.Descendants("asset").Select(asset => new { Guid = asset.Element("guid").Value, Path = asset.Element("path").Value });
                                    meshes.Clear();
                                    foreach (var asset in assets)
                                    {
                                        var mesh = string.CompareOrdinal(asset.Path, "null") == 0 ? null : (GameObject)AssetDatabase.LoadAssetAtPath(asset.Path, typeof(GameObject));
                                        if (mesh == null)
                                        {
                                            mesh = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset.Guid), typeof(GameObject));
                                        }
                                        meshes.Add(mesh);
                                    }
                                    break;
#endif
                            }
                        }
                        break;
                    case "keyBindings":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "paint":
                                    paintKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                                case "delete":
                                    deleteKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                                case "combine":
                                    combineKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                                case "randomize":
                                    randomizeKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                                case "increaseRadius":
                                    increaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                                case "decreaseRadius":
                                    decreaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), subElement.Value);
                                    break;
                            }
                        }
                        break;
                    case "brush":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "radius":
                                    radius = float.Parse(subElement.Value);
                                    break;
                                case "color":
                                    color = new Color(float.Parse(subElement.Element("r").Value), float.Parse(subElement.Element("g").Value), float.Parse(subElement.Element("b").Value), float.Parse(subElement.Element("a").Value));
                                    break;
                                case "quantity":
                                    quantityRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "useDensity":
                                    useDensity = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "density":
                                    densityRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "offset":
                                    offsetRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "scattering":
                                    scatteringRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "delay":
                                    delay = float.Parse(subElement.Value);
                                    break;
                                case "yAxisTangent":
                                    yAxisTangent = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "strokeAlignment":
                                    strokeAlignment = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                            }
                        }
                        break;
                    case "slopes":
                        foreach (var subElement in element.Descendants())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "slopeInfluence":
                                    slopeInfluenceRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case enabledString:
                                    useSlopeFilter = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "inverse":
                                    inverseSlopeFilter = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "angleThreshold":
                                    angleThresholdRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "manualReferenceVectorSampling":
                                    manualReferenceVectorSampling = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "showReferenceVectorInSceneView":
                                    showReferenceVectorInSceneView = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "referenceVector":
                                    slopeReferenceVector = new Vector3(float.Parse(subElement.Element("x").Value), float.Parse(subElement.Element("y").Value), float.Parse(subElement.Element("z").Value));
                                    break;
                                case "referenceVectorSampleLocation":
                                    slopeReferenceVectorSampleLocation = new Vector3(float.Parse(subElement.Element("x").Value), float.Parse(subElement.Element("y").Value), float.Parse(subElement.Element("z").Value));
                                    break;
                            }
                        }
                        break;
                    case "randomizers":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "scale":
                                    foreach (var scaleElement in subElement.Descendants())
                                    {
                                        switch (scaleElement.Name.LocalName)
                                        {
                                            case "scaleUniformly":
                                                uniformRandomScale = string.CompareOrdinal(scaleElement.Value, trueString) == 0;
                                                break;
                                            case "uniform":
                                                randomScaleRange = new Vector2(float.Parse(scaleElement.Element(minString).Value), float.Parse(scaleElement.Element(maxString).Value));
                                                break;
                                            case "x":
                                                randomScaleRangeX = new Vector2(float.Parse(scaleElement.Element(minString).Value), float.Parse(scaleElement.Element(maxString).Value));
                                                break;
                                            case "y":
                                                randomScaleRangeY = new Vector2(float.Parse(scaleElement.Element(minString).Value), float.Parse(scaleElement.Element(maxString).Value));
                                                break;
                                            case "z":
                                                randomScaleRangeZ = new Vector2(float.Parse(scaleElement.Element(minString).Value), float.Parse(scaleElement.Element(maxString).Value));
                                                break;
                                            case "variation":
                                                randomScaleCurveVariation = float.Parse(scaleElement.Value);
                                                break;
                                            case "keys":
                                                randomScaleCurve = new AnimationCurve(scaleElement.Descendants("key").Select(key => new Keyframe(float.Parse(key.Element("time").Value), float.Parse(key.Element("value").Value), float.Parse(key.Element("inTangent").Value), float.Parse(key.Element("outTangent").Value))).ToArray());
                                                break;
                                        }
                                    }
                                    break;
                                case "rotation":
                                    // Pre-2.0 legacy template loading:
                                    if (templateVersion < 2.0f)
                                    {
                                        if (string.CompareOrdinal(subElement.Parent.Name.LocalName, "randomizerBrush") != 0)
                                        {
                                            randomRotationRangeY = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                        }
                                    }
                                    else
                                    {
                                        if (string.CompareOrdinal(subElement.Parent.Name.LocalName, "randomizerBrush") != 0)
                                        {
                                            var a = subElement.Element("x");
                                            randomRotationRangeX = new Vector2(float.Parse(a.Element(minString).Value), float.Parse(a.Element(maxString).Value));
                                            a = subElement.Element("y");
                                            randomRotationRangeY = new Vector2(float.Parse(a.Element(minString).Value), float.Parse(a.Element(maxString).Value));
                                            a = subElement.Element("z");
                                            randomRotationRangeZ = new Vector2(float.Parse(a.Element(minString).Value), float.Parse(a.Element(maxString).Value));
                                        }
                                    }
                                    break;
                                case "randomizerBrush":
                                    var randomizerBrushElement = subElement.Element("position");
                                    if (randomizerBrushElement != null)
                                    {
                                        positionBrushRandomizer = string.CompareOrdinal(randomizerBrushElement.Value, trueString) == 0;
                                    }

                                    randomizerBrushElement = subElement.Element("rotation");
                                    if (randomizerBrushElement != null)
                                    {
                                        rotationBrushRandomizer = string.CompareOrdinal(randomizerBrushElement.Value, trueString) == 0;
                                    }

                                    randomizerBrushElement = subElement.Element("scale");
                                    if (randomizerBrushElement != null)
                                    {
                                        scaleBrushRandomizer = string.CompareOrdinal(randomizerBrushElement.Value, trueString) == 0;
                                    }
                                    break;
                            }
                        }
                        break;
                    case "overlapFilter":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case enabledString:
                                    useOverlapFilter = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "minimumAbsoluteDistance":
                                    minimumAbsoluteDistanceRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                            }
                        }
                        break;
                    case "additiveScale":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "scaleUniformly":
                                    uniformAdditiveScale = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "uniform":
                                    additiveScaleRange = new Vector2(float.Parse(subElement.Element(minString).Value), float.Parse(subElement.Element(maxString).Value));
                                    break;
                                case "nonUniform":
                                    additiveScaleNonUniform = new Vector3(float.Parse(subElement.Element("x").Value), float.Parse(subElement.Element("y").Value), float.Parse(subElement.Element("z").Value));
                                    break;
                            }
                        }
                        break;
                    case "optimization":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "autoIgnoreRaycast":
                                    autoIgnoreRaycast = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "autoSelectOnCombine":
                                    autoSelectOnCombine = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "autoStatic":
                                    autoStatic = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                            }
                        }
                        break;
                    case "rangeLimits":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "quantity":
                                    maxQuantityLimit = int.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "density":
                                    maxDensityLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "offset":
                                    minOffsetLimit = float.Parse(subElement.Element(minString).Value);
                                    maxOffsetLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "delay":
                                    maxDelayLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "minimumAbsoluteDistance":
                                    maxMinimumAbsoluteDistanceLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "randomScale":
                                    maxRandomScaleLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                                case "additiveScale":
                                    maxAdditiveScaleLimit = float.Parse(subElement.Element(maxString).Value);
                                    break;
                            }
                        }
                        break;
                    case "inspectorFoldouts":
                        foreach (var subElement in element.Elements())
                        {
                            switch (subElement.Name.LocalName)
                            {
                                case "help":
                                    helpFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "templatesHelp":
                                    helpTemplatesFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "generalUsageHelp":
                                    helpGeneralUsageFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "optimizationHelp":
                                    helpOptimizationFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "meshes":
                                    meshesFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "templates":
                                    templatesFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "keyBindings":
                                    keyBindingsFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "brush":
                                    brushFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "slopes":
                                    slopesFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "randomizers":
                                    randomizersFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "overlapFilter":
                                    overlapFilterFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "additiveScale":
                                    additiveScaleFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                                case "optimization":
                                    optimizationFoldout = string.CompareOrdinal(subElement.Value, trueString) == 0;
                                    break;
                            }
                        }
                        break;
                    case "globalPaintingMode":
                        globalPaintingMode = string.CompareOrdinal(element.Element(enabledString).Value, trueString) == 0;
                        layerMask = element.Descendants("layer").Select(layerElement => string.CompareOrdinal(layerElement.Value, falseString) != 0).ToArray();
                        break;
                }
            }

            return true;
        }
    }
}

// Copyright (C) Raphael Beck | Glitched Polygons, 2019
