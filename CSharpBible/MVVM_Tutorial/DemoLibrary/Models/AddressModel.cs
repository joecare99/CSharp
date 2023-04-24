// ***********************************************************************
// Assembly         : DemoLibrary
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 06-17-2022
// ***********************************************************************
// <copyright file="AddressModel.cs" company="DemoLibrary">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace DemoLibrary.Models
{
    /// <summary>
    /// Class AddressModel.
    /// </summary>
    public class AddressModel {
		/// <summary>
		/// Gets or sets the address identifier.
		/// </summary>
		/// <value>The address identifier.</value>
		public int AddressId { get; set; }
		/// <summary>
		/// Gets or sets the additional line.
		/// </summary>
		/// <value>The additional line.</value>
		public string AdditionalLine { get; set; } = "";
		/// <summary>
		/// Gets or sets the street address.
		/// </summary>
		/// <value>The street address.</value>
		public string StreetAddress { get; set; } = "";
		/// <summary>
		/// Gets or sets the city.
		/// </summary>
		/// <value>The city.</value>
		public string City { get; set; } = "";
		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>The state.</value>
		public string State { get; set; } = "";
		/// <summary>
		/// Gets or sets the zip code.
		/// </summary>
		/// <value>The zip code.</value>
		public string ZipCode { get; set; } = "";
		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		/// <value>The country.</value>
		public string Country { get; set; } = "";

		/// <summary>
		/// Gets the full address.
		/// </summary>
		/// <value>The full address.</value>
		public string FullAddress => $"{StreetAddress}, {City}, {State} {ZipCode}, {Country}".Trim(new char[]{ ' ', ',' });

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString() => FullAddress;
	}
}
