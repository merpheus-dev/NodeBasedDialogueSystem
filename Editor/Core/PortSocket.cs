using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PortSocket : Port
{
    public PortSocket(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type,
        EdgeConnectionListener connectionListener) : base(portOrientation, portDirection, portCapacity, type)
    {
        this.m_EdgeConnector = new EdgeConnector<Edge>(connectionListener);
        this.AddManipulator(edgeConnector);
    }

    public void EmptyLabel()
    {
        m_ConnectorText.text = "s";
    }
}