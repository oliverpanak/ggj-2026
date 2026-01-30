using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsPlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveForce = 10f;

    private Vector3 inputDirection;

    private void FixedUpdate()
    {
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            rb.AddForce(inputDirection * moveForce, ForceMode.Force);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        inputDirection = new Vector3(input.x, 0, input.y);
    }
}
