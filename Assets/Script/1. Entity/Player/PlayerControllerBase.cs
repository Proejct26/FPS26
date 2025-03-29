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
        StateMachine = new PlayerStateMachine();
    }

    protected virtual void Update()
    {
        StateMachine?.Update();
    }

    public abstract PlayerStateData ToPlayerStateData();
    public abstract void ApplyNetworkState(PlayerStateData data);

    public virtual void SetMoveAnim(bool isMoving) => animator?.SetBool("IsMoving", isMoving);
    public virtual void SetJumpAnim(bool isJumping) => animator?.SetBool("IsJumping", isJumping);
    public virtual void PlayFireAnim() => animator?.SetTrigger("Fire");

    public virtual bool HasMoveInput() => false;

    public virtual bool IsGrounded() => true;

    public virtual bool IsJumpInput() => false;

    public virtual bool IsFiring() => false;
}