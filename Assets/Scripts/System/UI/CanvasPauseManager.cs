using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class CanvasPauseManager : BaseOpenCanvas{
    private Button back_game_button;
    private CursorManager cursor_manager;
    /// <summary>
    /// 画面を開いたとき
    /// </summary>
    public override void OpenDisplay(UnityAction close){
        base.OpenDisplay(close);
        cursor_manager.CursorIsEnable(true);
        GameController.Instance.DisplayState = GameDisplayState.Pause;
    }
    /// <summary>
    /// 画面を閉じたとき
    /// </summary>
    public override void CloseDisplay(){
        base.CloseDisplay();
        cursor_manager.CursorIsEnable(false);
        GameController.Instance.DisplayState = GameDisplayState.Main;
    }
    public CanvasPauseManager():base(){
        open_canvas = GameController.Instance.GetParentCanvas.transform.Find("Pause").GetComponent<Canvas>();
        cursor_manager = GameController.Instance.GetCursorManager;
        back_game_button = open_canvas.transform.Find("Button").GetComponent<Button>();
        back_game_button.onClick.AddListener(CloseDisplay);
    }

}