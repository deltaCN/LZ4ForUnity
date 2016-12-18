using System;
using System.Collections.Generic;
using System.IO;
using LZ4;
using Pathfinding.Ionic.Zip;
using UnityEditor;
using UnityEngine;

public static class LZ4Tool
{

    [MenuItem("Assets/LZ4/CompressToLZ4", false, 110)]
    public static void CompressToLZ4()
    {
        string[] GUIDs = Selection.assetGUIDs;
        LZ4File lz4File = new LZ4File();
        foreach (string guid in GUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            byte[] bytes = File.ReadAllBytes(assetPath);
            lz4File.AddEntry(Path.GetFileName(assetPath), bytes);
        }
        FileStream stream = File.Create("Assets/TestLZ4HC.bytes");
        lz4File.Save(stream);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/LZ4/DecompressToLZ4", false, 111)]
    public static void DecompressLZ4()
    {
        string[] GUIDs = Selection.assetGUIDs;
        foreach (string guid in GUIDs)
        {
            try
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (textAsset == null)
                    continue;

                string fileName = Path.GetFileName(assetPath);
                string extractDirectorName = fileName.Replace(Path.GetExtension(fileName), string.Empty);
                DirectoryInfo info =
                    Directory.CreateDirectory(Path.Combine(Directory.GetParent(assetPath).FullName, extractDirectorName));


                MemoryStream memoryStream = new MemoryStream();
                byte[] textBytes = textAsset.bytes;
                memoryStream.Write(textAsset.bytes, 0, textBytes.Length);
                memoryStream.Position = 0;
                LZ4File lz4File = LZ4File.Read(memoryStream);
                foreach (string entryFileName in lz4File.EntryFileNames)
                {
                    LZ4Entry entry = lz4File[entryFileName];
                    using (FileStream extractMemoryStream = File.Create(Path.Combine(info.FullName, entryFileName)))
                    {
                        entry.Extract(extractMemoryStream);
                        extractMemoryStream.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        AssetDatabase.Refresh();

    }
    [MenuItem("Assets/LZ4/PKZipToLZ4", false, 112)]
    public static void PKZipToLZ4()
    {
        string[] GUIDs = Selection.assetGUIDs;
        foreach (string guid in GUIDs)
        {
            try
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                MemoryStream memoryStream = new MemoryStream();
                byte[] textBytes = textAsset.bytes;
                memoryStream.Write(textAsset.bytes, 0, textBytes.Length);
                memoryStream.Position = 0;
                ZipFile zip = ZipFile.Read(memoryStream);
                LZ4File lz4File = new LZ4File();
                foreach (string entryFileName in zip.EntryFileNames)
                {
                    ZipEntry entry = zip[entryFileName];
                    MemoryStream extractMemoryStream = new MemoryStream();
                    entry.Extract(extractMemoryStream);
                    lz4File.AddEntry(entryFileName, extractMemoryStream.ToArray());
                }

                string fileName = Path.GetFileName(assetPath);
                fileName = fileName.Replace(Path.GetExtension(fileName), String.Empty);
                FileStream saveFileStream = File.Create(assetPath.Replace(fileName, fileName + "LZ4"));
                lz4File.Save(saveFileStream);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/BuilAssetsdBundle/BuildLZ4", false, 110)]
    public static void BuildAssetsBundleLZ4()
    {
        string[] GUIDs = Selection.assetGUIDs;
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        foreach (string guid in GUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string assetName = Path.GetFileName(assetPath);
            assetName = assetName.Replace(Path.GetExtension(assetName), string.Empty);
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild()
            {
                assetBundleName = assetName,
                assetNames = new[] { assetPath }
            };
            assetBundleBuilds.Add(assetBundleBuild);

        }
        DirectoryInfo info = Directory.CreateDirectory("AssetBundleBuild");
        BuildPipeline.BuildAssetBundles("AssetBundleBuild", assetBundleBuilds.ToArray(),
                     BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        EditorUtility.RevealInFinder(info.GetFiles()[0].FullName);
    }


}
