using UnityEngine;
using UnityEditor;

namespace MeshBrush
{
    public class EditRangeLimitsWizard : ScriptableWizard
    {
        const string minString = "Min", maxString = "Max";

        /// <summary>
        /// Boolean value indicating whether this <see cref="T:MeshBrush.EditRangeLimitsWizard"/> is initialized.
        /// </summary>
        public bool Initialized { get; private set; }

        const string uninitializedErrorMessage = "This EditRangeLimitsWizard hasn't been initialized. Please call the EditRangeLimitsWizard.Initialize method before doing anything else with this wizard.";

        SerializedObject meshBrushInstance;

        SerializedProperty groupName;
        SerializedProperty maxQuantityLimit;
        SerializedProperty maxDelayLimit;
        SerializedProperty maxDensityLimit;
        SerializedProperty minOffsetLimit;
        SerializedProperty maxOffsetLimit;
        SerializedProperty maxMinimumAbsoluteDistanceLimit;
        SerializedProperty maxAdditiveScaleLimit;
        SerializedProperty maxRandomScaleLimit;

        /// <summary>
        /// Initialize the wizard.
        /// </summary>
        /// <param name="meshBrush">The linked MeshBrush instance (the one whose range limits are being edited).</param>
        public void Initialize(MeshBrush meshBrush)
        {
            meshBrushInstance = new SerializedObject(meshBrush);
            {
                groupName = meshBrushInstance.FindProperty("groupName");
                maxQuantityLimit = meshBrushInstance.FindProperty("maxQuantityLimit");
                maxDelayLimit = meshBrushInstance.FindProperty("maxDelayLimit");
                maxDensityLimit = meshBrushInstance.FindProperty("maxDensityLimit");
                minOffsetLimit = meshBrushInstance.FindProperty("minOffsetLimit");
                maxOffsetLimit = meshBrushInstance.FindProperty("maxOffsetLimit");
                maxMinimumAbsoluteDistanceLimit = meshBrushInstance.FindProperty("maxMinimumAbsoluteDistanceLimit");
                maxRandomScaleLimit = meshBrushInstance.FindProperty("maxRandomScaleLimit");
                maxAdditiveScaleLimit = meshBrushInstance.FindProperty("maxAdditiveScaleLimit");
            }

            helpString = "Edit the range limits of your MeshBrush instance here:";
            minSize = maxSize = new Vector2(450.0f, 250.0f);

            Initialized = true;
        }

        protected override bool DrawWizardGUI()
        {
            if (!Initialized)
            {
                EditorGUILayout.HelpBox(uninitializedErrorMessage, MessageType.Error);
                return false;
            }

            meshBrushInstance.Update();

            EditorGUILayout.LabelField("Group name: " + groupName.stringValue);
            EditorGUILayout.Space();

            EditorGUIUtility.labelWidth = 35.0f;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Quantity [meshes/stroke]:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 1", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxQuantityLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Mesh density [meshes/m²]:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 0.1", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxDensityLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Delay [s]:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 0.03", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxDelayLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Mesh offset [cm]:", GUILayout.Width(200f));

                EditorGUILayout.PropertyField(minOffsetLimit, new GUIContent(minString), GUILayout.ExpandWidth(false));

                EditorGUILayout.PropertyField(maxOffsetLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Minimum absolute distance [m]:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 0", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxMinimumAbsoluteDistanceLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Random scale limit:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 0.01", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxRandomScaleLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Additive scale limit:", GUILayout.Width(200f));
                EditorGUILayout.LabelField("Min = 0", GUILayout.ExpandWidth(false));
                EditorGUILayout.PropertyField(maxAdditiveScaleLimit, new GUIContent(maxString), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();


            EditorGUIUtility.labelWidth = 0;

            meshBrushInstance.ApplyModifiedProperties();
            return false;
        }

        // Values are updated in realtime. 
        // The "Done" button just closes the wizard.
        void OnWizardCreate() { }

        // The second wizard button is responsible for restoring the default range limit values.
        void OnWizardOtherButton()
        {
            maxQuantityLimit.intValue = 100;
            maxDensityLimit.floatValue = 10.0f;
            minOffsetLimit.floatValue = -50.0f;
            maxOffsetLimit.floatValue = 50.0f;
            maxMinimumAbsoluteDistanceLimit.floatValue = 3.0f;
            maxAdditiveScaleLimit.floatValue = 3.0f;
            maxRandomScaleLimit.floatValue = 3.0f;
            maxDelayLimit.floatValue = 1.0f;

            meshBrushInstance.ApplyModifiedProperties();
        }
    }
}

// Copyright (C) Raphael Beck, 2017