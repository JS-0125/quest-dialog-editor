using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Subtegral.DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public string DialogueText;     // Dialogue
        public string GUID;             // 노드 ID
        public bool EntyPoint = false;  // 
    }
}