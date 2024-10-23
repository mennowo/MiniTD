using System;
using System.IO;
using System.Reflection;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.CompileAndSignSetup);

    private readonly Configuration Configuration = Configuration.Release;

    [Solution] readonly Solution Solution;

    private static AbsolutePath SourceDirectory => RootDirectory / "src";
    private static AbsolutePath TestsDirectory => RootDirectory / "tests";
    private static AbsolutePath OutputDirectory => RootDirectory / "output";
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.GetProject("MiniTD")));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuildTasks.MSBuild(_ => _
                .SetProjectFile(Solution.GetProject("MiniTD"))
                .SetConfiguration(Configuration)
                .DisableRestore());
        });

    Target CompileAndSignSetup => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            MSBuildTasks.MSBuild(_ => _
                .SetProjectFile(Solution.GetProject("MiniTDSetup"))
                .SetConfiguration(Configuration));

            var sproject = Solution.GetProject("MiniTDSetup");
            var spublishDirectory = sproject.Directory / "bin" / "Release";
            var project = Solution.GetProject("MiniTD");
            var publishDirectory = project.Directory / "bin" / "Release";
            
            var version = Assembly.LoadFrom(publishDirectory / "MiniTD.exe").GetName().Version;

            SignToolTasks.SignTool(_ => _
                        .SetFile("C:\\Users\\menno\\CodingConnected\\Various\\CodeCert\\cert-cc-2023-2026.cer")
                        .SetDescription("MiniTD")
                        .SetFiles(spublishDirectory / "MiniTDSetup.msi"));

            var file = OutputDirectory / $"{DateTime.Now:yyyy-MM-dd}-Setup_MiniTD_v{version.ToString(3).Replace('.', '_')}.msi";
            if (!Directory.Exists(OutputDirectory)) Directory.CreateDirectory(OutputDirectory);
            if (File.Exists(file)) file.DeleteFile();
            File.Copy(spublishDirectory / "MiniTDSetup.msi", file);
        });

}
