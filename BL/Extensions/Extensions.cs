namespace BL.Extensions
{
    internal static class Extensions
    {
        public static string ReplacePointToComma(this string str) 
            => str.Trim().Replace(" ", "").Replace(".", ",");
    }
}
