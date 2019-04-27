namespace DaHo.SephirWatcher.Extensions
{
    public static class NumberExtensions
    {
        public static double? ParseOrNull(this string s)
        {
            if (double.TryParse(s, out var result))
            {
                return result;
            }

            return null;

        }
    }
}
