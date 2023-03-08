using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class CI {

    [MenuItem("CI/Build Android")]
    static void BuildAndroid() {
        EditorPrefs.SetBool("NdkUseEmbedded", true);
        EditorPrefs.SetBool("SdkUseEmbedded", true);
        EditorPrefs.SetBool("JdkUseEmbedded", true);
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        var result = Build(BuildTarget.Android);
        EditorApplication.Exit(result ? 0 : 1);
    }

    [MenuItem("CI/Build IOS")]
    static void BuildIOS() {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.iOSXcodeBuildConfig = XcodeBuildConfig.Debug;

        var result = Build(BuildTarget.iOS);
        EditorApplication.Exit(result ? 0 : 1);
    }

    static bool Build(BuildTarget buildTarget) {
        var outputPath = GetEnvVar("OUTPUT_PATH");
        var bundleId = GetEnvVar("BUNDLE_ID");
        var productName = GetEnvVar("PRODUCT_NAME");
        var companyName = GetEnvVar("COMPANY_NAME");

        if(string.IsNullOrEmpty(outputPath)) {outputPath = "Build";}
        if(!string.IsNullOrEmpty(companyName)) {PlayerSettings.companyName = companyName;}
        if(!string.IsNullOrEmpty(productName)) {PlayerSettings.productName = productName;}
        if(!string.IsNullOrEmpty(bundleId)) {PlayerSettings.applicationIdentifier = bundleId;}

        var buildOptions = BuildOptions.Development | BuildOptions.CompressWithLz4;
        outputPath = AddExpand(buildTarget, outputPath);

        var report = BuildPipeline.BuildPlayer(GetEnabledScenes(), outputPath, buildTarget, buildOptions);
        var summary = report.summary;

        if(summary.result == BuildResult.Succeeded) {
            Debug.Log($"Build Success: {outputPath}");
            return true;
        } else {
            Debug.Assert(false, $"Build Error: {report.name}");
            return false;
        }
    }

    static string GetEnvVar(string pKey) {
        return Environment.GetEnvironmentVariable(pKey);
    }

    static string AddExpand(BuildTarget buildTarget, string outputPath) {
        switch(buildTarget) {
            case BuildTarget.Android :
                outputPath += "/Android/Build.apk";
                break;
            case BuildTarget.iOS :
                outputPath += "/Xcode";
                break;
        }
        return outputPath;
    }

    static string[] GetEnabledScenes() {
        return (
            from scene in EditorBuildSettings.scenes
            where scene.enabled
            where !string.IsNullOrEmpty(scene.path)
            select scene.path
        ).ToArray();
    }
}
