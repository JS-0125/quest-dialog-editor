using Subtegral.DialogueSystem.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum successCondition
{
    ARRIVED,
    COLLECT,
    TALK,
    TIMELIMIT,
}
public abstract class SuccessCondition
{

}

public class Arrived : SuccessCondition
{
    public Collider destination;
}

public class Collect : SuccessCondition
{
    public GameObject collection;
    public int number;
}

public class Talk : SuccessCondition
{
    public int objId;
    public DialogueContainer dialogue;
}

public class TimeLimit : SuccessCondition
{
    public float limitSec;
}