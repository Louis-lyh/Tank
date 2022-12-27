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
        public static HexCoordinates  FromPosition(Vector3 position)
        {
            //x水平坐标 = 点击点的x坐标 / 外半径 * 2
            //当 z == 0时 x 和 y 坐标互为镜像
            float x = position.x / (HexMetrics.InnerRadius * 2f);
            float y = -x;
            //当 z 坐标增加时 x 和 y 坐标向坐标移动
            float offset = position.z / (HexMetrics.OuterRadius * 3f);
            x -= offset;
            y -= offset;
            //四舍五入 获得坐标
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);
            int iZ = Mathf.RoundToInt(-x - y);

            //当坐标之和不为零时 坐标是错误的
            if (iX + iY + iZ != 0)
            {
                //计算点与中心的距离
                float dX = Mathf.Abs(x - iX);
                float dY = Mathf.Abs(y - iY);
                float dZ = Mathf.Abs(-x - y - iZ);
                //重新计算与中心偏差最大的点
                if (dX > dY && dX > dZ)
                {
                    iX = -iY - iZ;
                }
                else if (dZ > dY)
                {
                    iZ = -iX - iY;
                }
            }
            return new HexCoordinates(iX, iZ);
        }
    }
}

