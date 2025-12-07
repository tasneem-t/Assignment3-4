using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);   // keep music playing across scenes
        }
        else
        {
            Destroy(gameObject);             // prevent duplicate audio
        }
    }
}
