using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


[Serializable]
public class SpawnData
{
    [SerializeField] private Transform _redTeamSpawnPointParent;
    [SerializeField] private Transform _blueTeamSpawnPointParent; 

    public List<Transform> RedTeamSpawnPoints {get; private set;} = new List<Transform>();
    public List<Transform> BlueTeamSpawnPoints {get; private set;} = new List<Transform>();

    private void Init()
    {
        foreach (Transform child in _redTeamSpawnPointParent)
            RedTeamSpawnPoints.Add(child);
        

        foreach (Transform child in _blueTeamSpawnPointParent)
            BlueTeamSpawnPoints.Add(child);
    }

    public Transform GetSpawnPosition(int team, int index = 0)
    {
        return team == 0 ? RedTeamSpawnPoints[index] : BlueTeamSpawnPoints[index];
    } 
}

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private SpawnData _spawnData = new SpawnData();

    public PlayerManager PlayerManager { get; private set; } = new PlayerManager();


    public virtual void EnterScene()
    {  
        
    }

    public virtual void ExitScene()
    {

    } 

    private void Start()
    {
        Managers.RegisterGameSceneManager(this); 
    }

    public Transform GetSpawnPosition(int team, int index = 0)
    {
        return _spawnData.GetSpawnPosition(team, index); 
    } 
 
}
