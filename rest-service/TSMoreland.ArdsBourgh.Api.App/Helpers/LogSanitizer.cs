namespace TSMoreland.ArdsBourgh.Api.App.Helpers
{
    public static class LogSanitizer
    {
        public static string Sanitize(string? source)
        {
            if (source is null)
            {
                return string.Empty;
            }

            return source.Replace('\r', '_').Replace('\n', '_');
        }
    }
}
