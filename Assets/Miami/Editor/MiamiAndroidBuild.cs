using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System;
public static class MiamiAndroidBuild
{
    public static void Build()
    {
        const string model="Assets/Miami/Models/city-real-place-25-77400-80-19400.obj";
        if(AssetDatabase.LoadAssetAtPath<GameObject>(model)!=null) MiamiPlayableGameSetup.CreateScene();
        else MiamiFallbackScene.CreateScene();
        Directory.CreateDirectory("build/Android");
        var options=new BuildPlayerOptions{scenes=new[]{"Assets/Miami/Scenes/Miami_FreeRoam.unity"},locationPathName="build/Android/MiamiFreeRoam.apk",target=BuildTarget.Android,options=BuildOptions.None};
        BuildReport report=BuildPipeline.BuildPlayer(options);
        if(report.summary.result!=BuildResult.Succeeded){Debug.LogError("Android build failed: "+report.summary.result+" / "+report.summary.totalErrors+" errors");throw new Exception("Miami Android build failed");}
        Debug.Log("APK ready: "+Path.GetFullPath(options.locationPathName));
    }
}