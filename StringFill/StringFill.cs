using System;
using System.Text;

namespace StringFill
{
    public static class StringFill
    {
        public static String Fill(String format, object parameters)
        {
            var sb = new StringBuilder();
            sb.AppendFill(format, parameters);
            return sb.ToString();
        }

        public static String Fill(IFormatProvider formatProvider, String format, object parameters)
        {
            var sb = new StringBuilder();
            sb.AppendFill(formatProvider, format, parameters);
            return sb.ToString();
        }
    }
}
