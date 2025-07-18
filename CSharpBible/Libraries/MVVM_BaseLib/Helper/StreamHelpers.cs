﻿// ***********************************************************************
// Assembly         : MVVM_BaseLib
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="StreamHelpers.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BaseLib.Helper;

public static class StreamHelpers
{
    public static void EnumerateToStream(this Stream stream, IEnumerable<(string, object)> values)
    {
        foreach (var value in values)
        {
            switch (value.Item2)
            {
                case Point p:
                    stream.Write(BitConverter.GetBytes(p.X), 0, sizeof(int));
                    stream.Write(BitConverter.GetBytes(p.Y), 0, sizeof(int));
                    break;
                case int i:
                    stream.Write(BitConverter.GetBytes(i), 0, sizeof(int));
                    break;
                case bool x:
                    stream.Write(BitConverter.GetBytes(x), 0, sizeof(bool));
                    break;
                case byte b:
                    stream.WriteByte(b);
                    break;
                case IEnumerable<int> ii:
                    if (!(ii is IReadOnlyList<int> il))
                        il = ii.ToList();
                    stream.Write(BitConverter.GetBytes((short)il.Count), 0, sizeof(short));
                    foreach (var value1 in il)
                    {
                        stream.Write(BitConverter.GetBytes(value1), 0, sizeof(int));
                    }
                    break;
                case IEnumerable<IPersistence> ip:
                    if (!(ip is IReadOnlyList<IPersistence> ilp))
                        ilp = ip.ToList();
                    stream.Write(BitConverter.GetBytes((int)ilp.Count), 0, sizeof(int));
                    foreach (var value1 in ilp)
                    {
                        stream.EnumerateToStream(value1.EnumerateProp());
                    }
                    break;
            }
        }
    }

    static public IEnumerable<(string, object)> StreamToEnumerable(this Stream stream, IEnumerable<(string, Type)> schema)
    {
        byte[] streamBytes;
        foreach (var e in schema)
            switch (e.Item2)
            {
                case Type t when t == typeof(Point):
                    streamBytes = new byte[sizeof(int) * 2];
                    if (stream.Read(streamBytes, 0, sizeof(int) * 2) == sizeof(int) * 2)
                        yield return (e.Item1, new Point(BitConverter.ToInt32(streamBytes, 0), BitConverter.ToInt32(streamBytes, sizeof(int))));
                    else
                        yield return (e.Item1, new Point()); // ?? default value 
                    break;
                case Type t when t == typeof(int):
                    streamBytes = new byte[sizeof(int)];
                    if (stream.Read(streamBytes, 0, sizeof(int))== sizeof(int))
                        yield return (e.Item1, BitConverter.ToInt32(streamBytes, 0));
                    else
                        yield return (e.Item1, 0); // ?? default value
                    break;
                case Type t when t == typeof(bool):
                    streamBytes = new byte[sizeof(bool)];
                    if (stream.Read(streamBytes, 0, sizeof(bool))== sizeof(bool))
                        yield return (e.Item1, BitConverter.ToBoolean(streamBytes, 0));
                    else
                        yield return (e.Item1, false); // ?? default value
                    break;
                case Type t when t == typeof(byte):
                    streamBytes = new byte[sizeof(byte)];
                    if (stream.Read(streamBytes, 0, sizeof(byte)) == sizeof(byte))
                        yield return (e.Item1, streamBytes[0]);
                    else
                        yield return (e.Item1, (byte)0); // ?? default value
                    break;
                case Type t when typeof(IEnumerable<int>).IsAssignableFrom(t):
                    streamBytes = new byte[sizeof(short)];
                    _ = stream.Read(streamBytes, 0, sizeof(short));
                    var count = BitConverter.ToInt16(streamBytes, 0);
                    streamBytes = new byte[sizeof(int) * count];
                    _ = stream.Read(streamBytes, 0, sizeof(int) * count);
                    yield return (e.Item1, streamBytes.Select<byte, int?>((b, i) => i % sizeof(int) == 0 ? BitConverter.ToInt32(streamBytes, i) : null).Where((i) => i != null).Select(i=>i!.Value).ToList());
                    break;
                case Type t when typeof(IEnumerable<IPersistence>).IsAssignableFrom(t):
                    Type t1 = typeof(IPersistence);
                    if (t.IsArray && t.HasElementType) t1=t.GetElementType();
                    streamBytes = new byte[sizeof(int)];
                    _ = stream.Read(streamBytes, 0, sizeof(int));
                    var count2 = BitConverter.ToInt32(streamBytes, 0);
                    var result1 = new IPersistence?[count2];
                    if (t1.IsClass && t1.GetConstructors().FirstOrDefault(c => c.IsPublic) != null)
                        for (var i = 0; i < count2; i++)
                        {
                            result1[i] = (IPersistence?)Activator.CreateInstance(t1);
                        }
                    else
                        for (var i = 0; i < count2; i++)
                        {
                            result1[i] = IoC.GetRequiredService<IPersistence>();
                        }
                    yield return (e.Item1, result1.Select(b => b?.ReadFromEnumerable(stream.StreamToEnumerable(b!.PropTypes)) ?? false ? b : null).ToList());
                    break;
                case Type t when t.IsGenericType && t.GetGenericTypeDefinition()== typeof(IEnumerable<>):
                    var t2 = t.GetGenericArguments()[0];
                    streamBytes = new byte[sizeof(int)];
                    _ = stream.Read(streamBytes, 0, sizeof(int));
                    var count32 = BitConverter.ToInt32(streamBytes, 0);
                    var result = new IPersistence?[count32];
                    if (t2.IsClass && t2.GetConstructors().FirstOrDefault(c => c.IsPublic)!=null)
                        for (var i =0;i<count32; i++)
                        {
                            result[i] = (IPersistence?)Activator.CreateInstance(t2);
                        }
                    else
                        for (var i = 0; i < count32; i++)
                        {
                            result[i] = IoC.GetRequiredService<IPersistence>();
                        }
                    yield return (e.Item1, result.Select(b => b?.ReadFromEnumerable(stream.StreamToEnumerable(b!.PropTypes))??false?b:null).ToList());
                    break;
            }
    }
}