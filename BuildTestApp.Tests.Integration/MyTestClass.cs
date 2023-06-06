// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xunit;

namespace BuildTestApp.Tests.Integration
{
    public class MyTestClass
    {
        [Fact]
        public void MyTestMethod1()
        {
            // Your test code here
            Assert.True(true);
        }

        [ReleaseCandidateFact]
        public void MyTestMethod2()
        {
            // Your test code here
            Assert.True(true);
        }
    }
}