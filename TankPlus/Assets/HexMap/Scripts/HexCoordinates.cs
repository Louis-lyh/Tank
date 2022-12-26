using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank.HexMap
{
    /// <summary>
    /// 六边形坐标系
    /// </summary>
    [System.Serializable]
    public struct HexCoordinates
    {
        public int X;
        public int Z;
        public int Y => -X - Z;

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
            return $"({X},{Y},{Z})";
        }

        public string ToStringSeparateLines()
        {
            return $"{X}\n{Y}\n{Z}";
            
        }
        //Vector3转换为六边形坐标系
        // public static HexCoordinates FromPosition(Vector3 position)
        // {
        //     //x水平坐标 = 点击点的x坐标 / 外半径 * 2
        //     float x = position.x / (HexMetrics.InnerRadius * 2f);
        //     float y = -x;
        //     
        // }
        
    }
}

