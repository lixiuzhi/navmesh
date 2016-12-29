using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor; 

public class ExportNavmesh_Triangle : UnityEditor.EditorWindow
{
    [UnityEditor.MenuItem("customNav/window_triangle")]
    static void show()
    {
        UnityEditor.EditorWindow.GetWindow<ExportNavmesh_Triangle>().Show();
    }

    [Range(500, 100000)]
    int cellSize = 1000;
    void OnGUI()
    {
        GUILayout.Label("单位毫米");
        cellSize = System.Convert.ToInt32(GUILayout.TextArea(cellSize.ToString()));

        if (GUILayout.Button("gen navmesh_triangle JSON"))
        {
            string outstring = GenNavMesh();
            string outfile = Application.dataPath + "/Art/PathInfos/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".bytes";
            System.IO.File.WriteAllText(outfile, outstring);
            AssetDatabase.Refresh();
        }
    }


    public bool InPoly(Int3 p, List<Int3> verts)
    {
        if (verts.Count < 3) return false;

        if (PathMath.IsClockwise(verts[0], p, verts[1])) return false;
        if (PathMath.IsClockwise(verts[verts.Count - 1], p, verts[0])) return false;

        int i = 2, j = verts.Count - 1;
        int line = -1;

        while (i <= j)
        {
            int mid = (i + j) >> 1;
            if ((PathMath.IsClockwiseMargin(verts[0], p, verts[mid])))
            {
                line = mid;
                j = mid - 1;
            }
            else i = mid + 1;
        }
        return PathMath.IsClockwise(verts[line], p, verts[line - 1]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="style">0 json 1 obj</param>
    /// <returns></returns>
    string GenNavMesh()
    {
        NavMeshTriangulation navtri = UnityEngine.NavMesh.CalculateTriangulation();

         {
               var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
               var mf = obj.GetComponent<MeshFilter>();
               Mesh m = new Mesh();

               AssetDatabase.CreateAsset(m, "Assets/Art/PathInfos/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_mesh.asset");
               m.vertices = navtri.vertices;
               m.triangles = navtri.indices;
               mf.mesh = m;
           }  

        List<Int3> vertices = new List<Int3>(navtri.vertices.Length);
        for (int i = 0; i < navtri.vertices.Length; i++)
        {
            vertices.Add((Int3)navtri.vertices[i]);
        }

        Dictionary<int, int> indexmap = new Dictionary<int, int>();
        List<Int3> repos = new List<Int3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            int ito = -1;
            for (int j = 0; j < repos.Count; j++)
            {
                if (Int3.Dist(vertices[i], repos[j]) <=20)
                {
                    ito = j;
                    break;
                }
            }
            if (ito < 0)
            {
                indexmap[i] = repos.Count;
                repos.Add(vertices[i]);
            }
            else
            {
                indexmap[i] = ito;
            }
        }

        //关系是 index 公用的三角形表示他们共同组成多边形
        //多边形之间的连接用顶点位置识别 
        List<int[]> polys = new List<int[]>();
        for (int i = 0; i < navtri.indices.Length / 3; i++)
        {
            int i0 = navtri.indices[i * 3 + 0];
            int i1 = navtri.indices[i * 3 + 1];
            int i2 = navtri.indices[i * 3 + 2];
            polys.Add(new int[] { i0,i1,i2});
        }

        //生成网格多边形对应关心

        //找到格子起点
        var origin = GetOrigin(repos);
        //取得格子 多边形索引
        Dictionary<int, List<int>> cellPolys = new Dictionary<int, List<int>>();

        //1.求多边形外接长方形
        //2.外接长方形按照cellsize分割成列表
        //3.剔除掉列表中没有与多边形相交的长方形(长方形每个点是否在都多边形内部)
        //4.保存
        for (int i = 0; i < polys.Count; i++)
        {
            //求外接长方形 
            Int3 min = new Int3();
            Int3 max = new Int3();
            min.x = int.MaxValue;
            min.z = int.MaxValue;
            max.x = int.MinValue;
            max.z = int.MinValue;
            List<Int3> verts = new List<Int3>();
            for (int j = 0; j < polys[i].Length; j++)
            {
                Int3 vecs = repos[indexmap[polys[i][j]]];

                verts.Add(vecs);

                if (vecs.x < min.x) min.x = vecs.x;
                if (vecs.y < min.y) min.y = vecs.y;
                if (vecs.z < min.z) min.z = vecs.z;
                if (vecs.x > max.x) max.x = vecs.x;
                if (vecs.y > max.y) max.y = vecs.y;
                if (vecs.z > max.z) max.z = vecs.z;
            }

            //格式化长方形
            min -= origin;
            max -= origin;
            min.x = origin.x + cellSize * (min.x / cellSize);
            min.z = origin.z + cellSize * (min.z / cellSize);

            max.x = origin.x + cellSize * (1 + max.x / cellSize);
            max.z = origin.z + cellSize * (1 + max.z / cellSize);

            //删除不相交的格子 
            int m = (max.x - min.x) / cellSize;
            int n = (max.z - min.z) / cellSize;
            for (int m1 = 0; m1 < m; m1++)
            {
                for (int n1 = 0; n1 < n; n1++)
                {
                    //判断四边形是否与多边形相交
                    Int3 v1 = new Int3 { x = min.x + cellSize * m1, y = 0, z = min.z + cellSize * n1 };
                    Int3 v2 = new Int3 { x = min.x + cellSize * (m1 + 1), y = 0, z = min.z + cellSize * n1 };
                    Int3 v3 = new Int3 { x = min.x + cellSize * (m1 + 1), y = 0, z = min.z + cellSize * (n1 + 1) };
                    Int3 v4 = new Int3 { x = min.x + cellSize * m1, y = 0, z = min.z + cellSize * (n1 + 1) };

                    //判断方式 4变形任何一点不在多变形内,且多变形任意一点不在四边形内
                    List<Int3> list = new List<Int3>();
                    list.Add(v1);
                    list.Add(v2);
                    list.Add(v3);
                    list.Add(v4);
                    bool inside = false;
                    for (int q = 0; q < verts.Count; q++)
                    {
                        inside = InPoly(verts[q], list);
                        if (inside)
                            break;
                    }

                    //判断边是否相交
                    if (!inside)
                    {
                        //得到每两条边交点，然后判断是否在两条边内部
                        for (int q = 0; q < verts.Count; q++)
                        { 
                            var ve1 = verts[q];
                            var ve2 = verts[(q + 1) % verts.Count];

                            for (int g = 0; g < list.Count; g++)
                            {
                                var ve3 = list[g];
                                var ve4 = list[(g + 1) % list.Count];

                                bool intersects = false;
                                PathMath.IntersectionPoint(ref ve1, ref ve2,ref  ve2, ref ve4, out intersects);
                                if (intersects)
                                {
                                    inside = true;
                                    break;
                                }
                            } 

                            if (inside)
                                break;
                        }
                    }

                    if (inside || InPoly(v1, verts) || InPoly(v2, verts) || InPoly(v3, verts) || InPoly(v4, verts))
                    {
                        //计算哈希值
                        int hash = 100000000 + (m1 + (min.x - origin.x) / cellSize) * 10000 + (n1 + (min.z - origin.z) / cellSize);
                        //添加索引
                        List<int> poly = null;
                        if (!cellPolys.TryGetValue(hash, out poly))
                        {
                            poly = new List<int>();
                            cellPolys[hash] = poly;
                        }
                        if (!poly.Exists((int iv) => { return iv == i; }))
                        {
                            poly.Add(i);
                        }
                    }
                }
            }
        }

        string outnav = "";

        outnav = "{";
        //写入cell size
        outnav += "\"cs\":" + cellSize + ",\n";
        //写入起点
        outnav += "\"o\":[" + origin.x + "," + origin.y + "," + origin.z + "],\n";

        //写入顶点
        outnav += "\"v\":[\n";
        for (int i = 0; i < repos.Count; i++)
        {
            if (i > 0)
                outnav += ",\n";

            outnav += "[" + repos[i].x + "," + repos[i].y + "," + repos[i].z + "]";
        }
        outnav += "\n],\"p\":[\n";
        //写入多边形index
        for (int i = 0; i < polys.Count; i++)
        {
            string outs = indexmap[polys[i][0]].ToString();
            for (int j = 1; j < polys[i].Length; j++)
            {
                outs += "," + indexmap[polys[i][j]];
            }

            if (i > 0)
                outnav += ",\n";

            outnav += "[" + outs + "]";
        }

        //写入cell区域index
        outnav += "\n],\"c\":[\n";
        int i11 = 0;
        foreach (var c in cellPolys)
        {
            string s = c.Key.ToString();
            for (int j = 0; j < c.Value.Count; j++)
            {
                s += "," + c.Value[j].ToString();
            }
            if (i11 > 0)
                outnav += ",\n";

            outnav += "[" + s + "]";
            i11++;
        }

        outnav += "\n]}";
        return outnav;
    }

    //得到起点
    public Int3 GetOrigin(List<Int3> vertexs)
    {
        var origin = new Int3();
        origin.x = int.MaxValue;
        origin.y = int.MaxValue;
        origin.z = int.MaxValue;

        for (var i = 0; i < vertexs.Count; i++)
        {
            if (vertexs[i].x < origin.x) origin.x = vertexs[i].x;
            if (vertexs[i].y < origin.y) origin.y = vertexs[i].y;
            if (vertexs[i].z < origin.z) origin.z = vertexs[i].z;
        }
        return origin;
    }
}
