using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class NoticeMessage : MonoBehaviour {
    
    private Canvas self;
    private Text notice_text;
    private Coroutine coroutine;
    public void Initialize(){
        self = GameController.Instance.GetParentCanvas.Find("NoticeMessage").GetComponent<Canvas>();
        notice_text = self.transform.Find("Text").GetComponent<Text>();
    }
    /// <summary>
    /// 表示する。時間経過で勝手に消える
    /// </summary>
    /// <param name="set_text"></param>
    public void Open(string set_text,float display_time){
        if(coroutine != null) StopCoroutine(coroutine);
        if(notice_text.text != set_text){
            notice_text.text = set_text;
        }
        self.enabled = true;
        coroutine = StartCoroutine(DisableNoticeDisplay(display_time));
    }
    private IEnumerator DisableNoticeDisplay(float time){
        yield return new WaitForSeconds(time);
        self.enabled = false;
    }
}