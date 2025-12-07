using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Light Settings")]
    public Light directionalLight;  // Drag your main Directional Light here
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.05f, 0.05f, 0.18f);
    public float intensityDay = 1.0f;
    public float intensityNight = 0.12f;

    [Header("Skybox (optional)")]
    [Tooltip("If your skybox shader supports an exposure/brightness property (eg '_Exposure' or '_SunSize'), set the name here.")]
    public string skyboxExposureProperty = "_Exposure";
    public float skyboxDayExposure = 1.0f;
    public float skyboxNightExposure = 0.3f;

    [Header("Cycle Settings")]
    public float cycleDuration = 10f; // seconds between toggles
    public bool smoothTransition = false;
    public float smoothSpeed = 2f; // interpolation speed when smoothTransition=true

    bool isDay = true;
    float timer = 0f;

    void Start()
    {
        // fallback: try to use RenderSettings.sun if not assigned
        if (directionalLight == null)
        {
            directionalLight = RenderSettings.sun;
        }

        // ensure directionalLight is valid
        if (directionalLight == null)
            Debug.LogWarning("DayNightCycle: no directionalLight assigned and RenderSettings.sun is null.");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cycleDuration)
        {
            timer = 0f;
            ToggleDayNight();
        }

        if (smoothTransition)
        {
            // Smoothly lerp towards current target values
            float t = Time.deltaTime * smoothSpeed;
            Color targetColor = isDay ? dayColor : nightColor;
            float targetIntensity = isDay ? intensityDay : intensityNight;

            if (directionalLight != null)
            {
                directionalLight.color = Color.Lerp(directionalLight.color, targetColor, t);
                directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, targetIntensity, t);
            }

            // Skybox exposure (if available)
            if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty(skyboxExposureProperty))
            {
                float current = RenderSettings.skybox.GetFloat(skyboxExposureProperty);
                float goal = isDay ? skyboxDayExposure : skyboxNightExposure;
                RenderSettings.skybox.SetFloat(skyboxExposureProperty, Mathf.Lerp(current, goal, t));
            }
        }
    }

    void ToggleDayNight()
    {
        isDay = !isDay;

        if (!smoothTransition)
        {
            if (directionalLight != null)
            {
                directionalLight.color = isDay ? dayColor : nightColor;
                directionalLight.intensity = isDay ? intensityDay : intensityNight;
            }

            // change skybox exposure only if shader supports property
            if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty(skyboxExposureProperty))
            {
                RenderSettings.skybox.SetFloat(skyboxExposureProperty, isDay ? skyboxDayExposure : skyboxNightExposure);
            }
        }

        Debug.Log(isDay ? "Switched to DAY" : "Switched to NIGHT");
    }
}
