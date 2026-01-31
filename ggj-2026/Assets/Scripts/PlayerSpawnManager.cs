using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;

    private List<GameObject> players = new List<GameObject>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        GameObject newPlayer = playerInput.gameObject;

        // Connect previous player's spring joint to this new player
        if (players.Count > 0)
        {
            GameObject previousPlayer = players[players.Count - 1];
            SpringJoint spring = previousPlayer.GetComponent<SpringJoint>();
            if (spring != null)
            {
                spring.connectedBody = newPlayer.GetComponent<Rigidbody>();
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
}
