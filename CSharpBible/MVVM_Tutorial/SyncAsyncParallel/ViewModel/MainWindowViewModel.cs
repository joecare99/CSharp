// ***********************************************************************
// Assembly         : SyncAsyncParallel
// Author           : Mir
// Created          : 12-26-2021
//
// Last Modified By : Mir
// Last Modified On : 12-29-2021
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.ViewModel;
using System.Net.Http;
using SyncAsyncParallel.Model;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace SyncAsyncParallel.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// The url's
        /// </summary>
        readonly string[] urls = new string[]{
            "https://www.stackoverflow.com",
            "https://www.microsoft.com",
            "https://www.youtube.com",
            "https://www.windows.com",
            "https://www.asm.net",
            "https://www.jc99.de",
            "https://www.google.com",
            "https://www.yahoo.com",
            "https://www.bing.com",
            "https://www.gmail.com"
        };


        /// <summary>
        /// The information text
        /// </summary>
        string infoText="";

        /// <summary>
        /// Gets or sets the information text.
        /// </summary>
        /// <value>The information text.</value>
        public string InfoText
        {
            get => infoText; set
            {
                if (value == infoText) return;
                infoText = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Gets the download synchronize command.
        /// </summary>
        /// <value>The download synchronize command.</value>
        public DelegateCommand Download_syncCmd { get; private set; }
        /// <summary>
        /// Gets the download asynchronous command.
        /// </summary>
        /// <value>The download asynchronous command.</value>
        public DelegateCommand Download_asyncCmd { get; private set; }
        /// <summary>
        /// Gets the download asynchronous para command.
        /// </summary>
        /// <value>The download asynchronous para command.</value>
        public DelegateCommand Download_async_paraCmd { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            Download_syncCmd = new DelegateCommand((o) => Download_sync());
            Download_asyncCmd = new DelegateCommand((o) =>_ = Download_async());
            Download_async_paraCmd = new DelegateCommand((o) =>_= Download_async_para());
        }

        /// <summary>
        /// Downloads the synchronize.
        /// </summary>
        void Download_sync()
        {

            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<DownloadResult> results = new(urls.Length);
            foreach (string url in urls)
            {
                results.Add(_DownloadUrl(url));
                ShowResults(results);
                Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
            }
            watch.Stop();
            InfoText += $"Total Execution Time {watch.ElapsedMilliseconds}ms";
        }

        /// <summary>
        /// Downloads the asynchronous.
        /// </summary>
        async Task Download_async()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<DownloadResult> results = new(urls.Length);
            foreach (string url in urls)
            {
                results.Add(await Task.Run(() => _DownloadUrl(url)));
                ShowResults(results);
            }
            watch.Stop();
            InfoText += $"Total Execution Time {watch.ElapsedMilliseconds}ms";
        }
        /// <summary>
        /// Downloads the asynchronous para.
        /// </summary>
        async Task Download_async_para()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<Task<DownloadResult>> tasklist =new(urls.Length);
            foreach (string url in urls)
            {
                tasklist.Add(Task.Run(() => _DownloadUrl(url)));
            }

            List<DownloadResult> results = new();
            while (tasklist.Count>0)
            {
                var result = await Task.WhenAny(tasklist);
                tasklist.Remove(result);
                results.Add(result.Result);
                ShowResults(results);
            }
            watch.Stop();
            InfoText += $"Total Execution Time {watch.ElapsedMilliseconds}ms";
        }

        /// <summary>
        /// Downloads the URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>DownloadResult.</returns>
        private DownloadResult _DownloadUrl(string url)
        {
            using HttpClient client = new();
            try
            {
                string html = client.GetStringAsync(url).Result;
                return new DownloadResult()
                {
                    Html = html,
                    Url = url
                };
            }
            catch (Exception e)
            {
                return new DownloadResult()
                {
                    Html = e.Message,
                    Url = url
                };

            }
        }
        /// <summary>
        /// Shows the results.
        /// </summary>
        /// <param name="results">The results.</param>
        private void ShowResults(List<DownloadResult> results)
        {
            StringBuilder text = new();
            foreach (var result in results)
            {
                text.Append(result.Url);
                text.Append('\t');
                text.Append(result.ContentLength);
                text.Append(Environment.NewLine);
            }
            this.InfoText = text.ToString();

        }
    }
}
