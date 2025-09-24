using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    private Vector2 moveInput;
    private Vector2 lookInput;

    void Update()
    {
        Vector3 dir = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, lookInput.x * lookSpeed/10, Space.World);
        transform.Rotate(Vector3.left, lookInput.y * lookSpeed/10, Space.Self);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

}
