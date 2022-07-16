using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public enum InputType{
    None = -1,
    Input = 0,
    InputDown = 1,
    InputUp = 2
}

public class KeyInputType{
    public KeyCode GetKeyCode{get;}
    public InputType GetInputType{get;}
    public KeyInputType(KeyCode key_code,InputType input_type){
        GetKeyCode = key_code;
        GetInputType = input_type;
    }
}

/// <summary>
/// 入力受付キーを設定するクラス。
/// GameControllerだけが持つクラス。
/// </summary>
public class InputSystem{
    private Dictionary<KeyInputType,UnityEvent> key_binds = new Dictionary<KeyInputType,UnityEvent>();
    /// <summary>
    /// キー入力を受け付けてもよいか
    /// </summary>
    /// <value></value>
    public bool IsKeyReception{get;set;} = true;
    /// <summary>
    /// キーを追加する。
    /// </summary>
    /// <param name="key_code"></param>
    /// <param name="action"></param>
    public void SetAddKey(KeyInputType key_code,UnityAction action){
        if(key_binds.Where(x => x.Key == key_code).Any()){
            key_binds[key_code].AddListener(action);
        }else{
            UnityEvent events = new UnityEvent();
            events.AddListener(action);
            key_binds.Add(key_code,events);
        }
    }

    /// <summary>
    /// キーを上書きする。
    /// </summary>
    /// <param name="key_code"></param>
    /// <param name="action"></param>
    public void OverrideKey(KeyInputType key_code,UnityAction action){
        if(key_binds.Where(x => x.Key == key_code).Any()){
            key_binds.Remove(key_code);
        }
        UnityEvent events = new UnityEvent();
        events.AddListener(action);
        key_binds.Add(key_code,events);
    }

    /// <summary>
    /// キー入力受付(Updateで呼び出す)
    /// </summary>
    public void InputReception(){
        if(!IsKeyReception) return;
        foreach(KeyValuePair<KeyInputType,UnityEvent> kvp in key_binds){
            KeyInputType key_code = kvp.Key;
            UnityEvent action = kvp.Value;
            if(key_code.GetInputType == InputType.Input){
                if(Input.GetKey(key_code.GetKeyCode)) action?.Invoke();
            }else if(key_code.GetInputType == InputType.InputDown){
                if(Input.GetKeyDown(key_code.GetKeyCode)) action?.Invoke();
            }else if(key_code.GetInputType == InputType.InputUp){
                if(Input.GetKeyUp(key_code.GetKeyCode)) action?.Invoke();
            }
        }
    }
}