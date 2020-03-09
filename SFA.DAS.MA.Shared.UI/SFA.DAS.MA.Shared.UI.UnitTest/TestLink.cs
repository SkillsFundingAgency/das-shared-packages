using SFA.DAS.MA.Shared.UI.Models;

namespace SFA.DAS.MA.Shared.UI.UnitTest
{
    public class TestLink : Link
    {
        private readonly string _text;

        public TestLink(string text = "") : base("")
        {
            _text = text;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return "IsSelected" + _text;
            }
            return _text;
        }
    }

    public class TestLink2 : Link
    {
        private readonly string _text;

        public TestLink2(string text = "") : base("")
        {
            _text = text;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return "IsSelected" + _text;
            }
            return _text;
        }
    }
}
