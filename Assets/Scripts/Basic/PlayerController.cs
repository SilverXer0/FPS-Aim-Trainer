using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;

    [Header("Fallback Sensitivity (used only if Manager is not present)")]
    [SerializeField] private float defaultMouseSensitivity = 100;

    private float verticalLookRotation;

    void Start()
    {
        Cursor.visible    = false;
        Cursor.lockState  = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Timer.GameEnded)
            return;

        float sens = defaultMouseSensitivity;
        if (SensitivityManager.Instance != null)
            sens = SensitivityManager.Instance.currentSensitivity;

        float mx = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        transform.Rotate(Vector3.up * mx);

        float my = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;
        verticalLookRotation -= my;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
    }
}
