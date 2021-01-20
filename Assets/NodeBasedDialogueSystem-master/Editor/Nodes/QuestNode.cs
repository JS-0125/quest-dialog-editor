using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestNode : Node
{
    public string guid;             
    public string questText;
    public bool entryPoint = false;  
}
