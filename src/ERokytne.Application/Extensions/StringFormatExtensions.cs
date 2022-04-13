namespace ERokytne.Application.Extensions
{
    public static class StringFormatExtensions
    {
        public static string SafeFormat(this string format, params object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }
    }
}