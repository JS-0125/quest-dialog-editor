using System;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        // 연결 관리 컨테이너
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        // 노드 관리 컨테이너
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        // 접근 파라미터 관리 컨테이너
        public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
        // 코멘트 관리 컨테이너
        public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
    }
}