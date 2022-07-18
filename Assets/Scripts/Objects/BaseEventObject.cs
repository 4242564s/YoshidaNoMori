using UnityEngine;

public abstract class BaseEventObject : MonoBehaviour {
    protected string key = "E  ";
    /// <summary>
    /// 実行したときのアクション
    /// </summary>
    public abstract void EventAction(ItemInfo item);
    /// <summary>
    /// 視点を合わせたときに表示されるメッセージ
    /// </summary>
    /// <value></value>
    public abstract string GetNavigationMessage{get;}
    /// <summary>
    /// イベントレイを受け付けるかどうか。
    /// </summary>
    /// <value></value>
    public bool IsActive{get;set;} = true;
}