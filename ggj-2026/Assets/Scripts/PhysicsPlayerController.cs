using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PhysicsPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform nextPlayer;

    [SerializeField] private Animator animatorController;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform visualTransform;

    public Transform NextPlayer
    {
        get => nextPlayer;
        set => nextPlayer = value;
    }

    private Vector3 inputDirection;

    private void Start()
    {
        SceneManager.activeSceneChanged += (Scene, Scene2) => { transform.position = new Vector3(0, 2, 0); };
    }

    private void FixedUpdate()
    {
        bool walking = inputDirection.sqrMagnitude > 0.01f;
        animatorController.SetBool("Walking", walking);

        if (walking)
        {
            rb.AddForce(inputDirection * moveForce, ForceMode.Force);

            // Rotate to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            Transform toRotate = visualTransform != null ? visualTransform : transform;
            toRotate.rotation = Quaternion.Slerp(toRotate.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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
