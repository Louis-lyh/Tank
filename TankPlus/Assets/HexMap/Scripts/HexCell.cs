using System;
using UnityEngine;

namespace Tank.HexMap
{
    public class HexCell : MonoBehaviour
    {
        //坐标
        public HexCoordinates Coordinates;
        //
        public Color color;
        //邻居
        [SerializeField]
        private HexCell[] neighbors = new HexCell[6];
        //获得邻居
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int) direction];
        }
        //设置邻居
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int) direction] = cell;
            cell.neighbors[(int) direction.Opposite()] = this;
        }
        
    }
    //邻居方向
    public enum HexDirection
    {
        NE = 0,
        E,
        SE,
        SW,
        W,
        NW
    }
    //方向扩展
    public static class HexDirectionExtensions {

        public static HexDirection Opposite (this HexDirection direction) {
            return (int)direction < 3 ? (direction + 3) : (direction - 3);
        }
    }
}
