using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private Color[] playerColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow
    };

    private List<GameObject> players = new List<GameObject>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
    GameObject newPlayer = playerInput.gameObject;

    // Assign color
    int colorIndex = players.Count % playerColors.Length;
    AssignPlayerColor(newPlayer, playerColors[colorIndex]);

    // If this is not the first player, connect NEW player to PREVIOUS player
    if (players.Count > 0)
    {
        GameObject previousPlayer = players[players.Count - 1];

        SpringJoint spring = newPlayer.GetComponent<SpringJoint>();
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

    players.Add(newPlayer);

    Debug.Log($"Player {players.Count} joined and connected");
}


    private void AssignPlayerColor(GameObject player, Color color)
    {
        Renderer renderer = player.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }
}
