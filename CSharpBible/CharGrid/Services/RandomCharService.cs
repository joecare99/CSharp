using System;

namespace CSharpBible.CharGrid.Services
{
 public class RandomCharService : IRandomCharService
 {
 private readonly Random _rnd = new Random();
 public char NextChar()
 {
 // ASCII uppercase letters
 return (char)('A' + _rnd.Next(0,26));
 }
 }
}
