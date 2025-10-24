using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShooter : MonoBehaviour
{
    public static Action OnTargetMissed;
    [SerializeField] Camera cam;

    void Update()
    {
        if (Timer.GameEnded) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Target target = hit.collider.GetComponent<Target>();
                Vector3 targetPos = target ? target.transform.position : ray.GetPoint(50f);

                FlickErrorTracker.Instance?.RegisterShot(hit, targetPos); // NEW

                if (target != null)
                {
                    AudioHub.Instance?.PlayHit();
                    target.Hit();
                }
                else
                {
                    AudioHub.Instance?.PlayMiss();
                    OnTargetMissed?.Invoke();
                    
                }
            }
            else
            {
                Vector3 farPoint = ray.GetPoint(50f);
                FlickErrorTracker.Instance?.RegisterShot(null, farPoint); // NEW
                OnTargetMissed?.Invoke();
            }
        }
    }
}
