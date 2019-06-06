using System;
using System.IO;
using System.Linq;

using UnityEngine;
using Object = UnityEngine.Object;
using System.Xml.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MeshBrush
{
    /// <summary>
    /// Utility class for migrating obsolete MeshBrush templates from previous versions to the newest format.
    /// </summary>
    public static class TemplateMigrationUtility
    {

#if UNITY_EDITOR

        [MenuItem("GameObject/MeshBrush/Migrate selected templates", priority = 100)]
        static void MigrateSelectedTemplates()
        {
            var templates = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets).Where(obj => string.CompareOrdinal(Path.GetExtension(AssetDatabase.GetAssetPath(obj)), ".meshbrush") == 0);
            foreach (var templatePath in templates.Select(template => AssetDatabase.GetAssetPath(template)))
            {
                TryMigrate(templatePath);
            }
        }

#endif

        /// <summary>
        /// Migrates an old MeshBrush template file from a previous version to the newest format as good as possible (a fully 100% accurate migration is not always possible).<para> </para>
        /// A completely new template file with the same name, properties and path will be saved, but with a suffix of "__migrated".
        /// </summary>
        /// <param name="filePath">The full file path of the template to migrate.</param>
        /// <returns>True if the migration succeeded; false if it failed somehow.</returns>
        public static bool TryMigrate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogError("MeshBrush: The specified template file path is invalid or doesn't exist! Cancelling migration process...");
                return false;
            }

            try
            {
                var templateDocument = XDocument.Load(filePath);
                var meshBrush = new GameObject("MeshBrush Template Migration Utility") { hideFlags = HideFlags.HideAndDontSave }.AddComponent<MeshBrush>();

                foreach (var element in templateDocument.Descendants())
                {
                    switch (element.Name.LocalName)
                    {
                        case "meshBrushTemplate":
                            var versionAttribute = element.Attribute("version");
                            if (versionAttribute != null && MeshBrush.version <= float.Parse(versionAttribute.Value))
                            {
                                Debug.LogWarning("MeshBrush: The template you tried to migrate actually is already up to date with the current format. Cancelling process...");
                                return false;
                            }
                            break;
                        case "active":
                        case "isActive":
                            meshBrush.active = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "groupName":
                            meshBrush.groupName = element.Value;
                            break;
#if UNITY_EDITOR
                        case "setOfMeshesToPaint":
                            var serializedMeshBrush = new SerializedObject(meshBrush);
                            var meshes = serializedMeshBrush.FindProperty("meshes");
                            meshes.ClearArray();
                            meshes.arraySize = 0;
                            foreach (var meshPath in element.Elements())
                            {
                                if (meshPath != null)
                                {
                                    meshes.arraySize++;
                                    meshes.GetArrayElementAtIndex(meshes.arraySize - 1).objectReferenceValue = string.CompareOrdinal(meshPath.Value, "null") == 0 ? null : (GameObject)AssetDatabase.LoadAssetAtPath(meshPath.Value, typeof(GameObject));
                                }
                            }
                            serializedMeshBrush.ApplyModifiedPropertiesWithoutUndo();
                            break;
#endif
                        case "classicUI":
                            meshBrush.classicUI = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "previewIconSize":
                            meshBrush.previewIconSize = float.Parse(element.Value);
                            break;
                        case "lockSceneView":
                            meshBrush.lockSceneView = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "trisCounter":
                            meshBrush.stats = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "globalPaintingLayers":
                            int i = 0;
                            foreach (var layer in element.Elements())
                            {
                                meshBrush.layerMask[i] = string.CompareOrdinal(layer.Value, "false") != 0;
                                i++;
                            }
                            break;
                        case "paintKey":
                            meshBrush.paintKey = (KeyCode)Enum.Parse(typeof(KeyCode), element.Value);
                            break;
                        case "deleteKey":
                            meshBrush.deleteKey = (KeyCode)Enum.Parse(typeof(KeyCode), element.Value);
                            break;
                        case "combineAreaKey":
                            meshBrush.combineKey = (KeyCode)Enum.Parse(typeof(KeyCode), element.Value);
                            break;
                        case "increaseRadiusKey":
                            meshBrush.increaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), element.Value);
                            break;
                        case "decreaseRadiusKey":
                            meshBrush.decreaseRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), element.Value);
                            break;
                        case "brushRadius":
                            meshBrush.radius = float.Parse(element.Value);
                            break;
                        case "color":
                        case "brushColor":
                            meshBrush.color = new Color(float.Parse(element.Element("r").Value), float.Parse(element.Element("g").Value), float.Parse(element.Element("b").Value), float.Parse(element.Element("a").Value));
                            break;
                        case "useMeshDensity":
                            meshBrush.useDensity = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "minMeshDensity":
                            meshBrush.densityRange.x = float.Parse(element.Value);
                            break;
                        case "maxMeshDensity":
                            meshBrush.densityRange.y = float.Parse(element.Value);
                            break;
                        case "minNrOfMeshes":
                            meshBrush.quantityRange.x = float.Parse(element.Value);
                            break;
                        case "maxNrOfMeshes":
                            meshBrush.quantityRange.y = float.Parse(element.Value);
                            break;
                        case "delay":
                            meshBrush.delay = float.Parse(element.Value);
                            break;
                        case "verticalOffset":
                            var offset = float.Parse(element.Value);
                            meshBrush.offsetRange = new Vector2(offset, offset);
                            break;
                        case "alignWithStroke":
                            meshBrush.strokeAlignment = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "slopeInfluence":
                            var slopeInfluence = float.Parse(element.Value);
                            meshBrush.slopeInfluenceRange = new Vector2(slopeInfluence, slopeInfluence);
                            break;
                        case "useSlopeFilter":
                            meshBrush.useSlopeFilter = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "maxSlopeFilterAngle":
                            var angleThreshold = float.Parse(element.Value);
                            meshBrush.angleThresholdRange = new Vector2(angleThreshold, angleThreshold);
                            break;
                        case "inverseSlopeFilter":
                            meshBrush.inverseSlopeFilter = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "manualReferenceVectorSampling":
                            meshBrush.manualReferenceVectorSampling = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "showReferenceVectorInSceneGUI":
                            meshBrush.showReferenceVectorInSceneView = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "slopeReferenceVector":
                            meshBrush.slopeReferenceVector = new Vector3(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value), float.Parse(element.Element("z").Value));
                            break;
                        case "slopeReferenceVector_HandleLocation":
                            meshBrush.slopeReferenceVectorSampleLocation = new Vector3(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value), float.Parse(element.Element("z").Value));
                            break;
                        case "yAxisIsTangent":
                            meshBrush.yAxisTangent = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "scattering":
                            var scattering = float.Parse(element.Value);
                            meshBrush.scatteringRange = new Vector2(scattering, scattering);
                            break;
                        case "autoStatic":
                            meshBrush.autoStatic = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "useOverlapFilter":
                            meshBrush.useOverlapFilter = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "randomAbsMinDist":
                            meshBrush.minimumAbsoluteDistanceRange = new Vector2(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value));
                            break;
                        case "uniformScale":
                            meshBrush.uniformRandomScale = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "constantUniformScale":
                            meshBrush.uniformAdditiveScale = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_SetOfMeshesToPaint":
                            meshBrush.meshesFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_Templates":
                            meshBrush.templatesFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_CustomizeKeyboardShortcuts":
                            meshBrush.keyBindingsFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_BrushSettings":
                            meshBrush.brushFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_Slopes":
                            meshBrush.slopesFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_Randomizers":
                            meshBrush.randomizersFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_OverlapFilter":
                            meshBrush.overlapFilterFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_ApplyAdditiveScale":
                            meshBrush.additiveScaleFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "foldoutState_Optimize":
                            meshBrush.optimizationFoldout = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                        case "randomUniformRange":
                            meshBrush.randomScaleRange = new Vector2(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value));
                            break;
                        case "randomNonUniformRange":
                            meshBrush.randomScaleRangeX = meshBrush.randomScaleRangeZ = new Vector2(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value));
                            meshBrush.randomScaleRangeY = new Vector2(float.Parse(element.Element("z").Value), float.Parse(element.Element("w").Value));
                            break;
                        case "constantAdditiveScale":
                            var additiveScale = float.Parse(element.Value);
                            meshBrush.additiveScaleRange = new Vector2(additiveScale, additiveScale);
                            break;
                        case "constantScaleXYZ":
                            meshBrush.additiveScaleNonUniform = new Vector3(float.Parse(element.Element("x").Value), float.Parse(element.Element("y").Value), float.Parse(element.Element("z").Value));
                            break;
                        case "randomRotation":
                            var randomRotation = float.Parse(element.Value);
                            meshBrush.randomRotationRangeY = new Vector2(randomRotation, randomRotation);
                            break;
                        case "autoSelectOnCombine":
                            meshBrush.autoSelectOnCombine = string.CompareOrdinal(element.Value, "true") == 0;
                            break;
                    }
                }

                meshBrush.SaveTemplate(filePath.Replace(".meshbrush", "__migrated.xml"));

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("MeshBrush: Failed to migrate template file \"" + filePath + "\". Perhaps the file is corrupted? " + e.ToString());
                return false;
            }

            return true;
        }
    }
}

// Copyright (C) Raphael Beck, 2017