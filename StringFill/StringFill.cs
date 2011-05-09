
using System;
using System.Text;

namespace StringFill
{
    public static class StringFill
    {
        public static String Fill(String format, object parameters)
        {
            return Fill(null, format, parameters);
        }

        public static String Fill(IFormatProvider formatProvider, String format, object parameters)
        {
            var sb = new StringBuilder();
            sb.AppendFill(formatProvider, format, parameters);
            return sb.ToString();
        }
    }
}
