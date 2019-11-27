using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DataParser
{
    private static CanvasNode _entryPointNode;

    private static IEnumerable<Edge> _edges;

    public static void SetEntryPoint(CanvasNode entryPointNode)
    {
        _entryPointNode = entryPointNode;
    }

    public static void SaveNodes(IEnumerable<Edge> edges, IEnumerable<Node> nodes, string fileName)
    {
        if (!edges.Any() || _entryPointNode == null) return;
        _edges = edges;
        var connectedSockets = edges.Where(x => x.input.node != null).ToArray();
        var narrativeParts = ScriptableObject.CreateInstance<NarrativeDataWrapper>();

        for (int i = 0; i < connectedSockets.Count(); i++)
        {
            var outputNode = (connectedSockets[i].output.node as CanvasNode);
            var inputNode = (connectedSockets[i].input.node as CanvasNode);
            narrativeParts.NarrativeData.Add(new NarrativeData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedSockets[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            });
        }

        foreach (var node in nodes.Cast<CanvasNode>())
        {
            if (node.EntyPoint) continue;
            narrativeParts.NarrativeTextData.Add(new NarrativeTextData
            {
                NodeGUID = node.GUID,
                DialogueText = node.DialogueText
            });
        }
        
        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets","Resources");
        AssetDatabase.CreateAsset(narrativeParts,$"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

        foreach (var narrativePart in narrativeParts.NarrativeData)
        {
            Debug.Log(
                $"{GetNodeName(narrativePart.BaseNodeGUID)}=[{narrativePart.PortName}]==>{GetNodeName(narrativePart.TargetNodeGUID)}");
        }
    }

    private static string GetNodeName(string guid)
    {
        var target = _edges.Where(x => (x.input.node as CanvasNode).GUID == guid);
        if (target.Count() <= 0) return string.Empty;
        var resultNode = target.First().input.node;
        if (resultNode == null)
        {
            resultNode = _edges.First(x =>
                (x.output.node as CanvasNode).GUID == guid).output.node;
            if (resultNode == null)
                throw new Exception("Node not found");
        }

        return (resultNode as CanvasNode).DialogueText;
    }
}