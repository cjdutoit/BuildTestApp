// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace BuildTestApp.Tests.Integration
{
    public class IntegrationTests
    {
        private readonly ITestOutputHelper output;
        private readonly IConfiguration configuration;

        public IntegrationTests(ITestOutputHelper output)
        {
            this.output = output;

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("BUILDTESTAPP_INTEGRATION_");

            this.configuration = configurationBuilder.Build();
        }

        [Fact]
        public void ThisTestShouldAlwaysRun()
        {
            output.WriteLine($"IS_RELEASE_CANDIDATE={configuration.GetValue<bool>("IS_RELEASE_CANDIDATE")}");
            Assert.True(true);
        }

        [ReleaseCandidateFact]
        public void ThisTestShouldOnlyRunWhenItIsAReleaseCandidate()
        {
            output.WriteLine($"This test should only run if IS_RELEASE_CANDIDATE is set to true.");
            output.WriteLine($"IS_RELEASE_CANDIDATE={configuration.GetValue<bool>("IS_RELEASE_CANDIDATE")}");

            Assert.Fail("This test should only be run when it's a release candidate");
        }
    }
}