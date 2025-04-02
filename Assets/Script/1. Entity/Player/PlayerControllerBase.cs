using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour
{
    [Header("공통 참조")]
    [SerializeField] protected Transform head;
    [SerializeField] public Animator animator;

    public PlayerStateMachine StateMachine { get; protected set; }
    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        StateMachine = new PlayerStateMachine(this);
    }

    protected virtual void Update()
    {
        StateMachine?.Update();
    }

    public abstract PlayerStateData ToPlayerStateData();
    public abstract void ApplyNetworkState(PlayerStateData data);
    public virtual void PlayFireAnim() => animator?.SetTrigger("Fire");
    public virtual bool HasMoveInput() => false;
    public virtual bool IsGrounded() => true;
    public virtual bool IsJumpInput() => false;
    public virtual bool IsFiring() => false;
    public abstract void HandleMovement();
    public abstract void ApplyGravity();
    public abstract void StartJump();
    public abstract void HandleFire(bool started);
    public abstract float GetVerticalVelocity();
    public virtual Vector3 GetMoveInput() => Vector3.zero;
}