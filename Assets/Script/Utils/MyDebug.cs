using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : Singleton<MyDebug>
{
    private Queue<string> _logQueue = new Queue<string>();

    public bool isDebug = false;
    private void OnGUI()
    {
        // GUI 스타일 설정
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 20;
        style.alignment = TextAnchor.UpperLeft;

        // 시작 위치 설정
        float yPos = 10f;
        float xPos = 10f;

        // 큐의 모든 로그 메시지를 순회하며 출력s
        foreach (string log in _logQueue)
        {
            // 화면 크기를 벗어나지 않도록 체크
            if (yPos > Screen.height - 30)
                break;

            // 로그 메시지 출력
            GUI.Label(new Rect(xPos, yPos, Screen.width - 20, 30), log, style);
            yPos += 30f; // 다음 메시지를 위한 y 위치 조정 
        }
    }

    public static void Log(string message)
    {
        // if (!Instance.isDebug)
        //     return; 

        Instance._logQueue.Enqueue(message);
        if (Instance._logQueue.Count > 10)
        {
            Instance._logQueue.Dequeue();
        }
    }

}
