using System;
using System.Collections.Generic;
using Unity.Netcode;

public static class NetworkListExtensions
{
    public static List<T> ToList<T>(this NetworkList<T> netList) where T : unmanaged, IEquatable<T>
    {
        var list = new List<T>(netList.Count);
        foreach (var item in netList)
        {
            list.Add(item);   
        }
        return list;
    }
}