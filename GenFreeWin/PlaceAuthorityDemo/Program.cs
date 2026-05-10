using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Places;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Authorities;

namespace PlaceAuthorityDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Place Authority Demo");

            using var httpClient = new HttpClient();

            // Example usage of GeoNamesAuthority
            IGenPlaceAuthority NamesAuthority = new NominatimAuthority(httpClient);
            var geoNamesResult = await NamesAuthority.SearchPlacesAsync(new GenPlaceQuery { Text = "Berlin" }, CancellationToken.None);
            Console.WriteLine("GeoNamesAuthority Result:");
            foreach (var place in geoNamesResult)
            {
                Console.WriteLine(place);
            }

            // Example usage of GovAuthority
            IGenPlaceAuthority govAuthority = new GovAuthority(httpClient);
            var govResult = await govAuthority.SearchPlacesAsync(new GenPlaceQuery { Text = "Berlin" }, CancellationToken.None);
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