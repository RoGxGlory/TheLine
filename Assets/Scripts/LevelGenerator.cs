using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> prefabList; // List of prefabs to spawn
    public Transform spawnPoint; // Spawn point offscreen
    public float moveSpeed = 2f; // Speed at which objects move toward the player
    public float destroyDelay = 10f; // Time after which spawned objects are destroyed

    private GameObject lastSpawnedPrefab; // Reference to the last spawned prefab
    private Transform landmark; // Landmark within the prefab

    GameObject spawnedObject = null;

    public bool bIsPlaying = false;   // Bool showing if the game is being played

    // REF to the object mover script
    ObjectMover Mover;

    public float moveSpeedMultiplier = 1f;

    void Start()
    {
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogError("Prefab list is empty! Assign prefabs in the Inspector.");
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned! Assign a spawn point in the Inspector.");
        }

        SpawnPrefab(); // Spawn the first prefab immediately
    }

    void Update()
    {
        if (bIsPlaying)
        {
            CheckLandmarkAndSpawn();
            // Debug.Log("Movespeed is : " + moveSpeed + ", movespeed x is : " + moveSpeedMultiplier);
        }

    }

    void CheckLandmarkAndSpawn()
    {
        if (lastSpawnedPrefab != null)
        {
            // Find the landmark within the last prefab
            if (landmark == null)
            {
                landmark = lastSpawnedPrefab.transform.Find("Landmark");
                if (landmark == null)
                {
                    Debug.LogError("Landmark not found in prefab! Make sure the prefab contains a child named 'Landmark'.");
                    return;
                }
            }

            float landmarkPositionX = landmark.position.x;
            float screenRightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, spawnPoint.position.z)).x;

            if (landmarkPositionX <= screenRightEdge)
            {
                // Spawn the next prefab when the landmark enters the screen
                SpawnPrefab();
                landmark = null; // Reset landmark for the next prefab
            }
        }
    }

    public void SpawnPrefab()
    {
        // Select a random prefab from the list
        GameObject prefab = prefabList[Random.Range(0, prefabList.Count)];

        // Instantiate the prefab at the spawn point
        spawnedObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Attach a movement script to the spawned object
        ObjectMover mover = spawnedObject.AddComponent<ObjectMover>();
        mover.SetMovement(Vector3.left, moveSpeed);

        if (spawnedObject.name == "Level 1(Clone)")
        {
            destroyDelay = 26f;
            Debug.Log("Delay set to : " + destroyDelay);
        }

        if (spawnedObject.name == "Level 2(Clone)")
        {
            destroyDelay = 31.2f;
            Debug.Log("Delay set to : " + destroyDelay);
        }

        // Destroy the object after a delay to avoid clutter
        Destroy(spawnedObject, destroyDelay);

        // Update the reference to the last spawned prefab
        lastSpawnedPrefab = spawnedObject;
    }

    public void UpdateSpeed()
    {
        moveSpeed = moveSpeed * moveSpeedMultiplier;
        ObjectMover mover = spawnedObject.GetComponent<ObjectMover>();
        mover.SetMovement(Vector3.left, moveSpeed);
    }

    public void ResetSpeed()
    {
        moveSpeedMultiplier = 1f;
        moveSpeed = 2f;
    }

    public void LowerSpeed()
    {
        moveSpeedMultiplier -= 1f; 
        UpdateSpeed();
    }
}
