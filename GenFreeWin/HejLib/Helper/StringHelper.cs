namespace WinAhnenCls.Helper
{
    public static class StringHelper
    {
        public static string? RightStr(this string? s, int len)
        {
            if (string.IsNullOrEmpty(s) || s!.Length <= len)
                return s;
            else
                return s.Substring(s.Length - len);
        }

        public static string? LeftStr(this string? s, int len)
        {
            if (string.IsNullOrEmpty(s) || s!.Length <= len)
                return s;
            else
                return s.Substring(0, len);
        }
    }
}
