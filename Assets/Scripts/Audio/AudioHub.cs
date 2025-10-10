using UnityEngine;

public class AudioHub : MonoBehaviour
{
    public static AudioHub Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip shotClip; 
    [SerializeField] private AudioClip hitClip;  

    [Header("Volumes")]
    [Range(0f,1f)] public float shotVolume = 0.6f;
    [Range(0f,1f)] public float hitVolume  = 0.8f;

    private AudioSource _src;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _src = gameObject.AddComponent<AudioSource>();
        _src.playOnAwake   = false;
        _src.loop          = false;
        _src.spatialBlend  = 0f;    
        _src.volume        = 1f;
    }

    public void PlayShot()
    {
        if (shotClip) _src.PlayOneShot(shotClip, shotVolume);
    }

    public void PlayHit()
    {
        if (hitClip) _src.PlayOneShot(hitClip,  hitVolume);
    }
}
