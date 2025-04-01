using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationHandler : MonoBehaviour
{
    private readonly static int _FireHash = Animator.StringToHash("Fire");
    private readonly static int _ReloadHash = Animator.StringToHash("Reload");
    private readonly static int _RunHash = Animator.StringToHash("Run");
    private readonly static int _AimModeHash = Animator.StringToHash("AimMode");
    private readonly static int _HasAmmoHash = Animator.StringToHash("HasAmmo"); 

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Fire()
    {
        _animator.SetTrigger(_FireHash);
    }

    public void Reload()
    {
        _animator.SetTrigger(_ReloadHash);
    }

    public void Run(bool active)
    {
        _animator.SetBool(_RunHash, active);
    } 

    public void AimMode(bool active)
    {
        _animator.SetBool(_AimModeHash, active);
    }
    
    public void HasAmmo(bool active)
    {
        _animator.SetBool(_HasAmmoHash, active);
    }

}
