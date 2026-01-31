using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

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

    private void Start()
    {
        // Set camera target to first manual player if available
        if (manualPlayers.Count > 0 && cinemachineCamera != null)
        {
            cinemachineCamera.Follow = manualPlayers[0];
        }
    }

    // Called by PlayerInputManager when a player joins
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Transform playerTransform = playerInput.transform;
        runtimePlayers.Add(playerTransform);

        // Set camera target to first player
        if (runtimePlayers.Count == 1 && cinemachineCamera != null)
        {
            cinemachineCamera.Follow = playerTransform;
        }

        Debug.Log($"GameManager: Player {runtimePlayers.Count} joined");
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        runtimePlayers.Remove(playerInput.transform);
        Debug.Log($"GameManager: Player left, {runtimePlayers.Count} remaining");
    }
}
