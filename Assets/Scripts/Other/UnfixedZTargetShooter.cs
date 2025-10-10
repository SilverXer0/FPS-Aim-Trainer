using System;
using UnityEngine;

public class UnfixedZTargetShooter : MonoBehaviour
{
    public static Action OnTargetMissed;

    [SerializeField] private Camera cam;

    void Update()
    {
        if (Timer.GameEnded)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var uzt = hit.collider.GetComponent<UnfixedZTarget>();
                if (uzt != null)
                {
                    uzt.Hit();
                }
                else
                {
                    OnTargetMissed?.Invoke();
                }
            }
            else
            {
                OnTargetMissed?.Invoke();
            }
        }
    }
}
