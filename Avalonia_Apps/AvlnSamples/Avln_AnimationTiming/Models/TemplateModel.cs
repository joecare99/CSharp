// ***********************************************************************
// Assembly       : Avln_AnimationTiming
// Author           : Mir
// Created          : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="TemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Timers;

namespace Avln_AnimationTiming.Models
{
    /// <summary>
    /// Class TemplateModel.
    /// Implements the <see cref="ObservableObject" />
    /// Implements the <see cref="Avln_AnimationTiming.Models.ITemplateModel" />
    /// </summary>
/// <seealso cref="ObservableObject" />
    /// <seealso cref="Avln_AnimationTiming.Models.ITemplateModel" />
    public partial class TemplateModel : ObservableObject, ITemplateModel
    {
        #region Properties
        /// <summary>
        /// The timer
        /// </summary>
        private readonly Timer _timer;
        
     /// <summary>
        /// Gets or sets the get now.
        /// </summary>
        /// <value>The get now.</value>
        public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
        
        /// <summary>
   /// Gets the now.
      /// </summary>
        /// <value>The now.</value>
 public DateTime Now { get => GetNow(); }
   #endregion

        #region Methods
      /// <summary>
        /// Initializes a new instance of the <see cref="TemplateModel"/> class.
    /// </summary>
        public TemplateModel()
        {
            _timer = new(250d);
            _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
 _timer.Start();
   }
#endregion
    }
}
