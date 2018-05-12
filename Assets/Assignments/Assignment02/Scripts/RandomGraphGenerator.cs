using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGraphGenerator : MonoBehaviour {

    [SerializeField] private int numberOfNodes;
    [SerializeField] private float neighbourDistance;
    [SerializeField] private float boundsValue;
    [SerializeField] private int maxEdgesPerNode;
    [SerializeField] private GameObject representationObject;
    [SerializeField] private Material green;
    [SerializeField] private bool turnOnLogger;
    [SerializeField] private bool showRepresentation;

    //Adjacency Matrix of the nodes
    private List<List<byte>> adjacencyMatrix;
    private List<Node> graphNodes;

    private List<NodeRecord> closed;
    private List<NodeRecord> open;

    private bool isDijkstra;

    //Analysis
    private int maxIterations = 50;

    private float averageDijkstraRunTime = 0.0f;
    private float totalDijkstraRunTime = 0.0f;
    private long totalDijkstraNodesIntoOpen = 0;
    private long totalDijkstraNodesIntoClose = 0;
    private float averageDijkstraNodesIntoOpen = 0.0f;
    private float averageDijkstraNodesIntoClose = 0.0f;
    private float totalDijkstraCost = 0.0f;
    private float averageDijkstraCost = 0.0f;

    private float averageAStarRunTime = 0.0f;
    private float totalAStarRunTime = 0.0f;
    private long totalAStarNodesIntoOpen = 0;
    private long totalAStarNodesIntoClose = 0;
    private float averageAStarNodesIntoOpen = 0.0f;
    private float averageAStarNodesIntoClose = 0.0f;
    private float totalAStarCost = 0.0f;
    private float averageAStarCost = 0.0f;


    #region Unity Lifecycle
    private void Start()
    {
        adjacencyMatrix = new List<List<byte>>(numberOfNodes);
        graphNodes = new List<Node>();
        CreateGraph();
    }

    #endregion

    private void CreateGraph()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            CreateNewNode(i);
        }
        FillAdjacencyMatrix();
        CreateAdjacencyMatrix();

        if(showRepresentation)
            CreateEdgeRepresentation();

        Pathfinding();
    }
    private void FillAdjacencyMatrix()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            adjacencyMatrix.Add(new List<byte>(numberOfNodes));
            for (int j = 0; j < numberOfNodes; j++)
            {
                adjacencyMatrix[i].Add(0);
            }
        }
    }
    private void CreateNewNode(int index)
    {
        Node newNode = new Node();
        newNode.nodeIndex = index;
        newNode.position = new Vector2(Random.Range(0, boundsValue), Random.Range(0, boundsValue));
        newNode.maxEdges = maxEdgesPerNode;
        newNode.connectedEdges = 0;
        graphNodes.Add(newNode);
    }
    private void CreateAdjacencyMatrix()
    {

        for (int i = 0; i < numberOfNodes; i++)
        {
            for (int j = 0; j < numberOfNodes; j++)
            {
                //The same node
                if (i == j)
                {
                    adjacencyMatrix[i][j] = 0;
                }

                //Nodes are within a certain distance so they are connected
                else if (Vector2.Distance(graphNodes[i].position, graphNodes[j].position) < neighbourDistance)
                {
                    if (graphNodes[i].connectedEdges < graphNodes[i].maxEdges)
                    {
                        if (adjacencyMatrix[i][j] == 0)
                        {
                            adjacencyMatrix[i][j] = 1;
                            graphNodes[i].connectedEdges++;

                            adjacencyMatrix[j][i] = 1;
                            graphNodes[j].connectedEdges++;
                        }
                    }
                }
            }
        }
    }
    private void CreateEdgeRepresentation()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            GameObject representationNode = Instantiate(representationObject, new Vector3(graphNodes[i].position.x, 0.0f, graphNodes[i].position.y), Quaternion.identity);
            representationNode.name = "Node " + graphNodes[i].nodeIndex.ToString();

            for (int j = 0; j < numberOfNodes; j++)
            {
                //The same node
                if (i == j)
                {
                    continue;
                }

                else if (adjacencyMatrix[i][j] == 1)
                {
                    

                    GameObject line = new GameObject();
                    line.name = "Edge";
                    line.AddComponent<LineRenderer>();
                    line.GetComponent<LineRenderer>().widthMultiplier = 0.05f;
                    line.GetComponent<LineRenderer>().receiveShadows = false;
                    line.GetComponent<LineRenderer>().material = green;
                    line.GetComponent<LineRenderer>().generateLightingData = true;
                    line.GetComponent<LineRenderer>().startColor = Color.green;
                    line.GetComponent<LineRenderer>().endColor = Color.green;
                    line.GetComponent<LineRenderer>().SetPosition(0, new Vector3(graphNodes[i].position.x, 0.0f, graphNodes[i].position.y));
                    line.GetComponent<LineRenderer>().SetPosition(1, new Vector3(graphNodes[j].position.x, 0.0f, graphNodes[j].position.y));

                }
            }
        }
    }
    private void Pathfinding()
    {
        if (turnOnLogger)
        {
            Logger.instance.InitializeWriter(Application.dataPath + "/" + "PathfindingData.txt");
        }

        //Run Dijkstra and Astar for the max amount of iterations
        for (int i = 0; i < maxIterations; i++)
        {
            Node startNode = graphNodes[Random.Range(0, numberOfNodes)];
            Node endNode = graphNodes[Random.Range(0, numberOfNodes)];
            //print(startNode.position);
            //print(endNode.position);

            while (endNode.nodeIndex == startNode.nodeIndex)
            {
                endNode = graphNodes[Random.Range(0, numberOfNodes)];
            }

            ///A Star
            System.DateTime AStarStartTime = System.DateTime.Now;
            List<NodeRecord> path = Astar(startNode, endNode);
            System.DateTime AStarEndTime = System.DateTime.Now;

            ///Dijkstra
            System.DateTime DijkstaStartTime = System.DateTime.Now;
            List<NodeRecord> pathD = Dijkstra(startNode, endNode);
            System.DateTime DijkstaEndTime = System.DateTime.Now;

            totalAStarRunTime += (AStarEndTime - AStarStartTime).Milliseconds;
            totalDijkstraRunTime += (DijkstaEndTime - DijkstaStartTime).Milliseconds;

        }

        //End of iterations

        //Astar data
        averageAStarRunTime = totalAStarRunTime / maxIterations;
        averageAStarNodesIntoOpen = totalAStarNodesIntoOpen / maxIterations;
        averageAStarNodesIntoClose = totalAStarNodesIntoClose / maxIterations;
        averageAStarCost = totalAStarCost / maxIterations;

        //Dijkstra data
        averageDijkstraRunTime = totalDijkstraRunTime / maxIterations;
        averageDijkstraNodesIntoOpen = totalDijkstraNodesIntoOpen / maxIterations;
        averageDijkstraNodesIntoClose = totalDijkstraNodesIntoClose / maxIterations;
        averageDijkstraCost = totalDijkstraCost / maxIterations;

        if (turnOnLogger)
        {
            Logger.instance.WriteToFile("A Star Values: ");
            Logger.instance.WriteToFile("Average AStar RunTime: " + averageAStarRunTime);
            Logger.instance.WriteToFile("Average AStar Nodes Into Open List: " + averageAStarNodesIntoOpen);
            Logger.instance.WriteToFile("Average AStar Nodes Into Closed List: " + averageAStarNodesIntoClose);
            Logger.instance.WriteToFile("Average AStar Cost: " + averageAStarCost);

            Logger.instance.WriteToFile("Dijkstra Values: ");
            Logger.instance.WriteToFile("Average Dijkstra RunTime: " + averageDijkstraRunTime);
            Logger.instance.WriteToFile("Average Dijkstra Nodes Into Open List: " + averageDijkstraNodesIntoOpen);
            Logger.instance.WriteToFile("Average Dijkstra Nodes Into Closed List: " + averageDijkstraNodesIntoClose);
            Logger.instance.WriteToFile("Average Dijkstra Cost: " + averageDijkstraCost);
        }

        print("Finished logging");

    }
    private NodeRecord GetSmallestElement(List<NodeRecord> openList)
    {
        float minDistance = Mathf.Infinity;

        NodeRecord targetNode = new NodeRecord();

        foreach (NodeRecord nodeRecordChild in openList)
        {
                if (nodeRecordChild.estimatedTotalCost < minDistance)
                {
                    minDistance = nodeRecordChild.estimatedTotalCost;
                    targetNode = nodeRecordChild;
                }
        }
        open.Remove(targetNode);
        return targetNode;
    }

    private List<NodeRecord> Astar(Node start, Node end)
    {
        isDijkstra = false;

        //Initialize start node
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start;
        startRecord.connection = null;
        startRecord.costSoFar = 0;
        startRecord.estimatedTotalCost = 0;

        //Initialize the open and closed lists
        open = new List<NodeRecord>();
        open.Add(startRecord);
        totalAStarNodesIntoOpen++;
        closed = new List<NodeRecord>();

        //Find the smallest element in the open list with estimated total cost
        NodeRecord current = new NodeRecord();
        //Iterate through processing each node
        while (open.Count > 0)
        {
            current = GetSmallestElement(open);
            closed.Add(current);
            totalAStarNodesIntoClose++;

            //If it is the goal node then terminate
            if (current.node.nodeIndex == end.nodeIndex)
                break;

            //Otherwise get the connections
            List<Connection> connections = RetrieveConnections(current.node);

            //Loop through each connection in turn
            foreach (Connection connection in connections)
            {
                //Get the cost estimate for the end node
                Node toNode = connection.toNode;
                float toNodeCost = current.costSoFar + connection.cost;

                NodeRecord endNodeRecord = new NodeRecord();

                if (Contains(closed, toNode))
                {
                        continue;
                }

                else
                {
                    endNodeRecord.node = toNode;
                }

                //Update the node
                endNodeRecord.costSoFar = toNodeCost;
                endNodeRecord.connection = connection;
                endNodeRecord.estimatedTotalCost = toNodeCost + Vector2.Distance(toNode.position,end.position);

                if (!Contains(open, toNode))
                {
                    open.Add(endNodeRecord);
                    totalAStarNodesIntoOpen++;
                }
            }
        }

        if (current.node.nodeIndex != end.nodeIndex)
        {
            Debug.LogError("No path");
            return null;
        }
        else
        {
            List<NodeRecord> path = new List<NodeRecord>();

            while (current.node.nodeIndex != startRecord.node.nodeIndex)
            {
                path.Add(current);
                current = Find(closed, current.connection.fromNode);
                totalAStarCost += current.costSoFar;
            }

            path.Reverse();
            return path;
        }
    }
    private List<NodeRecord> Dijkstra(Node start, Node end)
    {
        isDijkstra = true;

        //Initialize start node
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = start;
        startRecord.connection = null;
        startRecord.costSoFar = 0;
        startRecord.estimatedTotalCost = 0;

        //Initialize the open and closed lists
        open = new List<NodeRecord>();
        open.Add(startRecord);
        totalDijkstraNodesIntoOpen++;
        closed = new List<NodeRecord>();

        //Find the smallest element in the open list with estimated total cost
        NodeRecord current = new NodeRecord();
        //Iterate through processing each node
        while (open.Count > 0)
        {
            current = GetSmallestElement(open);
            closed.Add(current);
            totalDijkstraNodesIntoClose++;

            //If it is the goal node then terminate
            if (current.node.nodeIndex == end.nodeIndex)
                break;

            //Otherwise get the connections
            List<Connection> connections = RetrieveConnections(current.node);

            //Loop through each connection in turn
            foreach (Connection connection in connections)
            {
                //Get the cost estimate for the end node
                Node toNode = connection.toNode;
                float toNodeCost = current.costSoFar + connection.cost;

                NodeRecord endNodeRecord = new NodeRecord();

                if (Contains(closed, toNode))
                {
                    continue;
                }

                else
                {
                    endNodeRecord.node = toNode;
                }

                //Update the node
                endNodeRecord.costSoFar = toNodeCost;
                endNodeRecord.connection = connection;
                endNodeRecord.estimatedTotalCost = toNodeCost;

                if (!Contains(open, toNode))
                {
                    open.Add(endNodeRecord);
                    totalDijkstraNodesIntoOpen++;
                }
            }
        }

        if (current.node.nodeIndex != end.nodeIndex)
        {
            Debug.LogError("No path");
            return null;
        }
        else
        {
            List<NodeRecord> path = new List<NodeRecord>();

            while (current.node.nodeIndex != startRecord.node.nodeIndex)
            {
                path.Add(current);
                current = Find(closed, current.connection.fromNode);
                totalDijkstraCost += current.costSoFar;
            }

            path.Reverse();
            return path;
        }
    }

    private List<Connection> RetrieveConnections(Node node)
    {
        List<Connection> connections = new List<Connection>();
        List<byte> connectionArray = adjacencyMatrix[node.nodeIndex];
        for (int i = 0; i < connectionArray.Count; i++)
        {
                //The same node
                if (i != node.nodeIndex && connectionArray[i] == 1)
                {
                    Connection connection = new Connection();
                    connection.toNode = graphNodes[i];
                    connection.fromNode = node;
                    connection.cost = Vector2.Distance(connection.toNode.position, connection.fromNode.position);
                    connections.Add(connection);
                }
        }
        return connections;
    }
    private bool Contains(List<NodeRecord> nodeRecordList, Node endNode)
    {
        foreach (NodeRecord nodeRecord in nodeRecordList)
        {

            if (nodeRecord.node.nodeIndex == endNode.nodeIndex)
            {
                return true;
            }
        }
        return false;
    }
    private NodeRecord Find(List<NodeRecord> nodeRecordList, Node endNode)
    {
        foreach (NodeRecord nodeRecord in nodeRecordList)
        {
            if (nodeRecord.node.nodeIndex == endNode.nodeIndex)
            {
                return nodeRecord;
            }
        }
        return null;
    }
}
