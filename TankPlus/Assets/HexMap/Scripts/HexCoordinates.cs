using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形坐标系
/// </summary>
[System.Serializable]
public struct HexCoordinates
{
    public int X { get; private set; }
    public int Z { get; private set; }

    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }
    //返回偏移坐标
    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return  new HexCoordinates(x - z / 2,z);
    }

    public override string ToString()
    {
        return $"({X},{Z})";
    }

    public string ToStringSeparateLines()
    {
        return $"{X}\n{Z}";
    }
}
