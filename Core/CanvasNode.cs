using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CanvasNode : Node
{
    public PortSocket AddPort(Orientation portOrientation, Direction portDirection, Port.Capacity portCapacity,
        Type type,
        EdgeConnectionListener connectionListener)
    {
        var socket = new PortSocket(portOrientation, portDirection, portCapacity, type, connectionListener);
        return socket;
    }
}