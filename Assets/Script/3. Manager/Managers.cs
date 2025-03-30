
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
    [field: SerializeField] private DataManager data;
    [field: SerializeField] private InputManager input = new InputManager();
    [field: SerializeField] private ResourceManager resource = new ResourceManager();
    [field: SerializeField] private EventManager @event = new EventManager();
    [field: SerializeField] private SoundManager sound = new SoundManager();
    [field: SerializeField] private UIManager ui = new UIManager();
    [field: SerializeField] private PoolManager pool = new PoolManager();
      
    public static DataManager Data => Instance.data;
    public static InputManager Input => Instance.input;
    public static ResourceManager Resource => Instance.resource;
    public static EventManager Event => Instance.@event; 
    public static SoundManager Sound => Instance.sound; 
    public static UIManager UI => Instance.ui;
    public static PoolManager Pool => Instance.pool; 

    public static GameObject Player {get; private set;}

 
    protected override void Awake()
    {
        base.Awake();
  
        var pcb = FindFirstObjectByType<PlayerControllerBase>();
        if (pcb != null)
            Player = pcb.gameObject; 

        data = new DataManager();
        Init();
    }
    private void Start() 
    {
        //UI.ShowPopupUI<SkillPopupUI>(); 
       
	} 

    private void Update()
    {
        //_input.OnUpdate();
    }

    private static void Init()
    {

        Input.Init();
        Resource.Init();
        Sound.Init();
        UI.Init();
        Pool.Init();

        
	}

    public static void Clear()
    {

    }
    
    public static void SetTimer(Action action, float time)
    {
        Instance.StartCoroutine(Instance.SetTimerCoroutine(action, time));
    }
    
    private IEnumerator SetTimerCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
    
}
