using System;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGUID;     // 노드 ID
        public string DialogueText; // Dialogue 
        public Vector2 Position;    // 위치
    }
}