using Microsoft.VisualBasic;
using System.Text;

var q = "var(v,a,r)=(\"\",new byte[6<<6],new System.IO.Compression.BrotliDecoder());r.Decompress(Convert.FromBase64String(v),a,out _,out _);Console.Write(string.Join(\"\",a.Select(v=>(char)v)).Insert(13,v));";
var b= Encoding.UTF8.GetBytes(q);
var ob= new byte[System.IO.Compression.BrotliEncoder.GetMaxCompressedLength(b.Length)];
new System.IO.Compression.BrotliEncoder().Compress(b, ob, out _,out int w, true);
var i= Convert.ToBase64String(ob[..w]);
Console.WriteLine(q.Length);
Console.WriteLine(w);
Console.WriteLine(i.Length);
Console.WriteLine(q.Insert(13,i));


