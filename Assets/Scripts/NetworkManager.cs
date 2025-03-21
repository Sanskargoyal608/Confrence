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
        // Only the server spawns player objects
        if (!_runner.IsServer)
            return;

        // Check if a player with this authority is already spawned.
        // Using FindObjectsByType avoids deprecated calls.
        NetworkObject[] existingPlayers = UnityEngine.Object.FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
        foreach (var netObj in existingPlayers)
        {
            if (netObj.InputAuthority == player)
            {
                Debug.Log("Player already spawned, skipping spawn.");
                return;
            }
        }

        // Spawn a new networked VR player at a random position
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-2, 2), 0, UnityEngine.Random.Range(-2, 2));
        NetworkObject newPlayer = _runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        // If this is the local player, enable its VR controls
        if (newPlayer.HasInputAuthority)
        {
            PlayerVRController vrController = newPlayer.GetComponent<PlayerVRController>();
            if (vrController != null)
            {
                vrController.EnableVRControls();
            }
            else
            {
                Debug.LogWarning("PlayerVRController component missing on playerPrefab.");
            }
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
