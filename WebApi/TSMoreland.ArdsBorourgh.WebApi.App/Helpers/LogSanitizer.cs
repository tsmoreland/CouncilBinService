namespace TSMoreland.ArdsBorough.Api.App.Helpers
{
    /// <summary>
    /// Log Sanitizing, an attempt at either preventing log forging or silencing snyk warnings about it
    /// </summary>
    public static class LogSanitizer
    {
        /// <summary>
        /// replaces newline characters with '_'
        /// </summary>
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
