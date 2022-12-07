using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode
{
    public Color GradientColor;
    
    public float distance;
    
    public Vector3 vector;

    public bool isObstacle = false;

    public float minDistanceAround = 0f;
    
    public bool isBesideObstacle;


    public void Initialize()
    {
        GradientColor=Color.clear;
        distance = 0F;
        vector = Vector3.zero;
        isObstacle = false;
        minDistanceAround = 0f;
        isBesideObstacle = false;

    }
}
