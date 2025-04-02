using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 이동, 점프, 중력 및 상태 FSM을 관리하는 컨트롤러
/// </summary>
public class LocalPlayerController : PlayerControllerBase
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerStatHandler _statHandler;
    private PlayerInputHandler _input;
    


    /*[Header("무기 관련")]
    [SerializeField] private Transform weaponHolder;                      // 무기 부착 위치
    [SerializeField] private GameObject startingWeaponPrefab;             // 초기 무기 프리팹 */
    private WeaponBaseController _equippedWeapon;
    
    /// <summary>
    /// FSM 상태 머신 인스턴스
    /// </summary>
    private float _verticalVelocity;
    private float _gravity = -9.81f;

    public string playerId;
    private bool _hitSuccess;
    private string _hitTargetId;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        _statHandler.OnDeath += OnDeath;
    }

    private void Start()
    {
        StateMachine.ChangeState(new PlayerIdleState(this));

        var stateData = ToPlayerStateData();
        StartCoroutine(SendStateToServer(stateData));
    }

    protected override void Update()
    {
        base.Update();
 
    }
 

    private IEnumerator SendStateToServer(PlayerStateData data)
    {
       while (true)
       {
            CS_POS_INTERPOLATION posInterpolationPacket = new CS_POS_INTERPOLATION();
            posInterpolationPacket.PosX = transform.position.x;
            posInterpolationPacket.PosY = transform.position.y;
            posInterpolationPacket.PosZ = transform.position.z;

            Managers.Network.Send(posInterpolationPacket);


            
            yield return new WaitForSeconds(0.05f); 
       }
    }


    /// <summary>
    /// 플레이어 이동 입력이 있는지 확인
    /// </summary>
    /// <returns>이동 입력 여부</returns>
    public override bool HasMoveInput() => _input.MoveInput.sqrMagnitude > 0.01f;

    /// <summary>
    /// 점프 입력이 눌렸는지 확인
    /// </summary>
    /// <returns>점프 키가 눌렸는지 여부</returns>
    public override bool IsJumpInput() => _input.IsJumping;

    public override bool IsFiring() => _input.IsFiring;


    /// <summary>
    /// 현재 캐릭터가 땅 위에 있는지 여부
    /// </summary>
    public override bool IsGrounded() => _controller.isGrounded;

    /// <summary>
    /// 캐릭터의 수직 속도 값 반환 (점프, 낙하 판정용)
    /// </summary>
    public override float GetVerticalVelocity() => _verticalVelocity;



    /// <summary>
    /// 이동 입력을 받아 캐릭터 이동 처리
    /// </summary>
    public override void HandleMovement()
    {
        Vector3 input = _input.MoveInput;

        /*Vector3 move = head.right * input.x + head.forward * input.z;
        _controller.Move(move * _statHandler.MoveSpeed * Time.deltaTime);*/

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = right * input.x + forward * input.z;
        move.y = 0f; // 수직 방향 제거
        _controller.Move(move.normalized * _statHandler.MoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 중력 계산 및 적용
    /// </summary>
    public override void ApplyGravity()
    {
        if (IsGrounded() && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f; // 살짝 붙어 있도록 음수 유지
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }

        Vector3 gravityMove = new Vector3(0f, _verticalVelocity, 0f);
        _controller.Move(gravityMove * Time.deltaTime);
    }

    /// <summary>
    /// 점프 시작 시 수직 속도 계산
    /// </summary>
    public override void StartJump()
    {
        _verticalVelocity = Mathf.Sqrt(_statHandler.JumpPower * -2f * _gravity);
    }



    /// <summary>
    /// 무기를 장착하고 기존 무기는 제거
    /// </summary>
    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (_equippedWeapon != null)
            Destroy(_equippedWeapon.gameObject);

        /*GameObject newWeapon = Instantiate(weaponPrefab, weaponHolder);
        _equippedWeapon = newWeapon.GetComponent<WeaponBaseController>();*/

        if (_equippedWeapon == null)
            Debug.LogError("장착한 무기에 WeaponBaseController가 없습니다.");
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

    public override PlayerStateData ToPlayerStateData()
    {
        return null;       //데이터 전달하면 되지 않을까?
    }
    public override void ApplyNetworkState(PlayerStateData data)
    {
        
    }

    public void SetNetworkPosition(Vector3 serverPos)
    {
        float distance = Vector3.Distance(transform.position, serverPos);

        // 서버와 위치 차이가 너무 크면 순간이동
        if (distance > 5f)
        {
            transform.position = serverPos;
        }
        // 적당히 차이나면 보간으로 보정
        else if (distance > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, serverPos, Time.deltaTime * 5f);
        }
        // 거의 차이 없으면 무시
    }
    
    public void OnDeath()
    {
        GetComponentInChildren<PlayerWeaponHandler>().Clear(); 
        Managers.UI.ShowPopupUI<RespawnUI>("RespawnUI"); 
    }

    public void OnRespawn()
    {
        GetComponentInChildren<PlayerWeaponHandler>().InitWeapons(); 
        PlayerStatHandler playerStatHandler = GetComponentInChildren<PlayerStatHandler>();
        playerStatHandler.SetHealth(playerStatHandler.MaxHealth);

        Managers.Input.SetActive(true); 
    }


} 