using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Spring Joints")]
    [SerializeField] private float springStrength = 50f;
    [SerializeField] private float springDamper = 5f;
    [SerializeField] private float maxDistance = 2f;

    private List<Rigidbody> playerRigidbodies = new List<Rigidbody>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Rigidbody newPlayerRb = playerInput.GetComponent<Rigidbody>();
        if (newPlayerRb == null)
        {
            Debug.LogError("Spawned player has no Rigidbody!");
            return;
        }

        // Connect previous player's spring joint to this new player
        if (playerRigidbodies.Count > 0)
        {
            Rigidbody previousPlayer = playerRigidbodies[playerRigidbodies.Count - 1];
            SpringJoint spring = previousPlayer.gameObject.AddComponent<SpringJoint>();
            spring.spring = springStrength;
            spring.damper = springDamper;
            spring.minDistance = 0f;
            spring.maxDistance = maxDistance;
            spring.autoConfigureConnectedAnchor = true;
            spring.connectedBody = newPlayerRb;
        }

        playerRigidbodies.Add(newPlayerRb);
        Debug.Log($"Player {playerRigidbodies.Count} joined and connected");
    }
}
