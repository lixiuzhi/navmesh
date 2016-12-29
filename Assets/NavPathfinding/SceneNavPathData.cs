using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneNavPathData : MonoBehaviour
{
    public TextAsset data; 
    public NavMeshInfo info; 
    public static SceneNavPathData Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        var jsonData = MyJson.Parse(data.text);
        info = new NavMeshInfo();
        List<Int3> listVec = new List<Int3>();

        foreach (var json in jsonData.asDict()["v"].AsList())
        {
            Int3 v3 = new Int3();
            v3.x = (int)(json.AsList()[0].AsInt());
            v3.y = (int)(json.AsList()[1].AsInt());
            v3.z = (int)(json.AsList()[2].AsInt());
            listVec.Add(v3);
        }
        info.vecs = listVec;

        List<NavNode> polys = new List<NavNode>();
        var list = jsonData.asDict()["p"].AsList();

        //顶点-》多边形 对应信息
        info.vertexPolys = new Dictionary<int, List<int>>();

        for (var i = 0; i < list.Count; i++)
        {
            var json = list[i].AsList();
            NavNode node = new NavNode();
            node.nodeID = i;
            List<int> poly = new List<int>();
            foreach (var tt in json)
            {
                var vertexIndex = tt.AsInt();
                poly.Add(vertexIndex);

                //保存顶点->多边形信息
                List<int> polyindexs = new List<int>();
                if (!info.vertexPolys.TryGetValue(vertexIndex, out polyindexs))
                {
                    polyindexs = new List<int>();
                    info.vertexPolys.Add(vertexIndex, polyindexs);
                }
                polyindexs.Add(i);
            }

            node.triangleVertexIndexs = poly.ToArray();
            node.GenBorder();//这里生成的border 是顶点border
            node.GenCenter(info);
           // node.GenLinked(info);
            polys.Add(node);
        }
        info.nodes = polys;
        //读取格子size
        info.cellSize = jsonData.asDict()["cs"].AsInt();
        //读取格子起点
        list = jsonData.asDict()["o"].AsList();
        info.origin = new Int3(list[0].AsInt(), list[1].AsInt(), list[2].AsInt());
        //读取格子多边形对应关系信息
        list = jsonData.asDict()["c"].AsList();
        info.cellPolys = new Dictionary<int, List<int>>();

        for (var i = 0; i < list.Count; i++)
        {
            var arr = list[i].AsList(); 
            var ps = new List<int>();
            info.cellPolys[arr[0].AsInt()] = ps;
            for (int j = 1; j < arr.Count; j++)
            { 
                ps.Add(arr[j].AsInt()); 
            }
        } 
        //info.CalcBound();
        info.GenBorder();
        for(int i=0;i<info.nodes.Count;i++)
            info.nodes[i].GenLinked(info); 
        info.GenFindNodeCache();
    }

    public void DrawPath(List<Int3> points)
    {
        GameObject path = new GameObject();
        path.name = "_path_debug_";
        var line = path.AddComponent<LineRenderer>();
        line.SetWidth(0.15f, 0.15f);
        Vector3[] vec3s = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vec3s[i] = points[i].vec3;
        } 
        line.SetVertexCount(vec3s.Length);
        line.SetPositions(vec3s);
    } 

    public List<Int3> FindPath(Vector3 start,Vector3 end)
    {
        return PathFinding.FindPath(this.info, (Int3)start,(Int3)end);
    }

    public Vector3 Move(Vector3 start, Vector3 delta)
    {
        return PathFinding.InternalMove(this.info,(Int3)start, (Int3)delta).vec3;
    }

    //得到高度 
    public float GetH(Int3 pos)
    {
        return PathFinding.GetH(info,pos);
    }
}
