using Subtegral.DialogueSystem.DataContainers;
using Subtegral.DialogueSystem.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[System.Flags]
public enum successCondition
{
    None = 0, // Custom name for "Nothing" option
    ARRIVED = 0x00000001,   // 0001
    COLLECT = 0x00000002,   // 0010,
    TALK = 0x00000004,   // 0100,
    TIMELIMIT = 0x00000008,   // 1000,
    All = ~0, // Custom name for "Everything" option
}

[Serializable]
public class SuccessConditionObj
{
    public GameObject destination;
    public GameObject collection;
    public int number;
    public GameObject obj;
    public DialogueContainer dialogue;
    public float limitSec;
}