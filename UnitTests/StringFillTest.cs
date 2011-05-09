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