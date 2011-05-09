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
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldNotAlterStringWithoutFormatItems()
        {
            var sb = new StringBuilder();
            const string Text = "This is a test";
            sb.AppendFill(Text, new {});
            Assert.AreEqual(Text, sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldReturnSameInstance()
        {
            var sb = new StringBuilder();
            var result = sb.AppendFill("This is a test", new {});
            Assert.AreEqual(sb, result);
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldCollapseDoubleBraces()
        {
            var sb = new StringBuilder();
            const string Text = "This is a {{test}}";
            sb.AppendFill(Text, new {});
            Assert.AreEqual("This is a {test}", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldInterpolateByName()
        {
            var sb = new StringBuilder();

            sb.AppendFill("Hello {name}", new {name = "World"});
            Assert.AreEqual("Hello World", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldMultipleValuesInterpolateByName()
        {
            var sb = new StringBuilder();

            sb.AppendFill("Hello {name}. How is {name} today?", new { name = "World" });
            Assert.AreEqual("Hello World. How is World today?", sb.ToString());
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldMultipleDifferentValuesInterpolateByName()
        {
            var sb = new StringBuilder();

            sb.AppendFill("Hello {name}. What {question}?",
                          new {name = "World", question = "time is it", ignored = "Boo!"});
            Assert.AreEqual("Hello World. What time is it?", sb.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringBuilder_AppendFill_ShouldThrowIfEmptyName()
        {
            var sb = new StringBuilder();
            sb.AppendFill("Hello {}", new {});
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StringBuilder_AppendFill_ShouldThrowIfMissingName()
        {
            var sb = new StringBuilder();
            sb.AppendFill("{ham}", new { eggs = 1 });
        }

        [TestMethod]
        public void StringBuilder_AppendFill_ShouldFillCorrectlyUsingCulture()
        {
            var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");

            var parameters = new { arg1 = 1.30, arg2 = DateTime.FromOADate(1000) };
            var sb = new StringBuilder();
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
            var sb = new StringBuilder();
            sb.AppendFill("{", new {});
        }
    }
}
