using UnityEngine;

public class AudioHub : MonoBehaviour
{
    public static AudioHub Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private AudioClip uiClickClip;
    [Range(0f,1f)] [SerializeField] private float uiClickVolume = 0.7f;

    [Header("Gameplay")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip missClip;
    [Range(0f,1f)] [SerializeField] private float hitVolume  = 0.9f;
    [Range(0f,1f)] [SerializeField] private float missVolume = 0.7f;

    private AudioSource _src;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // keep only one
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake  = false;
        _src.loop         = false;
        _src.spatialBlend = 0f; // 2D
    }

    public void PlayUIClick() { if (uiClickClip) _src.PlayOneShot(uiClickClip, uiClickVolume); }
    public void PlayHit()     { if (hitClip)    _src.PlayOneShot(hitClip,     hitVolume);     }
    public void PlayMiss()    { if (missClip)   _src.PlayOneShot(missClip,    missVolume);    }
}
