using System.Collections.Generic;

namespace IntegrationTestApp.Models;

internal interface IPageProvider
{
 IEnumerable<DemoPage> GetPages();
}
