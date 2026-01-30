using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private bool connectInLoop = true;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int playerCount = 2;

    private List<Rigidbody> playerRigidbodies = new List<Rigidbody>();

    private void Start()
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            Vector3 spawnPos = spawnPoints != null && i < spawnPoints.Length
                ? spawnPoints[i].position
                : Vector3.right * i * 2f;

            GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            PlayerInput playerInput = player.GetComponent<PlayerInput>();

            // Assign action map
            string actionMapName = $"Player{i + 1}";
            playerInput.SwitchCurrentActionMap(actionMapName);

            AddPlayer(player.GetComponent<Rigidbody>());
            Debug.Log($"Spawned Player {i + 1} with action map: {actionMapName}");
        }
    }

    private void AddPlayer(Rigidbody newPlayerRb)
    {
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
        if (connectInLoop && playerRigidbodies.Count == playerCount && playerCount > 1)
        {
            SpringJoint spring = newPlayerRb.GetComponent<SpringJoint>();
            if (spring == null)
            {
                spring = newPlayerRb.gameObject.AddComponent<SpringJoint>();
                ConfigureSpringJoint(spring);
            }
            spring.connectedBody = playerRigidbodies[0];
        }
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
