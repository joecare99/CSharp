// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

static bool IsNeeded2(string input) => !Regex.IsMatch(input, "[\u200C\u200D][‌‍‌‍‌‍]");

string Zero1 = "H\u200De\u200Dl\u200Dl\u200Do";
string Zero2 = "H\u200D\u200Del\u200D\u200Clo";

Console.WriteLine($"Zero1: {Zero1} ({IsNeeded2(Zero1)})");
Console.WriteLine($"Zero2: {Zero2} ({IsNeeded2(Zero2)})");
