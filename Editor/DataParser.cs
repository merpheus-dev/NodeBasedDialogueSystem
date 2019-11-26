using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DataParser
{
    private static CanvasNode _entryPointNode;

    public static void SetEntryPoint(CanvasNode entryPointNode)
    {
        _entryPointNode = entryPointNode;
    }

    public static void SaveNodes(IEnumerable<Edge> edges)
    {
        if (!edges.Any() || _entryPointNode == null) return;

        var narrativeParts = new List<NarrativeNodeContainer>();
        CanvasNode currentNode = _entryPointNode;
        while (currentNode != null)
        {
            var connectedNodes =
                edges.Where(x => x.output.node == currentNode)
                    .Select(y => y.input.node).Cast<CanvasNode>();

            var nodeDataContainer = new NarrativeNodeContainer(currentNode.GUID);
            foreach (var connectedNode in connectedNodes)
            {
                foreach (var connectedPort in connectedNode.Outputs.Where(x => x.connected))
                {
                    
                    nodeDataContainer.AddPortConnection(connectedPort.portName, connectedNode);
                }
            }

            narrativeParts.Add(nodeDataContainer);
            if (connectedNodes.Count() == 0 || connectedNodes == null) break;
            currentNode = connectedNodes.First();
        }

        foreach (var narrativePart in narrativeParts)
        {
            foreach (var portPair in narrativePart.PortGuidPair)
            {
                var matchingNextNode = edges.Select(x => x.input.node).Cast<CanvasNode>().ToList()
                    .First(x => x.GUID == portPair.Value);
                var inceptionNode = edges.Select(x => x.output.node).Cast<CanvasNode>().ToList()
                    .First(x => x.GUID == narrativePart.Guid);
                Debug.Log($"{inceptionNode.DialogueText}=[{portPair.Key}]==>{matchingNextNode.DialogueText}");
            }
        }
    }

    private IEnumerator StepOverNodes(IEnumerable<Edge> edges)
    {
        CanvasNode currentNode = _entryPointNode;
        while (currentNode != null)
        {
            var connections =
                edges.Where(x => x.output.node == currentNode)
                    .Select(y => y.input.node); //edges.First(x => x.output.node == currentNode).input.node;
            foreach (var connection in connections)
            {
                Debug.Log(connection.title);
            }

            currentNode = connections.First() as CanvasNode;
            yield return null;
        }
    }
}