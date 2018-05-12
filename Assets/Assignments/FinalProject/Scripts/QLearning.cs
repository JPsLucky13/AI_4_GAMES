using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureGoAfterPill
{
    public double weight = 15.0;
    public int FeatureFunction(GameObject pacMan)
    {
        float distance = Mathf.Infinity;
        //Get all the neighboring nodes
        NodePacMan pacManCurrentNode = pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position);

        if (pacManCurrentNode)
        {
            NodePacMan[] neighbors = pacManCurrentNode.neighbors;

            for (int i = 0; i < neighbors.Length; i++)
            {
                float shortestDistance = Vector3.Distance(pacManCurrentNode.transform.position, neighbors[i].transform.position);

                if (shortestDistance < distance)
                {
                    distance = shortestDistance;
                }

            }
        }


        if (distance < 7.0f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public FeatureGoAfterPill()
    {
    }
}
public class FeatureGoAfterPowerPill
{
    public double weight = 25.0;
    public int FeatureFunction(GameObject pacMan)
    {
        float distance = Mathf.Infinity;
        //Get all the neighboring nodes
        NodePacMan pacManCurrentNode = pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position);

        if (pacManCurrentNode)
        {
            NodePacMan[] neighbors = pacManCurrentNode.neighbors;

            for (int i = 0; i < neighbors.Length; i++)
            {
                float shortestDistance = Vector3.Distance(pacManCurrentNode.transform.position, neighbors[i].transform.position);

                if (shortestDistance < distance)
                {
                    distance = shortestDistance;
                }

            }
        }

        if (distance < 7.0f)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public FeatureGoAfterPowerPill()
    {
    }
}
public class FeatureGoAfterEdibleGhost
{
    public double weight = 70.0;
    public int FeatureFunction(NodePacMan node)
    {

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        float closestGhostDistance = float.MaxValue;

        foreach (GameObject ghost in ghosts)
        {
            if (ghost.GetComponent<Ghost>().currentMode == Ghost.Mode.Frightened)
            {
                float shortestDistance = Vector3.Distance(node.transform.position, ghost.transform.position);

                if (shortestDistance < closestGhostDistance)
                {
                    closestGhostDistance = shortestDistance;
                }
            }
        }

        if (closestGhostDistance < 9.0f)
        {
            return 1;
        }
        else
            return 0;
    }

    public FeatureGoAfterEdibleGhost()
    { }

}
public class FeatureGhostIsClose
{
    public double weight = -99.0;
    public int FeatureFunction(NodePacMan node)
    {

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        float closestGhostDistance = float.MaxValue;

        foreach (GameObject ghost in ghosts)
        {
            if (ghost.GetComponent<Ghost>().currentMode == Ghost.Mode.Chase || ghost.GetComponent<Ghost>().currentMode == Ghost.Mode.Scatter)
            {
                float shortestDistance = Vector3.Distance(node.transform.position, ghost.transform.position);

                if (shortestDistance < closestGhostDistance)
                {
                    closestGhostDistance = shortestDistance;
                }
            }
        }

        if (closestGhostDistance < 3.0f)
        {
            return 1;
        }
        else
            return 0;
    }

    public FeatureGhostIsClose()
    { }

}
public class FeatureDangerousDirection
{
    public double weight = -99.0;
    public int FeatureFunction(NodePacMan node)
    {

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        float closestGhostDistance = float.MaxValue;

        foreach (GameObject ghost in ghosts)
        {
            if (ghost.GetComponent<Ghost>().currentMode == Ghost.Mode.Chase || ghost.GetComponent<Ghost>().currentMode == Ghost.Mode.Scatter)
            {
                float shortestDistance = Vector3.Distance(node.transform.position, ghost.transform.position);

                if (shortestDistance < closestGhostDistance)
                {
                    closestGhostDistance = shortestDistance;
                }
            }
        }

        if (closestGhostDistance < 3.0f)
        {
            return 1;
        }
        else
            return 0;
    }

    public FeatureDangerousDirection()
    { }

}

public class QLearning : MonoBehaviour{

    double discountFactor = 0.8;
    double learningRate = 0.7;
    int numberOfFeatures = 4;


    public List<NodePacMan> nodes;
    public Tile[] tiles;
    List<int> features;
    List<double> weights;

    private Vector2 bestDirection;
    private Dictionary<int, NodePacMan> nodeDictionary = new Dictionary<int, NodePacMan>();

    public enum Move
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public static Move bestMove = Move.LEFT;

    public double [,] qMatrix = new double[66, 66];
    public double[,] rMatrix = new double[66, 66];

    private GameObject pacMan;

    private void Start()
    {
        if(GameMenu.isAIPlayerGame)
        {
            pacMan = GameObject.Find("PacMan");
            features = new List<int>();
            weights = new List<double>();
            fillQMatrix();
            fillNodeDictionary();
            fillRewardMatrix();
        }
       
    }

    private void Update()
    {
        if (!GameBoard.didStartDeath && GameMenu.isAIPlayerGame)
        {
            NodePacMan pacManCurrentNode = pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position);

            if (pacManCurrentNode != null)
            {
                if (Vector3.Distance(pacMan.transform.position,pacManCurrentNode.transform.position) < 0.01f)
                {
                    fillRewardMatrix();

                    CalculateQValue(pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position));

                    //Get the best move
                    bestMove = GetMove();

                }
            }
        }
    }


    private void fillQMatrix()
    {
        for (int i = 0; i < 66; i++)
        {
            for (int j = 0; j < 66; j++)
            {
                qMatrix[i, j] = 0;
            }
        }
    }
    private void fillRewardMatrix()
    {
        for (int i = 0; i < 66; i++)
        {
            for (int j = 0; j < 66; j++)
            {
                NodePacMan node = nodeDictionary[i];
                NodePacMan otherNeighbor = nodeDictionary[j];

                if (i == j)
                {
                    rMatrix[i, j] = 0.0;
                }
                else
                {
                    foreach (NodePacMan neighbour in node.neighbors)
                    {
                        if (neighbour.name == otherNeighbor.name)
                        {
                            if(neighbour.GetComponent<Tile>().isPellet && !neighbour.GetComponent<Tile>().didConsumePlayerOne)
                                rMatrix[i, j] = 25.0;
                            else if (neighbour.GetComponent<Tile>().isPowerPellet && !neighbour.GetComponent<Tile>().didConsumePlayerOne)
                                rMatrix[i, j] = 50.0;
                            else if (neighbour.GetComponent<Tile>().isPortal)
                                rMatrix[i, j] = 10.0;
                            else
                                rMatrix[i, j] = 0.0;
                        }
                    }
                }
            }
        }
    }
    private void fillNodeDictionary()
    {
        int index = 0;

        foreach (NodePacMan node in nodes)
        {
            nodeDictionary.Add(index, node);
            index++;
        }
    }
    private Move GetMove()
    {
        //Get all the neighboring nodes
        NodePacMan pacManCurrentNode = pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position);

        if (pacManCurrentNode)
        {
            NodePacMan[] neighbors = pacManCurrentNode.neighbors;

            double reward = 0;

            for (int i = 0; i < neighbors.Length; i++)
            {
                int pacManCurrentNodeIndex = nodes.IndexOf(pacManCurrentNode);
                int neighborIndex = nodes.IndexOf(neighbors[i]);
                double rewardFromNeighbor = qMatrix[pacManCurrentNodeIndex, neighborIndex];

                if (rewardFromNeighbor > reward)
                {
                    reward = rewardFromNeighbor;
                    bestDirection = pacManCurrentNode.validDirections[i];
                }

            }
        }

        return GetDirection(bestDirection);

    }
    private Move GetDirection(Vector2 bestDirection)
    {
        if (bestDirection.x == -1 && bestDirection.y == 0)
        {
            return Move.LEFT;
        }
        else if (bestDirection.x == 1 && bestDirection.y == 0)
        {
            return Move.RIGHT;
        }
        else if (bestDirection.x == 0 && bestDirection.y == 1)
        {
            return Move.UP;
        }
        else if (bestDirection.x == 0 && bestDirection.y == -1)
        {
            return Move.DOWN;
        }

        return Move.DOWN;
    }

    private void CalculateQValue(NodePacMan node)
    {
        //Assign the new Q value
        NodePacMan pacManCurrentNode = pacMan.GetComponent<PacMan>().GetNodeAtPosition(pacMan.transform.position);

        if (pacManCurrentNode)
        {
            NodePacMan[] neighbors = pacManCurrentNode.neighbors;

            for (int i = 0; i < neighbors.Length; i++)
            {
                FeatureGoAfterPill goafterPill = new FeatureGoAfterPill();
                FeatureGoAfterPowerPill goafterPowerPill = new FeatureGoAfterPowerPill();
                FeatureGoAfterEdibleGhost goafterEdibleGhost = new FeatureGoAfterEdibleGhost();
                FeatureGhostIsClose ghostIsClose = new FeatureGhostIsClose();
                FeatureDangerousDirection dangerousDirection = new FeatureDangerousDirection();

                features.Add(goafterPill.FeatureFunction(pacMan));
                features.Add(goafterPowerPill.FeatureFunction(pacMan));
                features.Add(goafterEdibleGhost.FeatureFunction(neighbors[i]));
                features.Add(ghostIsClose.FeatureFunction(neighbors[i]));
                features.Add(dangerousDirection.FeatureFunction(pacManCurrentNode));

                weights.Add(goafterPill.weight);
                weights.Add(goafterPowerPill.weight);
                weights.Add(goafterEdibleGhost.weight);
                weights.Add(ghostIsClose.weight);
                weights.Add(dangerousDirection.weight);

                double qValue = 0.0;

                for (int j = 0; j < numberOfFeatures; j++)
                {
                    qValue += features[j] * weights[j];
                }

                int pacManCurrentNodeIndex = nodes.IndexOf(pacManCurrentNode);
                int neighborIndex = nodes.IndexOf(neighbors[i]);
                double newQValue = learningRate * (rMatrix[pacManCurrentNodeIndex, neighborIndex] + discountFactor * qValue);
                qMatrix[pacManCurrentNodeIndex, neighborIndex] = newQValue;

                features.Clear();
                weights.Clear();
            }
        }

        

    }

}
