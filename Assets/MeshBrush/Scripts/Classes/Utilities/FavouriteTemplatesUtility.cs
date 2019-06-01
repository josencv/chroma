using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace MeshBrush
{
    /// <summary>
    /// Class that provides functionality for saving and loading 
    /// lists of MeshBrush template files from and to xml files.
    /// </summary>
    public static class FavouriteTemplatesUtility
    {
        /// <summary>
        /// Saves a list of MeshBrush template file paths to an xml file.<para> </para>
        /// Make sure to include the full template file paths (including the .xml extension) in the provided parameter list; invalid or inexistent files won't be written to the final file.
        /// </summary>
        /// <param name="favouriteTemplates">The list of full template file paths (including the .xml extension) to write to the xml file.</param>
        /// <param name="filePath">The full path (including the .xml extension) to the file where the list of favourite MeshBrush templates should be stored.<para> </para>Existing files will be overwritten.</param>
        /// <returns>The <see cref="XDocument"/> of the favourite templates list that was saved.</returns>
        public static XDocument SaveFavouriteTemplates(List<string> favouriteTemplates, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath", "MeshBrush: the specified file path is null or empty (and thus invalid). Couldn't save favourite templates list...");
            }

            if (favouriteTemplates == null)
            {
                throw new ArgumentNullException("favouriteTemplates", "MeshBrush: The passed list of favourite templates is null. Cancelling saving operation...");
            }

            for (int i = favouriteTemplates.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(favouriteTemplates[i]))
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogWarning("MeshBrush: the specified MeshBrush template file " + favouriteTemplates[i] + " does not exist or has an invalid file path, and thus will be removed from the list of favourite templates.");
#endif
                    favouriteTemplates.RemoveAt(i);
                }
            }
            
            var xDocument = new XDocument(new XElement("favouriteMeshBrushTemplates",
                favouriteTemplates.Select(template => new XElement("template", new XElement("path", template)))));

            xDocument.Save(filePath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            return xDocument;
        }

        /// <summary>
        /// Loads a list of favourite MeshBrush template files from a specified xml file (returning a new list each time).
        /// </summary>
        /// <returns>A new list containing the loaded MeshBrush template file paths.</returns>
        /// <param name="filePath">The full path to the xml file that contains the favourite templates to load.</param>
        public static List<string> LoadFavouriteTemplates(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("MeshBrush: the specified file path is invalid or doesn't exist! Can't load favourite templates list...", "filePath");
            }

            return new List<string>(XDocument.Load(filePath).Descendants("path").Select(path => path.Value));
        }

        /// <summary>
        /// Loads a list of favourite MeshBrush template files from a specified xml file into an existing list of strings.<para> </para>
        /// Use this overload if you want to avoid the allocation cost of returning a new list each time, but be aware that the provided parameter list will be cleared irreversably before any data is written to it.
        /// </summary>
        /// <param name="filePath">The full path to the xml file that contains the favourite templates to load.</param>
        /// <param name="targetList">The list of strings into which the template file paths should be loaded.</param>
        /// <returns>True if the loading procedure was successful; false if it failed somehow.</returns>
        public static bool LoadFavouriteTemplates(string filePath, List<string> targetList)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("MeshBrush: the specified file path is invalid or doesn't exist! Can't load favourite templates list...", "filePath");
            }

            if (targetList == null)
            {
                throw new ArgumentNullException("targetList", "MeshBrush: cannot write favourite templates to the specified target list because it is null.");
            }

            try
            {
                targetList.Clear();
                foreach (var item in XDocument.Load(filePath).Descendants("path"))
                {
                    targetList.Add(item.Value);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("MeshBrush: loading favourite templates list failed. Error message: " + e.Message);
                return false;
            }

            return true;
        }
    }
}

// Copyright (C) Raphael Beck, 2017