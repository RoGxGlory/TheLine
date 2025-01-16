using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private float constantPushForce = 20f; // Constant push force applied regardless of player speed

    private Rigidbody rb;

    // REF to the score manager
    ScoreManager scoreManager;

    // REF to the game manager
    GameStateManager gameManager;

    // REF to the level generator
    LevelGenerator levelGenerator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the player object.");
        }
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found !");
        }
        gameManager = FindObjectOfType<GameStateManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found !");
        }
        levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator == null)
        {
            Debug.LogError("LevelGenerator not found !");
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        // Check for collision with objects tagged as "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player hit an obstacle.");
            // Call a game manager or handle the game over logic
            GameStateManager.Instance.GameOver();
        }
        else if(collision.gameObject.CompareTag("SpawnObstacle"))
        {
            Debug.Log("Player hit an obstacle.");
            // Call a game manager or handle the game over logic
            GameStateManager.Instance.GameOver();
        }
        else
        {
            Debug.Log("Collided with a non-obstacle object.");
            // Collisions with other objects (non-obstacle) are handled naturally by Rigidbody physics
            HandleNonObstacleCollision(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check for collision with objects classified as "Bonuses"
        if (collider.gameObject.CompareTag("Coin"))
        {
            Debug.Log("Player took a coin.");
            scoreManager.AddScore(5);
            gameManager.UpdateCurrentScore(scoreManager.CurrentScore);
            Destroy(collider.gameObject);
        }
        if (collider.gameObject.CompareTag("ScoreMultiplier"))
        {
            Debug.Log("Player took a score multiplier.");
            scoreManager.scoreMultiplier += 0.2f;
            Destroy(collider.gameObject);
            Debug.Log(scoreManager.scoreMultiplier);
        }
        if (collider.gameObject.CompareTag("Speed"))
        {
            Debug.Log("Player took a speed multiplier.");
            levelGenerator.UpdateSpeed();
            Destroy(collider.gameObject);
            Debug.Log(levelGenerator.moveSpeedMultiplier);
        }
    }

    private void HandleNonObstacleCollision(GameObject nonObstacle)
    {
        // Apply a strong, constant push force to the non-obstacle object
        Rigidbody objectRb = nonObstacle.GetComponent<Rigidbody>();
        if (objectRb != null)
        {
            Vector3 pushDirection = (nonObstacle.transform.position - transform.position).normalized;
            objectRb.AddForce(pushDirection * constantPushForce, ForceMode.Impulse);
        }
    }
}
