using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using System.Linq;

public struct KDAData
{
    public int playerId;
    public int team;
    public int kill;
    public int death;
    public int assist;
}

public class GameKdaData
{
    public List<KDAData> KDAInfoList = new List<KDAData>();
    
    public event Action OnChangedKDAInfo;

    public void AddKDAInfo(int playerId, int team, int kill, int death, int assist)
    {

        for (int i = 0; i < KDAInfoList.Count; i++)
        {
            if (KDAInfoList[i].playerId == playerId)
            {
                KDAData temp = KDAInfoList[i];
                temp.kill = kill;
                temp.death = death;
                temp.assist = assist;

                KDAInfoList[i] = temp;
                return;
            }
        }


        KDAInfoList.Add(new KDAData()
        {
            kill = kill,
            death = death,
            assist = assist,
            playerId = playerId, 
            team = team
        });

        OnChangedKDAInfo?.Invoke();
    }

    public List<KDAData> GetTeamData(int team)
    {
        return KDAInfoList.FindAll(data => data.team == team);
    }
}

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
    public GameKdaData GameKdaData {get; private set;} = new GameKdaData(); 

    public SpawnData SpawnData => _spawnData;
    public int PlayerId {get; private set;} = -1;
    public string Nickname {get; private set;} = "";

        private int _redTeamScore = 0;
    private int _blueTeamScore = 0;
    public event Action OnChangedTeamScore;


     public int RedTeamScore 
    {
        get
        {
            return _redTeamScore;
        }
        set
        {
            _redTeamScore = value;
            OnChangedTeamScore?.Invoke();
        }
    } 

    public int BlueTeamScore 
    {
        get
        {
            return _blueTeamScore; 
        } 
        set
        {
            _blueTeamScore = value;
            OnChangedTeamScore?.Invoke();
        }
    } 



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
  
    public GameObject SpawnLocalPlayer(int posIndex, int teamIdx, int playerId)
    {
        // 이미 생성되어있다면 리스폰폰
        if (PlayerId != -1) 
        {
            Managers.Player.GetComponent<LocalPlayerController>().OnRespawn();
            Transform tf = _spawnData.GetSpawnPosition(teamIdx, posIndex);
            Managers.Player.transform.position = tf.position; 
            Managers.Player.transform.rotation = tf.rotation; 

            Managers.UI.CloseAllPopupUI();
           return Managers.Player; 
        }


        PlayerId = playerId; 
        GameObject prefab = _spawnData._localPlayerPrefab;
        Transform spawnTf = _spawnData.GetSpawnPosition(teamIdx, posIndex);

        GameObject player = Managers.Pool.Get(prefab);
        player.transform.position = spawnTf.position; 
        player.transform.rotation = spawnTf.rotation;

        Managers.UI.ShowSceneUI<MainUI>("MainUI");  

        return player;
    }

}
