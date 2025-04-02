using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RemotePlayerController : PlayerControllerBase
{
    private PlayerStateData _networkData;   //서버로부터 받을 적의 데이터
    private DummyGunController _dummyGunController; 
    private Vector3 _networkPosition;

    private Vector3 _correctedPosition;
    private float _correctionSpeed = 20f;

    private Quaternion _networkRotation;

    private PlayerStatHandler _statHandler;

[SerializeField] private Transform weaponHolder; // 손에 붙이는 슬롯
    [SerializeField] private Transform weaponFix;   //방향 조정

    [Header("이름 태그")]
    [SerializeField] public PlayerNameTag nameTag;

    [SerializeField] private Transform model;   // HunterRio, CowboyRio
    [SerializeField] private Transform bodyCollider;  // Rigidbody + CapsuleCollider
    [SerializeField] private LayerMask groundLayer;


    private GameObject equippedWeapon;


    private float _currentYaw = 0f;

    private Vector3 _inputMove;
    private float _inputYaw;
    private float _inputPitch;
    private bool _isJumping, _isInAir;
    private float _jumpElapsed = 0f;
    private float _jumpDuration = 0.5f;
    private float _jumpHeight = 0.5f;
    private Vector3 _jumpStartPos;
    private bool _isJumpingUp = false;
    private float _lastY;
    private float _verticalVelocity;

    private Vector3 _lookDirection;

    private Queue<Vector3> _positionQueue = new Queue<Vector3>();
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _lerpTime = 0f;
    private float _lerpDuration = 0.05f; // 한 보간 단위 시간 (ms 단위로도 설정 가능)  
    private bool _isInterpolating = false;

    private Rigidbody _rb;

    public PlayerStateData PlayerStateData => _networkData;
    public DummyGunController DummyGunController => _dummyGunController; 
    //public PlayerStat

    protected override void Awake()
    {
        base.Awake();


        _rb = bodyCollider.GetComponent<Rigidbody>();
        _statHandler = GetComponent<PlayerStatHandler>();


        _jumpHeight = _statHandler.JumpPower;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        foreach (var collider in transform.GetComponentsInChildren<PlayerCollider>())
            collider.Parent = gameObject; 
         
        GetComponentInChildren<PlayerStatHandler>().OnDeath += OnDeath; 
    }


    protected override void Update()
    {
        base.Update();
        if (_networkData == null) return;

        // 수직 속도 계산
        _verticalVelocity = (transform.position.y - _lastY) / Time.deltaTime;

        if (_isJumpingUp)
            HandleJump();

        // 중력
        ApplyGravity();
        ApplyCorrection();


        if (_lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(_lookDirection.x, 0f, _lookDirection.z));

        if (head != null)
            head.localRotation = Quaternion.Euler(-_inputPitch, 0f, 0f);

        if (weaponFix != null)
            weaponFix.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

        StateMachine?.Update();
    }

    private void OnDeath()
    {
        Managers.GameSceneManager.PlayerManager.OnDeath(_networkData.networkId);
        MyDebug.Log($"RemotePlayerController OnDeath : {_networkData.networkId}"); 
    }
    private void HandleJump()
    {
        _jumpElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_jumpElapsed / _jumpDuration);
        float height = Mathf.SmoothStep(0f, _jumpHeight, t);
        Vector3 pos = _jumpStartPos;
        pos.y += height;
        transform.position = pos;

        if (t >= 1f)
            _isJumpingUp = false;
    }

    private Vector3 _velocity = Vector3.zero;

    private void ApplyCorrection()
    {
        if (_isInAir || _isJumpingUp) return;

        float dist = Vector3.Distance(transform.position, _correctedPosition);

        if (dist > 0.01f)
        {
            float lerpFactor = Mathf.Clamp01(_correctionSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, _correctedPosition, lerpFactor);
        }
    }


    public void SetNetworkInput(Vector3 moveInput, float pitch, float yaw, bool isJumping, Vector3 lookDir)
    {
        _inputMove = moveInput;
        _inputPitch = pitch;
        _inputYaw = yaw;
        _lookDirection = lookDir != Vector3.zero ? lookDir.normalized : _lookDirection;
        _isJumping = isJumping;

        if (isJumping && !_isInAir && IsGrounded())
            _isJumping = true;
    }

    public void SetNetworkPosition(Vector3 pos)
    {
        if (_isInAir || _isJumpingUp) return;

        Ray ray = new Ray(pos + Vector3.up * 1f, Vector3.down);
        _networkPosition = Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer) ? hit.point : pos;

        _correctedPosition = _networkPosition;
    }
    public override bool IsGrounded()
    {
        Vector3 origin = bodyCollider.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 1.0f, groundLayer);
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
            nameTag.SetName(_networkData.name); // playerName은 PlayerStateData에 있어야 함f
            nameTag.nameText.color = _networkData.team == 0 ? Color.red : Color.blue;
        }

        EquipWeapon(data.weapon);
    }

    public override PlayerStateData ToPlayerStateData()
    {
        return null; // 서버로 보낼 필요 없음
    }

   
    public override bool IsJumpInput() => _isJumping;
    public override bool HasMoveInput() => _inputMove.sqrMagnitude > 0.01f;
    public override bool IsFiring() => _networkData != null && _networkData.isFiring;

    // Remote 플레이어는 직접 이동 로직 없음 (서버 입력을 신뢰)
    public override void HandleMovement()
    {
        float moveSpeed = _statHandler.MoveSpeed;
        Vector3 move = -transform.right * _inputMove.x + transform.forward * _inputMove.z;
        move.y = 0f;

        if (move.sqrMagnitude > 0.01f)
        {
            Vector3 desiredPosition = transform.position + move.normalized * moveSpeed * Time.deltaTime;
            transform.position = desiredPosition;
        }
    }
    public void SetAirborne(bool value) => _isInAir = value;
    public override float GetVerticalVelocity() => _verticalVelocity;
    public override Vector3 GetMoveInput() => _inputMove;

    public override void ApplyGravity() {
        if (!_isJumpingUp && _isInAir)
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            _correctedPosition.y += _verticalVelocity * Time.deltaTime;

            if (IsGrounded())
            {
                _verticalVelocity = 0f;
                _isInAir = false;
                _correctedPosition.y = Mathf.Max(_correctedPosition.y, _networkPosition.y); // 지면 아래로 꺼지는 현상 방지
            }
        }
    }

    public override void StartJump() {
        if (!IsGrounded()) return;

        _isInAir = true;
        _isJumpingUp = true;
        _jumpElapsed = 0f;
        _jumpStartPos = transform.position;
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
        _dummyGunController = equippedWeapon.GetComponent<DummyGunController>(); 
    }
}