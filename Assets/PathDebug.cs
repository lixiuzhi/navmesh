using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarDebug {

    static Dictionary<int, MeshFilter> triangleObjs = new Dictionary<int, MeshFilter>();

   static Material mat;

    static public void DrawPoint(int id,Vector3 pos)
    {
        DrawTriangle(10000000+id,pos + Vector3.right * 0.03f, pos - Vector3.right * 0.03f, pos + Vector3.forward * 0.03f);
    }

    static public void DrawTriangle(int id,Vector3 a, Vector3 b, Vector3 c)
    {
#if !UNITY_EDITOR
        return;
#endif

        if (mat == null)
        {
         //   mat = new Material(Shader.Find("Shadow/Self_Illlumin_Diffuse")); 
        }

        MeshFilter mf = null;
        Mesh mesh;
        if (!triangleObjs.TryGetValue(id, out mf))
        {
            mf = new GameObject("_PDebug_" + id).AddComponent<MeshFilter>(); 
            mf.transform.position = new Vector3(0,0.05f,0);
            mf.transform.rotation = Quaternion.identity;
            triangleObjs[id] = mf;
            mesh = new Mesh(); 
            var mr = mf.gameObject.AddComponent<MeshRenderer>(); 
        }
        mesh = mf.mesh;
        mf.transform.position = a;
        mesh.vertices = new Vector3 []{  a-a, b-a,c-a };
        mesh.SetIndices(new int[] {0,1,2,2,1,0 }, MeshTopology.Triangles,0);
        mesh.UploadMeshData(false); 
    }

    static public void DrawPoly(int id, List<Vector3> vecs)
    {
#if !UNITY_EDITOR
        return;
#endif

        if (mat == null)
        {
            //mat = new Material(Shader.Find("Shadow/Self_Illlumin_Diffuse"));
            //mat.color = Color.cyan;
        }

        MeshFilter mf = null;
        Mesh mesh; 

        mf = new GameObject("_PDebug_" + id).AddComponent<MeshFilter>();  

        mf.transform.position = vecs[0];
        mf.transform.rotation = Quaternion.identity;
        mesh = new Mesh();
        var mr = mf.gameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mesh = mf.mesh; 

        List<Vector3> verts = new List<Vector3>();

        for (int i = 0; i < vecs.Count; i++)
        {
            verts.Add(vecs[i] - vecs[0]);
        } 
        mesh.vertices = verts.ToArray();

        var indexs = new int[3 * (vecs.Count - 2)]; 
        for (int i = 2, j = 0; i < vecs.Count; i++, j += 3)
        {
            indexs[j] = 0;
            indexs[j + 1] = i - 1;
            indexs[j + 2] = i; 
        }

        mesh.SetIndices(indexs, MeshTopology.Triangles, 0);
        mesh.UploadMeshData(false);
    }
}
