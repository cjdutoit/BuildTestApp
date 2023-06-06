// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;


namespace BuildTestApp.Tests.Integration
{
    public partial class GitBashCommandTests
    {
        [Fact]
        public void ExtractPackageReleaseNotes_ShouldReturnExpectedResult()
        {
            // Given

            StringBuilder result = new StringBuilder();
            result.AppendLine("Adding Support For Generic Job Definitions");
            result.AppendLine();
            result.AppendLine("BREAKING CHANGE:");
            result.AppendLine();
            result.AppendLine("We are excited to announce a significant update to how we define jobs for GitHub Actions using ADotNet!");
            result.AppendLine("Starting from this release, we have introduced a breaking change that allows the usage of generic job");
            result.AppendLine("definitions, replacing the previous job names specifically used for build, tagging, and release.");
            result.AppendLine("This change requires a small code modification in how you define jobs.You will now have the");
            result.AppendLine("flexibility to define jobs based on your specific needs, enhancing customization and providing");
            result.AppendLine("a more streamlined workflow.");
            result.AppendLine();
            result.AppendLine("Please refer to the updated documentation for guidance on adapting your");
            result.AppendLine("code to accommodate this change.");
            string expectedResult = result.ToString();

            List<string> gitBashCommands = new List<string>
            {
                "cd /C/repos/BuildTestApp/BuildTestApp.Tests.Integration/bin/Debug/net7.0",
                "package_release_notes=$(awk -v RS='' -F'</?PackageReleaseNotes>' 'NF>1{print $2}' Resources/Project.txt | sed -e 's/^[[:space:]]*//')",
                "echo 'package_release_notes<<EOF' >> $GITHUB_ENV",
                "echo -e \"$package_release_notes\" >> $GITHUB_ENV",
                "echo 'EOF' >> $GITHUB_ENV",
                "echo \"PackageReleaseNotes - ${{ env.package_release_notes }}\""
            };

            // When
            var commandClient = new CommandClient(
                commandPath: "bash",
                arguments: "",
                useShellExecute: false,
                createNoWindow: false);

            string actualResult = commandClient.ExecuteCommand(gitBashCommands);

            // Then
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}