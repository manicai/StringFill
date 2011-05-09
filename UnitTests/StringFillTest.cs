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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StringFill;

    /// <summary>
    /// This is a test class for StringFillTest and is intended
    /// to contain all StringFillTest Unit Tests
    /// </summary>
    [TestClass]
    public class StringFillTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void StringFill_Fill_ShouldFillCorrectly()
        {
            string actual = StringFill.Fill("exec SomeProc({arg1}, {arg2});",
                                            new { arg1 = "Test", arg2 = "Example" });
            Assert.AreEqual("exec SomeProc(Test, Example);", actual);
        }

        [TestMethod]
        public void StringFill_Fill_ShouldFillCorrectlyUsingCulture()
        {
            var turkishCulture = CultureInfo.GetCultureInfo("tr-TR");

            var parameters = new {arg1 = 1.30, arg2 = DateTime.FromOADate(1000)};
            string actual = StringFill.Fill(turkishCulture, "exec SomeProc({arg1}, {arg2});",
                                            parameters);

            string expected = String.Format(turkishCulture, "exec SomeProc({0}, {1});",
                                            parameters.arg1, parameters.arg2);
            Assert.AreEqual(expected, actual);
        }
    }
}