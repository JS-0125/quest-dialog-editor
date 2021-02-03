using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObjectManager : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material outlineMaterial2;
    [SerializeField] private float outlineScale;
    [SerializeField] private Color outlineColor;
    [SerializeField] private LayerMask outlineLayer;

    private List<Mesh> _originMeshes = new List<Mesh>();
    private List<Mesh> _createdSmoothMeshes = new List<Mesh>();

    void Start()
    {
        // ���͸����� ������Ƽ�� �����Ѵ�.
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_Scale", outlineScale);       
        outlineMaterial2.SetColor("_OutlineColor", outlineColor);
        outlineMaterial2.SetFloat("_Scale", outlineScale);

        // outline�� �ʿ��� ��� ��ü�� ������ outline ������ ���� ������Ʈ�� �����Ѵ�.
        OutlineObject[] outlineObjects = FindObjectsOfType<OutlineObject>();
        foreach(var obj in outlineObjects)
        {
            GameObject originModel = obj.GetModel();
            if (originModel != null)
            {
                // ���ο� ������Ʈ�� �����Ѵ�.
                GameObject outlineModel = Instantiate(originModel, originModel.transform.position, originModel.transform.rotation, originModel.transform);

                // outline ���� �������� �����Ѵ�.
                Renderer renderer = outlineModel.GetComponent<Renderer>();
                renderer.material = outlineMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.renderingLayerMask = (uint)outlineLayer.value;
                renderer.enabled = false;
                renderer.rendererPriority = -5;

                // ouline ���� collider�� ��Ȱ��ȭ �Ѵ�.
                Collider collider = outlineModel.GetComponent<Collider>();
                if(collider != null)
                    collider.enabled = false;

                // rigidbody�� �ִٸ� kinematic�� Ȱ��ȭ�Ѵ�.
                Rigidbody rigidbody = outlineModel.GetComponent<Rigidbody>();
                if (rigidbody != null)
                    rigidbody.isKinematic = true;

                // outline ���� ������Ʈ�� ������ �����Ѵ�.
                outlineModel.transform.SetParent(obj.transform);

                obj.OutlineModelTransform = outlineModel.transform;

            }
        }
    }

    //Mesh MeshNormalAvg(Mesh originMesh)
    //{
    //    Mesh tmpMesh = Instantiate(originMesh);

    //    Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();

    //    for (int v = 0; v < tmpMesh.vertexCount; ++v)
    //    {
    //        if (!map.ContainsKey(tmpMesh.vertices[v]))
    //        {
    //            map.Add(tmpMesh.vertices[v], new List<int>());
    //        }
    //        map[tmpMesh.vertices[v]].Add(v);
    //    }

    //    Vector3[] normals = tmpMesh.normals;
    //    Vector3 normal;
    //    int i = 0;

    //    foreach (var p in map)
    //    {
    //        normal = Vector3.zero;

    //        foreach (var n in p.Value)
    //        {
    //            Debug.Log(i + ":" + tmpMesh.normals[n].x + " " + tmpMesh.normals[n].y + " " + tmpMesh.normals[n].z);
    //            normal += tmpMesh.normals[n];
    //        }

    //        normal /= p.Value.Count;

    //        foreach (var n in p.Value)
    //        {
    //            normals[n] = normal;
    //        }
    //        ++i;
    //    }

    //    tmpMesh.normals = normals;
    //    return tmpMesh;
    //}

}
