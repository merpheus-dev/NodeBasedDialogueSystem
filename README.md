# Node Based Dialogue System for Unity

This is a node-based visual narrative flow creation tool that uses Unity's GraphView API.
![](https://i.ibb.co/JngH8yr/header.png)

## Features
- Infinite Branching and Merging dialogue capability.
- Dialogue&Graph save/load system.
- Minimap for easy navigation.
- Search window for node creation.
- Blackboard and exposed property support.
- Comment blocks for grouping nodes.
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

## ExposedProperty data class
Exposed properties can hold **unique** property names and their non-unique values. You can set values via blackboard or set values at runtime with ```string.Replace(propertyName,runtimeValue);```
