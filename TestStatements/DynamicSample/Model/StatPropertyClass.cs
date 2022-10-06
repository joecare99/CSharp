using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSample.Model
{
    /// <summary>
    /// Class StatPropertyClass.
    /// </summary>
    public class StatPropertyClass
    {
        /// <summary>
        /// Gets or sets the name of the string.
        /// </summary>
        /// <value>The name of the string.</value>
        public string strName { get; set; }
        /// <summary>
        /// Gets or sets the string street.
        /// </summary>
        /// <value>The string street.</value>
        public string strStreet { get; set; }
        /// <summary>
        /// Gets or sets the string PLZ.
        /// </summary>
        /// <value>The string PLZ.</value>
        public string strPLZ { get; set; }
        /// <summary>
        /// Gets or sets the string city.
        /// </summary>
        /// <value>The string city.</value>
        public string strCity { get; set; }
        /// <summary>
        /// Gets or sets the string country.
        /// </summary>
        /// <value>The string country.</value>
        public string strCountry { get; set; }
        /// <summary>
        /// Gets the full address.
        /// </summary>
        /// <value>The full address.</value>
        public string FullAddress => $"{strName}\r\n{strStreet}\r\n{strPLZ} {strCity}\r\n{strCountry}";
    }
}
