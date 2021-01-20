using Subtegral.DialogueSystem.DataContainers;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QuestContainer : ScriptableObject
{
    // ���� ���� �����̳�
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    // ��� ���� �����̳�
    public List<QuestNodeData> QuestNodeData = new List<QuestNodeData>();
    // ���� �Ķ���� ���� �����̳�
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    // �ڸ�Ʈ ���� �����̳�
    public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
}
