// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV3s;

namespace BuildTestApp.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var aDotNetClient = new ADotNetClient();

            var githubPipeline = new GithubPipeline
            {
                Name = ".Net",

                OnEvents = new Events
                {
                    Push = new PushEvent
                    {
                        Branches = new string[] { "main" }
                    },

                    PullRequest = new PullRequestEvent
                    {
                        Types = new string[] { "opened", "synchronize", "reopened", "closed" },
                        Branches = new string[] { "main" }
                    }
                },

                EnvironmentVariables = new Dictionary<string, string>
                {
                    { "IS_RELEASE_CANDIDATE", EnvironmentVariables.IsGitHubReleaseCandidate() }
                },

                Jobs = new Dictionary<string, Job>
                {
                    {
                        "build",
                        new Job
                        {
                            RunsOn = BuildMachines.UbuntuLatest,

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Setup .Net",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "7.0.201"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildTask
                                {
                                    Name = "Build"
                                },

                                new TestTask
                                {
                                    Name = "Test"
                                }
                            }
                        }
                    },
                    {
                        "add_tag",
                        new Job
                        {
                            RunsOn = BuildMachines.UbuntuLatest,

                            Needs = new string[] { "build" },

                            If =
                                $"needs.build.result == 'success' && {Environment.NewLine}"
                                + $"github.event.pull_request.merged && {Environment.NewLine}"
                                + $"github.event.pull_request.base.ref == 'main' && {Environment.NewLine}"
                                + $"startsWith(github.event.pull_request.title, 'RELEASES:') && {Environment.NewLine}"
                                + $"contains(github.event.pull_request.labels.*.name, 'RELEASES')",

                            Steps = new List<GithubTask>
                            {
                                new CheckoutTaskV3
                                {
                                    Name = "Checkout code",
                                    With = new Dictionary<string, string>
                                    {
                                        { "token", "${{ secrets.PAT_FOR_TAGGING }}" }
                                    }
                                },

                                new ConfigureGitTask()
                                {
                                    Name = "Configure Git",
                                },

                                new ExtractProjectPropertyTask(
                                    projectRelativePath: "BuildTestApp/BuildTestApp.csproj",
                                    propertyName: "Version",
                                    environmentVariableName: "version_number")
                                {
                                    Name = $"Extract Version"
                                },

                                new ExtractProjectPropertyTask(
                                    projectRelativePath: "BuildTestApp/BuildTestApp.csproj",
                                    propertyName: "PackageReleaseNotes",
                                    environmentVariableName: "package_release_notes")
                                {
                                    Name = $"Extract Package Release Notes"
                                },

                                new CreateGitHubTagTask(
                                    tagName: "v${{ env.version_number }}",
                                    tagMessage: "Release - v${{ env.version_number }}")
                                {
                                    Name = "Create GitHub Tag",
                                },

                                new CreateGitHubReleaseTask(
                                    releaseName: "Release - v${{ env.version_number }}",
                                    tagName: "v${{ env.version_number }}",
                                    releaseNotes: "${{ env.package_release_notes }}",
                                    githubToken: "${{ secrets.PAT_FOR_TAGGING }}")
                                {
                                    Name = "Create GitHub Release",
                                    Uses = "actions/create-release@v1",
                                },
                            }
                        }
                    },
                    {
                        "publish",
                        new Job
                        {
                            RunsOn = BuildMachines.UbuntuLatest,
                            Needs = new string[] { "add_tag" },

                            If =
                                "needs.add_tag.result == 'success'",

                            Steps = new List<GithubTask> {
                                new CheckoutTaskV3
                                {
                                    Name = "Check out"
                                },

                                new SetupDotNetTaskV3
                                {
                                    Name = "Setup .Net",

                                    With = new TargetDotNetVersionV3
                                    {
                                        DotNetVersion = "7.0.201"
                                    }
                                },

                                new RestoreTask
                                {
                                    Name = "Restore"
                                },

                                new DotNetBuildReleaseTask
                                {
                                    Name = "Build",
                                },

                                new PackNugetTaskWithSymbols
                                {
                                    Name = "Pack NuGet Package",
                                },

                                new NugetPushTask(nugetApiKey: "${{ secrets.NUGET_API_KEY }}")
                                {
                                    Name = "Push NuGet Package",
                                }
                            },
                        }
                    }
                }
            };

            string buildScriptPath = "../../../../.github/workflows/dotnet.yml";
            string directoryPath = Path.GetDirectoryName(buildScriptPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            aDotNetClient.SerializeAndWriteToFile(githubPipeline, path: buildScriptPath);
        }
    }
}