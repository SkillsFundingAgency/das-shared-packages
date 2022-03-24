using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Provider.Shared.UI.Extensions
{
    public static class HttpHelperZendeskExtensions
    {
        public static HtmlString SetZendeskSuggestion(this IHtmlHelper html, string suggestion)
        {
            return new HtmlString($"zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{EscapeApostrophes(suggestion)}' }});");
        }
        
        public static IHtmlContent SetZenDeskLabels(this IHtmlHelper html, params string[] labels)
        {
            var keywords = string.Join(",", labels
                .Where(label => !string.IsNullOrEmpty(label))
                .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                                + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                                + "] });";

            return new HtmlString(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }
    }
}