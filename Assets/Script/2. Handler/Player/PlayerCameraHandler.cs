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
    private float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //마우스 이동량 (InputSystem)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * _sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * _sensitivity * Time.deltaTime;

        // 좌우 회전 (플레이어)
        transform.Rotate(Vector3.up * mouseX);

        // 상하 회전 (카메라 홀더)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
         
        // 현재 카메라 홀더의 회전값을 기준으로 상대적 회전 적용
        Vector3 currentRotation = cameraHolder.localRotation.eulerAngles;
        Vector3 targetRotation = new Vector3(xRotation, currentRotation.y, currentRotation.z);
       cameraHolder.localRotation = Quaternion.Euler(targetRotation);
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

        // 4. 현재 카메라의 로컬 회전값 유지하면서 Yaw만 적용
        Vector3 currentLocalRotation = cameraHolder.localRotation.eulerAngles;
        Quaternion pitchRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // 5. Yaw는 world rotation으로, Pitch는 local rotation으로 적용
        cameraHolder.rotation = yawRotation;
        cameraHolder.localRotation = pitchRotation * cameraHolder.localRotation;
    }
}
