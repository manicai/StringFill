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
    using System.Reflection;
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

        public static StringBuilder AppendFill(this StringBuilder @this, 
                                               IFormatProvider provider, 
                                               string format, 
                                               object parameters)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("@this");
            }
            if (format == null || parameters == null)
            {
                throw new ArgumentNullException(format == null ? "format" : "parameters");
            }
            
            var formatSpecification = ConvertToFormatSpecification(format, parameters);
            @this.AppendFormat(provider, formatSpecification.Format, formatSpecification.Args);

            return @this;
        }

        private static FormatSpecification ConvertToFormatSpecification(string format, object parameters)
        {
            var names = new List<string>();

            int index = 0;
            int length = format.Length;
            var resultFormat = new StringBuilder(format.Length);

            // Scan for { and }
            while (index < length)
            {
                while (index < length)
                {
                    var ch = format[index++];
                    if (ch == '{')
                    {
                        if (index == length)
                        {
                            throw new FormatException("Invalid format string");
                        }
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

                Debug.Assert(format[index] == '{');
                resultFormat.Append('{').Append(names.Count);
                index++;

                var name = new StringBuilder();
                while (index < length)
                {
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
                           Args = GetValuesForNames(parameters, names)
                       };
        }

        private static object[] GetValuesForNames(object parameters, IEnumerable<string> names)
        {
            return names.Select(n => ValueForName(parameters, n)).ToArray();     
        }

        private static object ValueForName(object parameters, string name)
        {
            Type type = parameters.GetType();
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return property.GetValue(parameters, new object[0]);
            }

            var field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(parameters);
            }

            throw new FormatException("Named parameter not found: " + name);
        }
    }
}
