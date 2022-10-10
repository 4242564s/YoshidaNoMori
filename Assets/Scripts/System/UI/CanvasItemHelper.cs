using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CanvasItemHelper : MonoBehaviour{
    private Canvas self;
    private Text item_help;
    private Coroutine coroutine;

    public void Initialize(){
        self = GameController.Instance.GetParentCanvas.Find("ItemHelper").GetComponent<Canvas>();
        item_help = self.transform.Find("Window/Text").GetComponent<Text>();
    }
    /// <summary>
    /// 表示する。時間経過で勝手に消える
    /// </summary>
    /// <param name="set_text"></param>
    public void Open(string set_text,float display_time){
        if(coroutine != null) StopCoroutine(coroutine);
        if(item_help.text != set_text){
            item_help.text = set_text;
        }
        self.enabled = true;
        coroutine = StartCoroutine(DisableNoticeDisplay(display_time));
    }
    private IEnumerator DisableNoticeDisplay(float time){
        yield return new WaitForSeconds(time);
        self.enabled = false;
    }
}