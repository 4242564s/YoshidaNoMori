using UnityEngine;

public class InputRaycast : MonoBehaviour {
    /// <summary>
    /// レイを飛ばす対象
    /// </summary>
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float ray_length = 3;
    private Vector3 ray_direction = new Vector3(0.5f,0.5f,0.5f);
    private BaseEventObject event_object;
    protected CanvasNavigation canvas_navigation;
    private string nv;
    private ItemManager item_manager;
    public void Initialize(){
        canvas_navigation = GameController.Instance.GetCanvasNavigation;
        item_manager = GameController.Instance.GetItemManager;
    }
    public void RayForEventObject(){
        if(GameController.Instance.DisplayState == GameDisplayState.Pause) return;
        Ray ray = cam.ViewportPointToRay(ray_direction);
        RaycastHit hit = new RaycastHit();
        // If Ray hit something
        if (Physics.Raycast(ray, out hit, ray_length)) {
            if(hit.collider.gameObject.tag == "EventObject"){
                BaseEventObject ray_obj = hit.collider.gameObject.GetComponent<BaseEventObject>();
                if(event_object == null || 
                (event_object != ray_obj || nv != ray_obj.GetNavigationMessage) &&
                event_object.IsActive){
                    event_object = ray_obj;
                    nv = event_object.GetNavigationMessage;
                    canvas_navigation.Open(nv);
                }
                if(!event_object.IsActive){
                    canvas_navigation.Close();
                }
            }else{
                event_object = null;
                canvas_navigation.Close();
            }
        }else{
            event_object = null;
            canvas_navigation.Close();
        }
    }
    public void EventEffect(){
        event_object?.EventAction(item_manager.PickItem);
    }
}