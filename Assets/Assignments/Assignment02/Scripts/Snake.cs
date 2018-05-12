using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

    [SerializeField] private GameObject snakeBodyPart;

    private List<GameObject> bodyParts;
    private List<Vector3> positions;
    private SnakeGrid snakeGrid;

    private bool gameOver = false;
    private List<SnakeGrid.GridNodeRecord> path;



    // Use this for initialization
    void Start () {
        path = new List<SnakeGrid.GridNodeRecord>();
        gameOver = false;
        snakeGrid = GameObject.Find("SnakeGrid").GetComponent<SnakeGrid>();
        bodyParts = new List<GameObject>();
        positions = new List<Vector3>();
        positions.Add(transform.position);
        GeneratePath();
    }
	
	// Update is called once per frame
	void Update () {

        //Special case when no path is found
        if (path == null  &&  !gameOver)
        {
            GeneratePath();
            positions[0] = transform.position;
            transform.position = FindBestPosition();
            SnakeGrid.GridNode previous = snakeGrid.GetGridNodeFromPosition(positions[0]);
            previous.blocked = 0;
            SnakeGrid.GridNode newGridNode = snakeGrid.GetGridNodeFromPosition(transform.position);
            newGridNode.blocked = 1;
            UpdateBodyParts();
        }
        else
        {
            if (path != null && path.Count > 0 && !gameOver)
            {
                GeneratePath();
                positions[0] = transform.position;
                transform.position = path[0].node.position;
                SnakeGrid.GridNode previous = snakeGrid.GetGridNodeFromPosition(positions[0]);
                previous.blocked = 0;
                SnakeGrid.GridNode newGridNode = snakeGrid.GetGridNodeFromPosition(transform.position);
                newGridNode.blocked = 1;
                UpdateBodyParts();
                path.Remove(path[0]);
            }
        }
        
    }

    private void UpdateBodyParts()
    {
        if (bodyParts.Count > 0)
        {
            for (int i = 0; i < bodyParts.Count; i++)
            {
                positions[i + 1] = bodyParts[i].transform.position;
                bodyParts[i].transform.position = positions[i];
                SnakeGrid.GridNode previous = snakeGrid.GetGridNodeFromPosition(positions[i + 1]);
                previous.blocked = 0;
                SnakeGrid.GridNode newGridNode = snakeGrid.GetGridNodeFromPosition(bodyParts[i].transform.position);
                newGridNode.blocked = 1;
            }
            snakeGrid.CreateAdjacencyMatrix();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //Th snake touches the apple
        if (other.gameObject.layer == 12)
        {
            CreateBodyPart();
            Destroy(other.gameObject);
            snakeGrid.IncreaseScore();
            snakeGrid.SpawnApple();
            GeneratePath();
        }   
    }
    private Vector3 FindBestPosition()
    {
        Vector3 bestDirection = new Vector3(0.0f, 0.0f, 0.0f);
        float maxDistance = 0.0f;

        RaycastHit forwardHit;
        Ray forwardRay = new Ray(transform.position + transform.forward * 0.55f, transform.forward);

        RaycastHit rightHit;
        Ray rightRay = new Ray(transform.position + transform.right * 0.55f, transform.right);

        RaycastHit leftHit;
        Ray leftRay = new Ray(transform.position - transform.right * 0.55f, -transform.right);

        RaycastHit backHit;
        Ray backRay = new Ray(transform.position - transform.forward * 0.55f, -transform.forward);

        if (Physics.Raycast(forwardRay, out forwardHit, LayerMask.GetMask("Snake", "Bound")))
        {
            if (maxDistance <= forwardHit.distance)
            {
                maxDistance = forwardHit.distance;
                bestDirection = transform.position + (transform.forward);
            }
        }
        if (Physics.Raycast(rightRay, out rightHit, LayerMask.GetMask("Snake", "Bound")))
        {
            if (maxDistance <= rightHit.distance)
            {
                maxDistance = rightHit.distance;
                bestDirection = transform.position + (transform.right);
            }
        }
        if (Physics.Raycast(leftRay, out leftHit, LayerMask.GetMask("Snake","Bound")))
        {
            if (maxDistance <= leftHit.distance)
            {
                maxDistance = leftHit.distance;
                bestDirection = transform.position - (transform.right);
            }
        }

        if (Physics.Raycast(backRay, out backHit, LayerMask.GetMask("Snake", "Bound")))
        {
            if (maxDistance <= backHit.distance)
            {
                maxDistance = backHit.distance;
                bestDirection = transform.position - (transform.forward);
            }
        }

        return bestDirection;

    }
    private void OnCollisionEnter(Collision collision)
    {
        //The snake hit itself
        if (collision.gameObject.layer == 11 || collision.gameObject.layer == 13)
        {
            gameOver = true;
        }
    }
    private void CreateBodyPart()
    {
        GameObject newSnakeBodyPart = Instantiate(snakeBodyPart, positions[positions.Count - 1], Quaternion.identity);
        positions.Add(newSnakeBodyPart.transform.position);
        bodyParts.Add(newSnakeBodyPart);
    }
    private void GeneratePath()
    {
        path = snakeGrid.Pathfinding(gameObject, snakeGrid.GetComponent<SnakeGrid>().apple);
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            positions[0] = transform.position;
            transform.position = new Vector3(transform.position.x + 1.0f, transform.position.y, transform.position.z);
            UpdateBodyParts();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            positions[0] = transform.position;
            transform.position = new Vector3(transform.position.x - 1.0f, transform.position.y, transform.position.z);
            UpdateBodyParts();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            positions[0] = transform.position;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.0f);
            UpdateBodyParts();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            positions[0] = transform.position;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.0f);
            UpdateBodyParts();
        }

    }

}
