using UnityEngine;

public class ZapShooter : MonoBehaviour
{
    public static System.Action<float> OnZapHit;
    public static System.Action     OnZapMisclick;

    [SerializeField] private Camera cam;

    void Update()
    {
        if (Timer.GameEnded) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var z = hit.collider.GetComponent<ZapTarget>();
            if (z != null && z.GetComponent<Renderer>().enabled)
            {

                float reaction = Time.time - z.LastVisibleTime;
  
                z.Hit();
 
                OnZapHit?.Invoke(reaction);
            }
            else
            {
                OnZapMisclick?.Invoke();
            }
        }
        else
        {
            OnZapMisclick?.Invoke();
        }
    }
}
