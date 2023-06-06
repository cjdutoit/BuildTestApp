// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Xunit;
using Xunit.Sdk;

namespace BuildTestApp.Tests.Integration
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer(
        "BuildTestApp.Tests.Integration.ReleaseCandidateTestCaseDiscoverer",
        "BuildTestApp.Tests.Integration")]
    public class ReleaseCandidateFactAttribute : FactAttribute { }
}
