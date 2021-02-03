using UnityEngine;

public class HilightSelectionResponse : MonoBehaviour, IHilightSelectionResponse
{
    [SerializeField] private Material hilightMaterial;
    [SerializeField] private Material _defaultMaterial;

    public void OnSelect(Transform selection)
    {
        var selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        { 
            selectionRenderer.material = hilightMaterial;
        }
    }

    public void OnDeselect(Transform selection)
    {
        var selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null)
        {
            selectionRenderer.material = _defaultMaterial;
        }
    }
}