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
    public Transform GetParentCanvas{get;private set;}
    public PostProcessVolume GetMainPostVolume{get;private set;}
    public InputSystem GetInputSystem {get;private set;}
    public FPSController GetFpsController {get;private set;}
    public CursorManager GetCursorManager {get;private set;}
    public CanvasPauseManager GetCanvasPauseManager {get;private set;}

    public GameDisplayState DisplayState {get;set;}
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Awake(){
        base.Awake();
        GetMainPostVolume = GameObject.Find("Post-process Volume").GetComponent<PostProcessVolume>();
        GetFpsController = GameObject.Find("Player").GetComponent<FPSController>();
        GetParentCanvas = GameObject.Find("Canvas").transform;
        GetInputSystem = new InputSystem();
        GetCursorManager = new CursorManager();
        GetCanvasPauseManager = new CanvasPauseManager();
        AllComponentInitialize();
        SetInputs();
    }
    private void AllComponentInitialize(){
        GetFpsController.Initialize();
        GetCursorManager.CursorIsEnable(false);
        DisplayState = new GameDisplayState();
        DisplayState = GameDisplayState.Main;
    }
    /// <summary>
    /// 全てのゲーム描画をつかさどる
    /// </summary>
    private void Update(){
        //キー入力を受付
        GetInputSystem.InputReception();
        //自身を動かす
        GetFpsController.Move();
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
    }
}