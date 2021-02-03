using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    SkillManager _skillManager;
    IHilightSelectionResponse _highlihgtSelectionResponse;

    GravityObject _selection;

    [SerializeField] float range;
    [SerializeField] Transform eyePosition;
    [SerializeField] Transform firstCameraLookAtPostion;

    private void Awake()
    {
        _skillManager = FindObjectOfType<SkillManager>();
        _highlihgtSelectionResponse = GetComponent<IHilightSelectionResponse>();
        if (_skillManager.PlayerID != 0)
            enabled = false;
    }

    private void Update()
    {
        if (_selection != null)
            _highlihgtSelectionResponse.OnDeselect(_selection.transform);

        if (_selection != null)
            _selection = null;
        if (_skillManager.IsKeyHolding)
        {
            Check();
        }

        if (_selection != null)
        {
            _skillManager.Selection = _selection;
            _highlihgtSelectionResponse.OnSelect(_selection.transform);
        }       
    }

    private void Check()
    {      
        if (_selection != null)
            _selection = null;
        Vector3 look= firstCameraLookAtPostion.position - eyePosition.position;
        Debug.DrawRay(eyePosition.position, look, Color.blue,range);
        if(Physics.Raycast(eyePosition.position, look, out var hit, range))
        {
            var gravityObj = hit.transform.GetComponent<GravityObject>();
            if (gravityObj != null)
                _selection = gravityObj;
        }
    }
}
