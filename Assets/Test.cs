using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{

    public Transform start;
    public Transform end;

    public Transform player;
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 100), "寻路测试"))
        {
            var path = SceneNavPathData.Instance.FindPath(start.position, end.position);
            if (path != null)
            {
                SceneNavPathData.Instance.DrawPath(path);
                ListPool<Int3>.Release(path);
            }
        } 
    }

    public Vector3 moveDir = Vector3.zero;
    bool isKeyOperator = false;
    float curDegree = 0;
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            moveDir = player.forward;
        if (Input.GetKey(KeyCode.S))
            moveDir = -player.forward;
        if (Input.GetKey(KeyCode.A))
            moveDir =- player.right;
        if (Input.GetKey(KeyCode.D))
            moveDir = player.right;

        var newPos = player.position + SceneNavPathData.Instance.Move(player.position,moveDir *Time.deltaTime *6);
        newPos.y = SceneNavPathData.Instance.GetH((Int3)newPos);
        player.position = newPos;

        moveDir = Vector3.zero;

       var nodeIdx = PathFinding.GetPolyIndexByPos(SceneNavPathData.Instance.info,(Int3)newPos);
        if (nodeIdx != -1)
        {
            var node = SceneNavPathData.Instance.info.nodes[nodeIdx];
            var triangle = node.triangleVertexIndexs;
            AStarDebug.DrawTriangle(100, SceneNavPathData.Instance.info.vecs[triangle[0]].vec3, SceneNavPathData.Instance.info.vecs[triangle[1]].vec3, SceneNavPathData.Instance.info.vecs[triangle[2]].vec3);
        }
    }
}
