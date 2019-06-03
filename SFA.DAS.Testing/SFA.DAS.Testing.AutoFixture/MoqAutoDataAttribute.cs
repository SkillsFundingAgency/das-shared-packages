using System;
using AutoFixture.NUnit3;

namespace SFA.DAS.Testing.AutoFixture
{
    public class MoqAutoDataAttribute : AutoDataAttribute
    {
        public MoqAutoDataAttribute() : base(FixtureBuilder.MoqFixtureFactory)
        {}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MoqInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public MoqInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureBuilder.MoqFixtureFactory, arguments)
        {
        }
    }
}