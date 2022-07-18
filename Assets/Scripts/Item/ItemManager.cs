using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// アイテム管理。何を持っているかとか
/// </summary>
public class ItemManager{
    private ItemInfo pick_item;
    /// <summary>
    /// アイテムを持つ手の位置
    /// </summary>
    private Transform hand;
    /// <summary>
    /// 持っているアイテム
    /// </summary>
    public ItemInfo PickItem{
        get{return pick_item;}
        set{
            value.InstanceObj.transform.parent = hand.transform;
            value.InstanceObj.transform.localPosition = Vector3.zero;
            value.InstanceObj.transform.localScale = value.GetHadScale;

            value.InstanceObj.layer = 8;
            pick_item = value;
        }
    }
    /// <summary>
    /// アイテムを使う
    /// </summary>
    /// <param name="is_force">どんな状況でも強制的に使ったことにする</param>
    public void UseItem(bool is_force = false){
        if((pick_item != null && pick_item.GetIsAnyTimeUse) || is_force){
            pick_item.UseCallBack?.Invoke();
        }
    }
    public ItemManager(){
        hand = GameObject.Find("Player/Main Camera/HadItem").transform;
    }
}

public class ItemInfo{
    public string GetItemName{get;}
    public int GetItemId{get;}
    public int GetKeyId{get;}
    public bool GetIsExhausted{get;}
    public bool GetIsAnyTimeUse{get;}
    public Vector3 GetHadScale{get;set;}
    public GameObject InstanceObj{get;set;}
    public UnityEvent UseCallBack{get;set;}
    public ItemInfo(string item_name,int item_id,int key_id
    ,bool is_exhausted,bool is_any_time,Vector3 had_scale){
        GetItemName = item_name;
        GetItemId = item_id;
        GetKeyId = key_id;
        GetHadScale = had_scale;
        GetIsExhausted = is_exhausted;
        GetIsAnyTimeUse = is_any_time;
        UseCallBack = new UnityEvent();
    }
}