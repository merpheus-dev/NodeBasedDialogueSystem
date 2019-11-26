using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class NarrativeNodeContainer
{
    public string Guid;
    public List<KeyValuePair<string, string>> PortGuidPair;

    public NarrativeNodeContainer(string nodeGuid)
    {
        Guid = nodeGuid;
        PortGuidPair = new List<KeyValuePair<string, string>>();
    }

    public void AddPortConnection(string portString, CanvasNode targetNode)
    {
        PortGuidPair.Add(new KeyValuePair<string, string>(portString,targetNode.GUID));
    }
}
