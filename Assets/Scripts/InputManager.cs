using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Vector2 GetMovementInput()
    {
        // Keyboard input for vertical movement (visually horizontal in a top-down view)
        float verticalInput = Input.GetAxis("Vertical");

        // Mouse input for movement along the X-axis in a top-down 3D environment
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mousePosition.x; // Use X-axis for visually vertical movement

        // Combine both inputs (keyboard has priority)
        float movementX = verticalInput != 0 ? verticalInput : Mathf.Lerp(0, mouseX - Camera.main.transform.position.x, Time.deltaTime);

        return new Vector2(movementX, 0f);
    }
}
