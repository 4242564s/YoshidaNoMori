using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
/// <summary>
/// アイテム管理。何を持っているかとか
/// </summary>
public class ItemManager{
    private ItemInfo pick_item;
    private CanvasItemHelper item_helper;
    /// <summary>
    /// アイテムを持つ手の位置
    /// </summary>
    private Transform hand;
    private List<ItemInfo> have_item_infos = new List<ItemInfo>();
    public List<ItemInfo> GetHaveItemInfo{get => have_item_infos;}
    //UI部分と結合したくないが、面倒なのでUI部分をここで作る
    private Image item_image;
    /// <summary>
    /// アイテムを持ち帰ることができるかどうか
    /// </summary>
    /// <value></value>
    public bool IsChangeItem{get; set;} = true;
    /// <summary>
    /// 持っているアイテム
    /// </summary>
    public ItemInfo PickItem{
        get{return pick_item;}
        set{
            if(value != null){
                value.InstanceObj.transform.parent = hand.transform;
                value.InstanceObj.transform.localPosition = Vector3.zero;
                value.InstanceObj.transform.localScale = value.GetHadScale;
                value.InstanceObj.layer = 8;
                item_image.sprite = value.GetItemImage;

            }else{
                item_image.sprite = null;
            }

            item_image.enabled = value != null;
            pick_item = value;
        }
    }
    public void CheckItem(){
        if(pick_item != null){
            item_helper.Open(pick_item.GetItemHelp,3f);
        }
    }
    /// <summary>
    /// アイテムを持ち替える
    /// </summary>
    /// <param name="change_index"></param>]
    public void ChangeItem(int change_index){
        if(pick_item == null && have_item_infos.Count == 1){
            PickItem = have_item_infos[0];
            PickItem.InstanceObj.SetActive(true);
        }else if(pick_item != null && have_item_infos.Count >= 2){
            pick_item.InstanceObj.SetActive(false);
            if(pick_item.Index == 0 && change_index == -1){
                PickItem = have_item_infos[have_item_infos.Count - 1];
            }else if(pick_item.Index == have_item_infos.Count - 1 && change_index == 1){
                PickItem = have_item_infos[0];
            }else{
                PickItem = have_item_infos[PickItem.Index + change_index];
            }
            PickItem.InstanceObj.SetActive(true);
        }
    }
    /// <summary>
    /// アイテムを加える
    /// </summary>
    /// <param name="add_item"></param>
    public void AddItem(ItemInfo add_item){
        add_item.Index = have_item_infos.Count;
        if(PickItem == null) PickItem = add_item;
        else add_item.InstanceObj.SetActive(false);
        have_item_infos.Add(add_item);
    }
    /// <summary>
    /// アイテムを使う
    /// </summary>
    /// <param name="is_force">どんな状況でも強制的に使ったことにする</param>
    public void UseItem(bool is_force = false){
        if((pick_item != null && pick_item.GetIsAnyTimeUse) || is_force){
            pick_item.UseCallBack?.Invoke();
            if(pick_item.ExhaustedCount <= 0){
                PickItem = null;
                ChangeItem(1);
            }
        }
    }
    public ItemManager(){
        item_helper = GameController.Instance.GetCanvasItemHelper;
        hand = GameObject.Find("Player/Main Camera/HadItem").transform;
        item_image = GameController.Instance.GetParentCanvas.Find("ViewUI/ItemWindow/ItemImage").GetComponent<Image>();
    }
}

public class ItemInfo{
    public string GetItemName{get;}
    public int GetItemId{get;}
    public int GetKeyId{get;}
    public int Index{get;set;}
    public int ExhaustedCount{get;set;}
    public bool GetIsAnyTimeUse{get;}
    public string GetItemHelp{get;}
    public Vector3 GetHadScale{get;set;}
    public GameObject InstanceObj{get;set;}
    public UnityEvent UseCallBack{get;set;}
    public Sprite GetItemImage{get;}
    public ItemInfo(string item_name,int item_id,int key_id,string item_help
    ,int exhausted_count,bool is_any_time,string item_image,Vector3 had_scale){
        GetItemName = item_name;
        GetItemId = item_id;
        GetKeyId = key_id;
        GetHadScale = had_scale;
        ExhaustedCount = exhausted_count;
        GetIsAnyTimeUse = is_any_time;
        GetItemHelp = item_help;
        GetItemImage = Resources.Load<Sprite>("Sprites/" + item_image);
        UseCallBack = new UnityEvent();
    }
}