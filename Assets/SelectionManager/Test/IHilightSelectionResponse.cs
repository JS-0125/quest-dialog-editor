using UnityEngine;

public interface IHilightSelectionResponse
{
    void OnDeselect(Transform selection);
    void OnSelect(Transform selection);
}