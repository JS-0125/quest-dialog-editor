using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestNodeData
{
    public string NodeGUID;     // ��� ID
    public string QuestText; // Dialogue 
    public GameObject QeustGiver;
    public successCondition successConditionEnum;
    public SuccessConditionObj successCondition;
    public Vector2 Position;    // ��ġ
}
