using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkSetup : MonoBehaviour
{
    public GameObject spyPrefab;
    public GameObject sniperPrefab;
    public Button hostButton;
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

        // Ensure each client only spawns once
        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)) return;

        bool isSpy = (NetworkManager.Singleton.ConnectedClientsList.Count == 1);
        SpawnPlayer(clientId, isSpy);
    }

    private void SpawnPlayer(ulong clientId, bool isSpy)
    {
        GameObject playerPrefab = isSpy ? spyPrefab : sniperPrefab;
        
        // Rotate the sniper 180 degrees on spawn
        Quaternion spawnRotation = isSpy ? Quaternion.identity : Quaternion.Euler(0, -90, 0);

        GameObject playerInstance = Instantiate(playerPrefab, GetSpawnPosition(isSpy), spawnRotation);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        // Enable the correct UI for the player
        if (isSpy)
        {
            playerInstance.transform.Find("SpyCanvas")?.gameObject.SetActive(true);
            playerInstance.transform.Find("SniperCanvas")?.gameObject.SetActive(false);
        }
        else
        {
            playerInstance.transform.Find("SniperCanvas")?.gameObject.SetActive(true);
            playerInstance.transform.Find("SpyCanvas")?.gameObject.SetActive(false);
        }
    }

    private Vector3 GetSpawnPosition(bool isSpy)
    {
        return isSpy ? new Vector3(0, 1, 0) : new Vector3(15, 7, 0);
    }
}
