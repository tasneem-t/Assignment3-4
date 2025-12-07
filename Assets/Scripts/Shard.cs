using UnityEngine;

public class Shard : MonoBehaviour
{
    public int shardValue = 1;         // how many points this shard gives
    public AudioClip pickupSound;      // optional sound
    private static int totalShards = 0; // internal score tracker
    public static int shardsNeeded = 2;

    void OnTriggerEnter(Collider other)
    {
        // check if the player (ghost) touched it
        if (other.CompareTag("Player"))
        {
            totalShards += shardValue;
            Debug.Log("Shard collected! Total shards: " + totalShards);

            // play sound if assigned
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            if (ShardUIManager.instance != null)
                ShardUIManager.instance.AddShard(shardValue, shardsNeeded);

            // make the shard disappear
            Destroy(gameObject);
        }
    }
}
