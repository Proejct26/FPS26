using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RemotePlayerController : PlayerControllerBase
{
    private PlayerStateData _networkData;   //서버로부터 받을 적의 데이터

    private Vector3 _networkPosition;
    private Quaternion _networkRotation;

    [SerializeField] private Transform weaponHolder; // 손에 붙이는 슬롯
    [SerializeField] private Transform weaponFix;   //방향 조정

    [Header("이름 태그")]
    [SerializeField] public PlayerNameTag nameTag;

    [SerializeField] private Transform model;   // HunterRio, CowboyRio
    [SerializeField] private Transform bodyCollider;  // Rigidbody + CapsuleCollider
    [SerializeField] private LayerMask groundLayer;

    private GameObject equippedWeapon;

    private Vector3 _inputMove;
    private float _inputYaw;
    private float _inputPitch;
    private bool _isJumping;

    private Queue<Vector3> _positionQueue = new Queue<Vector3>();
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _lerpTime = 0f;
    private float _lerpDuration = 0.26f; // 한 보간 단위 시간 (ms 단위로도 설정 가능)
    private bool _isInterpolating = false;

    private Rigidbody _rb;

    public PlayerStateData PlayerStateData => _networkData;

    protected override void Awake()
    {
        base.Awake();

        _rb = bodyCollider.GetComponent<Rigidbody>();

        _rb.useGravity = true;  // 중력 작동
        _rb.isKinematic = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        foreach (var collider in transform.GetComponentsInChildren<PlayerCollider>())
            collider.Parent = gameObject; 
         
    }

    public void SetNetworkInput(Vector3 moveInput, float pitch, float yaw, bool isJumping)
    {
        _inputMove = moveInput;
        _inputPitch = pitch;
        _inputYaw = yaw;
        _isJumping = isJumping;
    }

    public void SetNetworkPosition(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer))
        {
            _networkPosition = hit.point;
        }
        else
        {
            _networkPosition = pos;
        }

        if (_positionQueue.Count > 3)
        {
            while (_positionQueue.Count > 2)
                _positionQueue.Dequeue(); // 오래된 것 버리기
        }

        _positionQueue.Enqueue(_networkPosition);
    }


    protected override void Update()
    {
        base.Update();

        if(_networkData == null) return;

        // // 캐릭터 transform은 rigidbody 따라가기
        // transform.position = _rb.position;
        // transform.rotation = _rb.rotation;

        InterpolatePosition(); // ← 보간 이동 처리


        // 시야 회전
        if (head != null)
            head.localRotation = Quaternion.Euler(-_inputPitch, 0f, 0f);

        // 무기 정렬
        if (weaponFix != null)
            weaponFix.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        // FSM 동작 (상태 전이 및 애니메이션 포함)
        StateMachine?.Update();
    }

    private void InterpolatePosition()
    {
        if (!_isInterpolating)
        {
            if (_positionQueue.Count > 0)
            {
                _startPos = transform.position;
                _targetPos = _positionQueue.Dequeue();
                _lerpTime = 0f;
                _isInterpolating = true;
            }
            else return;
        }

        _lerpTime += Time.deltaTime;
        float t = Mathf.Clamp01(_lerpTime / _lerpDuration);

        // 부드럽게 LERP
        transform.position = Vector3.Lerp(_startPos, _targetPos, t);

        if (t >= 1f)
        {
            transform.position = _targetPos;
            _isInterpolating = false;

            if (_positionQueue.Count > 0)
            {
                _startPos = _targetPos;
                _targetPos = _positionQueue.Dequeue();
                _lerpTime = 0f;

                transform.position = Vector3.Lerp(_startPos, _targetPos, 0f);
                _isInterpolating = true;
            }
        }
    }

    public override void ApplyNetworkState(PlayerStateData data)
    {
        if (_networkData == null)
        {
            _networkPosition = data.position;  //초기 위치 세팅
        }
        _networkData = data;

        if (data == null) return;

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
        Vector3 origin = bodyCollider.position + Vector3.up * 0.1f;
        Debug.DrawRay(origin, Vector3.down * 1.2f, Color.red);
        return Physics.Raycast(origin, Vector3.down, 1.2f, LayerMask.GetMask("Default"));
    }

    public override bool IsJumpInput() => _isJumping;
    public override bool HasMoveInput() => _inputMove.sqrMagnitude > 0.01f;
    public override bool IsFiring() => _networkData != null && _networkData.isFiring;
    
    // Remote 플레이어는 직접 이동 로직 없음 (서버 입력을 신뢰)
    public override void HandleMovement() { }
    public override float GetVerticalVelocity() => 0f;
    public override Vector3 GetMoveInput() => _inputMove;

    public override void ApplyGravity() { }
    public override void StartJump() {
        if (_rb == null) return;

        Vector3 velocity = _rb.velocity;
        velocity.y = Mathf.Sqrt(2f * 9.81f * 1.2f); // 높이 1.2m 기준 점프력
        _rb.velocity = velocity;
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

    public void EquipWeapon(int weaponId)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon);

        WeaponDataSO weaponData = Managers.Data.WeaponData.GetWeaponData(weaponId);

        if (weaponData == null)
            return;

        equippedWeapon = Instantiate(weaponData.dummyPrefab, weaponFix);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.Euler(0, 180f, 0);  
    }

}