using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            GameOverManager.Instance?.TriggerGameOver();
        }
    }
}
