using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PlayerStatHandler statHandler;

    private CharacterController _controller;
    private Vector3 _moveVelocity;
    private float _gravity = -9.81f;
    private float _verticalVelocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        Vector2 input = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _moveVelocity = move * statHandler.MoveSpeed;
        _controller.Move(_moveVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (_controller.isGrounded &&
            Managers.Input.GetInput(EPlayerInput.Jump).WasPressedThisFrame())
        {
            _verticalVelocity = Mathf.Sqrt(statHandler.JumpPower * -2f * _gravity);
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; // 땅에 붙임
        }

        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 gravityMove = new Vector3(0f, _verticalVelocity, 0f);
        _controller.Move(gravityMove * Time.deltaTime);
    }
}
