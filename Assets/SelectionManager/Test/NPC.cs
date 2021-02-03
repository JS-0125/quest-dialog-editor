using UnityEngine;

public class NPC : OutlineObject
{
    private Transform _modelTransform;  // ¸ðµ¨ º»Ã¼ÀÇ transform

    public Transform ModelTransform
    {
        get => _modelTransform;
    }

    private void Awake()
    {
        Renderer childRenderer = GetComponentInChildren<Renderer>();
        GameObject model = childRenderer.gameObject;

        _modelTransform = model.transform;
    }

    public override GameObject GetModel()
    {
        if(_modelTransform != null)
            return _modelTransform.gameObject;
        return null;
    }
}
