using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[AddComponentMenu("Guiding/GuidingOrbTrigger (debuggable)")]
public class GuidingOrbTrigger : MonoBehaviour
{
    [Tooltip("Name of scene to load when player reaches guiding orb")]
    public string nextSceneName = "level2";

    [Tooltip("How close the player must be to trigger scene load (meters)")]
    public float triggerDistance = 0.8f;

    [Tooltip("Seconds to ignore player proximity after the orb spawns (grace period)")]
    public float graceTime = 1.0f;

    [Tooltip("Optional: if true, only trigger once")]
    public bool oneShot = true;

    [Tooltip("Enable verbose debug logging to the Console")]
    public bool debugMode = true;

    [Tooltip("How often (seconds) to log distance while debugging")]
    public float logInterval = 0.5f;

    bool triggered = false;
    float spawnTime;
    float lastLogTime = 0f;

    void OnEnable()
    {
        spawnTime = Time.time;
        lastLogTime = Time.time;
        if (debugMode)
            Debug.Log($"[GuidingOrbTrigger] Enabled at {spawnTime:F2}. triggerDistance={triggerDistance}, graceTime={graceTime}");
    }

    void Update()
    {
        if (triggered && oneShot) return;

        // respect grace period
        if (Time.time - spawnTime < graceTime)
        {
            if (debugMode && Time.time - lastLogTime >= logInterval)
            {
                Debug.Log($"[GuidingOrbTrigger] In grace period ({Time.time - spawnTime:F2}/{graceTime:F2}s)");
                lastLogTime = Time.time;
            }
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            if (debugMode && Time.time - lastLogTime >= logInterval)
            {
                Debug.Log("[GuidingOrbTrigger] No GameObject with tag 'Player' found in scene.");
                lastLogTime = Time.time;
            }
            return;
        }

        // Try to use the player's collider for an accurate nearest-point distance
        float distance = float.MaxValue;
        Collider playerCol = player.GetComponentInChildren<Collider>();
        if (playerCol != null)
        {
            Vector3 closest = playerCol.ClosestPoint(transform.position);
            distance = Vector3.Distance(closest, transform.position);
        }
        else
        {
            distance = Vector3.Distance(player.transform.position, transform.position);
        }

        // periodic logging
        if (debugMode && Time.time - lastLogTime >= logInterval)
        {
            Debug.Log($"[GuidingOrbTrigger] distanceToPlayer={distance:F3} (triggerDistance={triggerDistance})");
            lastLogTime = Time.time;
        }

        if (distance <= triggerDistance)
        {
            Debug.Log($"[GuidingOrbTrigger] Player within {distance:F3} — triggering load of '{nextSceneName}'");
            triggered = true;
            StartCoroutine(LoadAfterDelay(0.25f)); // small delay to allow sound/particle
        }
    }

    IEnumerator LoadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }
}
