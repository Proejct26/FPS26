using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    public Transform player;
    public TextMeshProUGUI nameText;
    public float visibleDistance = 20f;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (player == null || mainCam == null) return;

        transform.position = player.position + Vector3.up * 2.2f; // 머리 위로 offset
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position); // billboard

        float dist = Vector3.Distance(mainCam.transform.position, player.position);
        gameObject.SetActive(dist <= visibleDistance);
    }

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }

}
