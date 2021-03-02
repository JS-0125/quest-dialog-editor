using Subtegral.DialogueSystem.DataContainers;
using Subtegral.DialogueSystem.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractGraph : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
    public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);

    public Blackboard Blackboard = new Blackboard();
    public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();


    public abstract void CreateNewNode(string nodeName, Vector2 position);

    public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
    {
        if (commentBlockData == null)
            commentBlockData = new CommentBlockData();
        var group = new Group
        {
            autoUpdateGeometry = true,
            title = commentBlockData.Title
        };
        AddElement(group);
        group.SetPosition(rect);
        return group;
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
    {
        var localPropertyName = property.PropertyName;
        var localPropertyValue = property.PropertyValue;
        if (!loadMode)
        {
            while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                localPropertyName = $"{localPropertyName}(1)";
        }

        var item = ExposedProperty.CreateInstance();
        item.PropertyName = localPropertyName;
        item.PropertyValue = localPropertyValue;
        ExposedProperties.Add(item);

        var container = new VisualElement();
        var field = new BlackboardField { text = localPropertyName, typeText = "string" };
        container.Add(field);

        var propertyValueTextField = new TextField("Value:")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
            ExposedProperties[index].PropertyValue = evt.newValue;
        });
        var sa = new BlackboardRow(field, propertyValueTextField);
        container.Add(sa);
        Blackboard.Add(container);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort;

        ports.ForEach((port) =>
        {
            var portView = port;
            if (startPortView != portView && startPortView.node != portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    public void RemovePort(Node node, Port socket)
    {
        var targetEdge = edges.ToList()
            .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        node.outputContainer.Remove(socket);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }
}
