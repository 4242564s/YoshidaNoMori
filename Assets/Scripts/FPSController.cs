using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FPSで操作するためのコントローラー
/// </summary>
public class FPSController : MonoBehaviour
{
    float x, z;
    [SerializeField]
    float walk_speed = 2f;
    [SerializeField]
    float run_speed = 4f;
    [SerializeField,Tooltip("アイテム切り替え可能までの時間")]
    float change_item_speed = 0.5f;
    public float GetChangeItemSpeed {get => change_item_speed;}
    public bool IsRun{get;set;}
    public Camera cam;
    Rigidbody rigidbody;
    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;
    
    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -90f, maxX = 90f;
    private GameObject light;
    private AudioSource audio;

    private BaseEventObject event_obj;
    // Start is called before the first frame update
    public void Initialize(){
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
        rigidbody = GetComponent<Rigidbody>();
        light = cam.transform.Find("FlashLight").gameObject;
    }
    // Update is called once per frame
    public void Move(){
        if(GameController.Instance.DisplayState == GameDisplayState.Pause){
            transform.localRotation = characterRot;
            return;
        }

        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;
        x = 0;
        z = 0;
        float speed = (IsRun ? run_speed : walk_speed);
        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        if(audio == null && (x != 0 || z != 0)){
            audio = SeManager.Instance.Play(transform,SeManager.WALK,false,false);
        }else if(audio != null && (x == 0 && z == 0)){
             SeManager.Instance.Stop(audio);
        }
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        //Updateの中で作成した関数を呼ぶ
        cameraRot = ClampRotation(cameraRot);
        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;
        
        Light();
    }
    private void Light(){
        if(Input.GetMouseButtonDown(0)){
            light.SetActive(!light.activeSelf);
        }
    }
    public void PhysicsFix()
    {    
        if(GameController.Instance.DisplayState == GameDisplayState.Pause) return;

        if(x != 0 || z != 0){
            rigidbody.velocity = transform.forward * z + transform.right * x;
        }else{
            rigidbody.velocity = Vector3.zero;
        }
    }
    
    //角度制限関数の作成
    private Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX,minX,maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }


}