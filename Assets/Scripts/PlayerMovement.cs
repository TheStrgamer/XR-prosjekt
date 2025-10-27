using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float moveSpeed = 5f;

    private Vector2 moveInput;

    // Update is called once per frame
    void Update()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
}
