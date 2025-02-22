using Unity.Netcode;
using UnityEngine;

public class SniperLook : NetworkBehaviour
{
    public float sensitivity = 2f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        if (!IsOwner) // ✅ Only the sniper controls this
        {
            enabled = false;
        }
        
        Camera cam = GetComponentInChildren<Camera>();
        if (!IsOwner && cam != null)
        {
            cam.enabled = false;
            cam.GetComponent<AudioListener>().enabled = false; // ✅ Disable audio listener
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // ✅ Limit up/down movement

        rotationY += mouseX;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
