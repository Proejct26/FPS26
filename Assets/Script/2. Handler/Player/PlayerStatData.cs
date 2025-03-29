using UnityEngine;

[System.Serializable]
public class PlayerStateData
{
    public string id;
    public int spawnIndex;
    public int KDA;

    public Vector3 position;    //플레이어 현 위치
    public Vector3 lookInput;   //어디를 보고 있는지 벡터     
    public Vector3 moveInput;   //이동 중인 벡터

    public float rotationX;
    public float rotationY;     //마우스 회전 값

    public bool isJumping;      //점프 중인지
    public bool isFiring;       //공격 중인지
    public bool hitSuccess;     //공격에 성공 했는지
    public string hitTargetId;  //공격한 타겟의 ID는 무엇인지
}