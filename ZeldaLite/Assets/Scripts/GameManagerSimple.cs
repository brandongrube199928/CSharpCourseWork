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

    //Create Singleton Instance
    public static GameManagerSimple Instance;

    //Define UI Elements
    [Header("UI")]
    public TextMeshProUGUI scoreText; // UI element for score display
    public TextMeshProUGUI messageText; // UI element for message display
    public TextMeshProUGUI healthText; // UI element for health display

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
        //Handle Win/Lose and restart
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            //Reload the current scene to restart
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

    // Void to set health
    public void SetHealth(int current, int max)
    {
        if (healthText != null) healthText.text = $"Health: {current} / {max}";
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
    }

    //Void to lose
    public void Lose()
    {
        gameOver = true;
        ShowMessage("Game Over! Press R to Restart.");
    }

    //Void to update UI
    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }
}
