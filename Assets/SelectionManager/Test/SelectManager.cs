using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    private IHilightSelectionResponse _hilightSelectionResponse;

    private Transform _selection;

    private void Awake()
    {
        _hilightSelectionResponse = GetComponent<IHilightSelectionResponse>();
    }
    private void Update()
    {
        if(_selection != null)
        {
            _hilightSelectionResponse.OnDeselect(_selection);
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        _selection = null;
        if(Physics.Raycast(ray,out var hit))
        {
            var selection = hit.transform;
            if(selection != null)
            {
                _selection = selection;
            }
        }

        if(_selection != null)
        {
            _hilightSelectionResponse.OnSelect(_selection);
        }
    }
}
