using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank.HexMap
{
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        //网格
        private Mesh _hexMesh;
        //网格顶点
        private List<Vector3> _vertices;
        //网格三角面
        private List<int> _triangles;
        //网格碰撞体
        private MeshCollider _meshCollider;
        private void Awake()
        {
            //网格
            GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
            //网格碰撞器
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _hexMesh.name = "Hex Mesh";
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
        }
        //三角测量 （计算网格）
        public void Triangulate(HexCell[] cells)
        {
            _hexMesh.Clear();
            _vertices.Clear();
            _triangles.Clear();
    
            for (int i = 0; i < cells.Length; i++)
            {
                Triangulate(cells[i]);
            }
    
            _hexMesh.vertices = _vertices.ToArray();
            _hexMesh.triangles = _triangles.ToArray();
            //从三角形和顶点重新计算网格的法线。
            _hexMesh.RecalculateNormals();
            //生成网格碰撞器
            _meshCollider.sharedMesh = _hexMesh;
        }
    
        public void Triangulate(HexCell cell)
        {
            Vector3 center = cell.transform.localPosition;
            for (int i = 0; i < 6; i++)
            {
                AddTriangle(center,center + HexMetrics.Corners[i],center + HexMetrics.Corners[(i + 1) % 6]);
            }
    
        }
        //添加三角形
        void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex+1);
            _triangles.Add(vertexIndex+2);
        }
        
    }
}


