using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LicenseplateValidator.Tests
{
    [TestClass]
    public class LicenseplateValidatorTests
    {
        [TestMethod]
        public void FormatPlate_Returns_CorrectResults()
        {
            var target = new LicenseplateValidator();

            Assert.AreEqual("AB-12-34", target.FormatPlate("AB1234", "NL"));
            Assert.AreEqual("12-34-AB", target.FormatPlate("1234AB", "NL"));
            Assert.AreEqual("12-AB-34", target.FormatPlate("12AB34", "NL"));
            Assert.AreEqual("AB-12-CD", target.FormatPlate("AB12CD", "NL"));
            Assert.AreEqual("AB-CD-12", target.FormatPlate("ABCD12", "NL"));
            Assert.AreEqual("12-AB-CD", target.FormatPlate("12ABCD", "NL"));
            Assert.AreEqual("12-ABC-3", target.FormatPlate("12ABC3", "NL"));
            Assert.AreEqual("1-ABC-23", target.FormatPlate("1ABC23", "NL"));
            Assert.AreEqual("AB-123-C", target.FormatPlate("AB123C", "NL"));
            Assert.AreEqual("A-123-BC", target.FormatPlate("A123BC", "NL"));
            Assert.AreEqual("ABC-12-D", target.FormatPlate("ABC12D", "NL"));

            Assert.AreEqual("AB-12-34", target.FormatPlate("AB 12 34", "NL"));
            Assert.AreEqual("12-34-AB", target.FormatPlate("1-2-3-4 AB", "NL"));

            Assert.AreEqual("AB-12-34", target.FormatPlate("AB-12-34", "NL", false));
            Assert.AreEqual("12-34-AB", target.FormatPlate("12-34-AB", "NL", false));
        }

        [TestMethod]
        public void IsValidPlate_Returns_CorrectResults()
        {
            var target = new LicenseplateValidator();

            Assert.IsTrue(target.IsValidPlate("AB-12-34", "NL"));
            Assert.IsTrue(target.IsValidPlate("12-34-AB", "NL"));
            Assert.IsTrue(target.IsValidPlate("12-AB-34", "NL"));
            Assert.IsTrue(target.IsValidPlate("AB-12-CD", "NL"));
            Assert.IsTrue(target.IsValidPlate("AB-CD-12", "NL"));
            Assert.IsTrue(target.IsValidPlate("12-AB-CD", "NL"));
            Assert.IsTrue(target.IsValidPlate("12-ABC-3", "NL"));
            Assert.IsTrue(target.IsValidPlate("1-ABC-23", "NL"));
            Assert.IsTrue(target.IsValidPlate("AB-123-C", "NL"));
            Assert.IsTrue(target.IsValidPlate("A-123-BC", "NL"));
            Assert.IsTrue(target.IsValidPlate("ABC-12-D", "NL"));

            Assert.IsTrue(target.IsValidPlate("AB - 12 - 34", "NL"));
            Assert.IsTrue(target.IsValidPlate(" AB - 12 - 34 ", "NL"));

            Assert.IsFalse(target.IsValidPlate("A1-B2-C3", "NL"));
            Assert.IsFalse(target.IsValidPlate("1A-2B-3C", "NL"));

            Assert.IsFalse(target.IsValidPlate("AB1234", "NL"));
            Assert.IsFalse(target.IsValidPlate("ab1234", "NL"));
            Assert.IsFalse(target.IsValidPlate("AB-12- -34", "NL"));
        }

        [TestMethod]
        public void IsValidPlate_Returns_CorrectResultsForCustomCountries()
        {
            var target = new LicenseplateValidator(new Dictionary<string, string[]>
            {
                { "XX", new[] { "XX-99-XX", "X9-9X-X9" } }
            });
            Assert.IsTrue(target.IsValidPlate("A1-2B-C3", "XX"));
            Assert.IsTrue(target.IsValidPlate("A12BC3", "XX", true));
            Assert.IsTrue(target.IsValidPlate("A1 - 2B - C3", "XX"));
            Assert.IsTrue(target.IsValidPlate("A1 2B C3", "XX", true));

            Assert.IsFalse(target.IsValidPlate("12-AB-34", "XX"));
            Assert.IsFalse(target.IsValidPlate("12AB34", "XX", true));
            Assert.IsFalse(target.IsValidPlate("12 - AB - 34", "XX"));
            Assert.IsFalse(target.IsValidPlate("12 AB 34", "XX", true));

        }

        [TestMethod]
        public void LicenseplateValidator_SupportsMultipleCountries()
        {
            var target = new LicenseplateValidator(new Dictionary<string, string[]>
            {
                { "XX", new[] { "XX-99-XX", "X9-9X-X9" } },
                { "YY", new[] { "99-XX-99", "XXX-999" } },
                { "ZZ", new[] { "X?-X?-X?" } },
            });
            Assert.IsTrue(target.IsValidPlate("AB-12-CD", "XX"));
            Assert.IsTrue(target.IsValidPlate("A1-2B-C3", "XX"));
            Assert.IsTrue(target.IsValidPlate("12-AB-34", "YY"));
            Assert.IsTrue(target.IsValidPlate("ABC-123", "YY"));

            Assert.IsTrue(target.IsValidPlate("A1-B2-C3", "ZZ"));
            Assert.IsTrue(target.IsValidPlate("AB-CD-E1", "ZZ"));

            Assert.IsFalse(target.IsValidPlate("AB-12-CD", "YY"));
            Assert.IsFalse(target.IsValidPlate("A1-2B-C3", "YY"));
            Assert.IsFalse(target.IsValidPlate("12-AB-34", "XX"));
            Assert.IsFalse(target.IsValidPlate("ABC-123", "XX"));

            Assert.IsFalse(target.IsValidPlate("A1B-2C3", "ZZ"));
            Assert.IsFalse(target.IsValidPlate("12-BC-D3", "ZZ"));

        }


        [TestMethod]
        public void TryormatPlate_ReturnsFalse_OnUnknownFormat()
        {
            var target = new LicenseplateValidator();
            Assert.IsFalse(target.TryFormatPlate("A1B2C3", "NL", out var _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryormatPlate_Throws_OnNullPlate()
        {
            var target = new LicenseplateValidator();
            target.TryFormatPlate(null, "NL", out var _);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryormatPlate_Throws_OnEmptyPlate()
        {
            var target = new LicenseplateValidator();
            target.TryFormatPlate("   ", "NL", out var _);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TryormatPlate_Throws_OnUnsupportedCountry()
        {
            var target = new LicenseplateValidator();
            target.TryFormatPlate("AB-12-CD", "XX", out var _);
        }

        [TestMethod]
        public void FindSideCode_Ignores_IncorrectDashes()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode("1234AB", "NL", true);
            target.FindSideCode("1-23-4AB", "NL", true);
        }


        [TestMethod]
        public void FindSideCode_Returns_MatchingSideCode()
        {
            var target = new LicenseplateValidator();
            Assert.AreEqual("XX-99-XX", target.FindSideCode("AB-12-CD", "NL"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_OnNullDictionary()
        {
            var target = new LicenseplateValidator(null);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FormatPlate_Throws_OnUnsupportedCountry()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate("1234AB", "XX");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FormatPlate_Throws_OnIncorrectDashes()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate("1234AB", "NL", false);
        }


        [TestMethod]
        public void FormatPlate_Ignores_IncorrectDashes()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate("1234AB", "NL", true);
            target.FormatPlate("1-23-4AB", "NL", true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatPlate_Throws_OnNullPlate()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate(null, "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FormatPlate_Throws_OnEmptyPlate()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate("   ", "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FormatPlate_Throws_OnUnknownSideCode()
        {
            var target = new LicenseplateValidator();
            target.FormatPlate("1A2B3C", "NL");
        }


        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindSideCode_Throws_OnUnknownSideCode()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode("A1-B2-C3", "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindSideCode_Throws_OnUnsupportedCountry()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode("AB-12-CD", "XX");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindSideCode_Throws_OnNullPlate()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode(null, "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindSideCode_Throws_OnEmptyPlate()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode("   ", "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindSideCode_Throws_OnIncorrectDashes()
        {
            var target = new LicenseplateValidator();
            target.FindSideCode("1234AB", "NL", false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidPlate_Throws_OnNullPlate()
        {
            var target = new LicenseplateValidator();
            target.IsValidPlate(null, "NL");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsValidPlate_Throws_OnEmptyPlate()
        {
            var target = new LicenseplateValidator();
            target.IsValidPlate("   ", "NL");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void IsValidPlate_Throws_OnUnsupportedCountry()
        {
            var target = new LicenseplateValidator();
            target.IsValidPlate("AB-12-CD", "XX");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsValidPlate_Throws_OnUnsupportedSidecodeCharacter()
        {
            var target = new LicenseplateValidator(new Dictionary<string, string[]>
            {
                { "XX", new[] { "XX-99-X*" } }
            });
            target.IsValidPlate("AB-12-CD", "XX");
        }
    }
}
