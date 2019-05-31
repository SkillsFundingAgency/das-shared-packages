using System;
using AutoFixture.NUnit3;

namespace SFA.DAS.Testing.AutoFixture
{
    public class RecursiveMoqAutoDataAttribute : AutoDataAttribute
    {
        public RecursiveMoqAutoDataAttribute() : base(FixtureBuilder.RecursiveMoqFixtureFactory)
        {}
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RecursiveMoqInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public RecursiveMoqInlineAutoDataAttribute(params object[] arguments)
            : base(FixtureBuilder.RecursiveMoqFixtureFactory, arguments)
        {
        }
    }
}