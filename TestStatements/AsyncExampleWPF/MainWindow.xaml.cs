using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
#if NET5_0_OR_GREATER
using System.Net.Http;
#endif
using System.Net;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace AsyncExampleWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            resultsTextBox.Text = "" ;
            SumPageSizes();
            resultsTextBox.Text += "\r\nControl returned to startButton_Click.";
        }

        private void SumPageSizes()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetURLContents returns the contents of url as a byte array.
                byte[] urlContents = GetURLContents(url);

                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\r\n\r\nTotal bytes returned:  {total}\r\n";
        }

        private async Task SumPageSizesAsync()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetURLContents returns the contents of url as a byte array.
                byte[] urlContents = await GetURLContentsAsync(url);

                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\r\n\r\nTotal bytes returned:  {total}\r\n";
        }

        private async Task SumPageSizesAsync2()
        {
            // Declare an HttpClient object and increase the buffer size. The
            // default buffer size is 65,536.
#if NET5_0_OR_GREATER
            using HttpClient client = new() { MaxResponseContentBufferSize = 1000000 };
#else
            using WebClient client = new();
#endif

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetByteArrayAsync returns the contents of url as a byte array.
#if NET5_0_OR_GREATER
                byte[] urlContents = await client.GetByteArrayAsync(url);
#else
                byte[] urlContents = await client.DownloadDataTaskAsync(new Uri(url));
#endif
                DisplayResults(url, urlContents);

                // Update the total.
                total += urlContents.Length;
            }

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\r\n\r\nTotal bytes returned:  {total}\r\n";
        }

        private async Task SumPageSizesAsync3()
        {
            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // Create a query.
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in urlList select ProcessURLAsync(url);

            // Use ToArray to execute the query and start the download tasks.
            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            // Await the completion of all the running tasks.
            int[] lengths = await Task.WhenAll(downloadTasks);

            int total = lengths.Sum();

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\r\n\r\nTotal bytes returned:  {total}\r\n";
        }

        private async Task SumPageSizesAsync4()
        {
            // Declare an HttpClient object and increase the buffer size. The
            // default buffer size is 65,536.
#if NET5_0_OR_GREATER
            using HttpClient client = new() { MaxResponseContentBufferSize = 1000000 };
#else
            using var client = new WebClient();
#endif

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // Create a query.
            IEnumerable<Task<int>> downloadTasksQuery =
                from url in urlList select ProcessURLAsync(url, client);

            // Use ToArray to execute the query and start the download tasks.
            Task<int>[] downloadTasks = downloadTasksQuery.ToArray();

            // Await the completion of all the running tasks.
            int[] lengths = await Task.WhenAll(downloadTasks);

            int total = lengths.Sum();

            // Display the total count for all of the web addresses.
            resultsTextBox.Text += $"\r\n\r\nTotal bytes returned:  {total}\r\n";
        }

        async Task<int> ProcessURLAsync(string url)
        {
            byte[] byteArray = await GetURLContentsAsync(url);
            DisplayResults(url, byteArray);
            return byteArray.Length;
        }

#if NET5_0_OR_GREATER
        async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            byte[] byteArray = await client.GetByteArrayAsync(url);
#else
        async Task<int> ProcessURLAsync(string url, WebClient client)
        {
            byte[] byteArray = await client.DownloadDataTaskAsync(new Uri(url));
#endif
            DisplayResults(url, byteArray);
            return byteArray.Length;
        }


    private List<string> SetUpURLList()
        {
            var urls = new List<string>
            {
                "https://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                "https://msdn.microsoft.com",
                "https://msdn.microsoft.com/library/hh290136.aspx",
                "https://msdn.microsoft.com/library/ee256749.aspx",
                "https://msdn.microsoft.com/library/hh290138.aspx",
                "https://msdn.microsoft.com/library/hh290140.aspx",
                "https://msdn.microsoft.com/library/dd470362.aspx",
                "https://msdn.microsoft.com/library/aa578028.aspx",
                "https://msdn.microsoft.com/library/ms404677.aspx",
                "https://msdn.microsoft.com/library/ff730837.aspx"
            };
                    return urls;
        }

        private byte[] GetURLContents(string url)
        {
#if NET5_0_OR_GREATER
            using HttpClient client = new();
            return client.GetByteArrayAsync(url).GetAwaiter().GetResult();
#else
            using WebClient client = new();
            return client.DownloadData(url);
#endif
        }

        private async Task<byte[]> GetURLContentsAsync(string url)
        {
#if NET5_0_OR_GREATER
            using HttpClient client = new();
            return await client.GetByteArrayAsync(url);
#else
            using WebClient client = new();
            return await client.DownloadDataTaskAsync(new Uri(url));
#endif
        }

        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format
            // is designed to be used with a monospaced font, such as
            // Lucida Console or Global Monospace.
            var bytes = content.Length;
            // Strip off the "https://".
            var displayURL = url.Replace("https://", "");
            resultsTextBox.Text += $"\n{displayURL,-58} {bytes,8}";
        }

        private async void startButtonAsync_Click(object sender, RoutedEventArgs e)
        {
            DisableStart();
            try
            {
                resultsTextBox.Text = "";
                await SumPageSizesAsync();
                resultsTextBox.Text += "\r\nControl returned to startButtonAsync_Click.";
            }
            finally
            {
                EnableStart();
            }
        }

        private async void startButtonAsync2_Click(object sender, RoutedEventArgs e)
        {
            DisableStart();
            try
            {
                resultsTextBox.Text = "";
                await SumPageSizesAsync2();
                resultsTextBox.Text += "\r\nControl returned to startButtonAsync_Click.";
            }
            finally
            {
                EnableStart();
            }
        }

        private async void startButtonAsync3_Click(object sender, RoutedEventArgs e)
        {
            DisableStart();
            try
            {
                resultsTextBox.Text = "";
                await SumPageSizesAsync3();
                resultsTextBox.Text += "\r\nControl returned to startButtonAsync_Click.";
            }
            finally
            {
                EnableStart();
            }

        }

        private async void startButtonAsync4_Click(object sender, RoutedEventArgs e)
        {
            DisableStart();
            try
            {
                resultsTextBox.Text = "";
                await SumPageSizesAsync4();
                resultsTextBox.Text += "\r\nControl returned to startButtonAsync_Click.";
            }
            finally
            {
                EnableStart();
            }
        }

        private static Stopwatch aStopWatch = new Stopwatch();

        private void EnableStart()
        {
            startButtonAsync4.IsEnabled =
            startButtonAsync3.IsEnabled = 
                startButtonAsync2.IsEnabled = 
            startButtonAsync.IsEnabled = true;
            aStopWatch.Stop();
            resultsTextBox.Text += string.Format("\r\nElapsed time [ms]: {0}", aStopWatch.ElapsedMilliseconds);
            aStopWatch.Reset();
        }

        private void DisableStart()
        {
            startButtonAsync4.IsEnabled =
            startButtonAsync3.IsEnabled = 
            startButtonAsync2.IsEnabled = 
            startButtonAsync.IsEnabled = false;
            aStopWatch.Start();
        }

    }
}
