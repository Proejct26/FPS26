using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RemotePlayerController : PlayerControllerBase
{
    private Vector3 _networkPosition;
    private Quaternion _networkRotation;
    private float _lerpSpeed = 10f;

    private Vector3 _moveInput;
    private bool _isJumping;
    private bool _isFiring;

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

        // 내부 상태 저장
        _moveInput = data.moveInput;
        _isJumping = data.isJumping;
        _isFiring = data.isFiring;
        string cur = "";
        // FSM 상태 처리
        if (_isFiring) {
            cur = "발사";
            StateMachine.ChangeState(new PlayerAttackState(this));
        }
        if (_isJumping){
            cur = "점프";
            StateMachine.ChangeState(new PlayerJumpState(this));
        }
        if (!IsGrounded())
            { cur = "낙하"; StateMachine.ChangeState(new PlayerFallState(this)); }
        if (_moveInput.magnitude > 0.1f)
            StateMachine.ChangeState(new PlayerMoveState(this));
        else
        { cur = "정지"; StateMachine.ChangeState(new PlayerIdleState(this)); }
        Debug.Log($"{cur}");
    }

    public override PlayerStateData ToPlayerStateData()
    {
        return null; // 서버로 보낼 필요 없음
    }

    public override bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, LayerMask.GetMask("Default"));
    }
}