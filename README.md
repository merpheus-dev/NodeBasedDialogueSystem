# Node Based Dialogue System for Unity

This is a node-based visual narrative flow creation tool that uses Unity's GraphView API.
![](https://i.ibb.co/M2kJGnr/img.png)

## Features
- Infinite Branching and Merging dialogue capability.
- Dialogue&Graph save/load system.
- Minimap for easy navigation.
- Backed by Unity's embedded GraphView api.
- Sample provided in the package.

## Usage
- Graph generates dialogue saves into _Resources_ folder as a scriptable objects.
- Create a field as **DialogueContainer**
- Use **DialogueContainer** to access _DialogueNodeData_ and _NodeLinks_

## NodeLinks
Node Links is a serialized class that holds node connection and *branching* data.

## DialogueNodeData
Dialogue Node Data is holding Dialogue Node's Dialogue Text and node's position data for graph.
