using Subtegral.DialogueSystem.DataContainers;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestNodeData
{
    public string NodeGUID;     // 노드 ID
    public string QuestText; // Dialogue 
    public string QeustGiver;
    public DialogueContainer questDialogue;
    public successCondition successConditionEnum;
    public SuccessConditionObj successCondition;
    public Vector2 Position;    // 위치
}
