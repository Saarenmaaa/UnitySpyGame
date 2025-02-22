using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI; // ✅ For pathfinding

public class SpyMovement : NetworkBehaviour
{
    private NavMeshAgent agent;
    public float rotationSpeed = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!IsOwner) // ✅ Only control movement for the local player
        {
            enabled = false;
        }

        // Disable extra cameras/audio for non-owners
        Camera cam = GetComponentInChildren<Camera>();
        if (!IsOwner && cam != null)
        {
            cam.enabled = false;
            cam.GetComponent<AudioListener>().enabled = false;
        }
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
            float mouseX = Input.GetAxis("Mouse X"); // Get horizontal mouse movement
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);
        }
    }

    [ServerRpc]
    private void MoveToPositionServerRpc(Vector3 position)
    {
        if (!IsServer) return;

        // ✅ Server moves the agent and tells all clients to follow
        agent.SetDestination(position);
        UpdateDestinationClientRpc(position);
    }

    [ClientRpc]
    private void UpdateDestinationClientRpc(Vector3 position)
    {
        if (IsOwner) return; // ✅ Only update remote players

        agent.SetDestination(position); // ✅ Remote players move naturally
    }
}
