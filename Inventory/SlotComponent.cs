using Items;
using UnityEngine;

namespace Inventory {
    /// <summary>
    /// Holds data for the slot component.
    /// </summary>
    public class SlotComponent : MonoBehaviour {
        /// <summary>
        /// This is the type of item that can be placed in this slot.
        /// </summary>
        public SlotType slotType;
        
        /// <summary>
        /// The current item in the slot.
        /// </summary>
        public ItemComponent itemComponent;
        
        /// <summary>
        /// This is the upper left position of the item in the inventory grid.
        /// </summary>
        public Vector2Int sourceSlotPosition;

        /// <summary>
        /// If this is the upper left position of the item in the inventory grid.
        /// </summary>
        public bool sourceSlot;
        
        
        
        /// <summary>
        /// Add an item to the slot.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ItemComponent item) {
            sourceSlot = true;
            SetItem(item);
        }

        /// <summary>
        /// Set the item in the slot.
        /// </summary>
        /// <param name="item"></param>
        private void SetItem(ItemComponent item) {
            itemComponent = item;
        }
        
        /// <summary>
        /// Remove the item from the slot.
        /// </summary>
        public void RemoveItem() {
            itemComponent = null;
            sourceSlot = false;
        }
    }
}
