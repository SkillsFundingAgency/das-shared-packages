using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Provider.Shared.UI.Extensions
{
    public static class HttpHelperZendeskExtensions
    {
        public static HtmlString SetZendeskSuggestion(this IHtmlHelper html, string suggestion)
        {
            return new HtmlString($"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{EscapeApostrophes(suggestion)}' }});</script>");
        }

        public static IHtmlContent SetZendeskLabels(this IHtmlHelper html, params string[] labels)
        {
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    if (!first) apiCallString += ",";
                    first = false;

                    apiCallString += $"'{ EscapeApostrophes(label) }'";
                }
            }

            apiCallString += "] });</script>";

            return new HtmlString(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }
    }
}