using UnityEngine;
using System.Collections;

public enum PowerupType
{
    SpeedBoost,
    CarryBoost,
    SlowDown,
    GhostInvisible
}

public class powerups : MonoBehaviour
{
    public PowerupType powerupType;
    public float value = 2f;        // multiplier or seconds depending on type
    public float duration = 5f;     // how long the effect lasts after pickup
    public AudioClip pickupSound;   // optional sound effect
    public float respawnTime = 5f;  // seconds before it becomes usable again

    Renderer[] renderers;
    Collider[] colliders;
    ParticleSystem[] particleSystems;

    void Start()
    {
        // cache all renderers/colliders/particles on this object and children
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Apply the effect
        GhostPowerupManager manager = other.GetComponent<GhostPowerupManager>();
        if (manager != null)
            manager.ActivatePowerup(powerupType, value, duration);

        // play pickup sound (optional)
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // disable visuals/colliders/particles instead of SetActive(false)
        SetActiveVisuals(false);
        SetActiveColliders(false);
        SetActiveParticles(false);

        // start respawn timer on this enabled MonoBehaviour (coroutine will run)
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        // re-enable visuals/colliders/particles
        SetActiveVisuals(true);
        SetActiveColliders(true);
        SetActiveParticles(true);
    }

    void SetActiveVisuals(bool on)
    {
        if (renderers == null) renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) if (r != null) r.enabled = on;
    }

    void SetActiveColliders(bool on)
    {
        if (colliders == null) colliders = GetComponentsInChildren<Collider>(true);
        foreach (var c in colliders)
        {
            if (c == null) continue;
            // leave any trigger used for detection enabled/disabled consistently
            c.enabled = on;
        }
    }

    void SetActiveParticles(bool on)
    {
        if (particleSystems == null) particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var p in particleSystems)
        {
            if (p == null) continue;
            if (on) p.Play();
            else p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
