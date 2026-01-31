using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Manual Testing")]
    [SerializeField] private List<Transform> manualPlayers = new List<Transform>();

    private List<Transform> runtimePlayers = new List<Transform>();

    public IReadOnlyList<Transform> Players => manualPlayers.Count > 0 ? manualPlayers : runtimePlayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Called by PlayerInputManager when a player joins
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Transform playerTransform = playerInput.transform;
        runtimePlayers.Add(playerTransform);
        Debug.Log($"GameManager: Player {runtimePlayers.Count} joined");
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        runtimePlayers.Remove(playerInput.transform);
        Debug.Log($"GameManager: Player left, {runtimePlayers.Count} remaining");
    }
}
