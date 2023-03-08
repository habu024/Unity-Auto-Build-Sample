using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class IOSBitcodePostprocessor : IPostprocessBuildWithReport {
    public int callbackOrder {get {return 0;}}
    public bool enableBitcode = false;
    
    public void OnPostprocessBuild(BuildReport report) {
        if(report.summary.platform != BuildTarget.iOS) {return;}

        var project = new PBXProject();
        var pbxPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
        project.ReadFromFile(pbxPath);
        SetupBitcodeFramework(project);
        SetupBitcodeMain(project);
        project.WriteToFile(pbxPath);
    }

    void SetupBitcodeFramework(PBXProject project) {
        SetupBitCode(project, project.GetUnityFrameworkTargetGuid());
    }

    void SetupBitcodeMain(PBXProject project) {
        SetupBitCode(project, project.GetUnityMainTargetGuid());
    }

    void SetupBitCode(PBXProject project, string targetGUID) {
        project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", enableBitcode ? "YES" : "NO");
    }
}
