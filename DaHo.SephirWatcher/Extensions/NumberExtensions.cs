namespace DaHo.SephirWatcher.Extensions
{
    public static class NumberExtensions
    {
        public static double? ParseOrDefault(this string s, double? defaultValue = default)
        {
            return double.TryParse(s, out var result) ? 
                result : 
                defaultValue;
        }
    }
}
