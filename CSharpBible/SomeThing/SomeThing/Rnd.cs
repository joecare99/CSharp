using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SomeThingTests")]
public class Rnd
{
    private Random _rnd;

    public static Func<Random> GetRnd {  get; set; }=()=>new Random();
    public Rnd()
    {
        _rnd=GetRnd();
    }

    public int Next(int iMax)=>_rnd.Next(iMax);
}