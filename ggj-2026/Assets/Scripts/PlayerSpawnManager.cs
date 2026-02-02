using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }

    [Header("General")]
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Material[] playerColors = new Material[4];
    [SerializeField] private string spawnPointTag = "SpawnPoint";
    [SerializeField] private float springRestDistance = 2f;

    private Transform spawnPosition;

    [Header("Prefabs")]
    [SerializeField] private GameObject controllerPlayerPrefab;

    [Header("Keyboard Player Prefabs (each with its own default action map)")]
    [SerializeField] private GameObject keyboardPlayerWASD;
    [SerializeField] private GameObject keyboardPlayerTFGH;
    [SerializeField] private GameObject keyboardPlayerIJKL;
    [SerializeField] private GameObject keyboardPlayerArrows;

    private List<GameObject> players = new List<GameObject>();
    private bool[] usedKeyboardSlots = new bool[4];
    private bool isSpawningKeyboardPlayer;

    // Keys to detect for each keyboard player slot
    private readonly Key[][] keyboardBindings = {
        new[] { Key.W, Key.A, Key.S, Key.D },
        new[] { Key.T, Key.F, Key.G, Key.H },
        new[] { Key.I, Key.J, Key.K, Key.L },
        new[] { Key.UpArrow, Key.LeftArrow, Key.DownArrow, Key.RightArrow }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        FindSpawnPoint();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSpawnPoint();
    }

    private void FindSpawnPoint()
    {
        GameObject spawnObj = GameObject.FindWithTag(spawnPointTag);
        spawnPosition = spawnObj != null ? spawnObj.transform : null;
    }

    public void DestroyAllPlayers()
    {
        foreach (var player in players)
        {
            if (player != null)
            {
                Destroy(player);
            }
        }
        players.Clear();

        for (int i = 0; i < usedKeyboardSlots.Length; i++)
        {
            usedKeyboardSlots[i] = false;
        }
    }

    private void Update()
    {
        if (players.Count >= maxPlayers) return;
        if (Keyboard.current == null) return;

        // Check for keyboard player joins
        for (int i = 0; i < keyboardBindings.Length; i++)
        {
            if (usedKeyboardSlots[i])
                continue;

            foreach (var key in keyboardBindings[i])
            {
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    SpawnKeyboardPlayer(i);
                    break;
                }
            }
        }
    }

    // Called by PlayerInputManager for controller joins
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Check if this is a keyboard player we're currently spawning
        if (isSpawningKeyboardPlayer)
        {
            // This is our keyboard player, don't process it here
            return;
        }

        if (players.Count >= maxPlayers)
        {
            Destroy(playerInput.gameObject);
            return;
        }

        // Check if this is a controller
        bool isController = false;
        foreach (var device in playerInput.devices)
        {
            if (device is Gamepad)
            {
                isController = true;
                break;
            }
        }

        if (!isController)
        {
            // Reject non-controller players that weren't spawned by us
            Destroy(playerInput.gameObject);
            return;
        }

        SetupPlayer(playerInput.gameObject);
    }

    private void SpawnKeyboardPlayer(int slotIndex)
    {
        GameObject prefab = slotIndex switch
        {
            0 => keyboardPlayerWASD,
            1 => keyboardPlayerTFGH,
            2 => keyboardPlayerIJKL,
            3 => keyboardPlayerArrows,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"PlayerSpawnManager: Keyboard player prefab for slot {slotIndex} is not assigned!");
            return;
        }

        usedKeyboardSlots[slotIndex] = true;

        Vector3 spawnPos = spawnPosition != null ? spawnPosition.position : Vector3.zero;

        // Set flag BEFORE Instantiate (OnPlayerJoined is called during Instantiate)
        isSpawningKeyboardPlayer = true;
        GameObject newPlayer = Instantiate(prefab, spawnPos, Quaternion.identity);
        isSpawningKeyboardPlayer = false;

        SetupPlayer(newPlayer);
    }

    private void SetupPlayer(GameObject newPlayer)
    {
        if (spawnPosition != null)
            newPlayer.transform.position = spawnPosition.position;

        DontDestroyOnLoad(newPlayer);

        // Assign color
        int colorIndex = players.Count % playerColors.Length;
        AssignPlayerColor(newPlayer, playerColors[colorIndex]);

        // Handle spring joints
        SpringJoint spring = newPlayer.GetComponent<SpringJoint>();

        if (players.Count == 0)
        {
            // First player - remove spring joint (single player support)
            if (spring != null)
            {
                Destroy(spring);
            }
        }
        else
        {
            // Connect to previous player
            GameObject previousPlayer = players[players.Count - 1];

            if (spring != null)
            {
                spring.autoConfigureConnectedAnchor = false;
                spring.connectedBody = previousPlayer.GetComponent<Rigidbody>();
                spring.anchor = Vector3.zero;
                spring.connectedAnchor = Vector3.zero;
                spring.minDistance = 0f;
                spring.maxDistance = springRestDistance;
            }

            // Set up line renderer chain
            PhysicsPlayerController prevController = previousPlayer.GetComponent<PhysicsPlayerController>();
            if (prevController != null)
            {
                prevController.NextPlayer = newPlayer.transform;
            }
        }

        players.Add(newPlayer);

        // Register with PlayerManager
        PlayerInput playerInput = newPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            PlayerManager.Instance?.OnPlayerJoined(playerInput);
        }

        Debug.Log($"Player {players.Count} joined");
    }

    private void AssignPlayerColor(GameObject player, Material color)
    {
        if (color == null) return;

        Transform child = player.transform.GetChild(0);
        if (child == null) return;

        Transform body = child.Find("Body");
        if (body == null) return;

        Renderer renderer = body.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = color;
        }
    }
}
