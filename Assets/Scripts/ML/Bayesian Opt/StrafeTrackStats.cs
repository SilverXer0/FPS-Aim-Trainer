using UnityEngine;

public class StrafeTrackStats : MonoBehaviour
{
    public static StrafeTrackStats Instance { get; private set; }

    [HideInInspector] public float OnTargetRatio01 = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
