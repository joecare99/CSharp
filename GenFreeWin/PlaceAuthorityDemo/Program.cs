using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Places;
using GenFreeBrowser.Places.Interface;

namespace PlaceAuthorityDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Place Authority Demo");

            using var httpClient = new HttpClient();

            // Example usage of GeoNamesAuthority
            IPlaceAuthority NamesAuthority = new NominatimAuthority(httpClient);
            var geoNamesResult = await NamesAuthority.SearchAsync(new PlaceQuery("Berlin"), CancellationToken.None);
            Console.WriteLine("GeoNamesAuthority Result:");
            foreach (var place in geoNamesResult)
            {
                Console.WriteLine(place);
            }

            // Example usage of GovAuthority
            IPlaceAuthority govAuthority = new GovAuthority(httpClient);
            var govResult = await govAuthority.SearchAsync(new PlaceQuery("Berlin"), CancellationToken.None);
            Console.WriteLine("GovAuthority Result:");
            foreach (var place in govResult)
            {
                Console.WriteLine(place);
            }

            Console.WriteLine("Demo completed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}