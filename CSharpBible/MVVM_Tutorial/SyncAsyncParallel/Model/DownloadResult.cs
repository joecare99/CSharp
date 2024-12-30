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
        public string Url { get; set; } = "";
        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        public string Html { get; set; } = "";
        /// <summary>
        /// Gets the content length.
        /// </summary>
        /// <value>The contentLength.</value>
        public int ContentLength => Html?.Length ?? 0;
    }

}
