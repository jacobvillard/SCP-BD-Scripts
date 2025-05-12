using UnityEngine;

namespace Inventory {
    
    /// <summary>
    /// This class manages the dynamic slots in the inventory.
    /// </summary>
    public class DynamicSlotManager : MonoBehaviour {
    
        [SerializeField] private int slotNumber = 10;   // The number of slots in the inventory
        [SerializeField] private GameObject slotPrefab; // The prefab for the slots

        [Button]
        public void UpdateSlots() {
            UpdateSlotNumber(slotNumber);
        }
        
        /// <summary>
        /// Updates the number of slots in the inventory.
        /// </summary>
        private void UpdateSlotNumber(int slotNumber) {
            //If the number of slots is greater than the current number of slots, remove the excess slots
            if(transform.childCount > slotNumber) {
                for (var i = transform.childCount - 1; i >= slotNumber; i--) {
                    var child = transform.GetChild(i);
                    // todo: check if child is empty before destroying
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        Destroy(child.gameObject); 
                    else
                        DestroyImmediate(child.gameObject); 
#else
                   Destroy(child.gameObject);  
#endif

                }
            }
            //If the number of slots is less than the current number of slots, add the missing slots
            else if (transform.childCount < slotNumber) {
                var numberOfSlotsToAdd = slotNumber - transform.childCount;
                for (var i = 0; i < numberOfSlotsToAdd; i++) {
                    var newSlot = Instantiate(slotPrefab, transform, true);
                    newSlot.name = $"Slot {i + 1}";
                }
            }
        }
   
   
    }
}
