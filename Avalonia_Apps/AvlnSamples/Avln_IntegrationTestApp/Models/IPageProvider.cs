using System.Collections.Generic;

namespace IntegrationTestApp.Models;

internal interface IPageProvider
{
 IEnumerable<Page> GetPages();
}
