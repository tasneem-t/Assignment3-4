using UnityEngine;
using TMPro;

public class ShardUIManager : MonoBehaviour
{
    public TMP_Text shardsText;
    public TMP_Text memoriesText;

    public static ShardUIManager instance;

    private int totalShards = 0;
    private int totalMemories = 0;

    [Header("Guiding Orb (next-level)")]
    public GameObject guidingOrbPrefab;
    public Transform guidingSpawnPoint;          // optional; leave null to spawn at player
    public Vector3 guidingTargetPosition = Vector3.zero; // optional override
    public float guidingSpeed = 3f;

    //NEW: auto-detected shards needed per memory in this scene
    [Header("Shards / Memory")]
    [Tooltip("If <= 0, it will be auto-set to number of OrbPickup objects in the scene.")]

    public int shardsNeededThisLevel = 0;
    [Header("Shards / Memory")]

    [Header("Progression Hooks")]
    [Tooltip("If true, spawn guiding orb when the first memory is unlocked.")]
    public bool spawnGuidingOrbOnFirstMemory = true;

    [Tooltip("Optional: barrier object to disable when the first memory is unlocked (e.g. glass wall).")]
    public GameObject barrierToDisableOnFirstMemory;

    [Tooltip("Optional: trigger object to enable when the first memory is unlocked (for scene transition).")]
    public GameObject transitionTriggerToEnableOnFirstMemory;

    [Header("Bounds Change On First Memory (optional)")]
    [Tooltip("If true, will expand the ghost movement bounds when the first memory is unlocked.")]
    public bool changeBoundsOnFirstMemory = false;

    [Tooltip("New min bounds for the ghost after unlocking (only used if changeBoundsOnFirstMemory is true).")]
    public Vector3 newMinBounds;

    [Tooltip("New max bounds for the ghost after unlocking (only used if changeBoundsOnFirstMemory is true).")]
    public Vector3 newMaxBounds;



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (shardsNeededThisLevel <= 0)
        {
            int orbCount = FindObjectsOfType<OrbPickup>().Length;
            shardsNeededThisLevel = Mathf.Max(1, orbCount);
            Debug.Log("ShardUIManager: Auto shardsNeededThisLevel = " + shardsNeededThisLevel);
        }
    }

    public void AddShard(int amount, int shardsNeeded)
    {
        totalShards += amount;
        if (shardsText != null) shardsText.text = "Shards Collected: " + totalShards;

        int needed = (shardsNeededThisLevel > 0) ? shardsNeededThisLevel : shardsNeeded;

        if (totalShards >= needed)
        {
            totalMemories++;
            totalShards = 0;
            if (memoriesText != null) memoriesText.text = "Memories Unlocked: " + totalMemories;
            if (shardsText != null) shardsText.text = "Shards Collected: " + totalShards;

            Debug.Log($"ShardUIManager: Memory unlocked. totalMemories={totalMemories}, needed={needed}");

            if (totalMemories == 1)
            {
                // 🔹 Level-specific behavior:
                if (spawnGuidingOrbOnFirstMemory)
                    SpawnGuidingOrb();  // Level 1 / others

                if (barrierToDisableOnFirstMemory != null)
                    barrierToDisableOnFirstMemory.SetActive(false);  // Level 2: remove glass wall

                if (transitionTriggerToEnableOnFirstMemory != null)
                    transitionTriggerToEnableOnFirstMemory.SetActive(true);  // optional unlock of trigger
                if (changeBoundsOnFirstMemory)
                {
                    var player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        var mover = player.GetComponent<move>();
                        if (mover != null)
                        {
                            mover.minBounds = newMinBounds;
                            mover.maxBounds = newMaxBounds;
                            Debug.Log($"ShardUIManager: Updated ghost bounds to min={newMinBounds}, max={newMaxBounds}");
                        }
                        else
                        {
                            Debug.LogWarning("ShardUIManager: Player has no 'move' component to update bounds.");
                        }
                    }
                }

            }
        }

    }

    void SpawnGuidingOrb()
    {
        if (guidingOrbPrefab == null)
        {
            Debug.LogError("ShardUIManager.SpawnGuidingOrb: guidingOrbPrefab is NOT assigned in the Inspector.");
            return;
        }

        // Determine spawn position: use assigned spawn point, otherwise player's position, otherwise world origin
        Vector3 spawnPos = Vector3.zero;
        if (guidingSpawnPoint != null) spawnPos = guidingSpawnPoint.position;
        else
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) spawnPos = player.transform.position + Vector3.up * 1f; // spawn slightly above player
        }

        GameObject go = Instantiate(guidingOrbPrefab, spawnPos, Quaternion.identity);
        if (go == null)
        {
            Debug.LogError("ShardUIManager.SpawnGuidingOrb: Instantiate returned null.");
            return;
        }

        MoveOrb mover = go.GetComponent<MoveOrb>();
        if (mover != null)
        {
            // If a target override is given in inspector, use it; otherwise keep prefab's value
            if (guidingTargetPosition != Vector3.zero) mover.targetPosition = guidingTargetPosition;
            mover.speed = guidingSpeed;
        }
        else
        {
            Debug.LogWarning("ShardUIManager.SpawnGuidingOrb: spawned prefab has no MoveOrb component. It will not move.");
        }

        Debug.Log("ShardUIManager: Guiding orb spawned.");
    }
}
