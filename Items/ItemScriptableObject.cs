using Items;
using UnityEngine;

/// <summary>
/// An item scriptable object that holds the item name and type.
/// </summary>
[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
public class ItemScriptableObject : ScriptableObject {
    /// <summary>
    /// The name of the item.
    /// </summary>
    public string itemName;
    
    /// <summary>
    /// The description of the item.
    /// </summary>
    public Item itemType;
}

