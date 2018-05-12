using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    public float moveSpeed = 5.9f;
    public float normalMoveSpeed = 5.9f;
    public float frightenedModeMoveSpeed = 2.9f;
    public float consumedMoveSpeed = 15f;

    public bool canMove = true;

    public int pinkyReleaseTimer = 5;
    public int inkyReleaseTimer = 14;
    public int clydeReleaseTimer = 21;
    public float ghostReleaseTimer = 0.0f;

    public int frightenedModeDuration = 10;
    public int startBlinkingAt = 7;

    public bool isInGhostHouse = false;

    public NodePacMan startingPosition;
    public NodePacMan homeNode;
    public NodePacMan ghostHouse;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public Animator ghostAnimator;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0.0f;

    private float frightenedModeTimer = 0;
    private float blinkTimer = 0;

    private bool frightenedModeIsWhite = false;

    private float previousMoveSpeed;

    private AudioSource backgroundAudio;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    }

    public Mode currentMode = Mode.Scatter;
    Mode previousMode;

    public enum GhostType
    {
        Red,
        Pink,
        Blue,
        Orange
    }

    public GhostType ghostType = GhostType.Red;

    private GameObject pacMan;
    private NodePacMan currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    // Use this for initialization
    void Start() {

        if (GameBoard.isPlayerOneUp)
        {
            SetDifficultyForLevel(GameBoard.playerOneLevel);
        }
        else
        {
            SetDifficultyForLevel(GameBoard.playerTwoLevel);
        }
        

        backgroundAudio = GameObject.Find("Game").transform.GetComponent<AudioSource>();
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        NodePacMan node = GetNodeAtPosition(transform.position);
        if (node != null)
        {
            currentNode = node;
        }

        if (isInGhostHouse)
        {
            direction = Vector2.up;
            targetNode = currentNode.neighbors[0];
        }
        else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();
        }
        previousNode = currentNode;

        UpdateAnimatorController();
    }

    public void SetDifficultyForLevel(int level)
    {
        if (level == 1)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 5;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 20;

            frightenedModeDuration = 10;
            startBlinkingAt = 7;

            pinkyReleaseTimer = 5;
            inkyReleaseTimer = 14;
            clydeReleaseTimer = 21;

            moveSpeed = 5.9f;
            normalMoveSpeed = 5.9f;
            frightenedModeMoveSpeed = 2.9f;
            consumedMoveSpeed = 15f;

        }
        else if (level == 2)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 9;
            startBlinkingAt = 6;

            pinkyReleaseTimer = 4;
            inkyReleaseTimer = 12;
            clydeReleaseTimer = 18;

            moveSpeed = 6.9f;
            normalMoveSpeed = 6.9f;
            frightenedModeMoveSpeed = 3.9f;
            consumedMoveSpeed = 18f;

        }
        else if (level == 3)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 8;
            startBlinkingAt = 5;

            pinkyReleaseTimer = 3;
            inkyReleaseTimer = 10;
            clydeReleaseTimer = 15;

            moveSpeed = 7.9f;
            normalMoveSpeed = 7.9f;
            frightenedModeMoveSpeed = 4.9f;
            consumedMoveSpeed = 20f;
        }
        else if (level == 4)
        {
            scatterModeTimer1 = 7;
            scatterModeTimer2 = 7;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1033;

            frightenedModeDuration = 7;
            startBlinkingAt = 4;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 8;
            clydeReleaseTimer = 13;

            moveSpeed = 8.9f;
            normalMoveSpeed = 8.9f;
            frightenedModeMoveSpeed = 5.9f;
            consumedMoveSpeed = 22f;
        }
        else if (level == 5)
        {
            scatterModeTimer1 = 5;
            scatterModeTimer2 = 5;
            scatterModeTimer3 = 5;
            scatterModeTimer4 = 1;

            chaseModeTimer1 = 20;
            chaseModeTimer2 = 20;
            chaseModeTimer3 = 1037;

            frightenedModeDuration = 6;
            startBlinkingAt = 3;

            pinkyReleaseTimer = 2;
            inkyReleaseTimer = 6;
            clydeReleaseTimer = 10;

            moveSpeed = 9.9f;
            normalMoveSpeed = 9.9f;
            frightenedModeMoveSpeed = 6.9f;
            consumedMoveSpeed = 24f;
        }
    }

    public void MoveToStartingPosition()
    {
        if (transform.name != "Ghost_Blinky")
        {
            isInGhostHouse = true;
        }

        transform.position = startingPosition.transform.position;

        if (isInGhostHouse)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = Vector2.left;
        }

        UpdateAnimatorController();
    }

    public void Restart()
    {
        canMove = true;
        currentMode = Mode.Scatter;
        moveSpeed = normalMoveSpeed;
        previousMoveSpeed = 0;
        transform.position = startingPosition.transform.position;
        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;

        if (transform.name != "Ghost_Blinky")
        {
            isInGhostHouse = true;
        }

        currentNode = startingPosition;

        if (isInGhostHouse)
        {
            direction = Vector2.up;
            targetNode = currentNode.neighbors[0];
        }
        else
        {
            direction = Vector2.left;
            targetNode = ChooseNextNode();
        }

        previousNode = currentNode;
        UpdateAnimatorController();
    }
    

    // Update is called once per frame
    void Update () {

        if (canMove)
        {
            ModeUpdate();
            Move();
            ReleaseGhosts();
            CheckCollision();
            CheckIsInGhostHouse();
        }

    }

    private void CheckIsInGhostHouse()
    {
        if (currentMode == Mode.Consumed)
        {
            GameObject tile = GetTileAtPosition(transform.position);

            if (tile != null)
            {
                if (tile.GetComponent<Tile>() != null)
                {
                    if (tile.transform.GetComponent<Tile>().isGhostHouse)
                    {
                        moveSpeed = normalMoveSpeed;

                        NodePacMan node = GetNodeAtPosition(transform.position);

                        if (node != null)
                        {
                            currentNode = node;
                            direction = Vector2.up;
                            targetNode = currentNode.neighbors[0];

                            previousNode = currentNode;
                            currentMode = Mode.Chase;
                            UpdateAnimatorController();
                        }
                    }
                }
            }
        }
    }

    private void CheckCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size/4);
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size/4);

        if (ghostRect.Overlaps(pacManRect))
        {
            if (currentMode == Mode.Frightened)
            {
                Consumed();
            }
            else if(currentMode != Mode.Consumed)
            {
                GameObject.Find("Game").GetComponent<GameBoard>().StartDeath();
            }
        }
    }

    private void Consumed()
    {
        if (GameMenu.isOnePlayerGame)
        {
            GameBoard.playerOneScore += GameBoard.ghostConsumedRunningScore;
        }
        else
        {
            if (GameBoard.isPlayerOneUp)
            {
                GameBoard.playerOneScore += GameBoard.ghostConsumedRunningScore;
            }
            else
            {
                GameBoard.playerTwoScore += GameBoard.ghostConsumedRunningScore;
            }
        }

        currentMode = Mode.Consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedMoveSpeed;
        UpdateAnimatorController();

        GameObject.Find("Game").transform.GetComponent<GameBoard>().StartConsumed(this.GetComponent<Ghost>());

        GameBoard.ghostConsumedRunningScore = GameBoard.ghostConsumedRunningScore * 2;
    }

    private void UpdateAnimatorController()
    {
        if (currentMode != Mode.Frightened && currentMode != Mode.Consumed)
        {
            ghostAnimator.enabled = true;

            if (direction == Vector2.up)
            {
                ghostAnimator.Play("Up");
            }
            else if (direction == Vector2.down)
            {
                ghostAnimator.Play("Down");
            }
            else if (direction == Vector2.left)
            {
                ghostAnimator.Play("Left");
            }
            else if (direction == Vector2.right)
            {
                ghostAnimator.Play("Right");
            }
            else
            {
                ghostAnimator.Play("Left");
            }
        }

        else if (currentMode == Mode.Frightened)
        {
            ghostAnimator.enabled = true;
            ghostAnimator.Play("Frightened");
        }
        else if (currentMode == Mode.Consumed)
        {
            ghostAnimator.enabled = false;

            if (direction == Vector2.up)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            }
            else if (direction == Vector2.down)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            }
            else if (direction == Vector2.left)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            }
        }

    }

    private void Move()
    {
        if (targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
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

                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;
                UpdateAnimatorController();
            }
            else
            {
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void ModeUpdate()
    {
        if (currentMode != Mode.Frightened)
        {
            modeChangeTimer += Time.deltaTime;

            if (modeChangeIteration == 1)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                {
                    modeChangeIteration = 2;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 2)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                {
                    modeChangeIteration = 3;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 3)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }

                if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                {
                    modeChangeIteration = 4;
                    ChangeMode(Mode.Scatter);
                    modeChangeTimer = 0;
                }
            }
            else if (modeChangeIteration == 4)
            {
                if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                {
                    ChangeMode(Mode.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
        else if (currentMode == Mode.Frightened)
        {
            frightenedModeTimer += Time.deltaTime;

            if (frightenedModeTimer >= frightenedModeDuration)
            {
                backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioNormal;
                backgroundAudio.Play();

                frightenedModeTimer = 0;
                ChangeMode(previousMode);
            }

            if (frightenedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;

                if (blinkTimer >= 0.1f)
                {
                    blinkTimer = 0.0f;

                    if (frightenedModeIsWhite)
                    {
                        ghostAnimator.Play("Frightened");
                        frightenedModeIsWhite = false;
                    }
                    else
                    {
                        ghostAnimator.Play("White");
                        frightenedModeIsWhite = true;
                    }
                }
            }
        }
    }

    private void ChangeMode(Mode m)
    {
        if (currentMode == Mode.Frightened)
        {
            moveSpeed = previousMoveSpeed;
        }
        if (m == Mode.Frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frightenedModeMoveSpeed;
        }
        if (currentMode != m)
        {
            previousMode = currentMode;
            currentMode = m;
        }
        UpdateAnimatorController();
    }

    public void StartFrightenedMode()
    {
        if (currentMode != Mode.Consumed)
        {
            GameBoard.ghostConsumedRunningScore = 200;

            frightenedModeTimer = 0;
            backgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudioFrightened;
            backgroundAudio.Play();
            ChangeMode(Mode.Frightened);
        }
    }

    private Vector2 GetRedGhostTargetTile()
    {
        Vector2 pacManPosition = pacMan.transform.position;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        return targetTile;
    }

    private Vector2 GetPinkGhostTargetTile()
    {
        //Four tiles ahead of PacMan
        //taking into account the position and rotation

        Vector2 pacManPosition = pacMan.transform.position;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (4 * pacManOrientation);

        return targetTile;
    }

    private Vector2 GetBlueGhostTargetTile()
    {
        //Select the position 2 tiles in front if pac man
        //Draw vector from blinky to that position
        //double the length of the vector
        Vector2 pacManPosition = pacMan.transform.position;
        Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;

        int pacManPositionX = Mathf.RoundToInt(pacManPosition.x);
        int pacManPositionY = Mathf.RoundToInt(pacManPosition.y);

        Vector2 pacManTile = new Vector2(pacManPositionX, pacManPositionY);
        Vector2 targetTile = pacManTile + (2 * pacManOrientation);

        //Temporary Blinky Position
        Vector2 tempBlinkyPosition = GameObject.Find("Ghost_Blinky").transform.position;
        int blinkyPositionX = Mathf.RoundToInt(tempBlinkyPosition.x);
        int blinkyPositionY = Mathf.RoundToInt(tempBlinkyPosition.y);
        tempBlinkyPosition = new Vector2(blinkyPositionX, blinkyPositionY);
        float distance = GetDistance(tempBlinkyPosition, targetTile);
        distance *= 2;
        targetTile = new Vector2(tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);
        return targetTile;
    }

    private Vector2 GetOrangeGhostTargetTile()
    {
        //Calculate the distance from Pac Man
        //If the distance is greater than eight tiles targeting is the same as blinky
        //If the distance is less than eight tiles, the target is his home node, so same as scatter mode.
        Vector2 pacManPosition = pacMan.transform.position;

        float distance = GetDistance(transform.position, pacManPosition);
        Vector2 targetTile = Vector2.zero;

        if (distance > 8)
        {
            targetTile = new Vector2(Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
        }
        else if (distance < 8)
        {
            targetTile = homeNode.transform.position;
        }
        return targetTile;
    }

    private Vector2 GetTargetTile()
    {
        Vector2 targetTile = Vector2.zero;

        if (ghostType == GhostType.Red)
        {
            targetTile = GetRedGhostTargetTile();
        }
        else if (ghostType == GhostType.Pink)
        {
            targetTile = GetPinkGhostTargetTile();
        }
        else if (ghostType == GhostType.Blue)
        {
            targetTile = GetBlueGhostTargetTile();
        }
        else if (ghostType == GhostType.Orange)
        {
            targetTile = GetOrangeGhostTargetTile();
        }
        return targetTile;
    }

    Vector2 GetRandomTile()
    {
        int x = Random.Range(0, 28);
        int y = Random.Range(0, 36);

        return new Vector2(x, y);
    }

    private void ReleasePinkGhost()
    {
        if (ghostType == GhostType.Pink && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    private void ReleaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    private void ReleaseOrangeGhost()
    {
        if (ghostType == GhostType.Orange && isInGhostHouse)
        {
            isInGhostHouse = false;
        }
    }

    private void ReleaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > pinkyReleaseTimer)
        {
            ReleasePinkGhost();
        }
        if (ghostReleaseTimer > inkyReleaseTimer)
        {
            ReleaseBlueGhost();
        }
        if (ghostReleaseTimer > clydeReleaseTimer)
        {
            ReleaseOrangeGhost();
        }
    }

    NodePacMan ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;

        if (currentMode == Mode.Chase)
        {
            targetTile = GetTargetTile();
        }
        else if (currentMode == Mode.Scatter)
        {
            targetTile = homeNode.transform.position;
        }
        else if (currentMode == Mode.Frightened)
        {
            targetTile = GetRandomTile();
        }
        else if (currentMode == Mode.Consumed)
        {
            targetTile = ghostHouse.transform.position;
        }


        NodePacMan moveToNode = null;

        List<NodePacMan> foundNodes = new List<NodePacMan>();

        List <Vector2> foundNodesDirection = new List<Vector2>();

        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] != direction * -1)
            {
                if (currentMode != Mode.Consumed)
                {
                    GameObject tile = GetTileAtPosition(currentNode.transform.position);

                    if (tile.transform.GetComponent<Tile>().isGhostHouseEntrance)
                    {
                        //Found ghost house, no movement
                        if (currentNode.validDirections[i] != Vector2.down)
                        {
                            foundNodes.Add(currentNode.neighbors[i]);
                            foundNodesDirection.Add(currentNode.validDirections[i]);
                        }
                    }
                    else
                    {
                        foundNodes.Add(currentNode.neighbors[i]);
                        foundNodesDirection.Add(currentNode.validDirections[i]);
                    }
                }
                else
                {
                    foundNodes.Add(currentNode.neighbors[i]);
                    foundNodesDirection.Add(currentNode.validDirections[i]);
                }
            }
        }

        if (foundNodes.Count == 1)
        {
            moveToNode = foundNodes[0];
            direction = foundNodesDirection[0];
        }

        if (foundNodes.Count > 1)
        {
            float leastDistance = Mathf.Infinity;

            for (int i = 0; i < foundNodes.Count; i++)
            {
                if (foundNodesDirection[i] != Vector2.zero)
                {
                    float distanceToNeighbor = GetDistance(currentNode.transform.position,foundNodes[i].transform.position);
                    float distance = distanceToNeighbor + GetDistance(foundNodes[i].transform.position, targetTile);

                    if (distance < leastDistance)
                    {
                        leastDistance = distance;
                        moveToNode = foundNodes[i];
                        direction = foundNodesDirection[i];
                    }
                }
            }
        }
        return moveToNode;
    }

    NodePacMan GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];

        if (tile != null)
        {
            if (tile.GetComponent<NodePacMan>() != null)
            {
                return tile.GetComponent<NodePacMan>();
            }
        }
        return null;
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").transform.GetComponent<GameBoard>().board[tileX, tileY];

        if (tile != null)
        {
            return tile;
        }
        return null;
    }

    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            if (tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                return otherPortal;
            }
        }
        return null;
    }

    private float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    private bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);
        return nodeToSelf > nodeToTarget;
    }

    private float GetDistance(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }
}
