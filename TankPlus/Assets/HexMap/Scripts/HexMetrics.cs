using UnityEngine;

public static class HexMetrics
{
    //六边形外接圆半径
    public const float OuterRadius = 10f;
    //六边形内接圆半径
    public const float InnerRadius = OuterRadius * 0.866025404f;
    //六个角的局部坐标
    public static Vector3[] Corners =
    {
        new Vector3(0f,0f,OuterRadius),
        new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
        new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(0f, 0f, -OuterRadius),
        new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
        new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius)
    };
}
