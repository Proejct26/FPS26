using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EPlayerInput
{
	move,		//wasd
	interaction,
	Fire,	//발사
	Zoom,	//줌
	Jump,		//점프
	Info,		//정보창

 	MainWeapon,
	SubWeapon,
	KnifeWeapon,
	Reload,
	AimMode,
}

[System.Serializable]
public class InputManager : IManager
{
	[SerializeField] private InputActionAsset inputActionAsset; 
	private Dictionary<EPlayerInput, InputAction> playerInputs = new Dictionary<EPlayerInput, InputAction>();

	// Input Action Map   
	private InputActionMap playerInputMap;
	 
	public InputAction Fire => playerInputs[EPlayerInput.Fire];
	public InputAction Jump => playerInputs[EPlayerInput.Jump]; 
	public InputAction MainWeapon => playerInputs[EPlayerInput.MainWeapon];
	public InputAction SubWeapon => playerInputs[EPlayerInput.SubWeapon];
	public InputAction KnifeWeapon => playerInputs[EPlayerInput.KnifeWeapon];   

	// === Input Actions ===
	public InputAction GetInput(EPlayerInput type) => playerInputs[type];   


    public void Init()
    {
		if (inputActionAsset == null)
			return;

        BindAction(typeof(EPlayerInput));
		inputActionAsset.Enable(); 
    }


    public void Clear()
    {
        
    }
 
	public void SetActive(bool active)
	{ 
		if (active)
			inputActionAsset.Enable();
		
		else 
			inputActionAsset.Disable(); 
	}

	private void BindAction(Type type)
	{
		if (inputActionAsset == null)
			return; 

		string mapName = type.Name;
		if (mapName[0] == 'E')
			mapName = mapName.Substring(1);

		playerInputMap = inputActionAsset.FindActionMap(mapName);
		foreach (EPlayerInput t in Enum.GetValues(type)){ 
			string name = t.ToString(); 
			playerInputs[t] = playerInputMap.FindAction(name);
		}
	} 


}
