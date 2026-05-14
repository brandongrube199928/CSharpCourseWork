using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Global manager for score and UI
//Create an empty GameObject "GameManager" and attach this script to it.
//Assign ScoreText and MessageText in the inspector

public class GameManagerSimple : MonoBehaviour
{

    //Create Singleton instance
    public static GameManagerSimple Instance;

    //Ensure that all four are collected before winning. Assign in inspector.
    [Header("Collectibles")]
    public int diamondsCollected = 0;
    public int diamondsRequired = 4;


    //Define UI Elements
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;

    //Define Global Variables
    private int score = 0;
    private bool gameOver = false;

    //Simple singleton loop using void Awake()
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreUI();
        ShowMessage(""); //Empty at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            //Restart the game by reloading the scene
            Time.timeScale = 1f;   // unpause before reload; double checks that timescale is reset on win/lose
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    //Void to add score
    public void AddScore(int amount)
    {
        if (gameOver) return; //Don't add score if game is over
        score += amount;
        //Add method to update the UI
        UpdateScoreUI();
    }
    //Void to show message
    public void ShowMessage(string msg)
    {
        if (messageText != null) messageText.text = msg;
    }
    //Void to win
    public void Win()
    {
        gameOver = true;
        ShowMessage("You Win! Press R to Restart.");
        Time.timeScale = 0f;   // pause on win
    }

    //Void to collect diamonds to track progress towards winning.
    public void CollectDiamond()
    {
        diamondsCollected++;
        UpdateScoreUI(); //Ensure UI is properly updated when collecting diamonds
    }


    //Void to lose
    public void Lose()
    {
        gameOver = true;
        ShowMessage("Game Over! Press R to Restart.");
        //Set timescale to 0 to pause until restart
        Time.timeScale = 0f;
    }

    //Void to update UI
    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}
