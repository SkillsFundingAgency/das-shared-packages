using System.Text.Json.Serialization;

namespace SFA.DAS.Authorization.TokenGenerator.Models;

public class EmployerUser
{
	[JsonPropertyName("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")]
	public string? EmailAddress { get; set; }

	[JsonPropertyName("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")]
	public string? NameIdentifier { get; set; }

	[JsonPropertyName("sub")]
	public string? Sub { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/id")]
	public string? Id { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/email_address")]
	public string? EmailAddressClaim { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/associatedAccounts")]
	public Dictionary<string,AssociatedAccounts>? AssociatedAccounts { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/given_name")]
	public string? GivenName { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/family_name")]
	public string? FamilyName { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/display_name")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("http://das/employer/identity/claims/account")]
	public string? Account { get; set; }

	[JsonPropertyName("exp")]
	public int? Exp { get; set; }
}

public class AssociatedAccounts
{
        public string? AccountId { get; set; }
	public string? EmployerName { get; set; }
	public string? Role { get; set; }
}
