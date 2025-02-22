using Unity.Netcode;
using UnityEngine;

public class SniperLook : NetworkBehaviour
{
    public float sensitivity = 2f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    public Camera sniperCamera;
    public AudioListener sniperAudio;  // ✅ Correct name
    public GameObject sniperScopeUI;
    public Canvas sniperCanvas;  // ✅ Defined properly

    public float zoomedFOV = 15f;
    public float normalFOV = 60f;
    public float zoomSpeed = 5f;

    private void Start()
    {
        if (!IsOwner)
        {
            enabled = false;
            if (sniperCanvas) sniperCanvas.enabled = false;
            if (sniperCamera) sniperCamera.enabled = false;
            return;
        }

        if (sniperCamera)
        {
            sniperCamera.enabled = true;
            if (sniperAudio) sniperAudio.enabled = true; // ✅ Corrected name
        }

        if (sniperScopeUI) sniperScopeUI.SetActive(false);
        sniperCamera.fieldOfView = normalFOV;

        Cursor.lockState = CursorLockMode.Locked; // Hide & lock cursor
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY; // ✅ Subtracting for correct movement
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); 
        rotationY += mouseX;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    private void HandleZoom()
    {
        bool isZooming = Input.GetMouseButton(1);

        if (sniperScopeUI) sniperScopeUI.SetActive(isZooming);
        if (sniperCamera)
        {
            float targetFOV = isZooming ? zoomedFOV : normalFOV;
            sniperCamera.fieldOfView = Mathf.Lerp(sniperCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}
