using System;
using System.Linq;

namespace SFA.DAS.Telemetry.RedactionService;

internal static class UriRedactionOptionsFactory
{
    internal static UriRedactionOptions Create(string keysForRedaction)
    {
        return new UriRedactionOptions
        {
            RedactionList = keysForRedaction
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList()
        };
    }
}
