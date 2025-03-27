using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder; // 카메라 부모 (X축 회전만 담당)
    [SerializeField] private float _sensitivity = 2f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ✅ 마우스 이동량 (InputSystem)
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
}
