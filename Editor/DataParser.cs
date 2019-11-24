using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DataParser
{
    private static Node _entryPointNode;

    public static void SetEntryPoint(Node entryPointNode)
    {
        _entryPointNode = entryPointNode;
    }

    public static void SaveNodes(IEnumerable<Edge> edges)
    {
        if (!edges.Any() || _entryPointNode==null) return;
        //Debug.Log(edges.First().input.node.title+ "Output"+edges.First().output.node.title);
        var firstConnection = edges.First(x => x.output.node == _entryPointNode).input.node;
        Debug.Log(firstConnection.title);
    }
}