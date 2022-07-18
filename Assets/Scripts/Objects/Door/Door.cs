using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// ドア
/// </summary>
public class Door : BaseEventObject{
#region プロパティ
    /// <summary>
    /// ドアの開閉速度
    /// </summary>
    [SerializeField]
    public float Speed = 1.0f;
    /// <summary>
    /// ドアID（勝手に割り振られる）
    /// </summary>
    /// <value></value>
    public int GetId{get;private set;}

    [SerializeField,Tooltip("鍵ID、鍵を書けないのであれば-1")]
    private int lock_id = -1;
    /// <summary>
    /// ロックID
    /// </summary>
    /// <value></value>
    public int GetLockId{get => lock_id;}
    [SerializeField,Tooltip("鍵がかかっているかどうか")]
    private bool is_lock = true;
    /// <summary>
    /// 鍵がかかっているかどうか
    /// </summary>
    /// <value></value>
    public bool GetIsLock{get => is_lock;}
    [SerializeField,Tooltip("可動域 通常ドアはY軸操作")]
    private Vector3 range;
    [SerializeField,Tooltip("どちら向きに開くか")]
    private bool is_front_open;
    /// <summary>
    /// どちら向きに開くか
    /// </summary>
    /// <value></value>
    public bool IsFrontOpen{get => is_front_open;}
    /// <summary>
    /// 扉は開いているか
    /// </summary>
    /// <value></value>
    [SerializeField,Tooltip("最初のドアの状態")]
    private DoorState door_state;
    public DoorState GetDoorState{get => door_state;}
    [SerializeField,Tooltip("ドアのタイプ")]
    private DoorType door_type;
    public DoorType GetDoorType{get => door_type;}
    private string navigation = "開ける";
    public override string GetNavigationMessage{get => key + navigation;}
    
#endregion
    public void Initialize(int set_id){
        GetId = set_id;
        switch(GetDoorState){
            case DoorState.Open:
                Open(0,null,true);
            break;
            case DoorState.Most:
                Debug.LogWarning($"ID{GetId}Mostに設定されている扉がある");
            break;
        }
    }
    /// <summary>
    /// イベントを起こす
    /// </summary>
    public override void EventAction(ItemInfo item){
        switch(GetDoorState){
            case DoorState.Close:
                Open(Speed,item,false);
            break;
            case DoorState.Open:
                Close(Speed,false);
            break;
            case DoorState.Most:
            break;
        }
    }
    /// <summary>
    /// ドアを開ける
    /// </summary>
    public void Open(float speed,ItemInfo item,bool is_force){
        if(!GetIsLock){
            if((GetDoorState == DoorState.Close && GetDoorType == DoorType.OpenAndClose ) || is_force){
                door_state = DoorState.Most;
                IsActive = false;
                transform
                .DOLocalRotate(is_front_open ? -range : range,speed,RotateMode.LocalAxisAdd)
                .OnComplete(()=> {
                    door_state = DoorState.Open;
                    navigation = "閉める";
                    if(GetDoorType == DoorType.OpenOnly){
                        IsActive = false;
                    }else{
                        IsActive = true;
                    }
                });
            }
        }else if(item != null && item.GetKeyId == GetLockId){
            GameController.Instance.GetNoticeMessage.Open("鍵をあけた",2);
            is_lock = false;
            Open(speed,null,false);
            GameController.Instance.GetItemManager.UseItem(true);
        }else{
            GameController.Instance.GetNoticeMessage.Open("鍵がかかっている",2);
        }
    }
    /// <summary>
    /// ドアを閉める
    /// </summary>
    public void Close(float speed,bool is_force){
        if(GetDoorState == DoorState.Open || is_force){
            door_state = DoorState.Most;
            IsActive = false;
            transform
            .DOLocalRotate(is_front_open ? range : -range,speed,RotateMode.LocalAxisAdd)
            .OnComplete(()=> {
                door_state = DoorState.Close;
                navigation = "開ける";
                if(GetDoorType == DoorType.OpenAndClose){
                    IsActive = true;
                }
            });
        }
    }
    /// <summary>
    /// 解錠
    /// </summary>
    public void Unlock(){
        is_lock = false;
        GameController.Instance.GetNoticeMessage.Open("鍵を開けた",2);
    }
    /// <summary>
    /// 施錠
    /// </summary>
    public void Lock(){
        is_lock = true;
    }

    public enum DoorState{
        Open,//開いているドア
        Close,//閉じているドア
        Most,//開いている最中or閉じている最中
    }
    public enum DoorType{
        OpenAndClose,//開け閉め可能
        OpenOnly,//開けられるだけ
    }
}
