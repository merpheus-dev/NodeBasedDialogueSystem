using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EdgeConnectionListener : IEdgeConnectorListener
{
    private NarrativeGraph _graph;

    public EdgeConnectionListener(NarrativeGraph graph)
    {
        _graph = graph;
    }
    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        Debug.Log("Drop outside");
        //if(!edge.isGhostEdge)
    }

    public void OnDrop(GraphView graphView, Edge edge)
    {
        edge?.input.Connect(edge);
        edge?.output.Connect(edge);
        graphView.Add(edge);
        Debug.Log("Drop inside");
    }
}
