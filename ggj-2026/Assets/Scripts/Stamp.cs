using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Stamp : MonoBehaviour
{
    [Header("Timing")]
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
    [SerializeField] private float crushForce = 20f;
    [SerializeField] private GameObject stampColliderObject;
    [SerializeField] private float colliderDisableTime = 0.2f;

    [Header("Barrier")]
    [SerializeField] private Transform barrier;
    [SerializeField] private Transform barrierLiftTarget;

    private Vector3 startPosition;
    private Vector3 downPosition;
    private Vector3 barrierStartPosition;
    private float timer;
    private StampState state = StampState.Waiting;
    private bool barrierLifted;

    private IReadOnlyList<Transform> Players => PlayerManager.Instance?.Players;

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

        if (barrier != null)
            barrierStartPosition = barrier.position;

        // Subscribe to GameManager_Y's stamp event
        if (GameManager_Y.Instance != null)
        {
            GameManager_Y.Instance.onStamp += OnStampTriggered;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager_Y.Instance != null)
        {
            GameManager_Y.Instance.onStamp -= OnStampTriggered;
        }
    }

    private void OnStampTriggered()
    {
        // Only trigger if we're in waiting state
        if (state == StampState.Waiting)
        {
            if (stampColliderObject != null)
                stampColliderObject.SetActive(false);
            state = StampState.GoingDown;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case StampState.Waiting:
                // Stamp is triggered by GameManager_Y.onStamp event
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
                    // Check if barrier should lift when stamp starts going up
                    if (!barrierLifted && AreAllPlayersInZone())
                    {
                        barrierLifted = true;
                    }
                    state = StampState.GoingUp;
                }
                break;

            case StampState.GoingUp:
                stampHead.localPosition = Vector3.MoveTowards(
                    stampHead.localPosition,
                    startPosition,
                    upSpeed * Time.deltaTime
                );

                // Lift barrier at same time and speed as stamp
                if (barrierLifted && barrier != null && barrierLiftTarget != null)
                {
                    barrier.position = Vector3.MoveTowards(
                        barrier.position,
                        barrierLiftTarget.position,
                        upSpeed * Time.deltaTime
                    );
                }

                if (Vector3.Distance(stampHead.localPosition, startPosition) < 0.01f)
                {
                    stampHead.localPosition = startPosition;
                    state = StampState.Waiting;
                }
                break;
        }
    }

    private bool AreAllPlayersInZone()
    {
        if (Players == null) return false;

        foreach (var player in Players)
        {
            if (player == null) continue;
            if (!IsPlayerInStampZone(player.position))
                return false;
        }
        return true;
    }

    private void OnStampDown()
    {
        bool anyPlayerCrushed = false;

        if (Players == null) return;

        // Apply force to crushed players
        foreach (var player in Players)
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
                anyPlayerCrushed = true;
            }
        }

        // Trigger game over if a player was crushed
        if (anyPlayerCrushed)
        {
            GameOverManager.Instance?.TriggerGameOver();
        }

        // Re-enable collider (with delay if someone was crushed, immediately otherwise)
        if (stampColliderObject != null)
        {
            if (anyPlayerCrushed)
            {
                StartCoroutine(ReenableCollider());
            }
            else
            {
                stampColliderObject.SetActive(true);
            }
        }
    }

    private IEnumerator ReenableCollider()
    {
        yield return new WaitForSeconds(colliderDisableTime);
        stampColliderObject.SetActive(true);
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
