using SFA.DAS.Authorization.Services;

namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Recruitment : Link
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly string _class;
        private readonly string _role;

        public Recruitment(IAuthorizationService authorizationService, string href, string @class = "", string role = "menuitem") : base(href)
        {
            _authorizationService = authorizationService;
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (_authorizationService != null &&  _authorizationService.IsAuthorized("EmployerFeature.Recruitments"))
            {
                return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">Recruitment</a>";
            }

            return string.Empty;
        }
    }
}
