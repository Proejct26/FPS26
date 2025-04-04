
using Server;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public interface IManager
{
    void Init();
    void Clear();
}

public class Managers : Singleton<Managers>
{ 
    [field: SerializeField] private DataManager data  = new DataManager();
    [field: SerializeField] private InputManager input = new InputManager();
    [field: SerializeField] private ResourceManager resource = new ResourceManager();
    [field: SerializeField] private EventManager @event = new EventManager();
    [field: SerializeField] private SoundManager sound = new SoundManager();
    [field: SerializeField] private UIManager ui = new UIManager();
    [field: SerializeField] private PoolManager pool = new PoolManager();
    [field: SerializeField] private NetworkManager network = new NetworkManager();
    [field: SerializeField] private PacketManager packet = new PacketManager();

    public static DataManager Data => Instance.data;
    public static InputManager Input => Instance.input;
    public static ResourceManager Resource => Instance.resource;
    public static EventManager Event => Instance.@event; 
    public static SoundManager Sound => Instance.sound; 
    public static UIManager UI => Instance.ui;
    public static PoolManager Pool => Instance.pool;
    public static NetworkManager Network => Instance.network;
    public static PacketManager Packet => Instance.packet;

    private static GameObject player;
    public static GameObject Player 
    {
        get {return player == null ? player = FindAnyObjectByType<LocalPlayerController>().gameObject : player;}  
        private set{player = value;}
    }

    public static GameSceneManager GameSceneManager {get; private set;}
    public static ChatDataManager Chat { get; private set; } = new ChatDataManager();
 
 
    protected override void Awake()
    {
        base.Awake();
        
        Application.targetFrameRate = 60;
        JobQueue.Push(()=>{});  
  
        Init();
    }

    private void Update()
    {
        Network.Update();
    }

    public static void RegisterGameSceneManager(GameSceneManager gameSceneManager)
    {
        gameSceneManager.EnterScene(); 

        GameSceneManager = gameSceneManager;  
    }

    private static void Init()
    {
        Input.Init();
        Resource.Init();
        Sound.Init();
        UI.Init();
        Pool.Init();
        Network.Init();
        Packet.Init();
        Chat.Init();
    }

    public static void Clear()
    {

    }

}
