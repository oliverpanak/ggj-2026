using UnityEngine;
using UnityEngine.Splines;

public class Stamp : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float interval = 3f;
    [SerializeField] private float downSpeed = 5f;
    [SerializeField] private float upSpeed = 2f;
    [SerializeField] private float stayDownDuration = 1f;

    [Header("Movement")]
    [SerializeField] private Transform stampHead;
    [SerializeField] private Collider stampZone;

    [Header("Safe Zone")]
    [SerializeField] private SplineContainer safeZoneSpline;
    [SerializeField] private float safeDistance = 1f;
    [SerializeField] private int splineSamplePoints = 20;

    [Header("Players")]
    [SerializeField] private Transform[] players;
    [SerializeField] private float crushForce = 20f;

    private Vector3 startPosition;
    private Vector3 downPosition;
    private float timer;
    private StampState state = StampState.Waiting;

    private enum StampState
    {
        Waiting,
        GoingDown,
        StayingDown,
        GoingUp
    }

    private void Start()
    {
        if (stampHead == null)
            stampHead = transform;

        startPosition = stampHead.localPosition;
        downPosition = Vector3.zero;
        timer = interval;
    }

    private void Update()
    {
        switch (state)
        {
            case StampState.Waiting:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = StampState.GoingDown;
                }
                break;

            case StampState.GoingDown:
                stampHead.localPosition = Vector3.MoveTowards(
                    stampHead.localPosition,
                    downPosition,
                    downSpeed * Time.deltaTime
                );

                if (Vector3.Distance(stampHead.localPosition, downPosition) < 0.01f)
                {
                    stampHead.localPosition = downPosition;
                    OnStampDown();
                    timer = stayDownDuration;
                    state = StampState.StayingDown;
                }
                break;

            case StampState.StayingDown:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = StampState.GoingUp;
                }
                break;

            case StampState.GoingUp:
                stampHead.localPosition = Vector3.MoveTowards(
                    stampHead.localPosition,
                    startPosition,
                    upSpeed * Time.deltaTime
                );

                if (Vector3.Distance(stampHead.localPosition, startPosition) < 0.01f)
                {
                    stampHead.localPosition = startPosition;
                    timer = interval;
                    state = StampState.Waiting;
                }
                break;
        }
    }

    private void OnStampDown()
    {
        foreach (var player in players)
        {
            if (player == null) continue;

            if (!IsPlayerInStampZone(player.position)) continue;

            if (!IsPlayerSafe(player.position))
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.AddForce(Vector3.up * crushForce, ForceMode.Impulse);
                }
                Debug.Log($"Player {player.name} was crushed by stamp!");
            }
        }
    }

    private bool IsPlayerInStampZone(Vector3 playerPosition)
    {
        if (stampZone == null) return true; // No zone defined, always in zone
        return stampZone.bounds.Contains(playerPosition);
    }

    private bool IsPlayerSafe(Vector3 playerPosition)
    {
        if (safeZoneSpline == null || safeZoneSpline.Spline == null)
            return false;

        float closestDistance = float.MaxValue;

        // Sample points along the spline to find closest distance
        for (int i = 0; i <= splineSamplePoints; i++)
        {
            float t = i / (float)splineSamplePoints;
            Vector3 splinePoint = safeZoneSpline.EvaluatePosition(t);

            float distance = Vector3.Distance(playerPosition, splinePoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        return closestDistance <= safeDistance;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw safe zone around spline
        if (safeZoneSpline == null || safeZoneSpline.Spline == null)
            return;

        Gizmos.color = Color.green;
        int segments = 50;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = safeZoneSpline.EvaluatePosition(t);

            // Draw sphere at each point showing safe distance
            Gizmos.DrawWireSphere(point, safeDistance);
        }
    }
}
