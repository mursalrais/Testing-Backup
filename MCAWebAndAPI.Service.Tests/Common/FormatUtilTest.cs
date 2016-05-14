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
    }
}
