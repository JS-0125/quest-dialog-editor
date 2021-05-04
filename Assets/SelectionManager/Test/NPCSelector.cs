using Subtegral.DialogueSystem.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSelector : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    private Transform[] _npcTransforms;

    [SerializeField] private float range;
    [SerializeField] private float fov;

    private Transform _selection = null;

    // UI
    private IHilightSelectionResponse _hilightSelectionResponse;

    // Quest Parser
    private QuestParser questParser;

    private PlayerAction playerAction;

    private void Awake() 
    {
        NPC[] npcs = FindObjectsOfType<NPC>();
        _npcTransforms = new Transform[npcs.Length];
        for(int i = 0; i < npcs.Length; ++i)
        {
            _npcTransforms[i] = npcs[i].GetComponent<Transform>();
        }
        _hilightSelectionResponse = GetComponent<IHilightSelectionResponse>();

        questParser = Camera.main.transform.GetComponent<QuestParser>();
        playerAction = _playerTransform.GetComponent<PlayerAction>();
    } 
    private void Update()
    {
        // UI를 지운다.
        if(_selection != null)
        {
            _hilightSelectionResponse.OnDeselect(_selection);
        }

        Check();
        // test gizmos
        //DrawTestGizmos();

        // UI를 띄운다
        if (_selection != null) {
            _hilightSelectionResponse.OnSelect(_selection);

            if (Input.GetButtonDown("LeftClick") && playerAction.State !=  PlayerState.Talk)
            {
                if (questParser.IsQuestGiver(_selection.gameObject))
                {
                    playerAction.State = PlayerState.Talk;
                }
                else if (questParser.CheckTalkPartner(_selection.gameObject))
                {
                    playerAction.State = PlayerState.Talk;
                }
                else if(_selection.GetComponent<NPC>().dialogue != null)
                {
                    playerAction.State = PlayerState.Talk;
                    DialogueParser dialogueParser = Camera.main.GetComponent<DialogueParser>();
                    dialogueParser.dialogue = _selection.GetComponent<NPC>().dialogue;
                    dialogueParser.StartTalk("not quest give");
                }
            }
        }
    }

    private void Check()
    {
        _selection = null;

        // 모든 NPC들의 거리를 계산해 range에 포함되는 NPC만 남긴다.
        List<int> inRangeNpcIdxs = new List<int>();

        for(var  i = 0; i < _npcTransforms.Length; ++i)
        {
            var dist = (_npcTransforms[i].transform.position - _playerTransform.position).magnitude;
            if (dist <= range)
                inRangeNpcIdxs.Add(i);
        }

        // fov 값을 벗어난 뱡향에 있는경우 제외시킨다.
        for (int i = 0; i <inRangeNpcIdxs.Count; ++i)
        {
            var dir = (_npcTransforms[inRangeNpcIdxs[i]].transform.position - _playerTransform.position).normalized;

            if (Vector3.Angle(_playerTransform.forward,dir) > fov * 0.5)
            {
                inRangeNpcIdxs.RemoveAt(i);
            }
        }

        if (inRangeNpcIdxs.Count > 0)
        {
            var selectedNPC = inRangeNpcIdxs[0];            // 선택되는 npc가 한개인 경우
            // 선택되는 npc가 여러개인 경우에 대해 처리한다. -> 최소 거리에 있는 npc를 선택하도록 한다.
            if (inRangeNpcIdxs.Count > 1)
            {
                var minDist = (_npcTransforms[inRangeNpcIdxs[0]].transform.position - _playerTransform.position).magnitude;
                for (var i = 1; i < inRangeNpcIdxs.Count; ++i)
                {
                    var dist = (_npcTransforms[inRangeNpcIdxs[i]].transform.position - _playerTransform.position).magnitude;
                    if (minDist > dist)
                    {
                        selectedNPC = inRangeNpcIdxs[i];
                        minDist = dist;
                    }
                }
            }
            _selection = _npcTransforms[selectedNPC];
        }
    }

    void DrawTestGizmos()
    {
        if (_selection != null)
        {
            Debug.DrawRay(_playerTransform.position, (_selection.position - _playerTransform.position).normalized * range, Color.red);

        }
        Debug.DrawRay(_playerTransform.position, _playerTransform.forward * range, Color.green);
        foreach (var npc in _npcTransforms)
        {
            Debug.DrawRay(_playerTransform.position, (npc.transform.position - _playerTransform.position).normalized * range, Color.yellow);
        }
    }
}
