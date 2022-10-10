using UnityEngine;
using UnityEngine.Events;
public class ItemObject : BaseEventObject {
    private ItemManager item_manager;
    [SerializeField]
    private ItemScriptableObject master;
    [SerializeField,Tooltip("持っているときにイベントオブジェクトに対して効果がある")]
    public int GetKeyId { get => master.KeyId;}
    private string GetItemName{get => master.ItemName;}
    public override string GetNavigationMessage{get => key+"拾う";}
    private void Start(){
        item_manager = GameController.Instance.GetItemManager;
    }
    public override void EventAction(ItemInfo item){
        //拾う
        GameController.Instance.GetNoticeMessage.Open($"{GetItemName} を手に入れた",2f);
        ItemInfo pick_item = new ItemInfo(
            GetItemName,master.ItemId,
            GetKeyId,master.ItemHelp,master.ExhaustedCount,
            master.IsAnyTime,master.SourcePath,master.HadScale
        );
        GameObject obj = Instantiate(gameObject);
        pick_item.InstanceObj = obj;
        if(obj.TryGetComponent<BoxCollider>(out var x)){
            Destroy(x);
        }
        pick_item.UseCallBack.AddListener(()=>{
            pick_item.ExhaustedCount--;
            if(pick_item.ExhaustedCount <= 0){
                item_manager.GetHaveItemInfo.Remove(pick_item);
                Destroy(pick_item.InstanceObj);
            }
        });
        
        Destroy(obj.GetComponent<ItemObject>());
        item_manager.AddItem(pick_item);
        IsActive = false;
        Destroy(gameObject);
    }
}