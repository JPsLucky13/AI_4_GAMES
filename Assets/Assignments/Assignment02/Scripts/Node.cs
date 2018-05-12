using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    public int nodeIndex;
    public Vector2 position;
    public int connectedEdges = 0;
    public int maxEdges;
}

public class NodeRecord
{
    public Node node;
    public Connection connection;
    public float costSoFar;
    public float estimatedTotalCost;
}

public class Connection
{
    public Node toNode;
    public Node fromNode;
    public float cost;
}
