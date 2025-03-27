using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    public TMP_InputField roomCodeInput; // Assign in Inspector
    public GameObject playerPrefab;       // Assign in Inspector – should have a NetworkObject, PhotonVoiceView, Recorder, Speaker, and tag "Player"

    [Header("Spawn Points")]
    public Transform centerChair;         // The designated host spawn position.
    public Transform[] otherChairs;       // The available chairs for non-host players.

    // To track which chairs are occupied.
    private Dictionary<Transform, PlayerRef> chairOccupancy = new Dictionary<Transform, PlayerRef>();
    // To quickly map a player to their chair.
    private Dictionary<PlayerRef, Transform> playerChairMapping = new Dictionary<PlayerRef, Transform>();

    void Start()
    {
        // Create a NetworkRunner component on this GameObject
        _runner = gameObject.GetComponent<NetworkRunner>();
    }

    // HOST GAME METHOD
    public void StartHost()
    {
        // Generate a random room code, e.g., "VRRoom1234"
        string roomCode = "VRRoom" + UnityEngine.Random.Range(1000, 9999);
        Debug.Log("Starting as Host... Room Code: " + roomCode);

        // If a non-networked local player already exists, destroy it
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            Destroy(existingPlayer);
        }

        // Start the game as Host with the given session name
        _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomCode,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            PlayerCount = 10
        });
    }

    // JOIN GAME METHOD
    public void JoinGame()
    {
        string roomCode = roomCodeInput.text;
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room Code is empty! Enter a valid room code.");
            return;
        }

        Debug.Log("Joining Room: " + roomCode);

        // If a non-networked local player exists, destroy it before joining
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null && existingPlayer.GetComponent<NetworkObject>() == null)
        {
            Destroy(existingPlayer);
        }

        _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomCode,
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            PlayerCount = 10
        });
    }

    // SPAWN PLAYER METHOD
    public void SpawnPlayer(PlayerRef player)
    {
        // Only the server spawns player objects.
        if (!_runner.IsServer)
            return;

        // Check for existing network objects with this player authority.
        NetworkObject[] existingPlayers = UnityEngine.Object.FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
        foreach (var netObj in existingPlayers)
        {
            if (netObj.InputAuthority == player)
            {
                Debug.Log("Player already spawned, skipping spawn.");
                return;
            }
        }

        Vector3 spawnPosition = Vector3.zero;
        Transform chosenChair = null;

        // Determine if this player is the host.
        // For this example, we assume the first player (when no chair is occupied) is the host.
        if (chairOccupancy.Count == 0)
        {
            // Host: assign the center chair.
            chosenChair = centerChair;
            spawnPosition = centerChair.position;
        }
        else
        {
            // Non-host: choose an available chair from otherChairs.
            List<Transform> availableChairs = new List<Transform>();
            foreach (Transform chair in otherChairs)
            {
                if (!chairOccupancy.ContainsKey(chair))
                {
                    availableChairs.Add(chair);
                }
            }
            if (availableChairs.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, availableChairs.Count);
                chosenChair = availableChairs[index];
                spawnPosition = chosenChair.position;
            }
            else
            {
                // No available chairs: fallback to a random spawn position.
                spawnPosition = new Vector3(UnityEngine.Random.Range(-2, 2), 0, UnityEngine.Random.Range(-2, 2));
            }
        }

        // Record the chair occupancy for this player.
        if (chosenChair != null)
        {
            chairOccupancy[chosenChair] = player;
            playerChairMapping[player] = chosenChair;
        }

        // Spawn the player at the designated chair position.
        NetworkObject newPlayer = _runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        // Setup the player name on the avatar (customize as needed).
        NetworkVRAvatar avatar = newPlayer.GetComponent<NetworkVRAvatar>();
        if (avatar != null && newPlayer.HasInputAuthority)
        {
            // Set the player's name (replace with the actual name from the homescreen if available).
            avatar.SetPlayerName("Player " + player);
        }
        else
        {
            Debug.LogWarning("NetworkVRAvatar component missing on VR Avatar Prefab.");
        }
    }

    // INetworkRunnerCallbacks IMPLEMENTATION
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined the room.");
        SpawnPlayer(player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left the room.");
        // Free the chair used by the departing player.
        if (playerChairMapping.TryGetValue(player, out Transform chair))
        {
            chairOccupancy.Remove(chair);
            playerChairMapping.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
}
