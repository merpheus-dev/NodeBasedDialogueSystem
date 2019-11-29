using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Subtegral.DialogueSystem.Editor
{
    public class EdgeConnectionListener : IEdgeConnectorListener
    {
        public void OnDropOutsidePort(Edge edge, Vector2 position){}

        public void OnDrop(GraphView graphView, Edge edge)
        {
            if (edge.output.connected) return;
            edge?.input.Connect(edge);
            edge?.output.Connect(edge);
            graphView.Add(edge);
        }
    }
}