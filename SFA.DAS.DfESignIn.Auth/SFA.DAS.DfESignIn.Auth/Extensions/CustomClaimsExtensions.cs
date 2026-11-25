using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.Extensions
{
    internal static class CustomClaimsExtensions
    {
        internal static async Task PopulateCustomClaimsAsync(
            this TokenValidatedContext context)
        {
            var customClaims = context.HttpContext.RequestServices
                .GetService<ICustomClaims>();

            if (customClaims == null)
            {
                return;
            }

            var additionalClaims = await customClaims.GetClaimsAsync(context);

            if (context.Result?.Handled == true ||
                context.Result?.Skipped == true ||
                context.Result?.Failure != null)
            {
                return;
            }

            var identity = context.Principal?.Identity as ClaimsIdentity;

            if (identity == null)
            {
                throw new InvalidOperationException(
                    "The custom claims handler must provide a principal with a claims identity.");
            }

            if (additionalClaims == null)
            {
                throw new InvalidOperationException(
                    "The custom claims handler must return a claims collection.");
            }

            identity.AddClaims(additionalClaims);
        }

        internal static async Task PopulateCustomClaimsAsync(
            this TicketReceivedContext context)
        {
            var customClaimsContext = new TokenValidatedContext(
                context.HttpContext,
                context.Scheme,
                (OpenIdConnectOptions)context.Options,
                context.Principal,
                context.Properties);

            await customClaimsContext.PopulateCustomClaimsAsync();

            if (customClaimsContext.Result?.Handled == true)
            {
                context.HandleResponse();
            }
            else if (customClaimsContext.Result?.Skipped == true)
            {
                context.SkipHandler();
            }
            else if (customClaimsContext.Result?.Failure != null)
            {
                context.Fail(customClaimsContext.Result.Failure);
            }
            else
            {
                context.Principal = customClaimsContext.Principal;
                context.Properties = customClaimsContext.Properties;
            }
        }
    }
}