using UnityEngine;

/// <summary>
/// 플레이어의 이동, 점프, 중력 및 상태 FSM을 관리하는 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerStatHandler statHandler;


    /*[Header("무기 관련")]
    [SerializeField] private Transform weaponHolder;                      // 무기 부착 위치
    [SerializeField] private GameObject startingWeaponPrefab;             // 초기 무기 프리팹 */
    private WeaponBaseController _equippedWeapon;


    /// <summary>
    /// FSM 상태 머신 인스턴스
    /// </summary>
    public PlayerStateMachine StateMachine { get; private set; }

    private float _verticalVelocity;
    private float _gravity = -9.81f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        StateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        StateMachine.ChangeState(new PlayerIdleState(this));
    }

    private void Update()
    {
        StateMachine.Update();
    }

    /// <summary>
    /// 플레이어 이동 입력이 있는지 확인
    /// </summary>
    /// <returns>이동 입력 여부</returns>
    public bool HasMoveInput()
    {
        Vector2 input = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        return input.sqrMagnitude > 0.01f;
    }

    /// <summary>
    /// 점프 입력이 눌렸는지 확인
    /// </summary>
    /// <returns>점프 키가 눌렸는지 여부</returns>
    public bool IsJumpInput()
    {
        return Managers.Input.GetInput(EPlayerInput.Jump).IsPressed();
    }

    /// <summary>
    /// 이동 입력을 받아 캐릭터 이동 처리
    /// </summary>
    public void HandleMovement()
    {
        Vector2 input = Managers.Input.GetInput(EPlayerInput.move).ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _controller.Move(move * statHandler.MoveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 중력 계산 및 적용
    /// </summary>
    public void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        _verticalVelocity += _gravity * Time.deltaTime;

        Vector3 gravityMove = new Vector3(0f, _verticalVelocity, 0f);
        _controller.Move(gravityMove * Time.deltaTime);
    }

    /// <summary>
    /// 점프 시작 시 수직 속도 계산
    /// </summary>
    public void StartJump()
    {
        _verticalVelocity = Mathf.Sqrt(statHandler.JumpPower * -2f * _gravity);
    }

    /// <summary>
    /// 캐릭터의 수직 속도 값 반환 (점프, 낙하 판정용)
    /// </summary>
    public float GetVerticalVelocity() => _verticalVelocity;

    /// <summary>
    /// 현재 캐릭터가 땅 위에 있는지 여부
    /// </summary>
    public bool IsGrounded() => _controller.isGrounded;

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
    public void HandleFire()
    {
        if (_equippedWeapon == null)
        {
            Debug.LogWarning("무기가 없습니다.");
            return;
        }
        _equippedWeapon.Attack();
    }
}