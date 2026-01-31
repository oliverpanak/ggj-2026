using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    private Vector3 inputDirection;
	[SerializeField] float speed;

	private void FixedUpdate()
	{
		controller.Move(inputDirection * speed * Time.fixedDeltaTime);
	}

	public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 input = context.ReadValue<Vector2>();
		inputDirection = new Vector3(input.x, 0, input.y);

	}
}
