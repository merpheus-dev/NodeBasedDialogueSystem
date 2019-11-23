using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

public class NarrativeGraph : EditorWindow
{
    private NarrativeGraphView currentInstance;

    [MenuItem("Graph/Narrative Graph")]
    public static void CreateGraphViewWindow()
    {
        var window = GetWindow<NarrativeGraph>();
        window.titleContent = new GUIContent("Narrative Graph");
    }

    private void OnEnable()
    {
        currentInstance = new NarrativeGraphView
        {
            name = "Narrative Graph"
        };
        currentInstance.StretchToParentSize();
        rootVisualElement.Add(currentInstance);

        var options = new VisualElement
        {
            style = {alignContent = Align.Center}
        };

        var toolbar = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row,
                flexGrow = 0,
                backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.75f)
            }
        };

        toolbar.Add(options);
        toolbar.Add(new Button()
        {
            text = "Explore Asset",
        });
        // rootVisualElement.Add(toolbar);
        //rootVisualElement.Add(group);
        var node = CreateNode("Once upon a time...");
        node.SetPosition(new Rect(0, 0, 0, 0));
        currentInstance.Add(node);
        var node2 = CreateNode("There is a weird guy");
        currentInstance.Add(node2);
        var node3 = CreateNode("Called Mert");
        currentInstance.Add(node3);
        //group.Add(node);
        //group.Focus();

//        var edge = new Edge
//        {
//            input = node.inputContainer[0] as Port,
//            output = node2.outputContainer[0] as Port
//        };
//        edge?.input.Connect(edge);
//        edge?.output.Connect(edge);

        node.RefreshPorts();
        node2.RefreshPorts();

       // currentInstance.Add(edge);

        var stack = new StackNode();
        stack.Add(new Label("Example Stack"));
        currentInstance.Add(stack);


//        var minimap = new MiniMap();
//        minimap.SetPosition(new Rect(0, 0, 100, 100));
//        currentInstance.Add(minimap);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(currentInstance);
    }

    private Node CreateNode(string nodeName)
    {
        var nodeCache = new CanvasNode()
        {
            title = nodeName,
            style =
            {
                width = 250.0f
            }
        };

        nodeCache.mainContainer.Add(new Label(nodeName));
        // nodeCache.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
//InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        var edgeListener = new EdgeConnectionListener(this);
        PortSocket realPort =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(float),
                edgeListener);
        realPort.portName = "Next";
        nodeCache.outputContainer.Add(realPort);

       edgeListener = new EdgeConnectionListener(this);
        PortSocket realPort2 = 
            nodeCache.AddPort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float),
                edgeListener);
        realPort2.portName = "Input";
        nodeCache.inputContainer.Add(realPort2);


        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        return nodeCache;
    }
    
}