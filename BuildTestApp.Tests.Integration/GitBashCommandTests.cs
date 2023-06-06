// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace BuildTestApp.Tests.Integration
{
    public partial class GitBashCommandTests
    {
        private readonly ITestOutputHelper output;
        private readonly IConfiguration configuration;

        public GitBashCommandTests(ITestOutputHelper output)
        {
            this.output = output;
        }
    }
}