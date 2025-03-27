using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EPlayerInput
{
	move,
	interaction,
	LeftMouse
}

[System.Serializable]
public class InputManager : IManager
{
	[SerializeField] private InputActionAsset inputActionAsset; 
	private Dictionary<EPlayerInput, InputAction> playerInputs = new Dictionary<EPlayerInput, InputAction>();

	// Input Action Map  
	private InputActionMap playerInputMap;
	 

	// === Input Actions ===
	public InputAction GetInput(EPlayerInput type) => playerInputs[type];  

    public void Init()
    {
        BindAction(typeof(EPlayerInput));
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
		foreach (EPlayerInput t in Enum.GetValues(type))
			playerInputs[t] = playerInputMap.FindAction(type.ToString());
	} 


}
