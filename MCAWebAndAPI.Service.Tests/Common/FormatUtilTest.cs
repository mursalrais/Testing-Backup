using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Tests.Common
{
    [TestClass]
    public class FormatUtilTest
    {
        [TestMethod]
        public void ConvertToDigitNumber_NumberWithoutZero_ShouldHave_ReturnedCorrectNumber()
        {
            var expectedResult = "0012";
            Assert.AreEqual(expectedResult, FormatUtil.ConvertToDigitNumber(12, 4));
        }

        [TestMethod]
        public void ConvertToDigitNumber_NumberWithZero_ShouldHave_ReturnedCorrectNumber()
        {
            var expectedResult = "00100";
            Assert.AreEqual(expectedResult, FormatUtil.ConvertToDigitNumber(100, 5));
        }

        [TestMethod]
        public void ConvertMultipleLine_StringWithHTMLTag_ShouldHave_Returned_CleanString()
        {
            var expectedResult = "Clean String";
            var input = string.Format("<div>{0}</div>", expectedResult);

            Assert.AreEqual(expectedResult, FormatUtil.ConvertMultipleLine(input));
        }

        [TestMethod]
        public void ConvertMultipleLine_StringWithoutHTMLTag_ShouldHave_Returned_CleanString()
        {
            var expectedResult = "Clean String";
            var input = expectedResult;

            Assert.AreEqual(expectedResult, FormatUtil.ConvertMultipleLine(input));
        }

        [TestMethod]
        public void ConvertToCleanSiteUrl_StringWithQuotes_ShouldHave_Returned_StringWithoutQuote()
        {
            var expectedResult = "http://google.com";
            var input = "\"http://google.com\"";

            Assert.AreEqual(expectedResult, FormatUtil.ConvertToCleanSiteUrl(input));
        }

        [TestMethod]
        public void ConvertToCleanSiteUrl_StringWithoutQuotes_ShouldHave_Returned_StringWithoutQuote()
        {
            var expectedResult = "http://google.com";
            var input = "http://google.com";

            Assert.AreEqual(expectedResult, FormatUtil.ConvertToCleanSiteUrl(input));
        }

        [TestMethod]
        public void ConvertToCleanPhoneNumber_StringWithUnderscore_ShouldHave_Returned_StringWithoutUnderscore()
        {
            var expectedResult = "08561298";
            var input = "085_61_298";

            Assert.AreEqual(expectedResult, FormatUtil.ConvertToCleanPhoneNumber(input));
        }

        [TestMethod]
        public void ConvertToCleanPhoneNumber_StringWithoutUnderscore_ShouldHave_Returned_StringWithoutUnderscore()
        {
            var expectedResult = "08561298";
            var input = "08561298";

            Assert.AreEqual(expectedResult, FormatUtil.ConvertToCleanPhoneNumber(input));
        }
    }
}
