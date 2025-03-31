using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    public Transform player;    // 따라갈 플레이어
    public Vector3 offset = new Vector3(0, 2.2f, 0); // 머리 위 위치
    public TextMeshProUGUI nameText;
    public float visibleDistance = 10000f;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (Camera.main == null || player == null)
            return;

        // 위치 조정 (머리 위로)
        transform.position = player.position + offset;


        float distance = Vector3.Distance(mainCam.transform.position, player.position);
        nameText.gameObject.SetActive(distance <= visibleDistance);

        if (nameText.gameObject.activeSelf)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        }
    }

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }

}
