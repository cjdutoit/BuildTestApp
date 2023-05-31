// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace BuildTestApp.Tests.Unit
{
    public class UnitTest
    {
        private readonly ITestOutputHelper output;
        private readonly IConfiguration configuration;

        public UnitTest(ITestOutputHelper output)
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
            Assert.True(true);
        }
    }
}