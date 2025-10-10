using System;
using UnityEngine;

public class UnfixedZTarget : MonoBehaviour
{
    public static Action OnTargetHit;

    void Start()
    {
        RandomizePosition();
    }

    public void Hit()
    {
        RandomizePosition();
        OnTargetHit?.Invoke();
    }

    public void RandomizePosition()
    {
        transform.position = TargetBounds.Instance.GetRandomPosition();
    }
}
