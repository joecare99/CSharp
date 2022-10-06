using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading;
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
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            var total = 0;
            foreach (var url in urlList)
            {
                // GetByteArrayAsync returns the contents of url as a byte array.
                byte[] urlContents = await client.GetByteArrayAsync(url);
             
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
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

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

        async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            byte[] byteArray = await client.GetByteArrayAsync(url);
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
            // The downloaded resource ends up in the variable named content.
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL.
#if NET5_0_OR_GREATER
            var webReq = (HttpWebRequest)WebRequest.Create(url);
#else
            var webReq = (HttpWebRequest)WebRequest.Create(url);
#endif

            // Send the request to the Internet resource and wait for
            // the response.
            // Note: you can't use HttpWebRequest.GetResponse in a Windows Store app.
            using (WebResponse response = webReq.GetResponse())
            {
                // Get the data stream that is associated with the specified URL.
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Read the bytes in responseStream and copy them to content.
                    responseStream.CopyTo(content);
                }
            }

            // Return the result as a byte array.
            return content.ToArray();
        }

        private async Task<byte[]> GetURLContentsAsync(string url)
        {
            // The downloaded resource ends up in the variable named content.
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL.
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for
            // the response.
            // Note: you can't use HttpWebRequest.GetResponse in a Windows Store app.
            using (WebResponse response = await webReq.GetResponseAsync())
            {

                // Get the data stream that is associated with the specified URL.
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Read the bytes in responseStream and copy them to content.
                    await responseStream.CopyToAsync(content);
                }
            }

            // Return the result as a byte array.
            return content.ToArray();
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
