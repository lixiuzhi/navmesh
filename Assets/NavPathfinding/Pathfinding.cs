
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//v0.1
//http://blog.csdn.net/ynnmnm/article/details/44833007
public class FindNode
{
    public int nodeid;
    public int pathSessionId;
    public int ParentID = -1;
    public bool Open;
    public long HValue;   //H评估值
    public long GValue;   //G评估值
    public int ArrivalWall;
    /// <summary>
    /// 计算三角形估价函数（h值）
    /// 使用该三角形的中心点（3个顶点的平均值）到路径终点的x和y方向的距离。
    /// </summary>
    /// <param name="endPos">终点</param>
    public void CalcHeuristic(NavMeshInfo info, Int3 endPos)
    {
        var c = info.nodes[this.nodeid].center;
        long xDelta = Math.Abs(c.x - endPos.x);
        long zDelta = Math.Abs(c.z - endPos.z);
        HValue =  IntMath.Sqrt(xDelta * xDelta + zDelta * zDelta); //xDelta* xDelta +zDelta * zDelta;
    }

    public long GetCost(NavMeshInfo info, int neighborID)
    {
        var bc = info.nodes[neighborID].center;
        var nc = info.nodes[this.nodeid].center;

        long xd = bc.x - nc.x;
        long yd = bc.y - nc.y;
        long zd = bc.z - nc.z;
        var d = IntMath.Sqrt(xd * xd + yd * yd + zd * zd); //xd* xd +yd * yd + zd * zd;
        return d;
    }
}

public static class PathMath
{   
    //是否顺时针，包括在边上的情况
    public static bool IsClockwiseMargin(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) <= 0L;
    }
    public static bool Intersects(Int3 a, Int3 b, Int3 a2, Int3 b2)
    {
        return Left(a, b, a2) != Left(a, b, b2) && Left(a2, b2, a) != Left(a2, b2, b);
    }
    public static Int3 IntersectionPoint(ref Int3 start1, ref Int3 end1, ref Int3 start2, ref Int3 end2, out bool intersects)
    {
        Int3 a = end1 - start1;
        Int3 vInt = end2 - start2;
        long num = (long)vInt.z * (long)a.x - (long)vInt.x * (long)a.z;
        if (num == 0L)
        {
            intersects = false;
            return start1;
        }
        long m = (long)vInt.x * ((long)start1.z - (long)start2.z) - (long)vInt.z * ((long)start1.x - (long)start2.x);
        intersects = true;
        Int3 lhs = IntMath.Divide(a, m, num);
        return lhs + start1;
    }
    public static bool IsColinear(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) == 0L;
    }

    public static bool Left(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) <= 0L;
    }
    public static bool LeftNotColinear(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) < 0L;
    }
    public static bool IsClockwise(Int3 a, Int3 b, Int3 c)
    {
        return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) < 0L;
    }
    public static bool InTriangle( Int3 v1, Int3 v2, Int3 v3, Int3 p)
    {  //IsClockwiseMargin
        if (IsClockwise(v1, p,v2)) return false;
        if (IsClockwise(v2, p, v3)) return false;
        if (IsClockwise(v3, p, v1)) return false;
        return true;
    }
}

public class PathFinding
{
    /// <summary>
    /// 得到位置所在三角形index 
    /// 生成数据的时候用格子优化
    /// </summary> 
    ///  UPDATE__
    public static int GetPolyIndexByPos(NavMeshInfo info, Int3 pos)
      {
          //计算所在格子
          Int3 vec3 = pos - info.origin;
          int x = vec3.x / info.cellSize;
          int z = vec3.z / info.cellSize;

          int hash = 100000000 + x * 10000 + z;

          List<int> nodes = null;
          int ret = -1;
          int offset = int.MaxValue;
          int it = 0;
          if (info.cellPolys.TryGetValue(hash, out nodes))
          {
              for (int i = 0; i < nodes.Count; i++)
              {
                  var v = info.nodes[nodes[i]]; 
                  if(info.InTriangle(pos, v.triangleVertexIndexs[0], v.triangleVertexIndexs[1], v.triangleVertexIndexs[2]))
                  {
                      it = pos.y - v.minH;
                      if ((ret == -1 || it >= 0) && it < offset)
                      {
                          ret = nodes[i];
                          offset = it;
                      }
                  }
              }
          }
          return ret;
      }


    /*  public static int GetPolyIndexByPos(NavMeshInfo info, Int3 pos)
    {
        for (int p = 0; p < info.nodes.Count; p++)
        {
            var ti = info.nodes[p].triangleVertexIndexs;
            if ( Polygon.ContainsPoint(info.vecs[ti[0]], info.vecs[ti[1]], info.vecs[ti[2]], pos))
            {
                return p;
            }
        }
        return -1;
    } */ 
     
    public static NavNode GetPolyIndexByPos(NavMeshInfo info, Int3 start,Int3 end, out int edge)
    {
        edge = -1;
       int id = GetPolyIndexByPos(info, start);
        if (id == -1)
        { 
            return null;
        }
        var node = info.nodes[id];
        long num2 = 9223372036854775807L;
        for (int j = 0; j < 3; j++)
        {
            int num3 = j;
            int num4 = (j + 1) % 3;
            Int3 v3 = node.GetVertex(info, num3);
            Int3 v4 = node.GetVertex(info, num4);
            if (PathMath.Intersects(v3, v4, start, end))
            {
                bool flag;
                Int3 vInt = PathMath.IntersectionPoint(ref v3, ref v4, ref start, ref end, out flag);

                long num5 = start.XZSqrMagnitude(ref vInt);
                if (num5 < num2)
                {
                    num2 = num5; 
                    edge = j;
                }
            }
        } 
        return node;
    }
     
    public static float GetH(NavMeshInfo info, Int3 pos)
    {
        int polyIndex = GetPolyIndexByPos(info, pos);
        if (polyIndex == -1)
            return pos.y * 1f / Int3.Precision;
         
        var node = info.nodes[polyIndex];

        //遍历多边形内部的三角形，判断在那个三角内部,然后求出点
        for (int i = 2, j = 0; i < node.triangleVertexIndexs.Length; i++, j += 3)
        {
            if (info.InTriangle(pos, node.triangleVertexIndexs[0], node.triangleVertexIndexs[i - 1], node.triangleVertexIndexs[i]))
            {
                //AStarDebug.DrawTriangle(0, info.vecs[node.poly[0]].vec3, info.vecs[node.poly[i - 1]].vec3, info.vecs[node.poly[i]].vec3);
                return GetTriangleY(info.vecs[node.triangleVertexIndexs[0]].vec3, info.vecs[node.triangleVertexIndexs[i - 1]].vec3, info.vecs[node.triangleVertexIndexs[i]].vec3, pos.x * 0.001f, pos.z * 0.001f);
            }
        }
        return 0;
    }
     
    public static float GetTriangleY(Vector3 p, Vector3 p2, Vector3 p3, float x4, float z4)
    {
        float num = (p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z);
        float num2 = 1f / num;
        float num3 = (p2.z - p3.z) * (x4 - p3.x) + (p3.x - p2.x) * (z4 - p3.z);
        num3 *= num2;
        float num4 = (p3.z - p.z) * (x4 - p3.x) + (p.x - p3.x) * (z4 - p3.z);
        num4 *= num2;
        float num5 = 1f - num3 - num4;
        return num3 * p.y + num4 * p2.y + num5 * p3.y;
    }

    private static void getMinMax(out int min, out int max, long axis, ref IntFactor factor)
    {
        long num = axis * factor.numerator;
        int num2 = (int)(num / factor.denominator);
        if (num < 0L)
        {
            min = num2 - 1;
            max = num2;
        }
        else
        {
            min = num2;
            max = num2 + 1;
        }
    }

    private static bool makePointInTriangle(NavMeshInfo info, ref Int3 result, NavNode node, int minX, int maxX, int minZ, int maxZ, Int3 offset)
    {
        Int3 vInt = node.GetVertex(info,0);
        Int3 vInt2 = node.GetVertex(info, 1);
        Int3 vInt3 = node.GetVertex(info, 2);

        long num = (long)(vInt2.x - vInt.x);
        long num2 = (long)(vInt3.x - vInt2.x);
        long num3 = (long)(vInt.x - vInt3.x);
        long num4 = (long)(vInt2.z - vInt.z);
        long num5 = (long)(vInt3.z - vInt2.z);
        long num6 = (long)(vInt.z - vInt3.z);
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minZ; j <= maxZ; j++)
            {
                int num7 = i + offset.x;
                int num8 = j + offset.z;
                if (num * (long)(num8 - vInt.z) - (long)(num7 - vInt.x) * num4 <= 0L && num2 * (long)(num8 - vInt2.z) - (long)(num7 - vInt2.x) * num5 <= 0L && num3 * (long)(num8 - vInt3.z) - (long)(num7 - vInt3.x) * num6 <= 0L)
                {
                    result.x = num7;
                    result.z = num8;
                    return true;
                }
            }
        }
        return false;
    }
    private static void moveAlongEdge(NavMeshInfo info, NavNode node, int edge, Int3 srcLoc, Int3 destLoc, out Int3 result, bool checkAnotherEdge = true)
    {
        Int3 vertex = node.GetVertex(info,edge);
        Int3 vertex2 = node.GetVertex(info, (edge + 1) % 3);
        Int3 vInt = destLoc - srcLoc;
        vInt.y = 0;
        Int3 vInt2 = vertex2 - vertex;
        vInt2.y = 0;
        vInt2.NormalizeTo(1000);
        int num;

        num = vInt2.x * vInt.x + vInt2.z * vInt.z;

        bool flag;
        Int3 rhs = PathMath.IntersectionPoint(ref vertex, ref vertex2, ref srcLoc, ref destLoc, out flag);
        if (!flag)
        {
            if (!PathMath.IsColinear(vertex, vertex2, srcLoc) || !PathMath.IsColinear(vertex, vertex2, destLoc))
            {
                result = srcLoc;
                return;
            }
            if (num >= 0)
            {
                int num2 = vInt2.x * (vertex2.x - vertex.x) + vInt2.z * (vertex2.z - vertex.z);
                int num3 = vInt2.x * (destLoc.x - vertex.x) + vInt2.z * (destLoc.z - vertex.z);
                rhs = ((num2 <= num3) ? vertex2 : destLoc);
            }
            else
            {
                int num4 = -vInt2.x * (vertex.x - vertex2.x) - vInt2.z * (vertex.z - vertex2.z);
                int num5 = -vInt2.x * (destLoc.x - vertex2.x) - vInt2.z * (destLoc.z - vertex2.z);
                rhs = ((Mathf.Abs(num4) <= Mathf.Abs(num5)) ? vertex : destLoc);
            }
        }
        int num6 = -IntMath.Sqrt(vertex.XZSqrMagnitude(rhs) * 1000000L);
        int num7 = IntMath.Sqrt(vertex2.XZSqrMagnitude(rhs) * 1000000L);
        if (num >= num6 && num <= num7)
        {
            result = IntMath.Divide(vInt2, (long)num, 1000000L) + rhs;
            if (!node.ContainsPoint(info,result))
            {
                Vector3 vector = (Vector3)(vertex2 - vertex);
                vector.y = 0f;
                vector.Normalize();
                Int3 lhs = vertex2 - vertex;
                lhs.y = 0;
                lhs *= 10000;
                long num8 = (long)lhs.magnitude;
                IntFactor vFactor = default(IntFactor);
                vFactor.numerator = (long)num;
                vFactor.denominator = num8 * 1000L;
                int num9;
                int num10;
                getMinMax(out num9, out num10, (long)lhs.x, ref vFactor);
                int num11;
                int num12;
                getMinMax(out num11, out num12, (long)lhs.z, ref vFactor);
                if (!makePointInTriangle(info,ref result, node, num9, num10, num11, num12, srcLoc) && !makePointInTriangle(info,ref result, node, num9 - 4, num10 + 4, num11 - 4, num12 + 4, srcLoc))
                {
                    result = srcLoc;
                }
            } 
        }
        else
        {
            int rhs2;
            int edge2;
            Int3 vInt4;
            if (num < num6)
            {
                rhs2 = num - num6;
                edge2 = (edge + 2) % 3;
                vInt4 = vertex;
            }
            else
            {
                rhs2 = num - num7;
                edge2 = (edge + 1) % 3;
                vInt4 = vertex2;
            }
            Int3 vInt5 = vInt2 * rhs2 / 1000000f;
            int startEdge;
            NavNode neighborByEdge = node.GetNeighborByEdge(info,edge2, out startEdge);
            if (neighborByEdge != null)
            {
                checkedNodes.Add(node);
                MoveFromNode(info,neighborByEdge, startEdge, vInt4, vInt5 + vInt4, out result);
            }
            else
            {
                if (checkAnotherEdge)
                {
                    Int3 vertex3 = node.GetVertex(info,(edge + 2) % 3);
                    Int3 lhs2 = (vertex3 - vInt4).NormalizeTo(1000);
                    if (Int3.Dot(lhs2, vInt5) > 0)
                    {
                        checkedNodes.Add(node);
                        moveAlongEdge(info,node, edge2, vInt4, vInt5 + vInt4, out result, false);
                        return;
                    }
                }
                result = vInt4;
            }
        }
    }


    private static List<NavNode> checkedNodes = new List<NavNode>();

    public static Int3 InternalMove(NavMeshInfo info, Int3 srcLoc, Int3 delta)
    {   
        Int3 vInt = srcLoc + delta;  
        int startEdge = -1;
        int id = GetPolyIndexByPos(info,srcLoc);
        if (id == -1)
        {
            return Int3.zero;
        }

        NavNode triangleMeshNode = info.nodes[id];
        if (triangleMeshNode == null)
        {
            NavNode triangleMeshNode2 = GetPolyIndexByPos(info, srcLoc, vInt,out startEdge);
            if (triangleMeshNode2 == null)
            {
                return Int3.zero;
            }
            triangleMeshNode = triangleMeshNode2;
        }
        Int3 lhs;

        MoveFromNode(info,triangleMeshNode, startEdge, srcLoc, vInt, out lhs);

        checkedNodes.Clear(); 
        return lhs - srcLoc;
    }

    private static void MoveFromNode(NavMeshInfo info,NavNode node,int startEdge, Int3 srcLoc, Int3 destLoc, out Int3 result)
    {
        result = srcLoc;
        while (node != null)
        {
            int count = 2;
            int i;
            if (node.IsVertex(info, srcLoc, out i))  //如果是顶点
            {
                int vertexIndex = node.GetVertexIndex(i);

                List<TMNodeInfo> nodeInfos = null;
                info.GetAllNodesByVert(ref nodeInfos, node, vertexIndex);
                NavNode triangleMeshNode = null;
                int num = -1;
                for (int j = 0; j < nodeInfos.Count; j++) //找接下来穿出多边形
                {
                    var tMNodeInfo = nodeInfos[j];
                    if (!checkedNodes.Contains(tMNodeInfo.node) && !PathMath.LeftNotColinear(tMNodeInfo.v0, tMNodeInfo.v2, destLoc) && PathMath.Left(tMNodeInfo.v0, tMNodeInfo.v1, destLoc))
                    {
                        triangleMeshNode = tMNodeInfo.node;
                        num = tMNodeInfo.vi;
                        break;
                    }
                }
                if (triangleMeshNode != null)
                {
                    node = triangleMeshNode;
                    startEdge = (num + 1) % 3;
                    count = 1;
                }
                else
                {
                    int edge = -1;
                    IntFactor b = new IntFactor
                    {
                        numerator = -2L,
                        denominator = 1L
                    };
                    for (int k = 0; k < nodeInfos.Count; k++)
                    {
                        var tMNodeInfo2 = nodeInfos[k];// info.nodes[list[k]];
                        if (!checkedNodes.Contains(tMNodeInfo2.node))
                        {
                            int num2;
                            IntFactor cosineAngle = tMNodeInfo2.GetCosineAngle(destLoc, out num2);
                            if (cosineAngle > b)
                            {
                                b = cosineAngle;
                                edge = num2;
                                triangleMeshNode = tMNodeInfo2.node;
                            }
                        }
                    }
                    if (triangleMeshNode != null)
                    {
                        moveAlongEdge(info, triangleMeshNode, edge, srcLoc, destLoc, out result, true);
                        break;
                    }
                }
                if (nodeInfos != null)
                    ListPool<TMNodeInfo>.Release(nodeInfos);
            }
            int num3;
            if (startEdge == -1)
            {
                num3 = node.EdgeIntersect(info,srcLoc, destLoc);
            }
            else
            {
                num3 = node.EdgeIntersect(info,srcLoc, destLoc, startEdge, count);
            }
            if (num3 == -1)
            {
                if (node.ContainsPoint(info,destLoc))
                {
                    result = destLoc; 
                }
                else
                {
                    num3 = node.GetColinearEdge(info,srcLoc, destLoc);
                    if (num3 != -1)
                    {
                        moveAlongEdge(info,node, num3, srcLoc, destLoc, out result, true);
                    }
                }
                break;
            }
            int num4;
            var neighborByEdge = node.GetNeighborByEdge(info,num3, out num4);
            if (neighborByEdge == null)
            {
                moveAlongEdge(info,node, num3, srcLoc, destLoc, out result, true);
                break;
            }
            node = neighborByEdge;
            startEdge = num4 + 1;
        }

    } 

    /*
public static float GetH(NavMeshInfo info, Int3 pos)
{
    UnityEngine.NavMeshHit hit;
    if (UnityEngine.NavMesh.SamplePosition(pos.vec3,out hit,0, NavMesh.AllAreas))
    {
        return hit.position.y;
    }
    return pos.y/1000;
}*/
  
    static List<FindNode> nodeFind; 
    private static List<int> CalcAStarPolyPath(NavMeshInfo info, int startPoly, int endPoly, Int3 endPos)
    {
        info.ReInitFindNodeCache();
        nodeFind = info.nodeFindCache;
         
        if (endPos == null)
        {
            endPos = info.nodes[endPoly].center;
        }
        var startTri = nodeFind[startPoly];
        startTri.nodeid = startPoly;

        //////////////////////////////////// A*算法///////////////////////////////////////
        int pathSessionId = 1;
        bool foundPath = false;
        List<int> openList = ListPool<int>.Claim();// new List<int>();   //开放列表
        List<int> closeList = ListPool<int>.Claim();// new List<int>();

        startTri.pathSessionId = pathSessionId;

        openList.Add(startPoly);
         
        while (openList.Count > 0)
        {
            //1.把当前节点从开放列表删除, 加入到封闭列表
            FindNode currNode;
            currNode = nodeFind[openList[openList.Count - 1]];
            openList.Remove(currNode.nodeid);
            closeList.Add(currNode.nodeid);

             //AStarDebug.DrawTriangle(currNode.nodeid, info.vecs[ info.nodes[currNode.nodeid].triangleVertexIndexs[0]].vec3, info.vecs[info.nodes[currNode.nodeid].triangleVertexIndexs[1]].vec3, info.vecs[info.nodes[currNode.nodeid].triangleVertexIndexs[2]].vec3);

            //已经找到目的地
            if (currNode.nodeid == endPoly)
            {
                foundPath = true;
                break;
            }

            // 2. 对当前节点相邻的每一个节点依次执行以下步骤:
            // 遍历所有邻接三角型
            var nlink = info.nodes[currNode.nodeid].links; 

          //  Debug.LogError("currNode.nodeid:" + currNode.nodeid + "  " + nlink.Count);

            for (int i = 0; i < nlink.Count; i++)
            {
                int neighborID = nlink[i];
                FindNode neighborTri;

                // 3. 如果该相邻节点不可通行,则什么操作也不执行,继续检验下一个节点;
                if (neighborID < 0)
                {
                    //没有该邻居节点
                    continue;
                }
                else
                {
                    neighborTri = nodeFind[neighborID];

                    if (neighborTri == null || neighborTri.nodeid != neighborID)
                    { 
                        ListPool<int>.Release(openList);
                        ListPool<int>.Release(closeList); 
                        return null;
                    }
                }
                //if (neighborTri.GetGroupID() == startTri.GetGroupID())
                {
                    if (neighborTri.pathSessionId != pathSessionId)
                    {
                        //judge the side is wide enough to to pass in offset
                        var b = info.nodes[neighborTri.nodeid].IsLinkTo(info, currNode.nodeid);
                         
                        if (b != -1)  //&& info.borders[b].length > 0
                        {
                            // 4. 如果该相邻节点不在开放列表中,则将该节点添加到开放列表中, 
                            //    并将该相邻节点的父节点设为当前节点,同时保存该相邻节点的G和F值;
                            neighborTri.pathSessionId = pathSessionId;
                            neighborTri.ParentID = currNode.nodeid;
                            neighborTri.Open = true;

                            // 计算启发值h
                            neighborTri.CalcHeuristic(info, endPos);
                            // 计算三角形花费g
                            neighborTri.GValue = currNode.GValue + currNode.GetCost(info, neighborTri.nodeid);

                            //放入开放列表并排序
                            openList.Add(neighborTri.nodeid);

                            openList.Sort((x, y) =>
                            {
                                var xFvalue = nodeFind[x].HValue + nodeFind[x].GValue;
                                var yFvalue = nodeFind[y].HValue + nodeFind[y].GValue;

                                if (xFvalue < yFvalue)
                                    return 1;
                                if (xFvalue > yFvalue)
                                    return -1;
                                return 0;
                            });

                        //    Sort(openList,0, openList.Count-1);

                            //保存穿入边
                            neighborTri.ArrivalWall = currNode.nodeid;
                        }
                    }
                    else
                    {
                        // 5. 如果该相邻节点在开放列表中, 
                        //    则判断若经由当前节点到达该相邻节点的G值是否小于原来保存的G值,
                        //    若小于,则将该相邻节点的父节点设为当前节点,并重新设置该相邻节点的G和F值
                        if (neighborTri.Open)
                        {
                            if (neighborTri.GValue + neighborTri.GetCost(info, currNode.nodeid) < currNode.GValue)
                            {
                                currNode.GValue = neighborTri.GValue + neighborTri.GetCost(info, currNode.nodeid);
                                currNode.ParentID = neighborTri.nodeid;
                                currNode.ArrivalWall = neighborTri.nodeid;
                            }
                        }
                        else
                        {
                            neighborTri = null;
                            continue;
                        }

                    }
                }
            } 
        }

        List<int> outpath = ListPool<int>.Claim();// new List<int>();
        if (closeList.Count != 0)
        {
            FindNode path = nodeFind[closeList[closeList.Count - 1]];
            outpath.Add(path.nodeid);

            while (path.ParentID != -1)
            {
                outpath.Add(path.ParentID);
                path = nodeFind[path.ParentID];
            }
        }

        //if (closeList.Count != 0)
        //{
        //    NavTriangle path = closeList[closeList.Count - 1];
        //    pathList.Add(path);
        //    while (path.GetParentID() != -1)
        //    {
        //        pathList.Add(this.m_lstTriangle[path.GetParentID()]);
        //        path = this.m_lstTriangle[path.GetParentID()];
        //    }
        //}

        ListPool<int>.Release(openList);
        ListPool<int>.Release(closeList);

        if (!foundPath)
        {
            ListPool<int>.Release(outpath);
            return null;
        }
        else
            return outpath;
    }

    public static List<Int3> FindPath(NavMeshInfo info, Int3 startPos, Int3 endPos)
    {

        int startPoly = GetPolyIndexByPos(info, startPos);
        int endPoly = GetPolyIndexByPos(info, endPos);
        
        var polypath = CalcAStarPolyPath(info, startPoly, endPoly, endPos);

        //画出所有多边形
          /*foreach(var v in polypath)
          {
              List<UnityEngine.Vector3> vecs = new List<UnityEngine.Vector3>();
              foreach (var v1 in info.nodes[v].triangleVertexIndexs)
              {
                  vecs.Add(info.vecs[v1].vec3);
              }
              AStarDebug.DrawPoly(v, vecs);
          }*/

        var res = CalcWayPoints(info, startPos, endPos, polypath);
        if (polypath != null)
            ListPool<int>.Release(polypath);
        return res;
    }

    public static List<Int3> CalcWayPoints(NavMeshInfo info, Int3 startPos, Int3 endPos, List<int> polyPath)
    {
        if (polyPath == null || polyPath.Count == 0)
            return null;
        var wayPoints = ListPool<Int3>.Claim();// new List<Int3>(); 
        var triPathList = ListPool<int>.Claim(polyPath.Count);// new List<int>(polyPath);
         
        for (int i = polyPath.Count-1; i >=0; i--)
        {
            triPathList.Add( polyPath[i]);
        } 
        wayPoints.Add(startPos);

        int ipoly = 0;//从第0个poly 开始检查
        Int3? dirLeft = null;
        int ipolyLeft = -1;
        Int3? dirRight = null;
        int ipolyRight = -1;
        int breakdir = 0;
        Int3? posLeft = null;
        Int3? posRight = null;
        Int3 posNow = startPos;

        for (int c = 0; c < 100; c++)//最多循环100次
        {
            for (int i = ipoly; i < triPathList.Count; i++)
            {
                if (i == triPathList.Count - 1)//最后一节
                {
                    if (dirLeft == null || dirRight == null)
                    {
                        breakdir = 0;
                        break;
                    }
                    else
                    {
                        Int3 dirFinal = Int3.NormalXZ(posNow, endPos);

                        var a1 = Int3.AngleXZ(dirLeft.Value, dirFinal);
                        var b1 = Int3.AngleXZ(dirRight.Value, dirFinal);

                        if (a1 * b1 > 0)
                        {
                            if (a1 > 0)
                                breakdir = 1;
                            else
                                breakdir = -1;
                            //System.Diagnostics.Debug.WriteLine("不能直接到达终点");
                        }
                        else
                        {
                            breakdir = 0;
                            break;
                        }
                    }
                }
                else//检查是否通过
                {
                    //寻找边
                    var n1 = triPathList[i];
                    var n2 = triPathList[i + 1];
                    int bname =100000000+ 10000* n1 + n2;
                    if (n2 < n1)
                        bname =100000000 +10000* n2  + n1; 

                    var border = info.borders[bname];
                    var pointA = info.vecs[border.pointA];
                    var pointB = info.vecs[border.pointB];

                    var dist1 = Int3.DistXZ(posNow, pointA);
                    var dist2 = Int3.DistXZ(posNow, pointB);
                    if (dist1 == 0 || dist2 == 0)  //如果在顶点上
                        continue;
                    if (dirLeft == null)
                    {
                        dirLeft = Int3.NormalXZ(posNow, pointA);
                        posLeft = pointA;
                        ipolyLeft = i;
                    }
                    if (dirRight == null)
                    {
                        dirRight = Int3.NormalXZ(posNow, pointB);
                        posRight = pointB;
                        ipolyRight = i;
                    }
                    var adir = Int3.AngleXZ(dirLeft.Value, dirRight.Value);
                    if (adir < 0)//change
                    {
                        var p = dirLeft;
                        var pp = posLeft;
                        var pi = ipolyLeft;
                        dirLeft = dirRight;
                        posLeft = posRight;
                        ipolyLeft = ipolyRight;
                        dirRight = p;
                        posRight = pp;
                        ipolyRight = pi;
                    }

                    if (ipolyLeft != i || ipolyRight != i)//检查是否能穿越
                    {
                        var ndirLeft = Int3.NormalXZ(posNow, pointA);//当前边点A测试向量
                        var ndirRight = Int3.NormalXZ(posNow, pointB);//当前边点B测试向量
                        var nadir = Int3.AngleXZ(ndirLeft, ndirRight);
                        if (nadir < 0)//change
                        {
                            var p = ndirLeft;
                            var pp = pointA;
                            ndirLeft = ndirRight;
                            pointA = pointB;
                            ndirRight = p;
                            pointB = pp;
                        }

                        var aLL = Int3.AngleXZ(dirLeft.Value, ndirLeft);//>0 右侧，<0 左侧
                        var aRL = Int3.AngleXZ(dirRight.Value, ndirLeft);//>0 右侧，<0 左侧
                        var aLR = Int3.AngleXZ(dirLeft.Value, ndirRight);//>0 右侧，<0 左侧
                        var aRR = Int3.AngleXZ(dirRight.Value, ndirRight);//>0 右侧，<0 左侧
                        if ((aLL < 0 && aLR < 0))//无法穿越
                        {
                            breakdir = -1;
                            break;
                        }
                        if (aRL > 0 && aRR > 0)//无法穿越
                        {
                            breakdir = 1;
                            break;
                        }
                        if (aLL > 0 && aRL < 0)
                        {
                            dirLeft = ndirLeft;
                            posLeft = pointA;
                            ipolyLeft = i;
                        }
                        if (aLR > 0 && aRR < 0)
                        {
                            dirRight = ndirRight;
                            posRight = pointB;
                            ipolyRight = i;
                        }
                    }

                }
            }

            if (breakdir == 0)
            {
                break;
            }
            else
            {
                if (breakdir == -1)
                {
                    wayPoints.Add(posLeft.Value);
                    posNow = posLeft.Value;
                    ipoly = ipolyLeft;
                }
                else
                {
                    wayPoints.Add(posRight.Value);
                    posNow = posRight.Value;
                    ipoly = ipolyRight;
                }
                dirLeft = null;
                dirRight = null;
                ipolyLeft = -1;
                ipolyRight = -1;
            }

        }
        wayPoints.Add(endPos);

        ListPool<int>.Release(triPathList);

        return wayPoints;
    }
}