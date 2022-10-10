using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public enum GameDisplayState{
    //通常のゲーム進行画面
    Main = 0,
    //アイテム選択画面
    Menu = 1,
    //Escape押したときとかの画面
    Pause = 2
}

public class GameController : SingletonMonoBehaviour<GameController> {
    public GameDisplayState DisplayState {get; set;}
    
    public Transform GetParentCanvas{get;private set;}
    public PostProcessVolume GetMainPostVolume{get;private set;}
    public InputSystem GetInputSystem {get;private set;}
    public FPSController GetFpsController {get;private set;}
    public CursorManager GetCursorManager {get;private set;}
    public CanvasPauseManager GetCanvasPauseManager {get;private set;}
    public DoorManager GetDoorManager{get;private set;}
    public CanvasNavigation GetCanvasNavigation{get;private set;}
    public InputRaycast GetInputRaycast {get;private set;}
    public NoticeMessage GetNoticeMessage{get;private set;}
    public ItemManager GetItemManager{get;private set;}
    public CanvasItemHelper GetCanvasItemHelper{get;private set;}
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Awake(){
        base.Awake();
        Transform system = GameObject.Find("System").transform;
        GetParentCanvas = GameObject.Find("Canvas").transform;
        GetMainPostVolume = GameObject.Find("Post-process Volume").GetComponent<PostProcessVolume>();
        GetFpsController = GameObject.Find("Player").GetComponent<FPSController>();
        
        GetCanvasItemHelper = system.GetComponent<CanvasItemHelper>();
        GetInputSystem = new InputSystem();
        GetCursorManager = new CursorManager();
        GetCanvasPauseManager = new CanvasPauseManager();
        GetDoorManager = new DoorManager();
        GetCanvasNavigation = new CanvasNavigation();
        GetItemManager = new ItemManager();

        GetInputRaycast = system.GetComponent<InputRaycast>();
        GetNoticeMessage = system.GetComponent<NoticeMessage>();
    
        AllComponentInitialize();
        SetInputs();
    }
    private void AllComponentInitialize(){
        GetFpsController.Initialize();
        GetDoorManager.Initialize();
        GetInputRaycast.Initialize();
        GetNoticeMessage.Initialize();
        GetCanvasItemHelper.Initialize();
        GetCursorManager.CursorIsEnable(false);
        DisplayState = new GameDisplayState();
        DisplayState = GameDisplayState.Main;
    }
    /// <summary>
    /// 全てのゲーム描画をつかさどる
    /// </summary>
    private void Update(){
        GetInputRaycast.RayForEventObject();
        //キー入力を受付
        GetInputSystem.InputReception();
        //自身を動かす
        GetFpsController.Move();
        RealTimeInputs();
    }
    /// <summary>
    /// 全ての物理演算をつかさどる
    /// </summary>
    private void FixedUpdate() {
        GetFpsController.PhysicsFix();
    }
    /// <summary>
    /// InputSystemの詳細を決める
    /// </summary>
    private void SetInputs(){
        //Escape押したときの処理
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.Escape,InputType.InputDown),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetCanvasPauseManager.OpenDisplay(null);
            else if(DisplayState == GameDisplayState.Pause) GetCanvasPauseManager.CloseDisplay();
        });
        //E
        //アイテムを拾う、扉を開ける、閉める（ステージ上のギミックを行う時のコマンド）
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.E,InputType.InputDown),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetInputRaycast.EventEffect();
        });
        //LeftShift
        //押している間走る、離すと歩く。
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.LeftShift,InputType.InputDown),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetFpsController.IsRun = true;
        });
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.LeftShift,InputType.InputUp),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetFpsController.IsRun = false;
        });
        //Q
        //アイテムを使う
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.Q,InputType.InputDown),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetItemManager.UseItem();
        });
        //仮V
        //アイテムを調べる
        GetInputSystem.OverrideKey(new KeyInputType(KeyCode.V,InputType.InputDown),
        ()=>{
            if(DisplayState == GameDisplayState.Main) GetItemManager.CheckItem();
        });
    }
    private void RealTimeInputs(){
        //ホイール
        GetInputSystem.Axis("Mouse ScrollWheel",0.1f,true,()=>{
            GetItemManager.ChangeItem(1);
        });
        GetInputSystem.Axis("Mouse ScrollWheel",-0.1f,false,()=>{
            GetItemManager.ChangeItem(-1);
        });
    }

}