using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float verticalSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalInput;

    [Header("Dig Settings")]
    public float digRadius = 3f;
    public float digStrength = 3f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleDig();
    }

    private void HandleMovement()
    {
        Vector3 horizontalMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 verticalMove = Vector3.up * verticalInput;
        transform.position += (horizontalMove * moveSpeed + verticalMove * verticalSpeed) * Time.deltaTime;
    }
    private void HandleLook()
    {
        transform.Rotate(Vector3.up, lookInput.x * lookSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.left, lookInput.y * lookSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleDig()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Collider[] colliders = Physics.OverlapSphere(hit.point, digRadius);
                foreach (Collider c in colliders)
                {
                    //Debug.Log(c.gameObject.name);
                    MarchingCubes mc = c.GetComponent<MarchingCubes>();
                    if (mc != null)
                    {
                        mc.Dig(hit.point, digRadius, digStrength);
                    }
                }
            }
        }
    }
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnUpDown(InputValue value) => verticalInput = value.Get<float>();
}
