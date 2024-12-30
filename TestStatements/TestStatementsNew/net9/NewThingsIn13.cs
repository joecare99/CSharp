using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatementsNew.net9;
public class NewThingsIn13
{
    class S { public int[] buffer; }
    public static void ImplicitIndex()
    {
        var numbers = new S()
        {
            buffer =
    {
        [^1] = 0,
        [^2] = 1,
        [^3] = 2,
        [^4] = 3,
        [^5] = 4,
        [^6] = 5,
        [^7] = 6,
        [^8] = 7,
        [^9] = 8,
        [^10] = 9
    }
        };
        var index = 2;
        Console.WriteLine(numbers.buffer[index]);
    }


}
