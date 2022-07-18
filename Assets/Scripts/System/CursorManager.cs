using UnityEngine;

/// <summary>
/// カーソルに関しての制御
/// </summary>
public class CursorManager {
    /// <summary>
    /// カーソルを表示させるかどうか
    /// </summary>
    /// <param name="cursor_lock"></param>
    public void CursorIsEnable(bool cursor_lock){
        Cursor.visible = cursor_lock;
        Cursor.lockState = !cursor_lock ? CursorLockMode.Locked : CursorLockMode.None;
    }
}