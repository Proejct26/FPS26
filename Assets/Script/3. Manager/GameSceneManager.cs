using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


[Serializable]
public class SpawnData
{
    [Header("Player Prefab")]
    [SerializeField] public GameObject _redTeamPlayerPrefab;
    [SerializeField] public GameObject _blueTeamPlayerPrefab;  

    [Header("Spawn Point")]
    [SerializeField] private Transform _redTeamSpawnPointParent;
    [SerializeField] private Transform _blueTeamSpawnPointParent; 


    public List<Transform> RedTeamSpawnPoints {get; private set;} = new List<Transform>();
    public List<Transform> BlueTeamSpawnPoints {get; private set;} = new List<Transform>();

    public void Init()
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

    public GameObject GetPlayerPrefab(int team)
    {
        return team == 0 ? _redTeamPlayerPrefab : _blueTeamPlayerPrefab;
    } 
}

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private SpawnData _spawnData = new SpawnData();

    public virtual void EnterScene()
    {  
    }

    public virtual void ExitScene()
    {

    }
    void Awake()
    {
        _spawnData.Init();
        
    }
     private void Start()
    {
        Managers.RegisterGameSceneManager(this);  
 
    }

    public void SpawnRemotePlayer(int team, int index = 0)
    {
        GameObject prefab = _spawnData.GetPlayerPrefab(team);
        Transform spawnTf = _spawnData.GetSpawnPosition(team, index); 

        GameObject player = Managers.Pool.Get(prefab);
        player.transform.position = spawnTf.position;
        player.transform.rotation = spawnTf.rotation;

        //player.GetComponent<PlayerControllerBase>().Init(team);  
    } 


    
 
}
