using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Subtegral.DialogueSystem.DataContainers;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class QuestGraphSaveUtility
    {
        // edge 관리 리스트
        private List<Edge> Edges => _graphView.edges.ToList();

        // Dialogue 관리 리스트
        private List<QuestNode> Nodes => _graphView.nodes.ToList().Cast<QuestNode>().ToList();

        // 코멘트 블럭 관리 리스트
        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        // Dialogue 컨테이너
        // List<NodeLinkData> / List<DialogueNodeData> / List<ExposedProperty> / List<CommentBlockData>
        private QuestContainer _questContainer;

        // graph view
        private QuestGraphView _graphView;

        public static QuestGraphSaveUtility GetInstance(QuestGraphView graphView)
        {
            return new QuestGraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var questContainerObject = ScriptableObject.CreateInstance<QuestContainer>();
            if (!SaveNodes(fileName, questContainerObject)) return;
            SaveExposedProperties(questContainerObject);
            SaveCommentBlocks(questContainerObject);

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/Resources/{fileName}.asset", typeof(QuestContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset))
            {
                AssetDatabase.CreateAsset(questContainerObject, $"Assets/Resources/{fileName}.asset");
            }
            else
            {
                QuestContainer container = loadedAsset as QuestContainer;
                container.NodeLinks = questContainerObject.NodeLinks;
                container.QuestNodeData = questContainerObject.QuestNodeData;
                container.ExposedProperties = questContainerObject.ExposedProperties;
                container.CommentBlockData = questContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            // json 파일로 저장
            File.WriteAllText(Application.dataPath + $"/Resources/{fileName}.json", JsonUtility.ToJson(questContainerObject));

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, QuestContainer questContainerObject)
        {
            if (!Edges.Any()) return false;
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var outputNode = (connectedSockets[i].output.node as QuestNode);
                var inputNode = (connectedSockets[i].input.node as QuestNode);

                questContainerObject.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.guid,
                    PortName = connectedSockets[i].output.portName,
                    TargetNodeGUID = inputNode.guid
                });
            }

            foreach (var node in Nodes.Where(node => !node.entryPoint))
            {
                Debug.Log(node.successConditionEnum);
                questContainerObject.QuestNodeData.Add(new QuestNodeData
                {
                    NodeGUID = node.guid,
                    QuestText = node.questText,
                    QeustGiver = node.questGiver.name,
                    questDialogue = node.questDialogue,
                    successConditionEnum = node.successConditionEnum,
                    successCondition = node.successCondition,
                    Position = node.GetPosition().position
                }) ;
            }

            return true;
        }

        private void SaveExposedProperties(QuestContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_graphView.ExposedProperties);
        }

        private void SaveCommentBlocks(QuestContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is QuestNode).Cast<QuestNode>().Select(x => x.guid)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadNarrative(string fileName)
        {
            _questContainer = Resources.Load<QuestContainer>(fileName);
            if (_questContainer == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            // json 파일 불러오기
            //string jsonString = File.ReadAllText(Application.dataPath + $"/Resources/{fileName}.json");

            // parse 디버깅
            //Debug.Log(jsonString);

            // new 
            //_questContainer = ScriptableObject.CreateInstance<QuestContainer>();

            // ScriptableObject나 MonoBehavior를 상속받았을 시, JsonUtility.FromJson말고 JsonUtility.FromJsonOverwrite만 지원
            // https://docs.unity3d.com/Manual/JSONSerialization.html?_ga=2.70394898.304792601.1611044230-595836390.1590250109
            //JsonUtility.FromJsonOverwrite(jsonString, _questContainer);

            ClearGraph();
            GenerateDialogueNodes();
            ConnectDialogueNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            Nodes.Find(x => x.entryPoint).guid = _questContainer.NodeLinks[0].BaseNodeGUID;
            foreach (var perNode in Nodes)
            {
                if (perNode.entryPoint) continue;
                Edges.Where(x => x.input.node == perNode).ToList()
                    .ForEach(edge => _graphView.RemoveElement(edge));
                _graphView.RemoveElement(perNode);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateDialogueNodes()
        {
            foreach (var perNode in _questContainer.QuestNodeData)
            {
                var tempNode = _graphView.CreateNode(perNode, Vector2.zero);

                 _graphView.SuccessCondition(tempNode.successConditionEnum, tempNode);
                 _graphView.AddElement(tempNode);

                var nodePorts = _questContainer.NodeLinks.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
                nodePorts.ForEach(x => _graphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ConnectDialogueNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var k = i; //Prevent access to modified closure
                var connections = _questContainer.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[k].guid).ToList();
                for (var j = 0; j < connections.Count(); j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGUID;
                    var targetNode = Nodes.First(x => x.guid == targetNodeGUID);
                    LinkNodesTogether(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        _questContainer.QuestNodeData.First(x => x.NodeGUID == targetNodeGUID).Position,
                        _graphView.DefaultNodeSize));
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _graphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _questContainer.ExposedProperties)
            {
                _graphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _graphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _questContainer.CommentBlockData)
            {
                var block = _graphView.CreateCommentBlock(new Rect(commentBlockData.Position, _graphView.DefaultCommentBlockSize),
                     commentBlockData);
                block.AddElements(Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.guid)));
            }
        }
    }
}