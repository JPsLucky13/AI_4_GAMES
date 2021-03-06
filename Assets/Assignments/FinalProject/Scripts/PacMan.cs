﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

    [SerializeField] private float speed = 6.0f;

    public AudioClip chomp1;
    public AudioClip chomp2;
    public Vector2 orientation;
    public Sprite idleSprite;

    public bool canMove = true;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    private bool playedChomp1 = false;
    private AudioSource audioSource;

    private NodePacMan currentNode, previousNode, targetNode;

    private NodePacMan startingPosition;
	// Use this for initialization
	void Start () {
        audioSource = transform.GetComponent<AudioSource>();
        NodePacMan node = GetNodeAtPosition(transform.localPosition);

        startingPosition = node;

        if (node != null)
        {
            currentNode = node;
            //Debug.Log(currentNode);
        }

        direction = Vector2.left;
        orientation = Vector2.left;
        ChangePosition(direction);

        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }

	}

    public void SetDifficultyForLevel(int level)
    {
        if (level == 1)
        {
            speed = 6;
        }
        else if (level == 2)
        {
            speed = 7;
        }
        else if (level == 3)
        {
            speed = 8;
        }
        else if (level == 4)
        {
            speed = 9;
        }
        else if (level == 5)
        {
            speed = 10;
        }
    }

    public void MoveToStartingPosition()
    {
        transform.position = startingPosition.transform.position;

        transform.GetComponent<SpriteRenderer>().sprite = idleSprite;

        direction = Vector2.left;
        orientation = Vector2.left;

        UpdateOrientation();
    }

    public void Restart()
    {
        canMove = true;

        currentNode = startingPosition;
        nextDirection = Vector2.left;

        //Chomp animation
        transform.GetComponent<Animator>().Play("PacManAnimation");
        transform.GetComponent<Animator>().enabled = true;

        ChangePosition(direction);
    }

	// Update is called once per frame
	void Update () {
        if (canMove)
        {
            CheckInput();
            Move();
            UpdateOrientation();
            UpdateAnimationState();
            ConsumePellet();
        }
    }

    private void PlayChompSound()
    {
        if (playedChomp1)
        {
            audioSource.PlayOneShot(chomp2);
            playedChomp1 = false;
        }
        else
        {
            audioSource.PlayOneShot(chomp1);
            playedChomp1 = true;
        }
    }

    private void CheckInput()
    {
        if (!GameMenu.isAIPlayerGame)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangePosition(Vector2.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangePosition(Vector2.right);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangePosition(Vector2.up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangePosition(Vector2.down);
            }
        }

        else
        {
            //Get the direction from the Qlearning Algorithm to determine best move
            if (QLearning.bestMove == QLearning.Move.LEFT)
            {
                ChangePosition(Vector2.left);
            }
            else if (QLearning.bestMove == QLearning.Move.RIGHT)
            {
                ChangePosition(Vector2.right);
            }
            else if (QLearning.bestMove == QLearning.Move.UP)
            {
                ChangePosition(Vector2.up);
            }
            else if (QLearning.bestMove == QLearning.Move.DOWN)
            {
                ChangePosition(Vector2.down);
            }
        }
    }

    private void ChangePosition(Vector2 dir)
    {
        if (dir != direction)
        {
            nextDirection = dir;
        }
        if (currentNode != null)
        {
            NodePacMan moveToNode = CanMove(dir);

            if (moveToNode != null)
            {
                direction = dir;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    private void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (nextDirection == direction * -1)
            {
                direction *= -1;
                NodePacMan tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (OverShotTarget())
            {
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;

                    currentNode = otherPortal.GetComponent<NodePacMan>();
                }

                NodePacMan moveToNode = CanMove(nextDirection);

                if (moveToNode != null)
                {
                    direction = nextDirection;
                }
                if (moveToNode == null)
                {
                    moveToNode = CanMove(direction);
                }
                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
            }
        }
    }

    private void MoveToNode(Vector2 dir)
    {
        NodePacMan moveToNode = CanMove(dir);

        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    private void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            orientation = Vector2.left;
            transform.localScale = new Vector3(-1.0f,1.0f,1.0f);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (direction == Vector2.right)
        {
            orientation = Vector2.right;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (direction == Vector2.up)
        {
            orientation = Vector2.up;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }
        else if (direction == Vector2.down)
        {
            orientation = Vector2.down;
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        }
    }

    private void UpdateAnimationState()
    {
        if (direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    private void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);

        if (o != null)
        {
            Tile tile = o.GetComponent<Tile>();

            if (tile != null)
            {
                bool didConsume = false;

                if (GameBoard.isPlayerOneUp)
                {
                    if (!tile.didConsumePlayerOne && (tile.isPellet || tile.isPowerPellet))
                    {
                        didConsume = true;
                        tile.didConsumePlayerOne = true;

                        if (tile.isPowerPellet)
                            GameBoard.playerOneScore += 50;
                        else
                            GameBoard.playerOneScore += 10;

                        GameMenu.playerOnePelletsConsumed++;
                    }

                    if (tile.isBonusItem)
                        ConsumedBonusItem(1, tile);
                }
                else
                {
                    if (!tile.didConsumePlayerTwo && (tile.isPellet || tile.isPowerPellet))
                    {
                        didConsume = true;
                        tile.didConsumePlayerTwo = true;

                        if (tile.isPowerPellet)
                            GameBoard.playerTwoScore += 50;
                        else
                            GameBoard.playerTwoScore += 10;

                        GameMenu.playerTwoPelletsConsumed++;
                    }

                    if (tile.isBonusItem)
                        ConsumedBonusItem(2, tile);
                }

                if (didConsume)
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;

                    PlayChompSound();

                    if (tile.isPowerPellet)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

                        foreach (GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().StartFrightenedMode();
                        }
                    }
                }
            }
        }
    }

    void ConsumedBonusItem(int playerNum, Tile bonusItem)
    {
        if (playerNum == 1)
        {
            GameBoard.playerOneScore += bonusItem.pointValue;
        }
        else
        {
            GameBoard.playerTwoScore += bonusItem.pointValue;
        }

        GameObject.Find("Game").transform.GetComponent<GameBoard>().StartConsumedBonusItem(bonusItem.gameObject,bonusItem.pointValue);

    }

    private NodePacMan CanMove(Vector2 dir)
    {
        NodePacMan moveToNode = null;

        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] == dir)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }

        return moveToNode;
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX, tileY];

        if (tile != null)
        {
            return tile;
        }

        return null;
    }

    public NodePacMan GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<NodePacMan>();
        }

        return null;
    }

    private bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    private float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                    return otherPortal;
                }
            }
        }

        return null;
    }
}
