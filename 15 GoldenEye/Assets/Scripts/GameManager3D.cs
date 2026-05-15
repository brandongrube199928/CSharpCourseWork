using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//Manages Health, UI, WIN/LOSE conditions, and other game related things for the 3D version of the game
// This is a singleton class, meaning there will only be one instance of it in the game.
// This is useful for managing game state and UI across different scenes without having to worry about multiple instances conflicting with each other.
// The Awake() method ensures that if an instance already exists, any new instances will be destroyed, maintaining the singleton pattern.
// This documentary is for me to remember how to use singletons in Unity, and to explain the structure of the GameManager3D class for future reference.
// I remade this project with Gemini and Copilot; full credit to them!

public class GameManager3D : MonoBehaviour
{
   //Make the object for the singleton-something loop
   public static GameManager3D Instance;

    // UI elements
    [Header("UI")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI healthText;

    // Gamebool for if the game is over or not
    private bool gameOver = false;

    // void to set the singleton loop

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowMessage("");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            // Reload the current scene to restart the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Backup way to reload the scene if the above line doesn't work for some reason
            /// UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void UpdateHealthUI(int current, int max)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {current} / {max}";
        }
    }

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
    }

    public void Win()
    {
        gameOver = true;
        ShowMessage("You win! Press R to restart!");
    }

    public void Lose()
    {
        gameOver = true;
        ShowMessage("You lost! Press R to restart!");
    }
}
