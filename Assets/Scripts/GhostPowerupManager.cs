using System.Collections;
using UnityEngine;

[RequireComponent(typeof(move))]
public class GhostPowerupManager : MonoBehaviour
{
    move ghostMove;
    PickupController pickupController;

    // Original stats
    float baseSpeed;
    float baseHoldDuration;
    //invisbility
    Renderer[] renderersCache;
    bool[] rendererInitialState;

    void Start()
    {
        ghostMove = GetComponent<move>();
        pickupController = GetComponent<PickupController>();

        baseSpeed = (ghostMove != null) ? ghostMove.speed : 5f;
        baseHoldDuration = (pickupController != null) ? pickupController.defaultHoldDuration : 5f;

        // cache renderers so we can toggle visibility
        renderersCache = GetComponentsInChildren<Renderer>(includeInactive: true);
        rendererInitialState = new bool[renderersCache.Length];
        for (int i = 0; i < renderersCache.Length; i++)
            rendererInitialState[i] = renderersCache[i].enabled;
    }


    public void ActivatePowerup(PowerupType type, float value, float duration)
    {
        StopAllCoroutines(); // optional: remove if you want overlapping effects
        StartCoroutine(PowerupRoutine(type, value, duration));
    }

    IEnumerator PowerupRoutine(PowerupType type, float value, float duration)
    {
        switch (type)
        {
            case PowerupType.SpeedBoost:
                ghostMove.speed = baseSpeed * value;
                break;
            case PowerupType.CarryBoost:
                if (pickupController != null)
                    pickupController.canCarry = true;
                break;
            case PowerupType.SlowDown:
                if (ghostMove != null)
                    ghostMove.speed = baseSpeed * value; // value should be < 1 (e.g. 0.2)
                break;

            case PowerupType.GhostInvisible:
                // disable all renderers
                for (int i = 0; i < renderersCache.Length; i++)
                {
                    if (renderersCache[i] != null)
                        renderersCache[i].enabled = false;
                }
                break;
        }

        yield return new WaitForSeconds(duration);

        // reset after duration
        ghostMove.speed = baseSpeed;

        if (pickupController != null)
        {
            // auto-drop any held item
            pickupController.ManualDrop();
            pickupController.canCarry = false;
        }
        for (int i = 0; i < renderersCache.Length; i++)
        {
            if (renderersCache[i] != null)
                renderersCache[i].enabled = rendererInitialState[i];
        }
    }
}
