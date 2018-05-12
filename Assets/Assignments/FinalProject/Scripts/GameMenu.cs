using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

    public static bool isOnePlayerGame = true;
    public static bool isAIPlayerGame = false;

    public static int livesPlayerOne;
    public static int livesPlayerTwo;

    public static int playerOnePelletsConsumed = 0;
    public static int playerTwoPelletsConsumed = 0;

    public Text playerText1;
    public Text playerText2;
    public Text playerText3;
    public Text playerSelector;

    private int selectionIndex = 0;

    private void Start()
    {
        isOnePlayerGame = true;
        isAIPlayerGame = false;
        livesPlayerOne = 0;
        livesPlayerTwo = 0;
        playerOnePelletsConsumed = 0;
        playerTwoPelletsConsumed = 0;
        selectionIndex = 0;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (selectionIndex == 0)
            {
                isOnePlayerGame = true;
                selectionIndex = 0;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText1.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
            else if (selectionIndex == 1)
            {
                isOnePlayerGame = true;
                selectionIndex -= 1;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText1.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
            else if (selectionIndex == 2)
            {
                isOnePlayerGame = false;
                isAIPlayerGame = false;
                selectionIndex -= 1;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText2.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (selectionIndex == 0)
            {
                isOnePlayerGame = false;
                selectionIndex += 1;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText2.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
            else if (selectionIndex == 1)
            {
                isOnePlayerGame = true;
                isAIPlayerGame = true;
                selectionIndex += 1;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText3.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
            else if (selectionIndex == 2)
            {
                isOnePlayerGame = true;
                isAIPlayerGame = true;
                selectionIndex = 2;
                playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x, playerText3.transform.localPosition.y, playerSelector.transform.localPosition.z);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Return))
        {
            livesPlayerOne = 3;
            livesPlayerTwo = 3;

            if (isOnePlayerGame)
            {
                livesPlayerTwo = 0;
            }
            if (isAIPlayerGame)
            {
                livesPlayerOne = 100;
            }
            SceneManager.LoadScene("PacMan");
        }
	}
}
