using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Material[] playerColors = new Material[4];

    private List<GameObject> players = new List<GameObject>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject newPlayer = playerInput.gameObject;

        // Assign color to player
        int colorIndex = players.Count % playerColors.Length;
        AssignPlayerColor(newPlayer, playerColors[colorIndex]);

        // Connect previous player to this new player
        if (players.Count > 0)
        {
            GameObject previousPlayer = players[players.Count - 1];

            // Connect spring joint
            SpringJoint spring = previousPlayer.GetComponent<SpringJoint>();
            if (spring != null)
            {
                spring.connectedBody = newPlayer.GetComponent<Rigidbody>();
            }

            // Set next player for line renderer
            PhysicsPlayerController prevController = previousPlayer.GetComponent<PhysicsPlayerController>();
            if (prevController != null)
            {
                prevController.NextPlayer = newPlayer.transform;
            }
        }

        players.Add(newPlayer);

        // Remove spring joint on the last player
        if (players.Count == maxPlayers)
        {
            SpringJoint spring = newPlayer.GetComponent<SpringJoint>();
            if (spring != null)
            {
                Destroy(spring);
            }
        }

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
