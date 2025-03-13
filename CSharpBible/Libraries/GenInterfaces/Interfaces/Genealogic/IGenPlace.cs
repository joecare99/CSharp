namespace GenInterfaces.Interfaces.Genealogic
{
    public interface IGenPlace : IGenObject
    {
        /// <summary>Gets or sets the name of the place.</summary>
        /// <value>The name of the place.</value>
        string Name { get; set; }
        /// <summary>Gets or sets the type of the place.</summary>
        /// <value>The type of the place.</value>
        string Type { get; set; }
        /// <summary>Gets or sets the GOF-ID of the place.</summary>
        /// <value>The GOF-ID of the place.</value>
        string GOV_ID { get; set; }
        /// <summary>Gets or sets the latitude of the place.</summary>
        /// <value>The latitude of the place.</value>
        double Latitude { get; set; }
        /// <summary>Gets or sets the longitude of the place.</summary>
        /// <value>The longitude of the place.</value>
        double Longitude { get; set; }
        /// <summary>Gets or sets the altitude of the place.</summary>
        /// <value>The altitude of the place.</value>
        string Notes { get; set; }
        /// <summary>Gets or sets the parent-place.</summary>
        /// <value>The parent.</value>
        IGenPlace? Parent { get; set; }
    }
}