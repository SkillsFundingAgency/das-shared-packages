namespace SFA.DAS.Encoding.Mvc
{
    public static class ExtensionMethods
    {
        public static AutoDecodeMapping GetAutoDecodeMapping(this IAutoDecodeMappingProvider provider, string targetProperty)
        {
            var mappings = provider.AutoDecodeMappings;
            return mappings.ContainsKey(targetProperty) ? mappings[targetProperty] : null;
        }
    }
}
