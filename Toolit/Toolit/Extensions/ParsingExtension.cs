namespace Toolit.Extensions
{
    public static class ParsingExtension
    {
        public static string ParseUrlForUri(this string url)
        {
            if (string.IsNullOrWhiteSpace(url) || 
                url.StartsWith("http://") || 
                url.StartsWith("https://"))
            {
                return url;
            }

            return string.Concat("http://", url);
        }
    }
}