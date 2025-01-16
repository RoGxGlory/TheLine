using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    // REF to the player
    public GameObject Player;
    [SerializeField] private Player player; 

    // UI Panels
    public GameObject homeUI;
    public GameObject inGameUI;
    public GameObject gameOverUI;
    public GameObject inGamePauseUI;
    public GameObject leaderboardUI;

    // UI Elements
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI finalScoreText;

    // REF to the level generator
    [SerializeField] private LevelGenerator levelGenerator;

    // REF to the score manager
    [SerializeField] private ScoreManager scoreManager;

    // REF to the transform resetter
    public TransformResetter transformResetter;

    // Timer to track spawn intervals
    public float timer;

    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("GameStateManager");
                _instance = singletonObject.AddComponent<GameStateManager>();
                DontDestroyOnLoad(singletonObject);
            }
            return _instance;
        }
    }

    public GameState CurrentState { get; private set; } = GameState.Menu;

    // Event for notifying subscribers of game state changes
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShowHomeUI();
    }

    private void Update()
    {
        // Input for pausing the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.InGame)
            {
                PauseGame();
            }
            else if (CurrentState == GameState.Pause)
            {
                ResumeGame();
            }
        }

        // Input for trace stopping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.bShouldTrace = !player.bShouldTrace;
        }

        if (CurrentState == GameState.InGame)
        {
            // Update the timer
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                scoreManager.AddScore(1);
                UpdateCurrentScore(scoreManager.CurrentScore);
                // levelGenerator.moveSpeedMultiplier += 0.01f;
                // levelGenerator.UpdateSpeed();
                timer = 0f;
            }
        }
    }

    // Home logic
    public void ShowHomeUI()
    {
        homeUI.SetActive(true);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(false);
        Player.SetActive(false);
        inGamePauseUI.SetActive(false);
        leaderboardUI.SetActive(false);
        //shopUI.SetActive(false);
    }

    public void StartGame()
    {
        // Reset the player's position
        transformResetter.ResetPlayerPosition();
        Player.SetActive(true);
        player.bIsPlaying = true;
        

        CurrentState = GameState.InGame;
        Time.timeScale = 1f;
        Debug.Log("Game Started");

        // Makes sure the game objects are cleanly deleted
        ClearGameObjects();


        // UI logic
        homeUI.SetActive(false);
        inGameUI.SetActive(true);
        gameOverUI.SetActive(false);
        inGamePauseUI.SetActive(false);
        ScoreManager.Instance.ResetScore();
        UpdateCurrentScore(0);

        // Game logic
        scoreManager.bIsScoreSubmitted = false;
        levelGenerator.bIsPlaying = true;
        levelGenerator.SpawnPrefab();
        scoreManager.ResetScoreMultiplier();
        levelGenerator.ResetSpeed();
        levelGenerator.UpdateSpeed();

        
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
        player.bIsPlaying = false;
        // Show Game Over UI or handle Game Over logic

        // Display final score and high score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Your Score: " + ScoreManager.Instance.CurrentScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + ScoreManager.Instance.HighScore;
            if (ScoreManager.Instance.CurrentScore > ScoreManager.Instance.HighScore)
            {
                ScoreManager.Instance.SaveScore();
            }
        }

        ShowGameOverUI();
    }

    public void ShowGameOverUI()
    {
        homeUI.SetActive(false);
        inGameUI.SetActive(false);
        gameOverUI.SetActive(true);
        inGamePauseUI.SetActive(false);
        Player.SetActive(false);
        transformResetter.ResetPlayerPosition();
    }
    public void PauseGame()
    {
        CurrentState = GameState.Pause;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
        // Show Pause UI
        inGamePauseUI.SetActive(true);
        player.bIsPlaying = false;
    }

    public void ResumeGame()
    {
        CurrentState = GameState.InGame;
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
        // Hide Pause UI
        inGamePauseUI.SetActive(false);
        player.bIsPlaying = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        transformResetter.ResetPlayerPosition();
        ClearGameObjects();
        StartGame();
        player.bIsPlaying = true;
    }

    public void ReturnToMenu()
    {
        CurrentState = GameState.Menu;
        Time.timeScale = 1f;
        Debug.Log("Returning to Menu");
        // Load Menu Scene
        ShowHomeUI();
        player.bIsPlaying = false;
    }

    public void BackToHome()
    {
        Time.timeScale = 1f; // Resume the game
        ShowHomeUI();
    }

    public void ShowLeaderboardUI()
    {
        homeUI.SetActive(false);
        leaderboardUI.SetActive(true);
    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentState == newState)
        {
            Debug.LogWarning("Game state is already " + newState);
            return;
        }

        CurrentState = newState;
        Debug.Log("Game state changed to: " + newState);

        // Notify subscribers about the state change
        OnGameStateChanged?.Invoke(newState);

        // Additional logic for state transitions can be added here
    }

    public bool IsGameState(GameState state)
    {
        return CurrentState == state;
    }

    // In-Game logic
    public void UpdateCurrentScore(float score)
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = "Score: " + score;
        }
    }

    public void ClearGameObjects()
    {
        GameObject[] Obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject Obstacle in Obstacles)
        {
            Destroy(Obstacle);
        }
        GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject Cube in Cubes)
        {
            Destroy(Cube);
        }
        GameObject[] Coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject Coin in Coins)
        {
            Destroy(Coin);
        }
        GameObject[] SPMultipliers = GameObject.FindGameObjectsWithTag("Speed");
        foreach (GameObject SPMultiplier in SPMultipliers)
        {
            Destroy(SPMultiplier);
        }
        GameObject[] SCMultipliers = GameObject.FindGameObjectsWithTag("ScoreMultiplier");
        foreach (GameObject SCMultiplier in SCMultipliers)
        {
            Destroy(SCMultiplier);
        }
    }
}
public enum GameState
{
    Menu,
    InGame,
    GameOver,
    Pause
}