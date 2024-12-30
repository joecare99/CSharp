//using DAO;
using System;
using System.Collections;

namespace GenFree.Interfaces.DB;

public interface IFieldsCollection : IEnumerable
{
    IField this[Enum idx] { get; }
    IField this[int idx] { get; }
    IField this[string name] { get; }

    //  IEnumerator GetEnumerator();
}