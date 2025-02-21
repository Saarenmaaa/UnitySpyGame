using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSetup : MonoBehaviour
{
    public GameObject spyPrefab;
    public GameObject sniperPrefab;
    public Button hostButton;  // ✅ Normal UI Button
    public Button clientButton;

    private void Start()
    {
        if (hostButton != null) 
            hostButton.onClick.AddListener(StartHost);

        if (clientButton != null) 
            clientButton.onClick.AddListener(StartClient);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void StartHost()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
    }

    private void StartClient()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; // ✅ Only the server should spawn players

        if (NetworkManager.Singleton.ConnectedClientsList.Count == 1)
        {
            SpawnPlayer(clientId, true); // First player = Spy
        }
        else
        {
            SpawnPlayer(clientId, false); // Second player = Sniper
        }
    }

    private void SpawnPlayer(ulong clientId, bool isSpy)
    {
        GameObject playerPrefab = isSpy ? spyPrefab : sniperPrefab;
        GameObject playerInstance = Instantiate(playerPrefab, GetSpawnPosition(isSpy), Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetSpawnPosition(bool isSpy)
    {
        return isSpy ? new Vector3(0, 1, 0) : new Vector3(15, 7, 0);
    }
}
