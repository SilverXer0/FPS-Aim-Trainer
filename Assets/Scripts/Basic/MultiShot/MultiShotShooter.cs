using UnityEngine;
using System;

public class MultiShotShooter : MonoBehaviour
{
    public static event Action OnMiss;   
    
    [SerializeField] private Camera cam;

    void Update()
    {
        if (Timer.GameEnded) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var gt = hit.collider.GetComponent<GrowingTarget>();
            Vector3 targetPos = gt ? gt.transform.position : ray.GetPoint(50f);

            FlickErrorTracker.Instance?.RegisterShot(hit, targetPos);

            if (gt != null)
            {
                AudioHub.Instance?.PlayHit(); 
                gt.Hit(); 
            }
            else
            {
                OnMiss?.Invoke(); 
            }
        }
        else
        {
            Vector3 farPoint = ray.GetPoint(50f);
            FlickErrorTracker.Instance?.RegisterShot(null, farPoint);

            OnMiss?.Invoke();     
        }
    }
}
