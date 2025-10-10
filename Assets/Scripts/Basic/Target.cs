using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static Action OnTargetHit;

    [SerializeField]
    private float fixedZ = 10.46f;  // fix: place inside class, with f suffix

    void Start()
    {
        RandomizePosition();
    }

    public void Hit()
    {
        RandomizePosition();
        OnTargetHit?.Invoke();
    }

    void RandomizePosition()
    {
        // Get a random X/Y from bounds, then override Z
        Vector3 pos = TargetBounds.Instance.GetRandomPosition();
        pos.z = fixedZ;
        transform.position = pos;
    }
}
