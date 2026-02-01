using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Material[] playerColors = new Material[4];
    [SerializeField] private bool isManualSetup;

    private List<GameObject> players = new List<GameObject>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject newPlayer = playerInput.gameObject;

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
                    spring.connectedBody = previousPlayer.GetComponent<Rigidbody>();
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
