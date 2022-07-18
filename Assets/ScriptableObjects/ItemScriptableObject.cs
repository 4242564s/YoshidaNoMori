using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemScriptableObject : ScriptableObject{
    /// <summary>
    /// アイテム名
    /// </summary>
    public string ItemName;
    /// <summary>
    /// アイテムID
    /// </summary>
    public int ItemId;
    /// <summary>
    /// キーID
    /// </summary>
    public int KeyId;
    /// <summary>
    /// 持った時の大きさ
    /// </summary>
    public Vector3 HadScale;
    /// <summary>
    /// 使った時消耗するか
    /// </summary>
    public bool IsExhausted; 
    public bool IsAnyTime;
}