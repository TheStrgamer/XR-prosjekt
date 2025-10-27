using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    private Vector2 mouseInput;

    public Transform playerBody;


    float yRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseInput.x);
    }

    public void OnLook(InputValue value) => mouseInput = value.Get<Vector2>();
}
