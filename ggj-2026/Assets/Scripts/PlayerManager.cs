using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private List<Transform> players = new List<Transform>();

    public IReadOnlyList<Transform> Players => players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Transform playerTransform = playerInput.transform;
        players.Add(playerTransform);
        Debug.Log($"PlayerManager: Player {players.Count} joined");
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput.transform);
        Debug.Log($"PlayerManager: Player left, {players.Count} remaining");
    }
}
