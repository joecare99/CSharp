// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="LicenseData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Model for license data with validation logic</summary>
// ***********************************************************************

namespace Gen_FreeWin.Models
{
    /// <summary>
    /// Represents license data with validation logic.
    /// </summary>
    public class LicenseData
    {
        /// <summary>
        /// Gets or sets the product identifier (part 0).
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturer identifier (part 1).
        /// </summary>
        public string ManufacturerId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the license key (part 2).
        /// </summary>
        public string LicenseKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets the full serialized license number as stored format: "LicText1-GB-LicText2-LicText3".
        /// </summary>
        public string SerialNumber
        {
            get
            {
                if (string.IsNullOrEmpty(ProductId) || string.IsNullOrEmpty(ManufacturerId) || string.IsNullOrEmpty(LicenseKey))
                {
                    return string.Empty;
                }
                return $"{ProductId}-GB-{ManufacturerId}-{LicenseKey}";
            }
        }

        /// <summary>
        /// Validates the license number format and calculates the checksum.
        /// </summary>
        /// <returns>True if the license is valid; otherwise false.</returns>
        public bool IsValid()
        {
            // Validate that product identifier starts with 'Q'
            if (string.IsNullOrEmpty(ProductId) || ProductId.Length < 3 || ProductId[2].ToString().ToUpper() != "Q")
            {
                return false;
            }

            // Validate lengths
            if (ProductId.Length != 10 || ManufacturerId.Length != 10 || LicenseKey.Length < 5)
            {
                return false;
            }

            // Validate checksum
            return ValidateChecksum();
        }

        /// <summary>
        /// Validates the checksum of the license key.
        /// Calculates sum of first 10 digits of LicText2 plus first digit of LicText3,
        /// divides by last digit of LicText3, and compares with digits 3-4 of LicText3.
        /// </summary>
        /// <returns>True if checksum is valid; otherwise false.</returns>
        private bool ValidateChecksum()
        {
            try
            {
                // Sum of first 10 characters of LicText2
                int sum = 0;
                for (int i = 0; i < 10 && i < ManufacturerId.Length; i++)
                {
                    if (char.IsDigit(ManufacturerId[i]))
                    {
                        sum += int.Parse(ManufacturerId[i].ToString());
                    }
                }

                // Add first digit of LicText3
                if (LicenseKey.Length > 0 && char.IsDigit(LicenseKey[0]))
                {
                    sum += int.Parse(LicenseKey[0].ToString());
                }

                // Get divisor from last digit of LicText3
                if (LicenseKey.Length < 5)
                {
                    return false;
                }

                if (!char.IsDigit(LicenseKey[4]))
                {
                    return false;
                }

                int divisor = int.Parse(LicenseKey[4].ToString());
                if (divisor == 0)
                {
                    return false;
                }

                // Calculate checksum
                int calculated = (sum - 1) / divisor;

                // Extract expected checksum from digits 3-4 of LicText3
                if (!int.TryParse(LicenseKey.Substring(2, 2), out int expected))
                {
                    return false;
                }

                return calculated == expected;
            }
            catch
            {
                return false;
            }
        }
    }
}
