using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandaler : MonoBehaviour
{
    private Animator _animator;
    private PlayerInputHandler _input;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        Vector3 move = _input.MoveInput;

        _animator.SetFloat("moveX", move.x);
        _animator.SetFloat("moveZ", move.z);
        _animator.SetBool("isJumping", _input.IsJumping);
        _animator.SetBool("isFiring", _input.IsFiring);
        _animator.SetBool("isZooming", _input.IsZooming); // 우클릭 줌
    }
}
