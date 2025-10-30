// ***********************************************************************
// Assembly      : Avln_AnimationTiming
// Author : Mir
// Created          : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="ITemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

namespace Avln_AnimationTiming.Models
{
 /// <summary>
    /// Interface ITemplateModel
  /// Implements the <see cref="INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public interface ITemplateModel : INotifyPropertyChanged
    {
        /// <summary>
 /// Gets the now.
        /// </summary>
        /// <value>The now.</value>
     DateTime Now { get; }
    }
}
