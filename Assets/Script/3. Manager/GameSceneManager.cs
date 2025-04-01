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
    [SerializeField] public GameObject _localPlayerPrefab;

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
    public PlayerManager PlayerManager {get; private set;} = new PlayerManager();
    public SpawnData SpawnData => _spawnData;
    public virtual void EnterScene()
    {  
        Managers.UI.ShowPopupUI<UI_StartPopup>("StartPopup");  
    }


    void Awake()
    {
        _spawnData.Init();
        Managers.RegisterGameSceneManager(this);  
    }

     private void Start()
    {
        PlayerManager.redTeamPrefab = _spawnData._redTeamPlayerPrefab;
        PlayerManager.blueTeamPrefab = _spawnData._blueTeamPlayerPrefab;
    }


    public GameObject SpawnRemotePlayer(int team, int index = 0)
    {
        GameObject prefab = _spawnData.GetPlayerPrefab(team);
        Transform spawnTf = _spawnData.GetSpawnPosition(team, index); 

        GameObject player = Managers.Pool.Get(prefab);
        player.transform.position = spawnTf.position;
        player.transform.rotation = spawnTf.rotation;

        return player;
    }  
 
    public GameObject SpawnLocalPlayer(int posIndex, int teamIdx)
    {
        GameObject prefab = _spawnData._localPlayerPrefab;
        Transform spawnTf = _spawnData.GetSpawnPosition(teamIdx, posIndex);

        GameObject player = Managers.Pool.Get(prefab);
        player.transform.position = spawnTf.position; 
        player.transform.rotation = spawnTf.rotation;

        return player;
    }
}
