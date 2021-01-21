using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // �� �׷��� ��Ҹ� �߰�. �׷��� ��Ҹ� �߰��� �� Add() ��� �� �ɼ��� ����ؾ� �մϴ�.
        // graphElement: https://stephenhodgson.github.io/UnityCsReference/api/UnityEditor.Experimental.UIElements.GraphView.GraphElement.html
        // start node element �߰�
        AddElement(GetEntryPointNodeInstance());

        AddSearchWindow(editorWindow);
    }


    public void AddSearchWindow(QuestGraph editorWindow)
    {
        //_searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow<QuestGraphView, QuestGraph>>();
        _searchWindow = new NodeSearchWindow<QuestGraphView, QuestGraph>();

        _searchWindow.Configure(editorWindow, this);

        // ����ڰ� ��� ���� â�� ǥ���ϵ��� ��û�� �� �ݹ�.
        // public Action<NodeCreationContext> nodeCreationRequest { get; set; }
        // �׷��� ���� �˻� â�� ��
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


        // input ��Ʈ ����
        var inputPort = GetPortInstance(tempQuestNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        tempQuestNode.inputContainer.Add(inputPort);

        tempQuestNode.RefreshExpandedState();
        tempQuestNode.RefreshPorts();
        tempQuestNode.SetPosition(new Rect(position,
            DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

        // ������Ʈ �ʵ�
        var objectField = new ObjectField("Quest Giver");
        tempQuestNode.mainContainer.Add(objectField);



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
        // Start node ����
        var nodeCache = new QuestNode()
        {
            title = "START",
            guid = Guid.NewGuid().ToString(),
            questText = "ENTRYPOINT",
            entryPoint = true
        };

        // next ��Ʈ ����
        var generatedPort = GetPortInstance(nodeCache, Direction.Output);
        generatedPort.portName = "Next";

        // ��� �����̳ʿ� �߰�
        nodeCache.outputContainer.Add(generatedPort);

        //nodeCache.capabilities &= ~Capabilities.Movable;
        nodeCache.capabilities &= ~Capabilities.Deletable;

        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        nodeCache.SetPosition(new Rect(100, 200, 100, 150));
        return nodeCache;
    }
}
