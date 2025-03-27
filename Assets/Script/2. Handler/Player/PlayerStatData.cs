using UnityEngine;

[System.Serializable]
public class PlayerStateData
{
    public Vector3 position;
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool isJumping;
    public bool isFiring;
    public bool hitSuccess;
    public string hitTargetId;
}