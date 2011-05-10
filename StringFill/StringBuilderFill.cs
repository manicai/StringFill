// Copyright (c) 2011 Ian Glover (ian@manicai.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
namespace StringFill
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public static class StringBuilderFill
    {
        private struct FormatSpecification
        {
            public String Format;
            public object[] Args;
        }

        public static StringBuilder AppendFill(this StringBuilder @this,
                                               string format,
                                               object parameters)
        {
            return AppendFill(@this, null, format, parameters);
        }

        public static StringBuilder AppendFill<T>(this StringBuilder @this,
                                                  string format,
                                                  IDictionary<string, T> parameters)
        {
            return AppendFill(@this, null, format, parameters);
        }

        public static StringBuilder AppendFill<T>(this StringBuilder @this,
                                                  IFormatProvider provider,
                                                  string format,
                                                  IDictionary<string, T> parameters)
        {
            CheckParameters(@this, format, parameters);

            Func<string, object> parameterLookup = name => parameters[name];

            return Fill(@this, format, provider, parameterLookup);
        }

        public static StringBuilder AppendFill(this StringBuilder @this, 
                                               IFormatProvider provider, 
                                               string format, 
                                               object parameters)
        {
            CheckParameters(@this, format, parameters);

            Func<string, object> parameterLookup = name => ValueForName(parameters, name);

            return Fill(@this, format, provider, parameterLookup);
        }

        private static StringBuilder Fill(StringBuilder stringBuilder, string format, 
                                          IFormatProvider provider,
                                          Func<string, object> parameterLookup)
        {
            // We don't try and fully understand format specifiers instead we
            // map the names to a indices and create a list so those indices
            // map to the right objects. This simplifies the parsing needed.
            //
            // The disadvantage of doing this as a translation is that we leave
            // some of the error handling to AppendFormat which could produce
            // confusing error messages in the case of invalid format strings.
            var formatSpecification = ConvertToFormatSpecification(format, parameterLookup);
            stringBuilder.AppendFormat(provider, formatSpecification.Format, formatSpecification.Args);

            return stringBuilder;
        }

        private static void CheckParameters(StringBuilder @this, string format, object parameters)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("@this");
            }
            if (format == null || parameters == null)
            {
                throw new ArgumentNullException(format == null ? "format" : "parameters");
            }
        }

        /// <summary>
        /// Convert the name based format string to index based on and return
        /// the result and an array of the appropriate items in the correct
        /// order.
        /// </summary>
        /// <param name="format">Name based format string</param>
        /// <param name="parameters">Parameter object instance.</param>
        private static FormatSpecification ConvertToFormatSpecification(string format, 
                                                                        Func<string, object> parameterLookup)
        {
            var names = new List<string>();

            int index = 0;
            int length = format.Length;
            var resultFormat = new StringBuilder(format.Length);

            // Scan for format specifiers.
            while (index < length)
            {
                // Until the start of the next format specifier or the end of
                // the string.
                while (index < length)
                {
                    var ch = format[index++];
                    if (ch == '{')
                    {
                        if (index == length)
                        {
                            throw new FormatException("Invalid format string");
                        }
                        // We need to leave quoted braces ("{{") for 
                        // AppendFormat to handle.
                        if (format[index] == '{')
                        {
                            index++;
                            resultFormat.Append("{{");
                        }
                        else
                        {
                            index--;
                            break;
                        }
                    }
                    else
                    {
                        resultFormat.Append(ch);
                    }
                }

                if (index == length)
                {
                    break;
                }

                // If not the end of the string it's a format specifier so 
                // we need to map the name to a new index.
                Debug.Assert(format[index] == '{');
                resultFormat.Append('{').Append(names.Count);
                index++;

                var name = new StringBuilder();
                while (index < length)
                {
                    // We could be more aggressive in ensuring that the 
                    // name matches as valid .NET property or field name.
                    // Instead just stop when we see a valid character for 
                    // the next part of the format specifier.
                    var ch = format[index++];
                    if (ch == ',' || ch == ':' || ch == '}')
                    {
                        index--;
                        break;
                    }
                    else
                    {
                        name.Append(ch);
                    }
                }

                if (name.Length == 0)
                {
                    throw new FormatException("Cannot have an empty name in format string.");
                }

                names.Add(name.ToString());
            }

            return new FormatSpecification
                       {
                           Format = resultFormat.ToString(),
                           Args = names.Select(n => parameterLookup(n)).ToArray()
                       };
        }

        private static object ValueForName(object parameters, string name)
        {
            Type type = parameters.GetType();
            var property = type.GetProperty(name);
            if (property != null)
            {
                return property.GetValue(parameters, new object[0]);
            }

            var field = type.GetField(name);
            if (field != null)
            {
                return field.GetValue(parameters);
            }

            throw new FormatException("Named parameter not found: " + name);
        }
    }
}
