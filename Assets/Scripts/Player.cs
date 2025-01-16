using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    public Transform topBound; // Reference to the top boundary
    public Transform bottomBound; // Reference to the bottom boundary

    public float offset = 0.6f;

    public bool bIsPlaying = false;

    public bool bShouldTrace = true;

    private void OnEnable()
    {
        GameStateManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the player object.");
        }

        // Lock the Rigidbody's constraints to ensure movement only on the Z-axis
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (bIsPlaying && bShouldTrace)
        {
            // Snap to the mouse's position on screen
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, mousePosition.z);

            // Clamp the Z position within the bounds
            newPosition.z = Mathf.Clamp(newPosition.z, bottomBound.position.z + offset, topBound.position.z - offset);

            // Apply the clamped position to the player
            transform.position = newPosition;
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.InGame)
        {
            EnablePlayerControl();
        }
        else
        {
            DisablePlayerControl();
        }
    }

    private void EnablePlayerControl()
    {
        rb.velocity = Vector3.zero; // Reset velocity
        enabled = true;
    }

    private void DisablePlayerControl()
    {
        rb.velocity = Vector3.zero; // Stop movement
        enabled = false;
    }

    
}
