using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestNode : Node
{
    public string guid;
    public GameObject questGiver;
    public string questText;
    public successCondition successConditionEnum;
    public SuccessConditionObj successCondition;
    public bool entryPoint = false;  
}
