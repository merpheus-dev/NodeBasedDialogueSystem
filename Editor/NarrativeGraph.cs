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
        toolbar.Add(new Button(()=>DataParser.SaveNodes(currentInstance.edges.ToList()))
        {
            text = "Save Data",
        });
        toolbar.Add(new Button(()=>
        {
            var _node = CreateNode("StoryLine");
            currentInstance.AddElement(_node);
            _node.RefreshPorts();
        })
        {
            text = "Create Node",
        });
        rootVisualElement.Add(toolbar);
        //rootVisualElement.Add(group);
        var node = CreateEntryPointNode();

        //node.SetPosition(new Rect(0, 0, 0, 0));
//        var node2 = CreateNode("There is a weird guy");
//        var node3 = CreateNode("Called Mert");
        //node.RemoveFromHierarchy();
        currentInstance.AddElement(node);
        DataParser.SetEntryPoint(node);
//
//        currentInstance.AddElement(node2);
//        currentInstance.AddElement(node3);
        
        //currentInstance.Add();
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
        //node2.RefreshPorts();

        // currentInstance.Add(edge);

//        var stack = new StackNode();
//        stack.Add(new Label("Example Stack"));
//        currentInstance.Add(stack);


//        var minimap = new MiniMap();
//        minimap.SetPosition(new Rect(0, 0, 100, 100));
//        currentInstance.Add(minimap);

    }

    public void GetNodeData()
    {
        currentInstance.edges.ForEach(x => { Debug.Log("Input:" + x.input?.node.title + " |Output:" + x.output?.node.title); });
     //currentInstance.nodes.ForEach(x => { Debug.Log(x.title);});
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
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };
        nodeCache.mainContainer.Add(new Label(nodeName));
        // nodeCache.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
//InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        AddPort(nodeCache);

       var edgeListener = new EdgeConnectionListener(this);
        PortSocket realPort2 =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float),
                edgeListener);
        realPort2.portName = "Input";
        nodeCache.inputContainer.Add(realPort2);

        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        
        var button = new Button(() =>
        {
            AddPort(nodeCache);
        })
        {
            text = "New Branch"
        };
        nodeCache.Add(button);
        
        var textField = new TextField(nodeCache.title);
        textField.RegisterValueChangedCallback(evt => { nodeCache.DialogueText = evt.newValue;});
        nodeCache.Add(textField);

        
        return nodeCache;
    }

    private void AddPort(CanvasNode nodeCache)
    {
        var edgeConnectionListener = new EdgeConnectionListener(this);
        PortSocket realPort3 =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float),
                edgeConnectionListener);
        realPort3.portName = "Output";
        nodeCache.outputContainer.Add(realPort3);
        nodeCache.RefreshPorts();
        nodeCache.RefreshExpandedState();
    }
    
    private CanvasNode CreateEntryPointNode()
    {
        var nodeCache = new CanvasNode()
        {
            title = "START"
        };

        // nodeCache.extensionContainer.style.backgroundColor = new Color(0.24f, 0.24f, 0.24f, 0.8f);
//InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
        var edgeListener = new EdgeConnectionListener(this);
        PortSocket realPort =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float),
                edgeListener);
        realPort.portName = "Next";
        nodeCache.outputContainer.Add(realPort);

        nodeCache.capabilities &= ~Capabilities.Movable;
        nodeCache.capabilities &= ~Capabilities.Deletable;
        
        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        return nodeCache;
    }
}