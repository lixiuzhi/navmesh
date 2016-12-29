
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class NavNode
{
    public int nodeID;
    public int[] triangleVertexIndexs;    //多边形
    public List<int> links;  
    public int[] borderByTriangle;      //形状对应的边
    public int[] borderByPoint;         //节点对应的边 
    public Int3 center; 
    //顶点最低的y值
    public int minH = int.MaxValue;

    public bool ContainsPoint(NavMeshInfo info, Int3 p)
    { 
        Int3 a = info.vecs[triangleVertexIndexs[0]];
        Int3 b = info.vecs[triangleVertexIndexs[1]];
        Int3 c = info.vecs[triangleVertexIndexs[2]];

        if ((long)(b.x - a.x) * (long)(p.z - a.z) - (long)(p.x - a.x) * (long)(b.z - a.z) > 0) return false; 
        if ((long)(c.x - b.x) * (long)(p.z - b.z) - (long)(p.x - b.x) * (long)(c.z - b.z) > 0) return false; 
        if ((long)(a.x - c.x) * (long)(p.z - c.z) - (long)(p.x - c.x) * (long)(a.z - c.z) > 0) return false;

        return true;
    }
    public int GetVertexIndex(int i)
    {
        return i == 0 ? triangleVertexIndexs[0] : (i == 1 ? triangleVertexIndexs[1] : triangleVertexIndexs[2]);
    }

    public NavNode GetNeighborByEdge(NavMeshInfo info, int edge, out int otherEdge)
    { 
        otherEdge = -1;
        if (edge < 0 || edge > 2)
        {
            return null;
        }
        var vertex = triangleVertexIndexs[edge % 3];// this.GetVertex(info,edge % 3);
        var vertex2 = triangleVertexIndexs[(edge + 1) % 3];// this.GetVertex(info,(edge + 1) % 3);

        NavNode result = null;

        for (int i = 0; i < borderByTriangle.Length; i++)
        {
            var border = info.borders[borderByTriangle[i]];
            NavNode triangleMeshNode1 = info.nodes[border.nodeA];
            NavNode triangleMeshNode2 = info.nodes[border.nodeB];
            NavNode triangleMeshNode = triangleMeshNode1 == this ? triangleMeshNode2 : triangleMeshNode1;

            if (triangleMeshNode.triangleVertexIndexs[1] == vertex && triangleMeshNode.triangleVertexIndexs[0] == vertex2)
            {
                otherEdge = 0;
            }
            else if (triangleMeshNode.triangleVertexIndexs[2] == vertex && triangleMeshNode.triangleVertexIndexs[1] == vertex2)
            {
                otherEdge = 1;
            }
            else if (triangleMeshNode.triangleVertexIndexs[0] == vertex && triangleMeshNode.triangleVertexIndexs[2] == vertex2)
            {
                otherEdge = 2;
            }
            if (otherEdge != -1)
            {
                result = triangleMeshNode;
                break;
            }
        }
        return result;
    }

    public int EdgeIntersect(NavMeshInfo info,Int3 a, Int3 b)
    {
        Int3 vInt = GetVertex(info,0);
        Int3 vInt2 = GetVertex(info, 1);
        Int3 vInt3 = GetVertex(info,2); 

        if (PathMath.Intersects(vInt, vInt2, a, b))
        {
            return 0;
        }
        if (PathMath.Intersects(vInt2, vInt3, a, b))
        {
            return 1;
        }
        if (PathMath.Intersects(vInt3, vInt, a, b))
        {
            return 2;
        }
        return -1;
    }

    static Int3[] staticVerts = new Int3[3];
    public int EdgeIntersect(NavMeshInfo info, Int3 a, Int3 b, int startEdge, int count)
    {
        staticVerts[0]= GetVertex(info, 0);
        staticVerts[1] = GetVertex(info, 1);
        staticVerts[2] = GetVertex(info,2);
         
        for (int i = 0; i < count; i++)
        {
            int num = (startEdge + i) % 3;
            int num2 = (num + 1) % 3;
            if (PathMath.Intersects(staticVerts[num], staticVerts[num2], a, b))
            {
                return num;
            }
        }
        return -1;
    } 

    public int GetColinearEdge(NavMeshInfo info, Int3 a, Int3 b)
    {
        Int3 vInt = GetVertex(info, 0);
        Int3 vInt2 = GetVertex(info, 1);
        Int3 vInt3 = GetVertex(info, 2);

        if (PathMath.IsColinear(vInt, vInt2, a) && PathMath.IsColinear(vInt, vInt2, b))
        {
            return 0;
        }
        if (PathMath.IsColinear(vInt2, vInt3, a) && PathMath.IsColinear(vInt2, vInt3, b))
        {
            return 1;
        }
        if (PathMath.IsColinear(vInt3, vInt, a) && PathMath.IsColinear(vInt3, vInt, b))
        {
            return 2;
        }
        return -1;
    }

    public Int3 GetVertex(NavMeshInfo info, int i)
    {
        return info.vecs[triangleVertexIndexs[i]];
    }
       
    public void GenBorder()
    {
        List<int> _bb = new List<int>();
        for (int i = 0; i < triangleVertexIndexs.Length; i++)
        {
            int i0 = i;
            int i1 = i + 1;
            if (i1 >= triangleVertexIndexs.Length) i1 = 0;

            int b1 = triangleVertexIndexs[i0];
            int b2 = triangleVertexIndexs[i1];
            if (b1 < b2)
                _bb.Add(100000000+  b1 *10000 + b2);
            else
                _bb.Add(100000000+  b2 *10000 + b1);

        }
        this.borderByPoint = _bb.ToArray();
    }
     

    //判断两个节点是否相连
    public int IsLinkTo(NavMeshInfo info, int nid)
    {
        if (nodeID == nid)
            return -1;
        if (nid < 0)
            return -1;
       // foreach (var b in borderByPoly)
       for(int i=0;i< borderByTriangle.Length;i++)
        {
            var b = borderByTriangle[i];
            if (info.borders.ContainsKey(b) == false)
                continue;
            if (info.borders[b].nodeA == nid || info.borders[b].nodeB == nid)
                return b;
        }
        return -1;
    }

   
    public bool IsVertex(NavMeshInfo info,Int3 vet,out int id)
    {
        for(int i=0;i< triangleVertexIndexs.Length;i++)
        {
            if (info.vecs[triangleVertexIndexs[i]] == vet)
            {
                id = triangleVertexIndexs[i];
                return true;
            } 
        }
        id = -1;
        return false;
    } 

    //获取连接节点
    public void GenLinked(NavMeshInfo info)
    {
        links = ListPool<int>.Claim();// new List<int>();
        for (int i = 0; i < borderByTriangle.Length; i++)
        {
            var b = borderByTriangle[i];
            int onode = -1;
            if (info.borders.ContainsKey(b) == false)
                continue;

            if (info.borders[b].nodeA == this.nodeID)
                onode = info.borders[b].nodeB;
            else
                onode = info.borders[b].nodeA;
            if (onode >= 0)
                links.Add(onode);
        } 
    }
    public void GenCenter(NavMeshInfo info)
    {
        this.center = new Int3();
        this.center.x = 0;
        this.center.y = 0;
        this.center.z = 0;
 
        for (int i = 0; i < triangleVertexIndexs.Length; i++)
        {
            //UPDATE__
            var vec3 = info.vecs[triangleVertexIndexs[i]];
            this.center.x += vec3.x;
            this.center.y += vec3.y;
            this.center.z += vec3.z;
            if (vec3.y < minH)
            {
                minH = vec3.y;
            }
        }
        this.center = IntMath.Divide(center, triangleVertexIndexs.Length);// (Int3)(center.vec3/poly.Length);
    }
}

public struct TMNodeInfo
{
    public NavNode node;

    public int vi;

    public Int3 v0;

    public Int3 v1;

    public Int3 v2;

    public IntFactor GetCosineAngle(Int3 dest, out int edgeIndex)
    {
        Int3 vInt = this.v1 - this.v0;
        Int3 vInt2 = this.v2 - this.v0;
        Int3 vInt3 = dest - this.v0;
        vInt3.NormalizeTo(1000);
        vInt.NormalizeTo(1000);
        vInt2.NormalizeTo(1000);
        long num = Int3.DotXZLong(ref vInt3, ref vInt);
        long num2 = Int3.DotXZLong(ref vInt3, ref vInt2);
        IntFactor result = default(IntFactor);
        result.denominator = 1000000L;
        if (num > num2)
        {
            edgeIndex = this.vi;
            result.numerator = num;
        }
        else
        {
            edgeIndex = (this.vi + 2) % 3;
            result.numerator = num2;
        }
        return result;
    }
}

[System.Serializable]
public class NavBorder
    {
        public int borderName;
        public int nodeA=-1;//边 对应的 节点
        public int nodeB=-1;//边 对应的 节点

        public int pointA;
        public int pointB;

        public long length;
        public Int3 center;
    }

[System.Serializable]
public class NavMeshInfo
{ 
    public Int3 origin;                                 //格子起点
    public int cellSize = 1000;                         //格子size,单位厘米 
    public List<Int3> vecs;                             //顶点
    public List<NavNode> nodes;                         //节点
    public Dictionary<int, NavBorder> borders;          //边 
    public Dictionary<int, List<int>> cellPolys;        //格子包含的多边形index信息 
    public Dictionary<int, List<int>> vertexPolys;      //包含顶点的所有多边形信息 
    public List<FindNode> nodeFindCache;

    //得到顶点所有邻边
    public void GetAllNodesByVert(ref List<TMNodeInfo> nodeInfos, NavNode startNode, int vertIndex)
    {
        if (nodeInfos == null)
        {
            nodeInfos = ListPool<TMNodeInfo>.Claim();
        }
        for (int i = 0; i < nodeInfos.Count; i++)
        {
            if (nodeInfos[i].node == startNode)
            {
                return;
            }
        }
        int num;
        if (startNode.triangleVertexIndexs[0] == vertIndex)
        {
            num = 0;
        }
        else if (startNode.triangleVertexIndexs[1] == vertIndex)
        {
            num = 1;
        }
        else
        {
            if (startNode.triangleVertexIndexs[2] != vertIndex)
            {
                return;
            }
            num = 2;
        }
        TMNodeInfo tMNodeInfo = default(TMNodeInfo);
        tMNodeInfo.vi = num;
        tMNodeInfo.node = startNode;
        tMNodeInfo.v0 = startNode.GetVertex(this, num % 3);
        tMNodeInfo.v1 = startNode.GetVertex(this, (num + 1) % 3);
        tMNodeInfo.v2 = startNode.GetVertex(this, (num + 2) % 3);
        nodeInfos.Add(tMNodeInfo);

        for (int j = 0; j < startNode.borderByTriangle.Length; j++)
        {
            var triangleMeshNode =  this.nodes[ this.borders[startNode.borderByTriangle[j]].nodeB];
            if (triangleMeshNode != null)
            {
                GetAllNodesByVert(ref nodeInfos, triangleMeshNode, vertIndex);
            }
        }
    } 

    /*public bool InPoly(Int3 p, int[] poly)
    {
        if (poly.Length < 3) return false;

        if (Polygon.IsClockwise(this.vecs[poly[0]], p, this.vecs[poly[1]])) return false;
        if (Polygon.IsClockwise(this.vecs[poly[poly.Length - 1]], p, this.vecs[poly[0]])) return false;

        int i = 2, j = poly.Length - 1;
        int line = -1;

        while (i <= j)
        {
            int mid = (i + j) >> 1;
            if ((Polygon.IsClockwiseMargin(this.vecs[poly[0]], p, this.vecs[poly[mid]])))
            {
                line = mid;
                j = mid - 1;
            }
            else i = mid + 1;
        }
        return Polygon.IsClockwise(this.vecs[poly[line]], p, this.vecs[poly[line - 1]]);
    }*/



    //是否顺时针，包括在边上的情况
    public static bool IsClockwiseMargin(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) <= 0L;
    }
 
     //是否在三角形内部(包括边上)
    public bool InTriangle(Int3 p,int i1,int i2,int i3)
    {  //IsClockwiseMargin
        if (PathMath.IsClockwise(this.vecs[i1], p, this.vecs[i2])) return false;
        if (PathMath.IsClockwise(this.vecs[i2], p, this.vecs[i3])) return false;
        if (PathMath.IsClockwise(this.vecs[i3], p, this.vecs[i1])) return false;
        return true; 
    }
    
    public void GenFindNodeCache()
    {
        nodeFindCache = new List<FindNode>(nodes.Count); 
        for (int i = 0; i < nodes.Count; i++)
        {
            var nn = new FindNode();
            nn.nodeid = nodes[i].nodeID;
            nodeFindCache.Add(nn);
        }
    }

    public void ReInitFindNodeCache()
    {
        for (int i = 0; i < nodeFindCache.Count; i++)
        {
            var n = nodeFindCache[i];
            n.nodeid = nodes[i].nodeID;
            n.pathSessionId = 0;
            n.ParentID = -1;
            n.Open = false;
            n.HValue = 0;
            n.GValue = 0;
            n.ArrivalWall = 0;
        }
    }

    public void GenBorder()
    {
        var __borders = new Dictionary<int, NavBorder>();
        foreach (var n in nodes)
        {
            foreach (var b in n.borderByPoint)
            {
                if (__borders.ContainsKey(b) == false)
                {
                    __borders[b] = new NavBorder();
                    __borders[b].borderName = b;
                    __borders[b].nodeA = n.nodeID;
                    __borders[b].nodeB = -1;
                    __borders[b].pointA = -1; 
                }
                else
                { 
                    __borders[b].nodeB = n.nodeID;
                    if (__borders[b].nodeA > __borders[b].nodeB)
                    {
                        __borders[b].nodeB = __borders[b].nodeA;
                        __borders[b].nodeA = n.nodeID;
                    }
                    var na = this.nodes[__borders[b].nodeA];
                    var nb = this.nodes[__borders[b].nodeB];
                    foreach (int i in na.triangleVertexIndexs)
                    {
                        if (nb.triangleVertexIndexs.Contains(i))
                        {
                            if (__borders[b].pointA == -1)

                                __borders[b].pointA = i;
                            else
                                __borders[b].pointB = i;
                        }
                    }
                    var left = __borders[b].pointA;
                    var right = __borders[b].pointB;

                    long xd = this.vecs[left].x - this.vecs[right].x;
                    long yd = this.vecs[left].y - this.vecs[right].y;
                    long zd = this.vecs[left].z - this.vecs[right].z;
                     
                   // __borders[b].length = IntMath.SqrtLong(xd * xd + yd * yd + zd * zd);// IntMath.Divide(IntMath.Sqrt(xd * xd + yd * yd + zd * zd), 1000);
                    __borders[b].center = Int3.zero;
                    __borders[b].center.x = (this.vecs[left].x + this.vecs[right].x) / 2;
                    __borders[b].center.y = (this.vecs[left].y + this.vecs[right].y) / 2;
                    __borders[b].center.z = (this.vecs[left].z + this.vecs[right].z) / 2;
                    __borders[b].borderName =100000000+ 10000* __borders[b].nodeA + __borders[b].nodeB;
                }

            }

        }
        Dictionary<int, int> namechange = new Dictionary<int, int>();
        foreach (var b in __borders.Keys.ToArray())
        {
            if (__borders[b].nodeB < 0)
            {
            }
            else
            {
                namechange[b] = __borders[b].borderName;
            }
        }
        this.borders = new Dictionary<int, NavBorder>();
        foreach (var b in __borders)
        {
            if (namechange.ContainsKey(b.Key))
            {
                this.borders[namechange[b.Key]] = b.Value;
            }
        }

        foreach (var v in nodes)
        {
            List<int> newborder = new List<int>();
            foreach (var b in v.borderByPoint)
            {
                if (namechange.ContainsKey(b))
                {
                    newborder.Add(namechange[b]);
                }
            }
            v.borderByTriangle = newborder.ToArray();
        }
    }
}