using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NarrativeDataWrapper : ScriptableObject
{
    public List<NarrativeData> NarrativeData = new List<NarrativeData>();
    public List<NarrativeTextData> NarrativeTextData = new List<NarrativeTextData>();
}