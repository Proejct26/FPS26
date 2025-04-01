using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RemotePlayerController : PlayerControllerBase
{
    private PlayerStateData _networkData;   //서버로부터 받을 적의 데이터

    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

    private float _verticalVelocity = 0f;   //수평 방향 속도
    private float _gravity = -9.81f;        //중력 값
    private float _lerpSpeed = 10f;         //보간 정도

    [SerializeField] private Transform weaponHolder; // 손에 붙이는 슬롯
    [SerializeField] private Transform weaponFix;   //방향 조정
    [SerializeField] private List<GameObject> weaponPrefabs; // 인덱스별 총기 리스트

    [Header("이름 태그")]
    [SerializeField] public PlayerNameTag nameTag;
    private GameObject equippedWeapon;

    protected override void Update()
    {
        base.Update();

        if (_networkData == null)
            return;

        // 보간 위치/회전 갱신
        _networkPosition = Vector3.Lerp(_networkPosition, _networkData.position, Time.deltaTime * _lerpSpeed);
        _networkRotation = Quaternion.Slerp(_networkRotation, Quaternion.Euler(0f, _networkData.rotationY, 0f), Time.deltaTime * _lerpSpeed);

        transform.position = _networkPosition;
        transform.rotation = _networkRotation;

        StateMachine?.Update();

        if (weaponFix != null)
        {
            // 캐릭터의 정면 방향을 따라 회전 고정
            weaponFix.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
    }

    public override void ApplyNetworkState(PlayerStateData data)
    {
        if (data == null) return;

        _networkData = data;

        if (head != null)
            head.localRotation = Quaternion.Euler(-_networkData.rotationX, 0f, 0f);

        if (nameTag != null)
        {
            nameTag.player = transform;
            nameTag.SetName(_networkData.name); // playerName은 PlayerStateData에 있어야 함
            nameTag.nameText.color = _networkData.team == 0 ? Color.red : Color.blue;
        }

        EquipWeapon(data.weapon);
    }

    public override PlayerStateData ToPlayerStateData()
    {
        return null; // 서버로 보낼 필요 없음
    }

    public override bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.8f, LayerMask.GetMask("Default"));
    }
    public override bool IsJumpInput() => _networkData != null && _networkData.isJumping;
    public override bool HasMoveInput() => _networkData != null && _networkData.moveInput.sqrMagnitude > 0.01f;
    public override bool IsFiring() => _networkData != null && _networkData.isFiring;

    public override float GetVerticalVelocity() => _verticalVelocity;

    public override void HandleMovement()
    {
        Vector3 forward = _networkRotation * Vector3.forward;
        Vector3 right = _networkRotation * Vector3.right;

        Vector3 move = right * _networkData.moveInput.x + forward * _networkData.moveInput.z;
        move.y = 0f;
        _networkPosition += move.normalized * Time.deltaTime * 3f;
    }

    public override void ApplyGravity()
    {
        if (IsGrounded())
        {
            if (_verticalVelocity < 0)
            {
                _verticalVelocity = -2f;

                if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 1.2f, LayerMask.GetMask("Default")))
                {
                    _networkPosition.y = hit.point.y;
                }
            }
        }

        _verticalVelocity += _gravity * Time.deltaTime;
        _networkPosition += new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime;
    }

    public override void StartJump()
    {
        _verticalVelocity = Mathf.Sqrt(5f * -2f * _gravity);
    }

    /// <summary>
    /// 발사 입력 시 호출
    /// </summary>
    public override void HandleFire(bool started)
    {
        /*if (_equippedWeapon == null)
        {
            Debug.LogWarning("무기가 없습니다.");
            return;
        }
        _equippedWeapon.Attack(started);*/
    }

    public void EquipWeapon(int weaponType)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon);

        if (weaponType < 0 || weaponType >= weaponPrefabs.Count)
        {
            Debug.LogWarning($"잘못된 무기 타입: {weaponType}");
            return;
        }

        equippedWeapon = Instantiate(weaponPrefabs[weaponType], weaponFix);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.Euler(0, 180f, 0);
    }

}