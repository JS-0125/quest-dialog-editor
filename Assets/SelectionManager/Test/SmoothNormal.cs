using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNormal : MonoBehaviour
{
    private Mesh smootMesh;

    Mesh MeshNormalAvg(Mesh originMesh)
    {
        Mesh tmpMesh = Instantiate(originMesh);

        Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();

        for (int v = 0; v < tmpMesh.vertexCount; ++v)
        {
            if (!map.ContainsKey(tmpMesh.vertices[v]))
            {
                map.Add(tmpMesh.vertices[v], new List<int>());
            }
            map[tmpMesh.vertices[v]].Add(v);
        }

        Vector3[] normals = tmpMesh.normals;
        Vector3 normal;
        int i = 0;

        foreach (var p in map)
        {
            normal = Vector3.zero;

            foreach (var n in p.Value)
            {
                Debug.Log(i + ":" + tmpMesh.normals[n].x + " " + tmpMesh.normals[n].y + " " + tmpMesh.normals[n].z);
                normal += tmpMesh.normals[n];
            }

            normal /= p.Value.Count;

            foreach (var n in p.Value)
            {
                normals[n] = normal;
            }
            ++i;
        }

        tmpMesh.normals = normals;
        return tmpMesh;
    }
}
