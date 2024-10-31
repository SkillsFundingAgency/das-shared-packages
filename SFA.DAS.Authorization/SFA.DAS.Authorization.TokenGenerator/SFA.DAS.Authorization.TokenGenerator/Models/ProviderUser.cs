using System.Text.Json.Serialization;

namespace SFA.DAS.Authorization.TokenGenerator.Models;

public class ProviderUser
{
	[JsonPropertyName("sub")]
	public string? Sub { get; set; }

	[JsonPropertyName("email")]
	public string? Email { get; set; }

	[JsonPropertyName("given_name")]
	public string? GivenName { get; set; }

	[JsonPropertyName("family_name")]
	public string? FamilyName { get; set; }

	[JsonPropertyName("organisation")]
	public Organisation? Organisation { get; set; }

	[JsonPropertyName("sid")]
	public string? Sid { get; set; }

	[JsonPropertyName("http://schemas.portal.com/service")]
	public string? PortalComService { get; set; }

	[JsonPropertyName("rolecode")]
	public string? RoleCode { get; set; }

	[JsonPropertyName("roleId")]
	public string? RoleId { get; set; }

	[JsonPropertyName("roleName")]
	public string? RoleName { get; set; }

	[JsonPropertyName("rolenumericid")]
	public string? RoleNumericId { get; set; }

	[JsonPropertyName("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")]
	public string? IdentityClaimsName { get; set; }

	[JsonPropertyName("http://schemas.portal.com/displayname")]
	public string? PortalComDisplayName { get; set; }

	[JsonPropertyName("http://schemas.portal.com/ukprn")]
	public string? PortalComUkPrn { get; set; }

	[JsonPropertyName("exp")]
	public int? Expiry { get; set; }
}

public class Organisation
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }
}
