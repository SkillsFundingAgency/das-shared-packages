using SFA.DAS.Authorization.TokenGenerator.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Authorization.TokenGenerator.Providers;

internal class UserProvider
    {
        internal static List<Claim> GenerateUserClaims(string userType, AppSettings appSettings)
        {
		switch (userType)
		{
			case "P":
				return GetProviderUserClaims(appSettings.ProviderUser);
			case "E":
				return GetEmployerUserClaims(appSettings.EmployerUser);
		} 
		throw new ArgumentException("Invalid user type");
        }

	private static List<Claim> GetEmployerUserClaims(EmployerUser? employerUser)
	{
		if (employerUser == null)
		{
			employerUser = new EmployerUser();
		}

		var claims = new List<Claim>
		{
			employerUser.CreateClaim(x => x.EmailAddress),
			employerUser.CreateClaim(x => x.NameIdentifier),
			employerUser.CreateClaim(x => x.Sub),
			employerUser.CreateClaim(x => x.GivenName),
			employerUser.CreateClaim(x => x.FamilyName),
			employerUser.CreateClaim(x => x.DisplayName),
			employerUser.CreateClaim(x => x.Id),
			employerUser.CreateClaim(x => x.EmailAddressClaim),
			employerUser.CreateClaim(x => x.Account),
			employerUser.CreateClaim(x => x.AssociatedAccounts),
			CreateExpiryClaim()
		};

		return claims;
	}

	private static List<Claim> GetProviderUserClaims(ProviderUser? providerUser)
        {
		if (providerUser == null)
		{
			providerUser = new ProviderUser();
		}

		var claims = new List<Claim>
		{
			providerUser.CreateClaim(x => x.Sub),
			providerUser.CreateClaim(x => x.Email),
			providerUser.CreateClaim(x => x.GivenName),
			providerUser.CreateClaim(x => x.FamilyName),
			providerUser.CreateClaim(x => x.Organisation),
			providerUser.CreateClaim(x => x.Sid),
			providerUser.CreateClaim(x => x.PortalComService),
			providerUser.CreateClaim(x => x.RoleCode),
			providerUser.CreateClaim(x => x.RoleId),
			providerUser.CreateClaim(x => x.RoleName),
			providerUser.CreateClaim(x => x.RoleNumericId),
			providerUser.CreateClaim(x => x.IdentityClaimsName),
			providerUser.CreateClaim(x => x.PortalComDisplayName),
			providerUser.CreateClaim(x => x.PortalComUkPrn),
			CreateExpiryClaim()
		};

		return claims;
	}

	private static Claim CreateExpiryClaim()
	{
		long unixTime = ((DateTimeOffset)DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds();
		return new Claim("exp", unixTime.ToString());
	}
}

internal static class UserExtensions
{
	internal static Claim CreateClaim<T>(this T user, Expression<Func<T, string?>> expression)
	{
		var value = expression.Compile().Invoke(user);
		return new Claim(GetJsonPropertyName<T, string>(expression), value ?? string.Empty);
	}

	internal static Claim CreateClaim<T>(this T user, Expression<Func<T, Organisation?>> expression)
	{
		var organisationValue = expression.Compile().Invoke(user);
		var organisationValueAsJson = JsonSerializer.Serialize(organisationValue);
		return new Claim(GetJsonPropertyName<T, Organisation?>(expression), organisationValueAsJson, JsonClaimValueTypes.Json);
	}

	internal static Claim CreateClaim<T, TDictionaryValue>(this T user, Expression<Func<T, Dictionary<string, TDictionaryValue>?>> expression)
	{
		var dictionary = expression.Compile().Invoke(user);
		var dictionaryAsJson = JsonSerializer.Serialize(dictionary);
		return new Claim(GetJsonPropertyName<T, Dictionary<string, TDictionaryValue>?>(expression), dictionaryAsJson, JsonClaimValueTypes.Json);
	}

	private static string GetJsonPropertyName<T, TValueType>(Expression<Func<T, TValueType?>> expression)
	{
		var memberExpression = expression.Body as MemberExpression;
		if (memberExpression == null)
		{
			throw new ArgumentException("Expression must be a member expression");
		}

		var objectPropertyName = memberExpression.Member.Name;

		var property = typeof(T).GetProperty(objectPropertyName);
		var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
		return attribute?.Name;
	}
}
