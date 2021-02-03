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
        // 메터리얼의 프로퍼티를 설정한다.
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_Scale", outlineScale);       
        outlineMaterial2.SetColor("_OutlineColor", outlineColor);
        outlineMaterial2.SetFloat("_Scale", outlineScale);

        // outline이 필요한 모든 객체의 하위에 outline 렌더를 위한 오브젝트를 생성한다.
        OutlineObject[] outlineObjects = FindObjectsOfType<OutlineObject>();
        foreach(var obj in outlineObjects)
        {
            GameObject originModel = obj.GetModel();
            if (originModel != null)
            {
                // 새로운 오브젝트를 생성한다.
                GameObject outlineModel = Instantiate(originModel, originModel.transform.position, originModel.transform.rotation, originModel.transform);

                // outline 모델의 렌더러를 설정한다.
                Renderer renderer = outlineModel.GetComponent<Renderer>();
                renderer.material = outlineMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.renderingLayerMask = (uint)outlineLayer.value;
                renderer.enabled = false;
                renderer.rendererPriority = -5;

                // ouline 모델의 collider를 비활성화 한다.
                Collider collider = outlineModel.GetComponent<Collider>();
                if(collider != null)
                    collider.enabled = false;

                // rigidbody가 있다면 kinematic을 활성화한다.
                Rigidbody rigidbody = outlineModel.GetComponent<Rigidbody>();
                if (rigidbody != null)
                    rigidbody.isKinematic = true;

                // outline 모델을 오브젝트의 하위로 설정한다.
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
