using Items;
using UnityEngine;

/// <summary>
/// This class represents an item component in the inventory.
/// </summary>
public class ItemComponent : MonoBehaviour {
    [SerializeField]private SlotType slotType;
    [SerializeField]private int slotWidth;
    [SerializeField]private int slotHeight;
    [SerializeField]private ItemScriptableObject item;
}
