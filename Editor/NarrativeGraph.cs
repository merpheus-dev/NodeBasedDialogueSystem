using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

public class NarrativeGraph : EditorWindow
{
    private NarrativeGraphView currentInstance;

    private string fileName = "NewNarrative";

    private Vector2 defaultNodeSize = new Vector2(200, 150);

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
        var entryPointNode = CreateEntryPointNode();

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
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);
        toolbar.Add(new Button(() =>
        {
            if (!string.IsNullOrEmpty(fileName))
                DataParser.SaveNodes(currentInstance.edges.ToList(), currentInstance.nodes.ToList(), fileName);
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        })
        {
            text = "Save Data",
        });

        toolbar.Add(new Button(() =>
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                LoadNarrative(fileName, entryPointNode);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        })
        {
            text = "Load Data"
        });
        toolbar.Add(new Button(() =>
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

        //node.SetPosition(new Rect(0, 0, 0, 0));
//        var node2 = CreateNode("There is a weird guy");
//        var node3 = CreateNode("Called Mert");
        //node.RemoveFromHierarchy();
        currentInstance.AddElement(entryPointNode);
        DataParser.SetEntryPoint(entryPointNode);
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

        entryPointNode.RefreshPorts();
        //node2.RefreshPorts();

        // currentInstance.Add(edge);

//        var stack = new StackNode();
//        stack.Add(new Label("Example Stack"));
//        currentInstance.Add(stack);


//        var minimap = new MiniMap();
//        minimap.SetPosition(new Rect(0, 0, 100, 100));
//        currentInstance.Add(minimap);
    }

    private void LoadNarrative(string fileName, CanvasNode entryPoint)
    {
        var narrativeObject = Resources.Load<NarrativeDataWrapper>(fileName);
        if (narrativeObject == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
            return;
        }


        #region Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too).

        var nodeList = currentInstance.nodes.ToList().Cast<CanvasNode>().ToList();
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].EntyPoint)
            {
                nodeList[i].GUID = narrativeObject.NarrativeData[0].BaseNodeGUID;
            }
            else
            {
                var edges = currentInstance.edges.ToList().Where(x => x.input.node == nodeList[i]);
                foreach (var edge in edges)
                {
                    currentInstance.RemoveElement(edge);
                }

                currentInstance.RemoveElement(nodeList[i]);
            }
        }

        #endregion

        #region Create All serialized nodes and assign their guid and dialogue text to them

        foreach (var perNode in narrativeObject.NarrativeTextData)
        {
            var tempNode = CreateNode(perNode.DialogueText,true);
            tempNode.GUID = perNode.NodeGUID;
            currentInstance.AddElement(tempNode);

            var nodePorts = narrativeObject.NarrativeData.Where(x => x.BaseNodeGUID == perNode.NodeGUID).ToList();
            nodePorts.ForEach(x=>AddPort(tempNode,x.PortName));
            tempNode.RefreshPorts();
        }

        #endregion

        //Load again after that dump
        nodeList = currentInstance.nodes.ToList().Cast<CanvasNode>().ToList();
        foreach (var perNode in nodeList)
        {
            var connections = narrativeObject.NarrativeData.Where(x => x.BaseNodeGUID == perNode.GUID);
            
            var targetNode = nodeList.First(x => x.GUID == connections.First().TargetNodeGUID);
            var tempEdge = new Edge()
            {
                output = perNode.outputContainer[0] as Port,
                input = targetNode.inputContainer[0] as Port
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            currentInstance.Add(tempEdge);
            targetNode.SetPosition(new Rect(new Vector2(100f, 0f), defaultNodeSize));
            
//            if (perNode.EntyPoint)
//            {
//                if (connections.Count() == 0)
//                    return; //If nothing is connected to entrypoint, not load any other node or connection
//
//                var targetNode = nodeList.First(x => x.GUID == connections.First().TargetNodeGUID);
//                var tempEdge = new Edge()
//                {
//                    output = perNode.outputContainer[0] as Port,
//                    input = targetNode.inputContainer[0] as Port
//                };
//                tempEdge?.input.Connect(tempEdge);
//                tempEdge?.output.Connect(tempEdge);
//                currentInstance.Add(tempEdge);
//                targetNode.SetPosition(new Rect(new Vector2(100f, 0f), defaultNodeSize));
//                continue;
//            }

//            foreach (var connection in connections)
//            {
//                AddPort(tempNode);
//               
//            }
        }
    }

    public void GetNodeData()
    {
        currentInstance.edges.ForEach(x =>
        {
            Debug.Log("Input:" + x.input?.node.title + " |Output:" + x.output?.node.title);
        });
        //currentInstance.nodes.ForEach(x => { Debug.Log(x.title);});
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(currentInstance);
    }

    private CanvasNode CreateNode(string nodeName, bool load = false)
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
        if (!load)
            AddPort(nodeCache);

        var edgeListener = new EdgeConnectionListener(this);
        PortSocket realPort2 =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float),
                edgeListener);
        realPort2.portName = "Input";
        nodeCache.inputContainer.Add(realPort2);

        nodeCache.RefreshExpandedState();
        nodeCache.RefreshPorts();
        nodeCache.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        var button = new Button(() => { AddPort(nodeCache); })
        {
            text = "New Branch"
        };
        nodeCache.Add(button);

        var textField = new TextField(nodeCache.title);
        textField.RegisterValueChangedCallback(evt => { nodeCache.DialogueText = evt.newValue; });
        nodeCache.Add(textField);


        return nodeCache;
    }

    private void AddPort(CanvasNode nodeCache, string overriddenPortName = "")
    {
        var edgeConnectionListener = new EdgeConnectionListener(this);
        PortSocket realPort3 =
            nodeCache.AddPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float),
                edgeConnectionListener);
        var portLabel = realPort3.contentContainer.Q<Label>();
        realPort3.contentContainer.Remove(portLabel);
        var outputPortCount = nodeCache.outputContainer.Query("connector").ToList().Count();
        var outputPortName = string.IsNullOrEmpty(overriddenPortName)
            ? $"Option {outputPortCount + 1}"
            : overriddenPortName;

        var textField = new TextField()
        {
            name = string.Empty,
            value = outputPortName
        };
        textField.RegisterValueChangedCallback(evt => realPort3.portName = evt.newValue);
        realPort3.contentContainer.Add(textField);
        realPort3.contentContainer.Add(new Label(" "));
        realPort3.portName = outputPortName;
        nodeCache.outputContainer.Add(realPort3);
        nodeCache.RefreshPorts();
        nodeCache.RefreshExpandedState();
    }

    private CanvasNode CreateEntryPointNode()
    {
        var nodeCache = new CanvasNode()
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntyPoint = true
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