using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tank.HexMap
{
    /// <summary>
    ///六边形网格 
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        public int width = 6;
        public int height = 6;
        //格子预制体
        public HexCell  cellPrefab;
        //所有格子
        private HexCell[] _cells;
        //文本
        public Text cellLabelPrefab;
        //画布
        private Canvas _gridCanvas;
        //六边形网格
        private HexMesh _hexMesh;
        //默认颜色
        public Color defaultColor = Color.white;
        //
        public Color touchedColor = Color.cyan;
        private void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh = GetComponentInChildren<HexMesh>();
            _cells = new HexCell[height * width];
    
            for (int z = 0,i = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
        }
    
        private void Start()
        {
            _hexMesh.Triangulate(_cells);
        }
    
        /// <summary>
        /// 生成网格
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="z">纵坐标</param>
        /// <param name="i">索引</param>
        private void CreateCell(int x, int z, int i)
        {
            //位置 
            Vector3 position;
            //相邻六边形单元个在X方向上的距离等于内半径的2倍
            position.x = x * (HexMetrics.InnerRadius * 2f);
            var xOffset = ( z * 0.5f -  z / 2) * (HexMetrics.InnerRadius * 2f);
            position.x += xOffset;
            position.y = 0f;
            //相邻六边形单元个z方向上的距离等于外半径的1.5倍
            position.z = z * (HexMetrics.OuterRadius * 1.5f);
            //生成格子
            HexCell cell = _cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(transform,false);
            cell.transform.localPosition = position;
            //设置坐标系坐标
            cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x,z);
            //设置颜色
            cell.color = defaultColor;
            //设置邻居
            if(x > 0) // 左邻居
                cell.SetNeighbor(HexDirection.W,_cells[i - 1]);
            if(z > 0)
                if ((x & 1) == 0) //偶数行
                {
                    //右下邻居
                    cell.SetNeighbor(HexDirection.SE,_cells[i - width]);
                    //西南 左下邻居
                    if(x > 0)
                        cell.SetNeighbor(HexDirection.SW,_cells[i - width - 1]);
                }
                else // 奇数行
                {
                    //右下邻居
                    cell.SetNeighbor(HexDirection.W,_cells[i - width]);
                    //左下令居
                    if(x < width - 1)
                        cell.SetNeighbor(HexDirection.SE,_cells[i - width + 1]);
                }
            
            //生成坐标文本
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(_gridCanvas.transform,false);
            //锚点坐标
            label.rectTransform.anchoredPosition = new Vector2(position.x,position.z);
            label.text = cell.Coordinates.ToString();
        }

     
        //点击方块
        public void TouchCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            //Vector3转换为六边形坐标系
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            //计算索引
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            //
            HexCell cell = _cells[index];
            cell.color = touchedColor;
            //重新计算网格
            _hexMesh.Triangulate(_cells);
        }

        public void ColorCell(Vector3 position, Color color)
        {
            position = transform.InverseTransformPoint(position);
            //世界坐标转为六边形坐标系
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            //计算HexCell的索引
            int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            //
            HexCell cell = _cells[index];
            cell.color = color;
            //重新计算网格
            _hexMesh.Triangulate(_cells);
        }
        
    }
}


