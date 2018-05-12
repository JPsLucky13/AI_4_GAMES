using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeGrid : MonoBehaviour {

    [SerializeField] private int numberOfTiles;
    [SerializeField] private GameObject gridTile;
    [SerializeField] private GameObject boundCube;
    [SerializeField] private float offset = 1.0f;
    [SerializeField] private GameObject snake;
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private Text scoreText;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material pathMaterial;

    public GameObject apple;

    //Adjacency Matrix of the nodes
    private List<List<byte>> adjacencyMatrix;

    private List<GridNodeRecord> closed;
    private List<GridNodeRecord> open;
    private List<GridNode> gridNodes;
    private List<GameObject> gridTiles;
    private List<MeshRenderer> gridTilesMeshRenderers;

    private int score;

    public class GridNode
    {
        public int nodeIndex;
        public Vector3 position;
        public int connectedEdges = 0;
        public int maxEdges;
        public byte blocked = 0;
    }

    public class GridNodeRecord
    {
        public GridNode node;
        public GridNodeConnection connection;
        public float costSoFar;
        public float estimatedTotalCost;
    }

    public class GridNodeConnection
    {
        public GridNode toNode;
        public GridNode fromNode;
        public float cost;
    }

    private void Start()
    {
        score = 0;
        gridTiles = new List<GameObject>();
        gridTilesMeshRenderers = new List<MeshRenderer>();
        closed = new List<GridNodeRecord>();
        open = new List<GridNodeRecord>();
        adjacencyMatrix = new List<List<byte>>(numberOfTiles);
        gridNodes = new List<GridNode>();
        GenerateTileSpace();
        FillAdjacencyMatrix();
        CreateAdjacencyMatrix();
        SpawnSnakeObject();
        SpawnApple();
        GenerateBounds();
    }
    private void GenerateTileSpace()
    {
        int nodeIndex = 0;
        //Create the board to play snake
        for (int i = 0; i < numberOfTiles; i++)
        {
            for (int j = 0; j < numberOfTiles; j++)
            {
                GameObject newGridTile = Instantiate(gridTile, new Vector3(i * offset, 0.0f, j * offset), Quaternion.identity);
                newGridTile.name = "Node " + nodeIndex;
                gridTiles.Add(newGridTile);
                gridTilesMeshRenderers.Add(newGridTile.GetComponent<MeshRenderer>());
                GridNode newGridnode = new GridNode();
                newGridnode.nodeIndex = nodeIndex;
                newGridnode.blocked = 0;
                nodeIndex++;
                newGridnode.position = new Vector3(i * offset, offset,j * offset);
                gridNodes.Add(newGridnode);
            }
        }

        
    }
    public GridNode GetGridNodeFromPosition(GameObject go)
    {
        for (int i = 0; i < gridNodes.Count; i++)
        {
            if (gridNodes[i].position == go.transform.position)
            {
                return gridNodes[i];
            }
        }
        return null;
    }
    public GridNode GetGridNodeFromPosition(Vector3 newPosition)
    {
        for (int i = 0; i < gridNodes.Count; i++)
        {
            if (gridNodes[i].position == newPosition)
            {
                return gridNodes[i];
            }
        }
        return null;
    }
    private void FillAdjacencyMatrix()
    {
        for (int i = 0; i < gridNodes.Count; i++)
        {
            adjacencyMatrix.Add(new List<byte>(gridNodes.Count));
            for (int j = 0; j < gridNodes.Count; j++)
            {
                adjacencyMatrix[i].Add(0);
            }
        }
    }
    private void SpawnSnakeObject()
    {
        Instantiate(snake, new Vector3(gridNodes[Random.Range(0, gridNodes.Count - 1)].position.x, offset, gridNodes[Random.Range(0, gridNodes.Count - 1)].position.z), Quaternion.identity);
    }
    public void SpawnApple()
    {
        Vector3 newPosition = new Vector3(gridNodes[Random.Range(0, gridNodes.Count - 1)].position.x, offset, gridNodes[Random.Range(0, gridNodes.Count - 1)].position.z);
        GridNode newGridNode = GetGridNodeFromPosition(newPosition);
        while (newGridNode.blocked == 1)
        {
            newPosition = new Vector3(gridNodes[Random.Range(0, gridNodes.Count - 1)].position.x, offset, gridNodes[Random.Range(0, gridNodes.Count - 1)].position.z);
            newGridNode = GetGridNodeFromPosition(newPosition);
        }

        apple = Instantiate(applePrefab, newPosition, Quaternion.identity);
    }
    public void CreateAdjacencyMatrix()
    {
        for (int i = 0; i < gridNodes.Count; i++)
        {
            for (int j = 0; j < gridNodes.Count; j++)
            {
                //The same node
                if (i == j)
                {
                    adjacencyMatrix[i][j] = 0;
                }

                //Nodes are within a certain distance so they are connected
                else if (Vector3.Distance(gridNodes[i].position, gridNodes[j].position) <= 1.0f)
                {
                    if (gridNodes[j].blocked == 0)
                    {
                        if (adjacencyMatrix[i][j] == 0)
                        {
                            adjacencyMatrix[i][j] = 1;
                            adjacencyMatrix[j][i] = 1;
                        }
                    }
                    else
                    {
                        adjacencyMatrix[i][j] = 0;
                    }
                    
                }
            }
        }
    }

    public List<GridNodeRecord> Pathfinding(GameObject snakeHead, GameObject apple)
    {
        //Run Dijkstra and Astar for the max amount of iterations
        GridNode startNode = GetGridNodeFromPosition(snakeHead);
        GridNode endNode = GetGridNodeFromPosition(apple);

        ///A Star
        List<GridNodeRecord> path = Astar(startNode, endNode);
        return path;
    }
    private GridNodeRecord GetSmallestElement(List<GridNodeRecord> openList)
    {
        float minDistance = Mathf.Infinity;

        GridNodeRecord targetNode = new GridNodeRecord();

        for(int i = 0; i < openList.Count;i++)
        {
            if (openList[i].estimatedTotalCost < minDistance)
            {
                minDistance = openList[i].estimatedTotalCost;
                targetNode = openList[i];
            }
        }
        open.Remove(targetNode);
        return targetNode;
    }
    private List<GridNodeRecord> Astar(GridNode start, GridNode end)
    {

        //Initialize start node
        GridNodeRecord startRecord = new GridNodeRecord();
        startRecord.node = start;
        startRecord.connection = null;
        startRecord.costSoFar = 0;
        startRecord.estimatedTotalCost = 0;

        //Initialize the open and closed lists
        open = new List<GridNodeRecord>();
        open.Add(startRecord);
        closed = new List<GridNodeRecord>();

        //Find the smallest element in the open list with estimated total cost
        GridNodeRecord current = new GridNodeRecord();
        //Iterate through processing each node
        while (open.Count > 0)
        {
            current = GetSmallestElement(open);
            closed.Add(current);

            //If it is the goal node then terminate
            if (current.node.nodeIndex == end.nodeIndex)
                break;

            //Otherwise get the connections
            List<GridNodeConnection> connections = RetrieveConnections(current.node);

            //Loop through each connection in turn
            for(int i =0; i < connections.Count;i++) 
            {
                //Get the cost estimate for the end node
                GridNode toNode = connections[i].toNode;
                float toNodeCost = current.costSoFar + connections[i].cost;

                GridNodeRecord endNodeRecord = new GridNodeRecord();

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
                endNodeRecord.connection = connections[i];
                endNodeRecord.estimatedTotalCost = toNodeCost + Vector3.Distance(toNode.position, end.position);

                if (!Contains(open, toNode))
                {
                    open.Add(endNodeRecord);
                }
            }
        }

        if (current.node.nodeIndex != end.nodeIndex)
        {
            return null;
        }
        else
        {
            List<GridNodeRecord> path = new List<GridNodeRecord>();

            while (current.node.nodeIndex != startRecord.node.nodeIndex)
            {
                path.Add(current);
                current = Find(closed, current.connection.fromNode);
            }

            path.Reverse();
            DisplayPath(path);
            return path;
        }
    }
    private List<GridNodeConnection> RetrieveConnections(GridNode node)
    {
        List<GridNodeConnection> connections = new List<GridNodeConnection>();
        List<byte> connectionArray = adjacencyMatrix[node.nodeIndex];
        for (int i = 0; i < connectionArray.Count; i++)
        {
            //The same node
            if (i != node.nodeIndex && connectionArray[i] == 1)
            {
                GridNodeConnection connection = new GridNodeConnection();
                connection.toNode = gridNodes[i];
                connection.fromNode = node;
                connection.cost = Vector3.Distance(connection.toNode.position, connection.fromNode.position);
                connections.Add(connection);
            }
        }
        return connections;
    }
    private bool Contains(List<GridNodeRecord> nodeRecordList, GridNode endNode)
    {
        for(int i = 0; i < nodeRecordList.Count; i++)
        {

            if (nodeRecordList[i].node.nodeIndex == endNode.nodeIndex)
            {
                return true;
            }
        }
        return false;
    }
    public void IncreaseScore()
    {
        score++;
        scoreText.text = "Snake Score: " + score.ToString();
    }
    private GridNodeRecord Find(List<GridNodeRecord> nodeRecordList, GridNode endNode)
    {
        for (int i = 0; i < nodeRecordList.Count; i++)
        {
            if (nodeRecordList[i].node.nodeIndex == endNode.nodeIndex)
            {
                return nodeRecordList[i];
            }
        }
        return null;
    }
    private void DisplayPath(List<GridNodeRecord> path)
    {
        for (int i = 0; i < gridTilesMeshRenderers.Count; i++)
        {
            gridTilesMeshRenderers[i].material = defaultMaterial;
        }


        for ( int i = 0; i < path.Count; i++)
        {
            for (int j = 0; j < gridTilesMeshRenderers.Count; j++)
            {
                Vector3 position = new Vector3(gridTiles[j].transform.position.x, offset, gridTiles[j].transform.position.z);
                if (position == path[i].node.position)
                {
                    gridTilesMeshRenderers[j].material = pathMaterial;
                }
            }
        } 

    }
    private void GenerateBounds()
    {
        //Lower bounds
        for (int i = 0; i < numberOfTiles; i++)
        {
            Instantiate(boundCube, new Vector3(-1.0f, offset, i),Quaternion.identity);
        }

        ////Right bounds
        for (int i = 0; i < numberOfTiles; i++)
        {
            Instantiate(boundCube, new Vector3(i, offset, -1.0f), Quaternion.identity);
        }

        //////Left bounds
        float maxBound = numberOfTiles;
        for (int i = 0; i < numberOfTiles; i++)
        {
            Instantiate(boundCube, new Vector3(i, offset, maxBound ), Quaternion.identity);
        }

        //////Upper bounds
        for (int i = 0; i < numberOfTiles; i++)
        {
            Instantiate(boundCube, new Vector3(maxBound, offset, i), Quaternion.identity);
        }
    }
}
