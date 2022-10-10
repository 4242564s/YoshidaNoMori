using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemData", order = 1)]
public class ItemScriptableObject : ScriptableObject{
    /// <summary>
    /// アイテム名
    /// </summary>
    public string ItemName;
    /// <summary>
    /// アイテム説明文
    /// </summary>
    public string ItemHelp;
    /// <summary>
    /// アイテムを使ってみようとしたときのメッセージ
    /// </summary>
    public string UseMessage;
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
    /// 使える回数
    /// </summary>
    public int ExhaustedCount; 
    /// <summary>
    /// どこでも使えるアイテム化
    /// </summary>
    public bool IsAnyTime;
    /// <summary>
    /// 素材の場所
    /// </summary>
    public string SourcePath;
}