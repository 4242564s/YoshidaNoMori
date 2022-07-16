using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Events;
public class BaseOpenCanvas {
    protected Canvas open_canvas{get;set;}
    private PostProcessVolume volume;
    private DepthOfField depth;
    /// <summary>
    /// 画面を閉じたときのコールバック
    /// </summary>
    protected UnityAction close_call_back;
    public BaseOpenCanvas(){
        volume = GameController.Instance.GetMainPostVolume;
        volume.profile.TryGetSettings<DepthOfField>(out depth);
    }
    public virtual void OpenDisplay(UnityAction call_back){
        DepthMax();
        close_call_back = call_back;
        open_canvas.enabled = true;
    }
    public virtual void CloseDisplay(){
        DepthNormal();
        open_canvas.enabled = false;
        close_call_back?.Invoke();
        close_call_back = null;
    }
    protected void DepthMax(){
        depth.focalLength.value = 300f;
    }
    protected void DepthNormal(){
        depth.focalLength.value = 27f;
    }
}