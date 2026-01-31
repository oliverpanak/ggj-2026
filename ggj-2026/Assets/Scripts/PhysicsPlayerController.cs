using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform nextPlayer;

    [SerializeField] private Animator animatorController;

    public Transform NextPlayer
    {
        get => nextPlayer;
        set => nextPlayer = value;
    }

    private Vector3 inputDirection;

    private void FixedUpdate()
    {
        bool walking = inputDirection.sqrMagnitude > 0.01f;
        animatorController.SetBool("Walking", walking);

        if (walking)
        {
            rb.AddForce(inputDirection * moveForce, ForceMode.Force);
        }
    }

    private void LateUpdate()
    {
        if (lineRenderer != null && NextPlayer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, NextPlayer.position);
        }
        else if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    public void OnMove(InputValue value)
    {

        Vector2 input = value.Get<Vector2>();
        inputDirection = new Vector3(input.x, 0, input.y);
    }
}
