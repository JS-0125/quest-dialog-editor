using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Subtegral.DialogueSystem.DataContainers;
using Subtegral.DialogueSystem.Editor;
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
            guid = Guid.NewGuid().ToString()
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
        tempQuestNode.mainContainer.Add(objectField);


        var enumField = new EnumField("Success Condition", successCondition.ARRIVED);
        enumField.RegisterValueChangedCallback(evt =>
        {
            SuccessCondition(enumField.value, tempQuestNode);
        });
        tempQuestNode.extensionContainer.Add(enumField);    

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

    private List<VisualElement> successCoditionFields = new List<VisualElement>();

    private void SuccessCondition(Enum condition, QuestNode tempQuestNode)
    {
        for (int i = 0; i < successCoditionFields.Count(); ++i)
            tempQuestNode.extensionContainer.Remove(successCoditionFields[i]);

        successCoditionFields.Clear();

        switch (condition)
        {
            case successCondition.ARRIVED:
                var destination = new ObjectField("Destination");
                destination.allowSceneObjects = true;
                destination.objectType = typeof(Collider);
                tempQuestNode.extensionContainer.Add(destination);
                successCoditionFields.Add(destination);
                break;
            case successCondition.COLLECT:
                var collection = new ObjectField("Collection");
                collection.allowSceneObjects = true;
                collection.objectType = typeof(GameObject);
                tempQuestNode.extensionContainer.Add(collection);
                successCoditionFields.Add(collection);

                var intField = new IntegerField("number");
                tempQuestNode.extensionContainer.Add(intField);
                successCoditionFields.Add(intField);
                break;
            case successCondition.TALK:
                var partner = new ObjectField("partner");
                partner.allowSceneObjects = true;
                partner.objectType = typeof(GameObject);
                tempQuestNode.extensionContainer.Add(partner);
                successCoditionFields.Add(partner);

                var dialogue = new ObjectField("dialogue");
                dialogue.allowSceneObjects = true;
                dialogue.objectType = typeof(DialogueContainer);
                tempQuestNode.extensionContainer.Add(dialogue);
                successCoditionFields.Add(dialogue);
                break;
            case successCondition.TIMELIMIT:
                var timeLimitSec = new FloatField("limit Sec");
                tempQuestNode.extensionContainer.Add(timeLimitSec);
                successCoditionFields.Add(timeLimitSec);
                break;
        }
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
