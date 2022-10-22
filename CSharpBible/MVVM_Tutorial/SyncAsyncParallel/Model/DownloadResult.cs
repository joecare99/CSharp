// ***********************************************************************
// Assembly         : SyncAsyncParallel
// Author           : Mir
// Created          : 12-26-2021
//
// Last Modified By : Mir
// Last Modified On : 12-26-2021
// ***********************************************************************
// <copyright file="DownloadResult.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncAsyncParallel.Model
{
    /// <summary>
    /// Class DownloadResult.
    /// </summary>
    class DownloadResult
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        public string Html { get; set; }
        /// <summary>
        /// Gets the contentlength.
        /// </summary>
        /// <value>The contentlength.</value>
        public int Contentlength => Html?.Length ?? 0;
    }

}
