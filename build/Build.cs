using System;
using System.IO;
using System.Reflection;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.CompileAndSignSetup);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    
    //static string timeServer = "http://timestamp.digicert.com";
    static string timeServer = "http://timestamp.sectigo.com";
    //static string timeServer = "http://timestamp.entrust.net/TSS/RFC3161sha2TS";
    //static string timeServer = "http://timestamp.comodoca.com/rfc3161";
    //static string timeServer = "http://timestamp.globalsign.com/scripts/timstamp.dll";
    //static string timeServer = "http://timestamp.apple.com/ts01";
    //static string timeServer = "http://timestamp.globalsign.com/?signature=sha2";
    
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.GetProject(Guid.Parse("{6873BEC7-9DB6-4F23-9441-65FF70E7B2B2}"))));
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

    [Parameter] [Secret] readonly string CertificatePassword;

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
                    .SetFile(@"E:\cert\codingconnected_cert.pfx")
                    .SetPassword(CertificatePassword)
                    .SetDescription("YAVV")
                    .SetRfc3161TimestampServerUrl(timeServer)
                    .SetTimestampServerDigestAlgorithm("SHA256")
                    .SetFileDigestAlgorithm("SHA256")
                    .SetFiles(spublishDirectory / "MiniTDSetup.msi"));

            var file = OutputDirectory / $"{DateTime.Now:yyyy-MM-dd}-Setup_MiniTD_v{version.ToString(3).Replace('.', '_')}.msi";
            if (!Directory.Exists(OutputDirectory)) Directory.CreateDirectory(OutputDirectory);
            if (File.Exists(file)) DeleteFile(file);
            File.Copy(spublishDirectory / "MiniTDSetup.msi", file);
        });

}
