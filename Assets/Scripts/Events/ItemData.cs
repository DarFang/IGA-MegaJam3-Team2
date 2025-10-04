using UnityEngine;

[CreateAssetMenu(fileName = "New Item" , menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;
}