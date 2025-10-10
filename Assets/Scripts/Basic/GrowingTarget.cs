using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrowingTarget : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("Initial uniform scale (1 = normal size, 0.5 = half).")]
    [SerializeField] private float startScale = 0.5f;

    [Tooltip("Peak uniform scale before shrinking.")]
    [SerializeField] private float maxScale = 3f;

    [Tooltip("Seconds from spawn until it reaches maxScale (and same to shrink back).")]
    [SerializeField] private float growTime = 5f;

    private float _spawnTime;
    private Transform _tf;

    public void OnEnable()
    {
        _spawnTime = Time.time;
        _tf = transform;
        _tf.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        float elapsed = Time.time - _spawnTime;

        float cycleDuration = growTime * 2f;
        if (elapsed >= cycleDuration)
        {
            Destroy(gameObject);
            return;
        }

        float t;
        float currentScale;

        if (elapsed < growTime)
        {
            t = elapsed / growTime;  
            currentScale = Mathf.Lerp(startScale, maxScale, t);
        }
        else
        {
            t = (elapsed - growTime) / growTime;  
            currentScale = Mathf.Lerp(maxScale, startScale, t);
        }

        _tf.localScale = Vector3.one * currentScale;
    }

    public void Hit()
    {
        Target.OnTargetHit?.Invoke();
        Destroy(gameObject);
    }
}
