using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    protected Transform _outlineModelTransfrom;
    public Transform OutlineModelTransform
    {
        set
        {
            _outlineModelTransfrom = value;
        }
        get => _outlineModelTransfrom;
    }

    virtual public GameObject GetModel()
    {
        return gameObject;
    }
}
