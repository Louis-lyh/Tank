using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshFilter))]
public class HexMesh : MonoBehaviour
{
    //网格
    private Mesh _hexMesh;
    //网格顶点
    private List<Vector3> _vector3s;
    //网格三角面
    private List<int> _triangles;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
        _hexMesh.name = "Hex Mesh";
        _vector3s = new List<Vector3>();
        _triangles = new List<int>();
    }

    public void Triangulate(HexCell[] cells)
    {
          
    }
    
}
