using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using Subtegral.DialogueSystem.Editor;
using Subtegral.DialogueSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class QuestGraphView : AbstractGraph // Inherits from:UIElements.VisualElement
{
    private NodeSearchWindow<QuestGraphView, QuestGraph> _searchWindow;


    public QuestGraphView(QuestGraph editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // https://stephenhodgson.github.io/UnityCsReference/api/UnityEngine.Experimental.UIElements.Manipulator.html
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());

        var grid = new GridBackground();
        // Insert an element into this element's contentContainer
        // https://stephenhodgson.github.io/UnityCsReference/api/UnityEngine.Experimental.UIElements.VisualElement.html
        Insert(0, grid);
        grid.StretchToParentSize();

        // 새 그래프 요소를 추가. 그래프 요소를 추가할 때 Add() 대신 이 옵션을 사용해야 합니다.
        // graphElement: https://stephenhodgson.github.io/UnityCsReference/api/UnityEditor.Experimental.UIElements.GraphView.GraphElement.html
        // start node element 추가
        AddElement(GetEntryPointNodeInstance());

        AddSearchWindow(editorWindow);
    }


    public void AddSearchWindow(QuestGraph editorWindow)
    {
        //_searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow<QuestGraphView, QuestGraph>>();
        _searchWindow = new NodeSearchWindow<QuestGraphView, QuestGraph>();

        _searchWindow.Configure(editorWindow, this);

        // 사용자가 노드 생성 창을 표시하도록 요청할 때 콜백.
        // public Action<NodeCreationContext> nodeCreationRequest { get; set; }
        // 그래프 위에 검색 창을 엶
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }


    public override void CreateNewNode(string nodeName, Vector2 position)
    {
        AddElement(CreateNode(nodeName, position));
    }

    public QuestNode CreateNode(string nodeName, Vector2 position)
    {
        var tempQuestNode = new QuestNode()
        {
            title = nodeName,
            questText = nodeName,
            guid = Guid.NewGuid().ToString(),
            successCondition = new SuccessConditionObj()
        };
        tempQuestNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));


        // input 포트 생성
        var inputPort = GetPortInstance(tempQuestNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        tempQuestNode.inputContainer.Add(inputPort);

        tempQuestNode.RefreshExpandedState();
        tempQuestNode.RefreshPorts();
        tempQuestNode.SetPosition(new Rect(position,
            DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

        // 오브젝트 필드
        // Quest giver 받는 필드
        var objectField = new ObjectField("Quest Giver");
        objectField.allowSceneObjects = true;
        objectField.objectType = typeof(GameObject);
        objectField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questGiver = (GameObject)evt.newValue;
        });
        tempQuestNode.mainContainer.Add(objectField);

        // Quest dialogue 받는 필드
        var questDialogueField = new ObjectField("Quest Dialogue");
        questDialogueField.allowSceneObjects = true;
        questDialogueField.objectType = typeof(DialogueContainer);
        questDialogueField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questDialogue = (DialogueContainer)evt.newValue;
        });
        tempQuestNode.mainContainer.Add(questDialogueField);


        var enumField = new EnumFlagsField("Success Condition", successCondition.ARRIVED);
        enumField.RegisterValueChangedCallback(evt =>
        {
            SuccessCondition((successCondition)evt.newValue, tempQuestNode);
        });
        enumField.SetValueWithoutNotify(successCondition.None);
        tempQuestNode.extensionContainer.Add(enumField);
        tempQuestNode.RefreshExpandedState();


        var textField = new TextField("");
        textField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questText = evt.newValue;
            tempQuestNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(tempQuestNode.title);
        tempQuestNode.mainContainer.Add(textField);


        var button = new Button(() => { AddChoicePort(tempQuestNode); })
        {
            text = "Add Choice"
        };
        tempQuestNode.titleButtonContainer.Add(button);

        return tempQuestNode;
    }

    // load할 때 사용
    public QuestNode CreateNode(QuestNodeData nodeData, Vector2 position)
    {
        var tempQuestNode = new QuestNode()
        {
            title = nodeData.QuestText,
            questText = nodeData.QuestText,
            guid = nodeData.NodeGUID,
            questGiver = GameObject.Find(nodeData.QeustGiver),
            questDialogue = nodeData.questDialogue,
            successCondition = nodeData.successCondition,
            successConditionEnum = nodeData.successConditionEnum
        };

        tempQuestNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));


        // input 포트 생성
        var inputPort = GetPortInstance(tempQuestNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        tempQuestNode.inputContainer.Add(inputPort);

        tempQuestNode.RefreshExpandedState();
        tempQuestNode.RefreshPorts();
        tempQuestNode.SetPosition(new Rect(position,
            DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

        // 오브젝트 필드
        // Quest giver 받는 필드
        var objectField = new ObjectField("Quest Giver");
        objectField.allowSceneObjects = true;
        objectField.objectType = typeof(GameObject);
        objectField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questGiver = (GameObject)evt.newValue;
        });
        objectField.SetValueWithoutNotify(tempQuestNode.questGiver);

        tempQuestNode.mainContainer.Add(objectField);

        // Quest dialogue 받는 필드
        var questDialogueField = new ObjectField("Quest Dialogue");
        questDialogueField.allowSceneObjects = true;
        questDialogueField.objectType = typeof(DialogueContainer);
        questDialogueField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questDialogue = (DialogueContainer)evt.newValue;
        });
        //Debug.Log(tempQuestNode.questDialogue);
        questDialogueField.SetValueWithoutNotify(tempQuestNode.questDialogue);
        tempQuestNode.mainContainer.Add(questDialogueField);


        var enumField = new EnumFlagsField("Success Condition", successCondition.ARRIVED);
        enumField.RegisterValueChangedCallback(evt =>
        {
            SuccessCondition((successCondition)evt.newValue, tempQuestNode);
        });
        enumField.SetValueWithoutNotify(tempQuestNode.successConditionEnum);
        tempQuestNode.extensionContainer.Add(enumField);

        tempQuestNode.RefreshExpandedState();


        var textField = new TextField("");
        textField.RegisterValueChangedCallback(evt =>
        {
            tempQuestNode.questText = evt.newValue;
            tempQuestNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(tempQuestNode.title);
        tempQuestNode.mainContainer.Add(textField);


        var button = new Button(() => { AddChoicePort(tempQuestNode); })
        {
            text = "Add Choice"
        };
        tempQuestNode.titleButtonContainer.Add(button);

        return tempQuestNode;
    }

    public void SuccessCondition(successCondition condition, QuestNode tempQuestNode)
    {
        // enum 설정
        tempQuestNode.successConditionEnum = (successCondition)condition;

        SuccessConditionObj successConditionObj = new SuccessConditionObj();

        // 만약 전에 있었던 field가 있다면 컨테이너에서 삭제
        for (int i = 1; i < tempQuestNode.extensionContainer.childCount; ++i)
        {
            tempQuestNode.extensionContainer.RemoveAt(i);
            --i;
        }

        if((condition & successCondition.ARRIVED) == successCondition.ARRIVED){
            var destination = new ObjectField("Destination");
            destination.allowSceneObjects = true;
            destination.objectType = typeof(GameObject);
            destination.RegisterValueChangedCallback(evt =>
            {
                if (tempQuestNode.successCondition.destination != null)
                {
                    var previousObject = tempQuestNode.successCondition.destination;

                    var previousValue = GameObject.Find(previousObject);
                    var collectionScript = previousValue.GetComponent<Destination>();
                    UnityEngine.Object.DestroyImmediate(collectionScript);

                    if (evt.newValue == null)
                    {
                        tempQuestNode.successCondition.destination = null;
                        return;
                    }

                    var destinationObj = (GameObject)evt.newValue;

                    destinationObj.AddComponent<Destination>();
                    destinationObj.GetComponent<Destination>().guid = tempQuestNode.guid;

                    tempQuestNode.successCondition.destination = destinationObj.name;
                }
                else
                {
                    if (evt.newValue == null)
                        return;
                    var destinationObj = (GameObject)evt.newValue;

                    destinationObj.AddComponent<Destination>();
                    destinationObj.GetComponent<Destination>().guid = tempQuestNode.guid;

                    tempQuestNode.successCondition.destination = destinationObj.name;
                }
            });
            var obj = GameObject.Find(tempQuestNode.successCondition.destination);
            if (obj != null)
                destination.SetValueWithoutNotify(obj);
            else
            {
                destination.SetValueWithoutNotify(null);
                tempQuestNode.successCondition.destination = null;
            }
            tempQuestNode.extensionContainer.Add(destination);
        }

        if ((condition & successCondition.COLLECT) == successCondition.COLLECT)
        {
            var button = new Button(() => {
                var collection = new ObjectField("Collection");
                collection.allowSceneObjects = true;
                collection.objectType = typeof(GameObject);
                tempQuestNode.successCondition.collection.Add(null);

                collection.RegisterValueChangedCallback(evt =>
                {
                    if (evt.previousValue != null)
                    {
                        var previousObject = tempQuestNode.successCondition.collection.Find(x => x == evt.previousValue.name);
                        int index = tempQuestNode.successCondition.collection.IndexOf(previousObject);
                       
                        var previousValue = GameObject.Find(previousObject);

                        var collectionScript = previousValue.GetComponent<Collection>();
                        UnityEngine.Object.DestroyImmediate(collectionScript);

                        if (evt.newValue == null)
                        {
                            tempQuestNode.successCondition.destination = null;
                            return;
                        }

                        var collectObj = (GameObject)evt.newValue;

                        collectObj.AddComponent<Collection>();
                        collectObj.GetComponent<Collection>().guid = tempQuestNode.guid;

                        tempQuestNode.successCondition.collection[index] = collectObj.name;
                    }
                    else
                    {
                        if (evt.newValue == null)
                            return;
                        int index = tempQuestNode.successCondition.collection.IndexOf(tempQuestNode.successCondition.collection.Find(x => x == null));
                        var collectObj = (GameObject)evt.newValue;

                        collectObj.AddComponent<Collection>();
                        collectObj.GetComponent<Collection>().guid = tempQuestNode.guid;

                        tempQuestNode.successCondition.collection[index] = collectObj.name;
                    }
                });
                tempQuestNode.extensionContainer.Add(collection);
            })
            {
                text = "Add Collection"
            };
            tempQuestNode.extensionContainer.Add(button);

            if(tempQuestNode.successCondition.collection.Count() != 0)
            {
                for(int i =0;i< tempQuestNode.successCondition.collection.Count(); ++i)
                {
                    var collection = new ObjectField("Collection");
                    collection.allowSceneObjects = true;
                    collection.objectType = typeof(GameObject);
                    collection.RegisterValueChangedCallback(evt =>
                    {
                        if (evt.previousValue != null)
                        {
                            string previousObject = tempQuestNode.successCondition.collection.Find(x => x == evt.previousValue.name);
                            var index = tempQuestNode.successCondition.collection.IndexOf(previousObject);

                            var previousValue = GameObject.Find(previousObject);

                            var collectionScript = previousValue.GetComponent<Collection>();
                            UnityEngine.Object.DestroyImmediate(collectionScript);

                            if (evt.newValue == null)
                            {
                                tempQuestNode.successCondition.destination = null;
                                return;
                            }

                            var collectObj = (GameObject)evt.newValue;

                            collectObj.AddComponent<Collection>();
                            collectObj.GetComponent<Collection>().guid = tempQuestNode.guid;

                            if (index == -1)
                            {
                                var tmp = tempQuestNode.successCondition.collection.Find(x => x == null);
                                tmp = collectObj.name;
                            }
                            else
                                tempQuestNode.successCondition.collection[index] = collectObj.name;
                        }
                        else
                        {
                            if (evt.newValue == null)
                                return;
                            int index = tempQuestNode.successCondition.collection.IndexOf(tempQuestNode.successCondition.collection.Find(x => x == null));
                            var collectObj = (GameObject)evt.newValue;

                            collectObj.AddComponent<Collection>();
                            collectObj.GetComponent<Collection>().guid = tempQuestNode.guid;

                            tempQuestNode.successCondition.collection[index] = collectObj.name;
                        }

                    });
                    var obj = GameObject.Find(tempQuestNode.successCondition.collection[i]);
                    if (obj != null)
                        collection.SetValueWithoutNotify(obj);
                    else
                    {
                        collection.SetValueWithoutNotify(null);
                        tempQuestNode.successCondition.collection[i] = null;
                    }
                    tempQuestNode.extensionContainer.Add(collection);
                }
            }
        }

        if ((condition & successCondition.TALK) == successCondition.TALK)
        {
            var partner = new ObjectField("partner");
            partner.allowSceneObjects = true;
            partner.objectType = typeof(GameObject);
            partner.RegisterValueChangedCallback(evt =>
            {
                tempQuestNode.successCondition.obj = ((GameObject)evt.newValue).name;
            });

            if(tempQuestNode.successCondition.obj != null)
            {
                var obj = GameObject.Find(tempQuestNode.successCondition.obj);
                if (obj != null)
                {
                    partner.SetValueWithoutNotify(obj);
                    Debug.Log("set value" + obj.name);
                }
                else
                {
                    Debug.Log("can not Found Partner obj!");
                    partner.SetValueWithoutNotify(null);
                    tempQuestNode.successCondition.obj = null;
                }
            }
            else
            {
                partner.SetValueWithoutNotify(null);
                tempQuestNode.successCondition.obj = null;
            }
            tempQuestNode.extensionContainer.Add(partner);

            var dialogue = new ObjectField("dialogue");
            dialogue.allowSceneObjects = true;
            dialogue.objectType = typeof(DialogueContainer);
            dialogue.RegisterValueChangedCallback(evt =>
            {
                tempQuestNode.successCondition.dialogue = (DialogueContainer)evt.newValue;
            });
            dialogue.SetValueWithoutNotify(tempQuestNode.successCondition?.dialogue ?? null);
            tempQuestNode.extensionContainer.Add(dialogue);
        }

        if ((condition & successCondition.TIMELIMIT) == successCondition.TIMELIMIT)
        {
            var timeLimitSec = new FloatField("limit Sec");
            timeLimitSec.RegisterValueChangedCallback(evt =>
            {
                tempQuestNode.successCondition.limitSec = evt.newValue;
            });
            timeLimitSec.SetValueWithoutNotify(tempQuestNode.successCondition?.limitSec ?? 0);
            tempQuestNode.extensionContainer.Add(timeLimitSec);
        }
        tempQuestNode.RefreshExpandedState();
    }

    public void AddChoicePort(QuestNode nodeCache, string overriddenPortName = "")
    {
        var generatedPort = GetPortInstance(nodeCache, Direction.Output);
        var portLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(portLabel);

        var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
        var outputPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Option {outputPortCount}"
            : overriddenPortName;


        var textField = new TextField()
        {
            name = string.Empty,
            value = outputPortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(nodeCache, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        generatedPort.portName = outputPortName;
        nodeCache.outputContainer.Add(generatedPort);
        nodeCache.RefreshPorts();
        nodeCache.RefreshExpandedState();
    }


    private Port GetPortInstance(QuestNode node, Direction nodeDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
    }

    private QuestNode GetEntryPointNodeInstance()
    {
        // Start node 생성
        var nodeCache = new QuestNode()
        {
            title = "START",
            guid = Guid.NewGuid().ToString(),
            questText = "ENTRYPOINT",
            entryPoint = true
        };

        // next 포트 생성
        var generatedPort = GetPortInstance(nodeCache, Direction.Output);
        generatedPort.portName = "Next";

        // 노드 컨테이너에 추가
        nodeCache.outputContainer.Add(generatedPort);

        //nodeCache.capabilities &= ~Capabilities.Movable;
        nodeCache.capabilities &= ~Capabilities.Deletable;

        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        nodeCache.SetPosition(new Rect(100, 200, 100, 150));
        return nodeCache;
    }
}
