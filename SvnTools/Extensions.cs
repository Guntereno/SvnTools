using System;

namespace SvnTools
{
    static class Extensions
    {
        public static string[] Lines(this string source)
        {
            return source.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
    }
}
