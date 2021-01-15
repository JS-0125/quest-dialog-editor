using System;
using System.Linq;

namespace Subtegral.DialogueSystem.DataContainers
{
    [Serializable]
    public class NodeLinkData
    {
        
        public string BaseNodeGUID;     // 노드 ID
        public string PortName;         // 포트 이름
        public string TargetNodeGUID;   // 연결 타켓 노드 ID
    }
}