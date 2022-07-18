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
            GetKeyId,master.IsExhausted,
            master.IsAnyTime,master.HadScale
        );
        GameObject obj = Instantiate(gameObject);
        pick_item.InstanceObj = obj;
        if(obj.TryGetComponent<BoxCollider>(out var x)){
            Destroy(x);
        }
        if(pick_item.GetIsExhausted){
            pick_item.UseCallBack.AddListener(()=>{
                Destroy(pick_item.InstanceObj);
            });
        }
        Destroy(obj.GetComponent<ItemObject>());
        item_manager.PickItem = pick_item;
        IsActive = false;
        Destroy(gameObject);
    }
}