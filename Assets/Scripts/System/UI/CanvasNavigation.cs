using UnityEngine;
using UnityEngine.UI;
public class CanvasNavigation{
    private Canvas self;
    private Text navigation_text;
    public CanvasNavigation(){
        self = GameController.Instance.GetParentCanvas.Find("Navigation").GetComponent<Canvas>();
        navigation_text = self.transform.Find("Text").GetComponent<Text>();
    }
    /// <summary>
    /// 表示
    /// </summary>
    /// <param name="set_text"></param>
    public void Open(string set_text){
        if(navigation_text.text != set_text){
            navigation_text.text = set_text;
        }
        self.enabled = true;
    }
    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close(){
        navigation_text.text = "";
        self.enabled = false;
    }
}