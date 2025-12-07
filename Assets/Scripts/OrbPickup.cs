using UnityEngine;

public class OrbPickup : MonoBehaviour
{
    [Tooltip("Prefab of the green orb to spawn when a rabbit is placed.")]
    public GameObject greenOrbPrefab;  // assign prefab in Inspector

    [Tooltip("Prefab of the shard reward to spawn when the orb turns green.")]
    public GameObject shardPrefab;     // assign shard prefab in Inspector

    [Tooltip("How close counts as \"same position\" for placement detection.")]
    public float activationDistance = 0.3f;

    [Tooltip("How far (max) from the orb the shard reward should spawn (horizontal plane).")]
    public float shardSpawnRadius = 10f;

    [Tooltip("Vertical offset for the spawned shard so it doesn't clip into floor.")]
    public float shardHeight = 0.1f;

    [Tooltip("Optional sound played when the shard reward spawns.")]
    public AudioClip rewardSound;

    void Update()
    {
        // find all Pickupable instances (explicit)
        Pickupable[] rabbits = FindObjectsOfType<Pickupable>();

        foreach (Pickupable p in rabbits)
        {
            if (p == null) continue;

            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist <= activationDistance)
            {
                // spawn green orb
                if (greenOrbPrefab != null)
                    Instantiate(greenOrbPrefab, transform.position, Quaternion.identity);

                // spawn shard reward next to the orb
                if (shardPrefab != null)
                {
                    // random horizontal offset inside a circle
                    Vector2 offset2D = Random.insideUnitCircle * shardSpawnRadius;
                    Vector3 spawnPos = transform.position + new Vector3(offset2D.x, shardHeight, offset2D.y);
                    Instantiate(shardPrefab, spawnPos, Quaternion.identity);

                    // optional reward sound
                    if (rewardSound != null)
                        AudioSource.PlayClipAtPoint(rewardSound, spawnPos);
                }

                // safely remove the rabbit (uses RemoveFromScene)
                p.RemoveFromScene();

                // remove this orange orb
                Destroy(gameObject);
                return;
            }
        }
    }
}
