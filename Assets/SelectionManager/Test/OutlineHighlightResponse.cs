using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHighlightResponse : MonoBehaviour,IHilightSelectionResponse
{
    public void OnDeselect(Transform selection)
    {
        selection.GetComponent<OutlineObject>().OutlineModelTransform.GetComponent<Renderer>().enabled = false;
    }

    public void OnSelect(Transform selection)
    {
        selection.GetComponent<OutlineObject>().OutlineModelTransform.GetComponent<Renderer>().enabled = true;
    }
}
