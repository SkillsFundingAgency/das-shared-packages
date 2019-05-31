using System;
using AutoFixture.NUnit3;

namespace SFA.DAS.Testing.AutoFixture
{
    public class EFFriendlyMoqAutoDataAttribute : AutoDataAttribute
    {
        public EFFriendlyMoqAutoDataAttribute() : base(FixtureBuilder.EFFriendlyMoqFixtureFactory)
        {}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EFFriendlyMoqInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public EFFriendlyMoqInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureBuilder.EFFriendlyMoqFixtureFactory, arguments)
        {
        }
    }
}