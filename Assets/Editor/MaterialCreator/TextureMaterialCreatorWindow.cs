using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;

public class TextureMaterialCreatorWindow : EditorWindow
{
    public string folderPath;
    public string outputFolderPath;
    public Material baseMaterial;
    public Shader shader;
    public string materialPrefix = "";
    public string materialSuffix = "";

    public bool overrideAlphaIsTransparency;
    public bool alphaIsTransparencyNew;
    public bool overrideTexturePropertyName;
    public string materialTexturePropertyName = "";
    public bool overrideExistingMaterials;

    [MenuItem("FMPUtils/Texture Material Creator")]
    public static void ShowWindow(){
        EditorWindow.GetWindow(typeof(TextureMaterialCreatorWindow));
    }

    private void OnGUI() {
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

        materialPrefix = EditorGUILayout.TextField("Material Prefix", materialPrefix);
        materialSuffix = EditorGUILayout.TextField("Material Suffix", materialSuffix);

        baseMaterial = EditorGUILayout.ObjectField("Template Material", baseMaterial, typeof(Material), false) as Material;
        shader = EditorGUILayout.ObjectField("Shader Override", shader, typeof(Shader), false) as Shader;

        EditorGUILayout.LabelField("Override alphaIsTransparency in Textures");
        overrideAlphaIsTransparency = EditorGUILayout.BeginToggleGroup("Override alphaIsTransparency", overrideAlphaIsTransparency);

        EditorGUILayout.LabelField("alphaIsTransparency new value");
        alphaIsTransparencyNew = EditorGUILayout.Toggle(alphaIsTransparencyNew);

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.LabelField("Use custom texture property name on material");
        overrideTexturePropertyName = EditorGUILayout.BeginToggleGroup("Use custom material property", overrideTexturePropertyName);

        materialTexturePropertyName = EditorGUILayout.TextField("Shader Texture Property Name", materialTexturePropertyName);

        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.LabelField("Should existing materials in output be overriden?");
        overrideExistingMaterials = EditorGUILayout.Toggle(overrideExistingMaterials);

        EditorGUILayout.LabelField("Folder Selection", EditorStyles.boldLabel);

        EditorGUILayout.LabelField(string.Format("Input folder: {0}", (!string.IsNullOrEmpty(folderPath) ? folderPath : "[not assigned]")));
        EditorGUILayout.LabelField(string.Format("Output folder: {0}", (!string.IsNullOrEmpty(outputFolderPath) ? outputFolderPath : "[not assigned]")));

        if (GUILayout.Button("Use folder of selected asset as input"))
        {
            AssignFolderOfSelectedObject(ref folderPath);
        }
        if (GUILayout.Button("Use folder of selected asset as output"))
        {
            AssignFolderOfSelectedObject(ref outputFolderPath);
        }

        if (GUILayout.Button("Select Texture Input Folder"))
        {
            string folderPathTemp = null;
            if (TryGetFolderSelection(out folderPathTemp, "Select folder with Textures")){
                folderPath = folderPathTemp;
            }
        }

        if (GUILayout.Button("Select Material Output Folder"))
        {
            string folderPathTemp = null;
            if (TryGetFolderSelection(out folderPathTemp, "Select folder to store Materials to")){
                folderPath = folderPathTemp;
            }
        }

        EditorGUILayout.LabelField("Asset Generation", EditorStyles.boldLabel);
        if (GUILayout.Button("Create Materials"))
        {
            CreateMaterials();
        }
    }

    private void CreateMaterials(){
        if (!Directory.Exists(folderPath))
        {
            DisplaySimpleDialog("Invalid folder", string.Format("Input folder path {0} is not valid", folderPath));
            return;
        }
        if (!Directory.Exists(outputFolderPath))
        {
            DisplaySimpleDialog("Invalid folder", string.Format("Output folder path {0} is not valid", outputFolderPath));
            return;
        }
        string inputFolderAssetsRelative = null;
        if (!TryGetAssetsLocalPathForExistingFolder(folderPath, out inputFolderAssetsRelative))
        {
            DisplaySimpleDialog("Invalid folder", string.Format("Input folder path {0} exists but was found to not belong into this project", folderPath));
            return;
        }
        string outputFolderAssetsRelative = null;
        if (!TryGetAssetsLocalPathForExistingFolder(outputFolderPath, out outputFolderAssetsRelative))
        {
            DisplaySimpleDialog("Invalid folder", string.Format("Input folder path {0} exists but was found to not belong into this project", outputFolderPath));
            return;
        }

        var baseMaterialPrev = baseMaterial;
        var shaderPrev = shader;

        if (baseMaterial == null)
        {
            Shader initShader = shader != null ? shader : Shader.Find("Standard");
            baseMaterial = new Material(initShader);
        }
        else if (shader != null)
        {
            baseMaterial.shader = shader;
        }

        Debug.Log(string.Format("Output folder path: {0}", outputFolderPath));
        string outputPathAssetsRelativeTrimmed = outputFolderAssetsRelative.TrimEnd('/').TrimEnd('\\');

        // Method returns the GUIDs of the assets: 
        // https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        string[] assetGUIDs = AssetDatabase.FindAssets("t:texture2D", new string[] { inputFolderAssetsRelative });
        foreach (var guid in assetGUIDs) 
        {
            try {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D currentTexture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
                if (currentTexture != null)
                {
                    if (overrideAlphaIsTransparency)
                    {
                        currentTexture.alphaIsTransparency = alphaIsTransparencyNew;
                        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        if (textureImporter != null)
                        {
                            textureImporter.alphaIsTransparency = alphaIsTransparencyNew;
                        } 
                    }
                    Material currentMaterial = new Material(baseMaterial); // copies all properties
                    if (overrideTexturePropertyName){
                        if (currentMaterial.HasProperty(materialTexturePropertyName)) 
                        {
                            currentMaterial.SetTexture(materialTexturePropertyName, currentTexture);
                        }
                        else {
                            Debug.LogError(string.Format("Override Texture Property Name: material does not have a property called {0}. Default property name is used", materialTexturePropertyName));
                            currentMaterial.mainTexture = currentTexture;
                        }
                    } 
                    else 
                    {
                        currentMaterial.mainTexture = currentTexture;
                    }
            
                    string textureName = currentTexture.name;
                    string fileName = GetSanetizedFileName(materialPrefix).Replace("/", "") + textureName + GetSanetizedFileName(materialSuffix).Replace("/", "") + ".mat";
                    string fullSavePath = outputPathAssetsRelativeTrimmed + "/" + fileName;

                    if (!overrideExistingMaterials && !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(fullSavePath)))
                    {
                        // if we don't allow material overwriting and there is an asset at our path (AssetDatabase.AssetPathToGUID not returning null) 
                        // just omit the asset creation
                        Destroy(currentMaterial);
                        continue;
                    }
                    // CreateAsset remark: If an asset already exists at path it will be deleted prior to creating a new asset. 
                    AssetDatabase.CreateAsset(currentMaterial, fullSavePath);
                }
            } 
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("Something has gone wrong while processing asset at path {0}: {1}", AssetDatabase.GUIDToAssetPath(guid), e));
            }
        }

        shader = shaderPrev;
        // If we created a material only for this purpose, destroy it again
        if (baseMaterial != null && baseMaterialPrev == null)
        {
            GameObject.DestroyImmediate(baseMaterial);
        }
        baseMaterial = baseMaterialPrev;
        // Save new alphaIsTransparency setting for asset loaders if neccessary
        AssetDatabase.SaveAssets(); 
        AssetDatabase.Refresh();
    }

    private string GetSanetizedFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }
        var invalidChars = System.IO.Path.GetInvalidFileNameChars();
        StringBuilder sb = new StringBuilder("");
        foreach (char c in fileName){
            if (System.Array.IndexOf(invalidChars, c) < 0) 
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Checks if the passed string folederPathFull starts with Application.dataPath. If that is not the case, 
    /// assumes that the root Assets folder of the project "Assets/" is the last Assets folder string in the supplied folderPathFull
    /// Already existing asset relative paths start with "Assets/", so we account for that
    /// stores the path result into assetLocalPath
    /// </summary>
    private bool TryGetAssetsLocalPathForExistingFolder(string folderPathFull, out string assetLocalPath)
    {
        if (string.IsNullOrEmpty(folderPathFull))
        {
            assetLocalPath = string.Empty;
            return false;
        }
        // try using Application.dataPath at first, 
        // Contains the path to the game data folder (Read Only).
        // Platform dependend: Unity Editor: <path to project folder>/Assets
        folderPathFull = folderPathFull.Trim(); // remove white space chars
        int startIndex = 0;
        string dataPath = Application.dataPath;
        if (folderPathFull.StartsWith(dataPath))
        {
            int charactersToStrip = "Assets".Length;
            startIndex = dataPath.Length - charactersToStrip;
        }
        else 
        {
            int assetsIndex = folderPathFull.LastIndexOf("Assets/");
            if (assetsIndex == -1){
                assetsIndex = folderPathFull.LastIndexOf(@"Assets\");
            }
            if (assetsIndex == -1){
                assetLocalPath = string.Empty;
                return false;
            }
            startIndex = assetsIndex;
        }  
        int charLength = folderPathFull.Length - startIndex;
        assetLocalPath = folderPathFull.Substring(startIndex, charLength);
        Debug.Log(string.Format("TryGetAssetsLocalPathForExistingFolder: asset local path {0}, is valid: {1}", assetLocalPath, AssetDatabase.IsValidFolder(assetLocalPath)));
        return (AssetDatabase.IsValidFolder(assetLocalPath));
    }

    private bool TryGetFolderSelection(out string resultPath, string dialogueTitle)
    {
        resultPath = EditorUtility.OpenFolderPanel(dialogueTitle, "", "");
        if (resultPath.Length != 0 && Directory.Exists(resultPath))
        {
            return true;
        }
        return false;
    }

    private void DisplaySimpleDialog(string title, string text)
    {
        EditorUtility.DisplayDialog(title, text, "Ok");
    }

    private void AssignFolderOfSelectedObject(ref string output)
    {
        var activeObj = Selection.activeObject;
            if (activeObj != null){
                string folderPathTemp = AssetDatabase.GetAssetPath(activeObj);
                if (Directory.Exists(folderPathTemp)){
                    output = folderPathTemp;
                }
            }
    }
}
