using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class LobbyManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public Button startGameButton;
    public TMP_Text playerCountText;

    public GameObject spyPrefab;
    public GameObject sniperPrefab;

    private void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
        startGameButton.onClick.AddListener(StartGame);

        startGameButton.gameObject.SetActive(false);

        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;

        SceneManager.sceneLoaded += OnSceneLoaded; // ✅ Listen for scene changes
    }

    private void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started");
        }
    }

    private void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client joined");
        }
    }

    private void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Starting game...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }

    private void UpdatePlayerCount(ulong clientId)
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        playerCountText.text = $"Players: {playerCount}";

        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.gameObject.SetActive(playerCount >= 2);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene" && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("GameScene Loaded, Spawning Players...");

            int i = 0;
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                ulong clientId = client.Key;
                bool isSpy = (i == 0); // First player is Spy, second is Sniper
                SpawnPlayer(clientId, isSpy);
                i++;
            }
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
