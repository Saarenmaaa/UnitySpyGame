using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine; // ✅ For smooth camera control

public class SpyMovement : NetworkBehaviour
{
    private NavMeshAgent agent;
    public float rotationSpeed = 5f;

    // 🎯 Assign these in the Unity Inspector
    public CinemachineVirtualCamera spyVirtualCamera;
    public Canvas spyCanvas;
    public Camera spyCamera; 
    public AudioListener spyCameraAudioListener; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!IsOwner)
        {
            enabled = false;
            if (spyCanvas) spyCanvas.enabled = false;
            if (spyCamera) spyCamera.enabled = false;
            return;
        }

        if (spyCamera)
        {
            spyCamera.enabled = true;

            // ✅ Ensure only the local Spy has an active AudioListener
            if (spyCameraAudioListener)
                spyCameraAudioListener.enabled = true;
        }

        // ✅ Enable only the local Spy's Cinemachine Virtual Camera
        if (spyVirtualCamera)
        {
            spyVirtualCamera.Priority = 10;
        }

        Cursor.lockState = CursorLockMode.None; // Show & unlock cursor
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0)) // ✅ Left-click to move
        {
            Ray ray = spyCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MoveToPositionServerRpc(hit.point); // ✅ Tell the server to move
            }
        }
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // ✅ Hold right-click to rotate
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        }
    }

    [ServerRpc]
    private void MoveToPositionServerRpc(Vector3 position)
    {
        if (!IsServer || agent == null) return; // ✅ Prevent errors if no agent

        // ✅ Server moves the agent and tells all clients to follow
        agent.SetDestination(position);
        UpdateDestinationClientRpc(position);
    }

    [ClientRpc]
    private void UpdateDestinationClientRpc(Vector3 position)
    {
        if (IsOwner || agent == null) return; // ✅ Only update remote players

        agent.SetDestination(position);
    }
}
