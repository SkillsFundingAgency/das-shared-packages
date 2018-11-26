using System;
using System.Collections.Specialized;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.AutoConfiguration.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class EnvironmentServiceTests : FluentTest<EnvironmentTestsFixture>
    {
        [TestCase(DasEnv.LOCAL)]
        [TestCase(DasEnv.AT)]
        [TestCase(DasEnv.TEST)]
        [TestCase(DasEnv.TEST2)]
        [TestCase(DasEnv.PREPROD)]
        [TestCase(DasEnv.PROD)]
        [TestCase(DasEnv.MO)]
        [TestCase(DasEnv.DEMO)]
        public void WhenGettingCurrent_ThenShouldReturnCurrentEnvironment(DasEnv env)
        {
            Run(f => f.SetCurrent(env), f => f.Current(), (f, r) => r.Should().Be(env));
        }

        [TestCase(false, DasEnv.PROD, new DasEnv[] { })]
        [TestCase(true, DasEnv.MO, new[] { DasEnv.MO })]
        [TestCase(false, DasEnv.MO, new[] { DasEnv.PROD })]
        [TestCase(true, DasEnv.DEMO, new[] { DasEnv.MO, DasEnv.DEMO })]
        [TestCase(false, DasEnv.DEMO, new[] { DasEnv.MO, DasEnv.PROD })]
        public void WhenSuppliedEnvironmentsAreCheckedIfAnyIsTheCurrentEnvironment_TheShouldReturnCorrectIfCurrentStatus(bool expected, DasEnv current, DasEnv[] toCheck)
        {
            Run(f => f.SetCurrent(current), f => f.IsCurrent(toCheck), (f, r) => r.Should().Be(expected));
        }

        [Test]
        public void WhenGetVariableCalledAndAVariableExists_ThenShouldReturnCorrectValue()
        {
            Run(f => f.SetVariable(f.ExpectedKey, f.ExpectedValue), f => f.GetVariable(f.ExpectedKey), (f, r) => r.Should().Be(f.ExpectedValue));
        }
    }

    public class EnvironmentTestsFixture
    {
        public EnvironmentService EnvironmentService { get; set; }
        public NameValueCollection AppSettings { get; set; }
        public string ExpectedKey { get; set; }
        public string ExpectedValue { get; set; }
        public string ExpectedPrefix { get; set; }

        public EnvironmentTestsFixture()
        {
            AppSettings = new NameValueCollection();
            ExpectedKey = "keyone";
            ExpectedValue = "valueone";
            ExpectedPrefix = "AppSettings_";
            EnvironmentService = new EnvironmentService();
        }

        public EnvironmentTestsFixture SetCurrent(DasEnv environment)
        {
            Environment.SetEnvironmentVariable("AppSettings_EnvironmentName", environment.ToString());
            return this;
        }

        public EnvironmentTestsFixture SetVariable(string key, string value)
        {
            Environment.SetEnvironmentVariable(ExpectedPrefix + key, value);
            return this;
        }

        public DasEnv Current()
        {
            return (DasEnv)Enum.Parse(typeof(DasEnv), EnvironmentService.GetVariable("EnvironmentName"));
        }

        public bool IsCurrent(params DasEnv[] environment)
        {
            return EnvironmentService.IsCurrent(environment);
        }

        public string GetVariable(string variableName)
        {
            return EnvironmentService.GetVariable(variableName);
        }
    }
}
