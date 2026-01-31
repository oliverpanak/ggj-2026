using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private bool connectInLoop = true;
    [SerializeField] private int maxPlayers = 4;

    private List<Rigidbody> playerRigidbodies = new List<Rigidbody>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Rigidbody newPlayerRb = playerInput.GetComponent<Rigidbody>();
        if (newPlayerRb == null)
        {
            Debug.LogError("Spawned player has no Rigidbody!");
            return;
        }

        // Assign action map based on player count (Player1, Player2, etc.)
        int playerNumber = playerRigidbodies.Count + 1;
        string actionMapName = $"Player{playerNumber}";
        playerInput.SwitchCurrentActionMap(actionMapName);
        Debug.Log($"Player {playerNumber} assigned to action map: {actionMapName}");

        // Connect previous player's spring joint to this new player
        if (playerRigidbodies.Count > 0)
        {
            Rigidbody previousPlayer = playerRigidbodies[playerRigidbodies.Count - 1];
            SpringJoint spring = previousPlayer.GetComponent<SpringJoint>();
            if (spring == null)
            {
                spring = previousPlayer.gameObject.AddComponent<SpringJoint>();
                ConfigureSpringJoint(spring);
            }
            spring.connectedBody = newPlayerRb;
        }

        playerRigidbodies.Add(newPlayerRb);

        // If we have all players and connectInLoop is true, connect last player back to first
        if (connectInLoop && playerRigidbodies.Count == maxPlayers && maxPlayers > 1)
        {
            SpringJoint spring = newPlayerRb.GetComponent<SpringJoint>();
            if (spring == null)
            {
                spring = newPlayerRb.gameObject.AddComponent<SpringJoint>();
                ConfigureSpringJoint(spring);
            }
            spring.connectedBody = playerRigidbodies[0];
        }

        Debug.Log($"Player {playerRigidbodies.Count} joined and connected");
    }

    private void ConfigureSpringJoint(SpringJoint spring)
    {
        spring.spring = 50f;
        spring.damper = 5f;
        spring.minDistance = 0f;
        spring.maxDistance = 2f;
        spring.autoConfigureConnectedAnchor = true;
    }
}
