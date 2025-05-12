using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleCreator
{
    [MenuItem("Assets/Create Asset Bundles")]
    private static void BuildAllAssetBundles()
    {
        string assetBundleDirPath = Application.dataPath + "/../DLCs";
        if(!Directory.Exists(assetBundleDirPath))
        {
            Directory.CreateDirectory(assetBundleDirPath);
        }
        try
        {
            BuildPipeline.BuildAssetBundles(assetBundleDirPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
