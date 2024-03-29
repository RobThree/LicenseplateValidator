﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseplateValidator
{
    /// <summary>
    /// Contains methods to validate and format license plates.
    /// </summary>
    public class LicenseplateValidator
    {
        // Supported countries and their sidecodes
        private readonly IDictionary<string, string[]> _supportedsidecodes;

        /// <summary>
        /// Initializes a <see cref="LicenseplateValidator"/> with support for only Dutch sidecodes.
        /// </summary>
        public LicenseplateValidator()
            : this(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase) {
                { "NL", new[] { "XX-99-99", "99-99-XX", "99-XX-99", "XX-99-XX", "XX-XX-99", "99-XX-XX", "99-XXX-9", "9-XXX-99", "XX-999-X", "X-999-XX", "X-999-XX", "XXX-99-X", "XXX-99-X", "X-99-XXX", "9-XX-999", "999-XX-9" }
            } })
        { }

        /// <summary>
        /// Initializes a <see cref="LicenseplateValidator"/> with support for the specified countries and their sidecodes.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when supportedSideCodes is <see langword="null"/>.</exception>
        public LicenseplateValidator(IDictionary<string, string[]> supportedSideCodes)
            => _supportedsidecodes = supportedSideCodes ?? throw new ArgumentNullException(nameof(supportedSideCodes));

        /// <summary>
        /// Formate a plate for a given country.
        /// </summary>
        /// <param name="plate">The plate to format.</param>
        /// <param name="countryContext">The country specifying which country to format the plate for.</param>
        /// <param name="ignoreDashes">When <see langword="true"/>, dashes in the plate will be ignored. When <see langword="false"/> dashes should be in the correct place in the plate.</param>
        /// <returns>The formatted plate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country or no sidecode for the given <paramref name="plate"/> could be found.</exception>
        public string FormatPlate(string plate, string countryContext, bool ignoreDashes = true)
        {
            if (TryFormatPlate(plate, countryContext, ignoreDashes, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException("No supported sidecode found for given plate and country");
        }

        /// <summary>
        /// Tries to format a plate for a given country context.
        /// </summary>
        /// <param name="plate">The licenseplate to format.</param>
        /// <param name="countryContext">The country specifying which country to format the plate for.</param>
        /// <param name="result">The formatted plate.</param>
        /// <returns>
        /// Returns <see langword="true"/> when the plate could be formatted and <paramref name="result"/> will contain the formatted plate, <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country.</exception>
        public bool TryFormatPlate(string plate, string countryContext, out string result)
            => TryFormatPlate(plate, countryContext, true, out result);

        /// <summary>
        /// Tries to format a plate for a given country context.
        /// </summary>
        /// <param name="plate">The licenseplate to format.</param>
        /// <param name="countryContext">The country specifying which country to format the plate for.</param>
        /// <param name="ignoreDashes">When <see langword="true"/>, dashes in the plate will be ignored. When <see langword="false"/> dashes should be in the correct place in the plate.</param>
        /// <param name="result">The formatted plate.</param>
        /// <returns>
        /// Returns <see langword="true"/> when the plate could be formatted and <paramref name="result"/> will contain the formatted plate, <see langword="false"/> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country.</exception>
        public bool TryFormatPlate(string plate, string countryContext, bool ignoreDashes, out string result)
        {
            result = "";
            if (TryFindSideCode(plate, countryContext, ignoreDashes, out var sidecode))    // Does the plate match a sidecode?
            {
                plate = RemoveDashes(NormalizeString(plate), true);

                // Then copy all characters in the desired format (advance plate pointer for every char unless a dash)
                var i = 0;
                foreach (var c in sidecode)
                {
                    result += (c == '-') ? '-' : plate[i++];
                }

                return true;    // Done, success!
            }
            return false;   // No matching sidecode found!
        }

        /// <summary>
        /// Returns true when a plate is deemed valid.
        /// </summary>
        /// <param name="plate">The plate to check.</param>
        /// <param name="countryContext">The country specifying which country to validate the plate for.</param>
        /// <param name="ignoreDashes">When <see langword="true"/>, dashes in the plate will be ignored. When <see langword="false"/> dashes should be in the correct place in the plate.</param>
        /// <returns>Returns true when the plate is valid, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country.</exception>
        public bool IsValidPlate(string plate, string countryContext, bool ignoreDashes = false) => ignoreDashes
                ? TryFormatPlate(plate, countryContext, out var _)
                : TryFindSideCode(plate, countryContext, ignoreDashes, out var _);

        /// <summary>
        /// Finds the sidecode matching the given plate.
        /// </summary>
        /// <param name="plate">The plate to find a matching sidecode for.</param>
        /// <param name="countryContext">The country specifying which country to validate the plate for.</param>
        /// <param name="ignoreDashes">When <see langword="true"/>, dashes in the plate will be ignored. When <see langword="false"/> dashes should be in the correct place in the plate.</param>
        /// <returns>Returns the sidecode for the given plate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country or no sidecode for the given <paramref name="plate"/> could be found.</exception>
        public string FindSideCode(string plate, string countryContext, bool ignoreDashes = false)
        {
            if (TryFindSideCode(plate, countryContext, ignoreDashes, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException("Unable to find sidecode for given plate."); //TODO: should be a custom exception (i.e. "SideCodeNotFoundException")
        }

        /// <summary>
        /// Tries to find the sidecode matching the given plate.
        /// </summary>
        /// <param name="plate">The plate to find a matching sidecode for.</param>
        /// <param name="countryContext">The country specifying which country to validate the plate for.</param>
        /// <param name="sideCode">The sidecode matched with the plate (if any); undefined otherwise.</param>
        /// <returns>Returns true when the plate matched a known sidecode, false otherwise. The out argument is the sidecode that matched the plate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country.</exception>
        public bool TryFindSideCode(string plate, string countryContext, out string sideCode)
            => TryFindSideCode(plate, countryContext, false, out sideCode);

        /// <summary>
        /// Tries to find the sidecode matching the given plate.
        /// </summary>
        /// <param name="plate">The plate to find a matching sidecode for.</param>
        /// <param name="countryContext">The country specifying which country to validate the plate for.</param>
        /// <param name="ignoreDashes">When <see langword="true"/>, dashes in the plate will be ignored. When <see langword="false"/> dashes should be in the correct place in the plate.</param>
        /// <param name="sideCode">The sidecode matched with the plate (if any); undefined otherwise.</param>
        /// <returns>Returns true when the plate matched a known sidecode, false otherwise. The out argument is the sidecode that matched the plate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when a <paramref name="plate"/> is <see langword="null"/> or empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when <paramref name="countryContext"/> contains an unsupported country.</exception>
        public bool TryFindSideCode(string plate, string countryContext, bool ignoreDashes, out string sideCode)
        {
            plate = RemoveDashes(NormalizeString(plate), ignoreDashes);
            if (string.IsNullOrEmpty(plate))
            {
                throw new ArgumentNullException(nameof(plate));
            }

            sideCode = null;
            foreach (var s in GetCountrySideCodes(countryContext))   // Try all sidecodes
            {
                if (MatchCode(plate, RemoveDashes(s, ignoreDashes)))    // Does the plate match the sidecode?
                {
                    sideCode = s;           // Let's return the sidecode
                    return true;            // Found it!
                }
            }
            return false;                   // No sidecode found!
        }

        /// <summary>
        /// Returns a country's sidecodes (if any).
        /// </summary>
        /// <param name="country">The country to retrieve the sidecodes for.</param>
        /// <returns>Returns an array of valid sidecodes for a country</returns>
        /// <exception cref="KeyNotFoundException">Thrown when an unsupported country was specified.</exception>
        private string[] GetCountrySideCodes(string country)
            => _supportedsidecodes.TryGetValue(country, out var v) ? v : throw new KeyNotFoundException($"Unsupported country {country}");

        /// <summary>
        /// Cleans a string from all whitespace and ensures all unicode characters have been normalized to FormC.
        /// </summary>
        /// <param name="value">The string to normalize.</param>
        /// <returns>Returns a normalized string or null when the <paramref name="value"/> was null or whitespace entirely.</returns>
        private static string NormalizeString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Normalize to FormC and remove all whitespace from the string
            return string.Join(string.Empty, value.Normalize(NormalizationForm.FormC)
                .Where(c => !char.IsWhiteSpace(c)))
                .ToUpperInvariant();
        }

        /// <summary>
        /// Compares if each character in a plate matches up with the given sidecode.
        /// </summary>
        /// <param name="plate">The licenseplate to match.</param>
        /// <param name="sideCode">The sidecode to match.</param>
        /// <returns></returns>
        /// <remarks>This method returns true when the <paramref name="plate"/> matches the <paramref name="sideCode"/>.</remarks>
        /// <exception cref="NotSupportedException">Thrown when sidecode definition contains invalid characters</exception>
        private static bool MatchCode(string plate, string sideCode)
        {
            // Difference in length? No match. Done.
            if (plate.Length != sideCode.Length)
            {
                return false;
            }

            // Assume plate matches; then check each position and update the macth variable accordingly.
            var match = true;
            var i = 0;
            do
            {
                match &= IsSideCodeMatch(sideCode[i], plate[i]);
            } while (++i < plate.Length && match);
            return match;  // Return result;
        }

        /// <summary>
        /// Checks if a given char from a plate matches a required digit/letter/dash from a sidecode.
        /// </summary>
        /// <param name="sidecodeChar">The sidecode char to match against.</param>
        /// <param name="plateChar">The plate char to match.</param>
        /// <returns>Returns true when the plate char matches, false otherwise.</returns>
        private static bool IsSideCodeMatch(char sidecodeChar, char plateChar)
            => sidecodeChar switch    // Get char from sidecode
            {
                // Char from plate should be a letter OR digit
                '?' => char.IsLetter(plateChar) || char.IsDigit(plateChar),
                // Char from plate should be a letter
                'X' => char.IsLetter(plateChar),
                // Char from plate should be a digit
                '9' => char.IsDigit(plateChar),
                // Char from plate should be a dash
                '-' => plateChar == '-',
                // Not supported
                _ => throw new NotSupportedException($"Invalid sidecode definition; cannot contain {sidecodeChar}"),
            };

        /// <summary>
        /// Removes dashes from a given string.
        /// </summary>
        /// <param name="value">The value to remove dashes from.</param>
        /// <returns>The string without any dashes.</returns>
        private static string RemoveDashes(string value, bool remove)
            => remove ? value?.Replace("-", string.Empty) : value;
    }
}