using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "ci",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.Push],
    InvokedTargets = [nameof(Test)],
    ImportSecrets = [nameof(NuGetApiKey)])]
class Build : NukeBuild
{
    [Solution]
    readonly Solution Solution;
    
    [Parameter("Configuration to build - Default is 'Release'")]
    readonly Configuration Configuration = Configuration.Release;
    
    [Parameter("NuGet API Key")]
    readonly string NuGetApiKey;
    
    [Parameter("NuGet Source URL")]
    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";
    
    [MinVer]
    readonly MinVer MinVer;
    
    AbsolutePath ArtifactsDirectory
        => RootDirectory / "artifacts";
    
    AbsolutePath ChangelogFile
        => RootDirectory / "CHANGELOG.md";
    
    AbsolutePath DocfxConfig
        => RootDirectory / "docfx.json";
    
    AbsolutePath DocfxSite
        => RootDirectory / "_site";
    
    AbsolutePath CliffConfig
        => RootDirectory / "cliff.toml";

    public static int Main()
        => Execute<Build>(x => x.Pack);

    Target Clean => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetDeterministic(true)
                // Inject versioning properties
                .SetProperty("Version", MinVer.Version)
                .SetProperty("AssemblyVersion", MinVer.AssemblyVersion)
                .SetProperty("FileVersion", MinVer.FileVersion));
        });
    
    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });
    
    Target GenerateChangelog => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            // Ensure git-cliff is installed
            // You can install git-cliff as a dev dependency or ensure it's available in PATH
            // Here, we assume git-cliff is installed and available in PATH

            Log.Information("Generating CHANGELOG.md using git-cliff...");

            // Check if config file exists
            if (!CliffConfig.FileExists())
            {
                throw new Exception($"Configuration file not found: {CliffConfig}");
            }
            
            // Run git-cliff command
            ProcessTasks.StartProcess("git-cliff", $"--config {CliffConfig} --output {ChangelogFile}");

            // Log.Information("CHANGELOG.md generated successfully.");
            Log.Information($"{ChangelogFile} generated successfully.");
            
            // Amend commit with the updated CHANGELOG.md
            GitTasks.Git($"add {ChangelogFile}");
            GitTasks.Git($"commit --amend --no-edit");
            
            Log.Information($"Amended commit with updated {ChangelogFile}.");   
        });

    Target Pack => _ => _
        .DependsOn(GenerateChangelog, Test)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetIncludeSymbols(true)
                .SetIncludeSource(true)
                .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg)
                // Use MinVer version for the package
                .SetVersion(MinVer.Version));
        });

    Target GenerateDocs => _ => _
        // .DependsOn(Pack)
        .Executes(() =>
        {
            Log.Information("Generating documentation with DocFX...");

            // Build documentation
            DocFXTasks.DocFXBuild(s => s
                .SetConfigFile(DocfxConfig)
                .SetOutputFolder(DocfxSite));

            Log.Information("Documentation generated successfully at '{0}'.", DocfxSite);
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            ArtifactsDirectory
                .GlobFiles("*.nupkg")
                .ForEach(package =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource(NuGetSource)
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                });

            ArtifactsDirectory
                .GlobFiles("*.snupkg")
                .ForEach(symbolsPackage =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(symbolsPackage)
                        .SetSource("https://api.nuget.org/v3/index.json")
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                });
        });
}
