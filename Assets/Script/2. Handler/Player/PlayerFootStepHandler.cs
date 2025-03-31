using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootStepHandler : MonoBehaviour
{
    [Header("발소리 클립")]
    public AudioClip[] footstepClips;

    [Header("설정")]
    public float baseInterval = 0.2f; // 걷기 간격
    public float runMultiplier = 0.3f; // 달릴 때 간격 줄이기(나중에 달리기 대비 코드)

    private AudioSource audioSource;
    private float timer;

    [Header("외부 참조")]
    public PlayerControllerBase controller;
    public bool isRunning = false;


    private bool wasMovingLastFrame = false;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D 사운드 설정
    }

    private void Update()
    {
        if (controller == null)
        {
            timer = 0f;
            wasMovingLastFrame = false;
            return;
        }

        bool isGrounded = controller.IsGrounded();
        bool isMoving = controller.HasMoveInput();

        if (!isGrounded || !isMoving)
        {
            timer = 0f;
            wasMovingLastFrame = false;
            return;
        }

        float interval = isRunning ? baseInterval * runMultiplier : baseInterval;
        timer += Time.deltaTime;

        // 처음 움직이기 시작했을 때 즉시 발소리
        if (!wasMovingLastFrame)
        {
            PlayFootstep();
            timer = 0f;
        }
        else if (timer >= interval)
        {
            PlayFootstep();
            timer = 0f;
        }

        wasMovingLastFrame = true;
    }

    private void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
            return;


        // 현재 재생 중이면 무시
        if (audioSource.isPlaying)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}
