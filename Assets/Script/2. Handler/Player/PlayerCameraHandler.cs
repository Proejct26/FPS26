using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder; // 카메라 부모 (X축 회전만 담당)
    [SerializeField] private Transform headBone;     // 애니메이션 본(head.x)
    [SerializeField] private float _sensitivity = 2f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 0f, 0f); // 본 위치에서 얼마나 앞뒤로 뺄지


    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        // 마우스 이동량 (InputSystem)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * _sensitivity * Time.deltaTime;

        // 좌우 회전 (플레이어)
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 (카메라 홀더)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void LateUpdate()
    {
        if (headBone == null) return;

        // head.x 위치에 따라 카메라 위치를 덮어씀
        cameraHolder.position = headBone.position + headBone.TransformVector(cameraOffset);

        // 2. 얼굴 방향에서 Yaw만 추출
        Vector3 flatForward = headBone.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        // 3. Yaw 회전 추출
        Quaternion yawRotation = Quaternion.LookRotation(flatForward, Vector3.up);

        // 4. 최종 회전 = 상하 Pitch(xRotation) + Yaw 조합
        cameraHolder.rotation = yawRotation * Quaternion.Euler(xRotation, 0f, 0f);
    }
}
