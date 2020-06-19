namespace AzureFunctionsUpdates.Storage
{
    public static class KeyFormatter
    {   
        public static string SanitizeKey(string key)
        {
            var illegalChars = new[] {'/', '\\', '#', '?'};
            const char replacementChar = '-';

            foreach (var illegalChar in illegalChars)
            {
                key = key.Replace(illegalChar, replacementChar);
            }

            return key;
        }
    }
}