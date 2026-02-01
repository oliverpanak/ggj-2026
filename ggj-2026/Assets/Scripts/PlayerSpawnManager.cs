using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Material[] playerColors = new Material[4];
    [SerializeField] private bool isManualSetup;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float springRestDistance = 2f;

    private List<GameObject> players = new List<GameObject>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        // Reject mouse/keyboard players
        foreach (var device in playerInput.devices)
        {
            if (device is Mouse || device is Keyboard)
            {
                Destroy(playerInput.gameObject);
                return;
            }
        }

        GameObject newPlayer = playerInput.gameObject;
        if(spawnPosition != null)
            playerInput.transform.position = spawnPosition.position;
        DontDestroyOnLoad(playerInput.gameObject);

        // Assign color
        int colorIndex = players.Count % playerColors.Length;
        AssignPlayerColor(newPlayer, playerColors[colorIndex]);

        // Only handle spring joints if not using manual setup
        if (!isManualSetup)
        {
            SpringJoint spring = newPlayer.GetComponent<SpringJoint>();

            // If this is the first player, disable the spring joint (single player support)
            if (players.Count == 0)
            {
                if (spring != null)
                {
                    Destroy(spring);
                }
            }
            else
            {
                // Connect NEW player to PREVIOUS player
                GameObject previousPlayer = players[players.Count - 1];

                if (spring != null)
                {
                    // Reset spring joint to use consistent rest distance
                    spring.autoConfigureConnectedAnchor = false;
                    spring.connectedBody = previousPlayer.GetComponent<Rigidbody>();
                    spring.anchor = Vector3.zero;
                    spring.connectedAnchor = Vector3.zero;
                    spring.minDistance = 0f;
                    spring.maxDistance = springRestDistance;
                }

                // Optional: still keep your line renderer chain
                PhysicsPlayerController prevController = previousPlayer.GetComponent<PhysicsPlayerController>();
                if (prevController != null)
                {
                    prevController.NextPlayer = newPlayer.transform;
                }
            }
        }

        players.Add(newPlayer);

        // Register with PlayerManager so stamps can detect players
        PlayerManager.Instance?.OnPlayerJoined(playerInput);

        Debug.Log($"Player {players.Count} joined and connected");
    }

    private void AssignPlayerColor(GameObject player, Material color)
    {
        Renderer renderer = player.transform.GetChild(0).transform.Find("Body").GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = color;
        }
    }
}
