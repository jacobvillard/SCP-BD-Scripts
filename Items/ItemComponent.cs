using Items;
using UnityEngine;

/// <summary>
/// This class represents an item component in the inventory.
/// </summary>
public class ItemComponent : MonoBehaviour {
    [SerializeField]private SlotType slotType;
    [SerializeField] public int slotWidth;
    [SerializeField] public int slotHeight;
    [SerializeField] private ItemScriptableObject item;
}
