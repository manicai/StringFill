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
using System.Collections.Generic;

namespace UnitTests
{
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringFill;

    /// <summary>
    /// Summary description for StringBuilder AppendFill extension method.
    /// </summary>
    [TestClass]
    public class StringBuilderFillTests
    {
        private StringBuilder sb;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void MyTestInitialize()
        {
            sb = new StringBuilder();
        }
        
        [TestMethod]
        public void StringBuilder_AppendFill_ShouldNotAlterStringWithoutFormatItems()
        {
            const string Text = "This is a test";
            sb.AppendFill(Text, new {});
            Assert.AreEqual(Text, sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldReturnSameInstance()
        {
            var result = sb.AppendFill("This is a test", new {});
            Assert.AreEqual(sb, result);
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldCollapseDoubleBraces()
        {
            const string Text = "This is a {{test}}";
            sb.AppendFill(Text, new {});
            Assert.AreEqual("This is a {test}", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldInterpolateByName()
        {
            sb.AppendFill("Hello {name}", new {name = "World"});
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldMultipleValuesInterpolateByName()
        {
            sb.AppendFill("Hello {name}. How is {name} today?", new { name = "World" });
            Assert.AreEqual("Hello World. How is World today?", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldMultipleDifferentValuesInterpolateByName()
        {
            sb.AppendFill("Hello {name}. What {question}?",
                          new {name = "World", question = "time is it", ignored = "Boo!"});
            Assert.AreEqual("Hello World. What time is it?", sb.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringBuilder_AppendFill_ShouldThrowIfEmptyName()
        {
            sb.AppendFill("Hello {}", new {});
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringBuilder_AppendFill_ShouldThrowIfMissingName()
        {
            sb.AppendFill("{ham}", new { eggs = 1 });
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldFillCorrectlyUsingCulture()
        {
            var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");

            var parameters = new { arg1 = 1.30, arg2 = DateTime.FromOADate(1000) };
            sb.AppendFill(turkishCulture, "exec SomeProc({arg1}, {arg2});",
                          parameters);

            string expected = String.Format(turkishCulture, "exec SomeProc({0}, {1});",
                                            parameters.arg1, parameters.arg2);
            Assert.AreEqual(expected, sb.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringBuilder_AppendFill_ShouldThrowOnInvalidFormatString()
        {
            sb.AppendFill("{", new {});
        }

        private class TestParameters
        {
            public string S { get; set; }
            public int I { get; set; }
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldFillUsingProperties()
        {
            var parameters = new TestParameters {S = "eggs", I = 201};
            sb.AppendFill("{S} = {I}", parameters);
            Assert.AreEqual("eggs = 201", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldBeAbleToUseDictionary()
        {
            var parameters = new Dictionary<string, object>();
            parameters["key"] = "value";

            sb.AppendFill("{key}", parameters);
            Assert.AreEqual("value", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldUseDictionaryMembersOverReflection()
        {
            var parameters = new Dictionary<string, object>();
            parameters["Count"] = "value";

            sb.AppendFill("{Count}", parameters);
            Assert.AreEqual("value", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldUserReflectionOverDictionaryMembersForObject()
        {
            var parameters = new Dictionary<string, object>();
            parameters["Count"] = "value";

            sb.AppendFill("{Count}", (object)parameters);
            Assert.AreEqual("1", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldWorkForAnyDictionaryValueType()
        {
            var parameters = new Dictionary<string, int>();
            parameters["Count"] = -300;

            sb.AppendFill("{Count}", parameters);
            Assert.AreEqual("-300", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldWorkForNonGenericDictionary()
        {
            var parameters = new System.Collections.Hashtable();
            parameters["Count"] = -300;

            sb.AppendFill("{Count}", parameters);
            Assert.AreEqual("-300", sb.ToString());
        }
    }
}
