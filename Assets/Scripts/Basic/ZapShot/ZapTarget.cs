using System;
using UnityEngine;

[RequireComponent(typeof(Renderer),typeof(Collider))]
public class ZapTarget : MonoBehaviour
{
    public static Action<float> OnZapVisible; 
    public static Action OnZapHidden;         

    [Header("Flash Settings")]
    [SerializeField] private float visibleDuration = 0.2f;
    [SerializeField] private float minInvisible   = 0.3f;
    [SerializeField] private float maxInvisible   = 0.8f;

    private Renderer _renderer;
    private Collider _collider;
    private float _lastVisibleTime;
    private Coroutine _flashRoutine;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    void OnEnable()
    {

        _flashRoutine = StartCoroutine(FlashLoop());
    }

    private System.Collections.IEnumerator FlashLoop()
    {
        _renderer.enabled = false;
        _collider.enabled = false;

        while (!Timer.GameEnded)
        {

            float off = UnityEngine.Random.Range(minInvisible, maxInvisible);
            yield return new WaitForSeconds(off);

            _renderer.enabled = true;
            _collider.enabled = true;
            _lastVisibleTime = Time.time;
            OnZapVisible?.Invoke(_lastVisibleTime);
            
            yield return new WaitForSeconds(visibleDuration);

            _renderer.enabled = false;
            _collider.enabled = false;
            OnZapHidden?.Invoke();
        }

        _renderer.enabled = false;
        _collider.enabled = false;
    }

    public void Hit()
    {
        if (_flashRoutine != null) 
            StopCoroutine(_flashRoutine);

        _renderer.enabled = false;
        _collider.enabled = false;
        OnZapHidden?.Invoke();


        _flashRoutine = StartCoroutine(FlashLoop());
    }

    public float LastVisibleTime => _lastVisibleTime;
}
