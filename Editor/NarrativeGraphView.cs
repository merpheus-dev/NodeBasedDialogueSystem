using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NarrativeGraphView : GraphView
{
    public NarrativeGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());
        // this.AddManipulator(new ContentZoomer());

        var grid = new GridBackground();


        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort as PortSocket;

        ports.ForEach((port) =>
        {
            var portView = port as PortSocket;
            if (startPortView != portView && startPortView.node!=portView.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }
}