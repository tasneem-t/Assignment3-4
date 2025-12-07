using UnityEngine;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    [Tooltip("UI panel or button GameObject to hide when the game starts")]
    public GameObject startUI;

    [Tooltip("List of components to enable when the game starts (eg move, PickupController, GhostPowerupManager, ShardCollector...)")]
    public MonoBehaviour[] componentsToEnable;

    [Tooltip("If true, timescale will be 0 before start and set to 1 when started")]
    public bool pauseTimeBeforeStart = true;

    void Awake()
    {
        // Disable listed components at start
        foreach (var comp in componentsToEnable)
        {
            if (comp != null) comp.enabled = false;
        }

        if (pauseTimeBeforeStart)
            Time.timeScale = 0f;
    }

    // Hook this to the Button.OnClick
    public void StartGame()
    {
        // enable components
        foreach (var comp in componentsToEnable)
        {
            if (comp != null) comp.enabled = true;
        }

        // hide the start UI (the whole panel or just the button)
        if (startUI != null)
            startUI.SetActive(false);

        if (pauseTimeBeforeStart)
            Time.timeScale = 1f;
    }
}
