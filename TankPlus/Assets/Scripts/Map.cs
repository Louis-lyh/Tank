using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour
{
    private Dictionary<Vector2Int, TileNode> mTileMap;

    [SerializeField] private float cellSize;

    [SerializeField] private Vector2Int boundMin;
    [SerializeField] private Vector2Int boundMax;

    [SerializeField] private Gradient mGradient;

    [SerializeField] private GameObject obstacle;


    public class Grid
    {
        private float cellSize;

        private BoundsInt boundsInt;
        public Bounds realBounds;

        private float maxDistence;
        private Gradient gradient;

        public Dictionary<Vector2Int, TileNode> tileMap;
        public Dictionary<Vector2Int, bool> obstacleMap;
        public Dictionary<Vector2Int, bool> openMap;

        public Plane plane;

        public Vector2Int destination;

        public Grid(float cellSize, Vector2Int min, Vector2Int max, Gradient gradient)
        {
            this.cellSize = cellSize;
            boundsInt = new BoundsInt();
            boundsInt.SetMinMax(new Vector3Int(min.x, 0, min.y), new Vector3Int(max.x, 1, max.y));

            Debug.LogWarning($"center: {boundsInt.center} min: {boundsInt.min} max: {boundsInt.max}");
            realBounds = new Bounds(boundsInt.center * cellSize, (Vector3) boundsInt.size * cellSize);

            maxDistence = Vector2.Distance(min, max);
            this.gradient = gradient;
            tileMap = new Dictionary<Vector2Int, TileNode>();
            obstacleMap = new Dictionary<Vector2Int, bool>();
            openMap = new Dictionary<Vector2Int, bool>();

            plane = new Plane(Vector3.up, realBounds.center);

            Debug.LogWarning($"maxDistance: {maxDistence}");
            GenerateRandomObstacle();
            
            
        }

        void VectorFieldClear()
        {
            foreach (var t in tileMap)
            {
                t.Value.Initialize();
                openMap[t.Key] = false;
            }
        }
        
        
        

        public  Dictionary<Vector2Int, TileNode> UpdateVectorField(Vector3 target)
        {
            Vector2Int p = WorldToCell(target);
            
            if (boundsInt.Contains2D(p)&&
                p!=destination&&
                !obstacleMap[p])
            {
                return GenerateTileMap(p);
            }
           

            return tileMap;
        }

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            return WorldToCell(new Vector2(worldPos.x, worldPos.z));
        }

        public Vector2Int WorldToCell(Vector2 worldPos_xz)
        {
            int xCell = (int) Mathf.Floor(worldPos_xz.x / cellSize);
            int zCell = (int) Mathf.Floor(worldPos_xz.y / cellSize);

            return new Vector2Int(xCell, zCell);
        }

        public Vector3 cellToWorld(int xCell, int zCell)
        {
            return cellToWorld(new Vector2Int(xCell, zCell));
        }

        public Vector3 cellToWorld(Vector2Int cellPos)
        {
            Vector3 vector = Vector3.zero;
            vector.x = (cellPos.x + 0.5f) * cellSize;
            vector.z = (cellPos.y + 0.5f) * cellSize;
            return vector;
        }

        public bool IsInBounds(Vector3 position)
        {
            return realBounds.Contains(position);
        }

        public bool IsInBounds(Vector2Int cellPosition)
        {
            return boundsInt.Contains2D(cellPosition);
        }

        public bool IsOpen(Vector2Int target)
        {
            return boundsInt.Contains2D(target);
        }

        void GenerateRandomObstacle()
        {
            foreach (var vi in boundsInt.allPositionsWithin)
            {
                Vector2Int v2 = new Vector2Int(vi.x, vi.z);
                // obstacleMap[new Vector2Int(vi.x, vi.z)] = false;
                obstacleMap[v2] = Random.Range(1, 30) == 1;

                openMap[v2] = false;
                tileMap[v2] = new TileNode();
            }

            for (int i = 0; i < 14; i++)
            {
                obstacleMap[new Vector2Int(5, i)] = true;
            }
            
            for (int i = 15; i >= 3; i--)
            {
                obstacleMap[new Vector2Int(9, i)] = true;
            }
        }

        //public Dictionary<Vector2Int, bool> debug;
        //public Dictionary<Vector2Int, float> debugDistance;

        public Vector3 GetVector(Vector3 pos)
        {
            Vector3 result = Vector3.zero;
            Vector2Int pi = WorldToCell(pos);
            if (boundsInt.Contains2D(pi))
            {
                result = tileMap[pi].vector;
            }
            return result;
        }


        void BFSDistanceCalculate(Vector2Int target)
        {
            //debug = new Dictionary<Vector2Int, bool>();
            //debugDistance = new Dictionary<Vector2Int, float>();
            Queue<Vector2Int> open = new Queue<Vector2Int>();

            open.Enqueue(target);
            if (!tileMap.ContainsKey(target))
            {
                var tar = new TileNode();
                tileMap[target] = tar;
            }
            
            

            //debugDistance[target] = 0f;

            float sqrt2 = 1.414f;
            while (open.Count > 0)
            {
                var current = open.Dequeue();
                for (int i = 0; i < 8; i++)
                {
                    var pd = GetPosByDirection((Direction) i, current);
                    if (!boundsInt.Contains2D(pd)) continue; //越界，跳过

                    float diff = i % 2 == 0 ? 1f : sqrt2; //距离取决于方向，这里奇数是斜方向，偶数是正方向
                    float to = tileMap[current].distance + diff;

                    if (!openMap[pd]) //没搜索到过
                    {
                        
                        var node = tileMap[pd];
                        if (node == null)
                        {
                            node = new TileNode();
                            tileMap[pd] = node;
                        }
                        
                        if (obstacleMap[pd]) //是障碍物
                        {
                            node.distance = float.MaxValue;
                            node.isObstacle = true;
                        }
                        else
                        {
                            node.distance = to;
                            openMap[pd] = true;
                            open.Enqueue(pd);
                            
                        }

                        
                    }
                    else //搜索到过但是用的别的路径
                    {
                        if (!obstacleMap[pd])
                        {
                            if (tileMap[pd].distance > to) //这条路径的距离值更短，更新
                            {
                                tileMap[pd].distance = to;
                                //open.Enqueue(pd); //需要再入队么？
                            }
                        }
                    }
                }
            }
        }


        public Dictionary<Vector2Int, TileNode> GenerateTileMap(Vector2Int target)
        {
            destination = target;
            if (tileMap == null)
            {
                tileMap = new Dictionary<Vector2Int, TileNode>();
            }
            if (!boundsInt.Contains2D(target))
            {
                return tileMap;
            }
            Profiler.BeginSample("grid_Clear");
            VectorFieldClear();
            Profiler.EndSample();
            
            Profiler.BeginSample("grid_BFS");
            //BFS
            BFSDistanceCalculate(target);
            Profiler.EndSample();

            
            Profiler.BeginSample("grid_generateData");
            //生成数据：minDistance
            foreach (var t in tileMap)
            {
                float temp = (t.Value.distance + 0.01f) / (maxDistence * 0.7f);
                t.Value.GradientColor = gradient.Evaluate(temp);

                float minAround = float.MaxValue;
                Vector2Int minDir = Vector2Int.zero;
                for (int i = 0; i < 8; i++)
                {
                    var pd = GetPosByDirection((Direction) i, t.Key);
                    if (tileMap.ContainsKey(pd))
                    {
                        if (t.Value.isObstacle)
                        {
                            tileMap[pd].isBesideObstacle = true;
                        }

                        if (!tileMap[pd].isObstacle && tileMap[pd].distance < minAround)
                        {
                            minAround = tileMap[pd].distance;
                            minDir = pd;
                        }
                    }
                }

                t.Value.minDistanceAround = minAround;
                var dir = (Vector2) (minDir - t.Key);
                dir.Normalize();
                t.Value.vector = new Vector3(dir.x, 0, dir.y);
            }
            Profiler.EndSample();


            Profiler.BeginSample("grid_generateVector");
            
            //生成向量场
            foreach (var t in tileMap)
            {
                if (t.Value.isObstacle)
                {
                    continue;
                } //自己是障碍物,跳过向量计算


                Vector2 v;

                if (t.Value.isBesideObstacle)
                {
                    v = new Vector2(t.Value.vector.x, t.Value.vector.z);

                    for (int i = 0; i < 8; i++)
                    {
                        var pd = GetPosByDirection((Direction) i, t.Key);
                        var dir = (Vector2) (pd - t.Key);

                        if (tileMap.ContainsKey(pd))
                        {
                            if (tileMap[pd].isObstacle)
                            {
                                v += dir * -0.2f;//参数根据需要可调整
                            }
                        }
                    }
                }
                else
                {
                    v = Vector2.zero;

                    for (int i = 0; i < 8; i++)
                    {
                        var pd = GetPosByDirection((Direction) i, t.Key);
                        var dir = (Vector2) (pd - t.Key);

                        if (tileMap.ContainsKey(pd))
                        {
                            float factor = 1f / (tileMap[pd].distance - t.Value.minDistanceAround + 1f);
                            if (tileMap[pd].isBesideObstacle) //这个方向上的单元格相邻障碍物，减少一部分权重
                            {
                                factor *= 0.6f;//参数根据需要可调整
                            }

                            v += dir * factor;
                        }
                    }
                }

                var result = v.normalized * 0.5f * cellSize;
                t.Value.vector = new Vector3(result.x, 0, result.y);
                //var result=t.Value.isBesideObstacle ? ((Vector2) (minDir - t.Key)).normalized : v.normalized;
                //var result = ((Vector2) (minDir - t.Key)).normalized;
            }
            Profiler.EndSample();

            return tileMap;
        }

        Vector2Int GetPosByDirection(Direction d, Vector2Int target)
        {
            int x = target.x;
            int y = target.y;

            switch (d)
            {
                case Direction.e:
                    return new Vector2Int(x + 1, y);
                case Direction.se:
                    return new Vector2Int(x + 1, y - 1);
                case Direction.s:
                    return new Vector2Int(x, y - 1);
                case Direction.sw:
                    return new Vector2Int(x - 1, y - 1);
                case Direction.w:
                    return new Vector2Int(x - 1, y);
                case Direction.nw:
                    return new Vector2Int(x - 1, y + 1);
                case Direction.n:
                    return new Vector2Int(x, y + 1);
                case Direction.ne:
                    return new Vector2Int(x + 1, y + 1);
            }

            return Vector2Int.zero;
        }

        enum Direction
        {
            e,
            se,
            s,
            sw,
            w,
            nw,
            n,
            ne
        }
    }

    private Grid mGrid;

    void Start()
    {
        mGrid = new Grid(cellSize, boundMin, boundMax, mGradient);
        mTileMap = mGrid.GenerateTileMap(Vector2Int.zero);

        var obstacleMap = mGrid.obstacleMap;
        foreach (var obstacle in obstacleMap)
        {
            if (obstacle.Value)
            {
                Vector3 pos = mGrid.cellToWorld(obstacle.Key);
                var go= Instantiate(this.obstacle, pos, Quaternion.identity);
                go.transform.localScale = Vector3.one * cellSize;
            }
        }
    }
    
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (mGrid.plane.Raycast(ray, out float enter))
        {
            var p = Camera.main.transform.position + ray.direction * enter;
            Profiler.BeginSample("UpdateVectorField");
            mTileMap = mGrid.UpdateVectorField(p);
            Profiler.EndSample();
            Debug.DrawRay(p,Vector3.up,Color.cyan);
        }
    }

    public Vector3 GetVector(Vector3 positon)
    {
        return mGrid.GetVector(positon);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && Application.isEditor)
        {
            if (mTileMap != null)
            {
                foreach (var f in mTileMap)
                {
                    //var pos = mGrid.cellToWorld(f.Key) + (Vector3.forward * cellSize / 2) + (Vector3.right * cellSize / 2);
                    var pos = mGrid.cellToWorld(f.Key);
                    if (f.Value.isObstacle)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(pos, new Vector3(cellSize, 0, cellSize));
                    }
                    else if (f.Key == mGrid.destination ) //target;
                    {
                        Vector3 vector3 = mGrid.cellToWorld(mGrid.destination);
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(vector3, cellSize / 4);
                    }
                    else
                    {
                        if (f.Value.isBesideObstacle)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0, cellSize));
                        }

                        Gizmos.color = f.Value.GradientColor;
                        DrawArrow.ForGizmo(pos, f.Value.vector);


                        Handles.Label(pos + Vector3.left * cellSize * 0.2f, f.Value.distance.ToString("0.0"));
                        Vector3 temp = Vector3.forward * cellSize * 0.25f;
                        Handles.Label(pos + temp + Vector3.left * cellSize * 0.3f, f.Key.ToString());
                    }
                }
            }

            Gizmos.color = Color.white;
            var b = mGrid.realBounds;
            Gizmos.DrawWireCube(b.center - Vector3.up * cellSize / 2, new Vector3(b.size.x, 0, b.size.z));
        }
    }
}

public static class BoundsIntExtra
{
    public static bool Contains2D(this BoundsInt boundsInt, Vector2Int target)
    {
        Vector3Int v = new Vector3Int(target.x, 0, target.y);
        return boundsInt.Contains(v);
    }
}