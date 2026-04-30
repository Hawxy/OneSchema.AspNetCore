using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


[GitHubActions(
    "Build & Test",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = ["main"],
    OnPullRequestBranches = ["main"],
    InvokedTargets = [nameof(Test)])]
[GitHubActions(
    "Manual Nuget Push",
    GitHubActionsImage.UbuntuLatest,
    On = [GitHubActionsTrigger.WorkflowDispatch],
    InvokedTargets = [nameof(NugetPush)],
    ImportSecrets = [nameof(NugetApiKey)])]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Solution(GenerateProjects = true)] readonly Solution SolutionRoot;
    
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(SolutionRoot));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(SolutionRoot)
                .SetConfiguration("Release")
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _.AddProcessAdditionalArguments("--project", SolutionRoot.GetProject("OneSchema.AspNetCore.Tests")!.Directory));
        });

    Target NugetPack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SolutionRoot.GetProject("OneSchema.AspNetCore"))
                .SetConfiguration("Release")
                .EnableContinuousIntegrationBuild()
                .SetOutputDirectory(ArtifactsDirectory));
        });

    [Parameter("Nuget Api Key")] [Secret] readonly string NugetApiKey;

    Target NugetPush => _ => _
        .DependsOn(NugetPack)
        .Requires(() => !string.IsNullOrEmpty(NugetApiKey))
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetTargetPath(ArtifactsDirectory / "*.nupkg")
                .EnableSkipDuplicate()
                .EnableNoSymbols()
                .SetApiKey(NugetApiKey));
        });

}
