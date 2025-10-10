using UnityEngine;
using System;

public class EntryShooter : MonoBehaviour
{
    public static event Action OnEntryMiss;
    [SerializeField] private Camera cam;

    void Update()
    {
        if (Timer.GameEnded) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var et = hit.collider.GetComponent<EntryTarget>();
            Vector3 targetPos = et ? et.transform.position : ray.GetPoint(50f);

            FlickErrorTracker.Instance?.RegisterShot(hit, targetPos);

            if (et != null)
            {
                AudioHub.Instance?.PlayHit();
                et.Hit();
                return;
            }
        }
        else
        {
            Vector3 farPoint = ray.GetPoint(50f);
            FlickErrorTracker.Instance?.RegisterShot(null, farPoint);
        }

        OnEntryMiss?.Invoke();
    }
}
