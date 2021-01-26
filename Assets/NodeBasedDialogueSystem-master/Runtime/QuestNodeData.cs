using System;
using UnityEngine;

[Serializable]
public class QuestNodeData
{
    public string NodeGUID;     // 노드 ID
    public string QuestText; // Dialogue 
    public SuccessCondition successCondition;
    public Vector2 Position;    // 위치
}
