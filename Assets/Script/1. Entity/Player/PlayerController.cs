using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerStatHandler statHandler;

    public PlayerStateMachine StateMachine { get; private set; }

    private float _verticalVelocity;
    private float _gravity = -9.81f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        StateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        StateMachine.ChangeState(new PlayerIdleState(this));
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public bool HasMoveInput()
    {
        Vector2 input = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        return input.sqrMagnitude > 0.01f;
    }

    public bool IsJumpInput()
    {
        return Managers.Input.GetInput(EPlayerInput.Jump).WasPressedThisFrame();
    }

    public void HandleMovement()
    {
        Vector2 input = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _controller.Move(move * statHandler.MoveSpeed * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 gravityMove = new Vector3(0f, _verticalVelocity, 0f);
        _controller.Move(gravityMove * Time.deltaTime);
    }

    public void StartJump()
    {
        _verticalVelocity = Mathf.Sqrt(statHandler.JumpPower * -2f * _gravity);
    }

    public float GetVerticalVelocity() => _verticalVelocity;

    public bool IsGrounded() => _controller.isGrounded;
}