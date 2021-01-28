using Subtegral.DialogueSystem.DataContainers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum successCondition
{
    ARRIVED,
    COLLECT,
    TALK,
    TIMELIMIT,
}
[Serializable]
public class SuccessConditionObj
{
    public Collider destination;
    public GameObject collection;
    public int number;
    public GameObject obj;
    public DialogueContainer dialogue;
    public float limitSec;
}