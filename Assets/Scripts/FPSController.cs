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
    float speed = 0.1f;

    public GameObject cam;
    Rigidbody rigidbody;
    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;
    
    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -90f, maxX = 90f;
    private GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
        rigidbody = GetComponent<Rigidbody>();
        light = cam.transform.Find("Spot Light").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        //Updateの中で作成した関数を呼ぶ
        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        Light();
        UpdateCursorLock();
    }
    private void Light(){
        if(Input.GetMouseButtonDown(0)){
            light.SetActive(!light.activeSelf);
        }
    }
    private void FixedUpdate()
    {
        x = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;
    
        //transform.position += new Vector3(x,0,z);
        if(x != 0 || z != 0){
            rigidbody.velocity = transform.forward * z + transform.right * x;
        }else{
            rigidbody.velocity = Vector3.zero;
        }
    }
    

    public void UpdateCursorLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if(Input.GetMouseButton(0))
        {
            cursorLock = true;
        }


        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
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