using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Snake_Base.Model
{
    /// <summary>
    /// Interface IPlacedObject
    /// </summary>
    public interface IPlacedObject
    {
        /// <summary>
        /// Occurs when [on place change].
        /// </summary>
        event EventHandler<(Point oP, Point nP)> OnPlaceChange;
        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>The place.</value>
        Point Place { get => GetPlace(); set => SetPlace(value); }

        /// <summary>
        /// Sets the place.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Name">The name.</param>
        void SetPlace(Point value, [CallerMemberName] string Name="");
        /// <summary>
        /// Gets the place.
        /// </summary>
        /// <returns>Point.</returns>
        Point GetPlace();
        /// <summary>
        /// Gets the old place.
        /// </summary>
        /// <returns>Point.</returns>
        Point GetOldPlace();
    }
}