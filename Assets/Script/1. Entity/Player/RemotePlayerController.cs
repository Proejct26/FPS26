using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RemotePlayerController : PlayerControllerBase
{
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;
    private float _lerpSpeed = 10f;

    protected override void Update()
    {
        base.Update();

        transform.position = Vector3.Lerp(transform.position, _networkPosition, Time.deltaTime * _lerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, _networkRotation, Time.deltaTime * _lerpSpeed);
    }

    public override void ApplyNetworkState(PlayerStateData data)
    {
        _networkPosition = data.position;
        _networkRotation = Quaternion.Euler(0f, data.rotationY, 0f);

        if (head != null)
            head.localRotation = Quaternion.Euler(-data.rotationX, 0f, 0f);

        // FSM 상태 처리
        if (data.isFiring)
            StateMachine.ChangeState(new PlayerAttackState(this));
        else if (data.isJumping)
            StateMachine.ChangeState(new PlayerJumpState(this));
        else if (!IsGrounded())
            StateMachine.ChangeState(new PlayerFallState(this));
        else if (data.moveInput.magnitude > 0.1f)
            StateMachine.ChangeState(new PlayerMoveState(this));
        else
            StateMachine.ChangeState(new PlayerIdleState(this));
    }

    public override PlayerStateData ToPlayerStateData()
    {
        return null; // 서버로 보낼 필요 없음
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, LayerMask.GetMask("Ground"));
    }
}