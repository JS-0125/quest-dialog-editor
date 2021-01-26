using System;
using UnityEngine;

[Serializable]
public class QuestNodeData
{
    public string NodeGUID;     // ��� ID
    public string QuestText; // Dialogue 
    public SuccessCondition successCondition;
    public Vector2 Position;    // ��ġ
}
