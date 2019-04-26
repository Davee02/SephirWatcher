namespace DaHo.SephirWatcher.Extensions
{
    public static class NumberExtensions
    {
        public static double ParseOrNaN(this string s)
        {
            return double.TryParse(s, out var result) 
                ? result 
                : double.NaN;
        }
    }
}
