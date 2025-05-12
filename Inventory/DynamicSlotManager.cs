using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory {
    
    /// <summary>
    /// This class manages the dynamic slots in the inventory.
    /// </summary>
    public class DynamicSlotManager : MonoBehaviour {
        [Header("Slot Configuration")]
        [SerializeField] private int slotNumber = 10;                               // The number of slots in the inventory
        [SerializeField] private int slotsPerRow = 4;                               // The number of slots in row
        [SerializeField] private GameObject slotPrefab;                             // The prefab for the slots
        
        [Header("Layout References")]
        [SerializeField] private RectTransform parentRectTransform;                 // The parent rect transform for the slots
        [SerializeField] private RectTransform verticalLayoutGroupRectTransform;    // The vertical layout group rect transform for the slots
        [SerializeField] private float  parentRectHeightStartSize = -30f;           // The parent rect transform for the slots
        
        
        // Temporary variables for the item spawning todo remove
        [Header("Item Spawning")]
        [SerializeField] private GameObject itemPrefab;                    
        [SerializeField] private Vector2 spawn = new Vector2(0, 0); // The spawn position for the item prefab            
        
         private SlotComponent[,] spawnedSlots;                     // The spawned slots in the inventory
         private const float SlotVerticalSpacing = 130f;            
         private const float LayoutPadding = 400f;
         private const float CellPadding = 64f;
         
        private void Awake() {
#if UNITY_EDITOR
            if(parentRectTransform == null) {
                Debug.LogError("Parent Rect Transform is not assigned in the inspector.");
            }
            
            if (verticalLayoutGroupRectTransform == null) {
                Debug.LogError("Vertical Layout Group Rect Transform is not assigned in the inspector.");
            }
#endif
            
        }

        [Button]
        private void UpdateSlots() {
            UpdateSlotNumber(slotNumber);
        }
        
        /// <summary>
        /// Updates the number of slots in the inventory.
        /// </summary>
        private void UpdateSlotNumber(int slotNumber) {
            DrawGrid(slotNumber);
            var visibleRows = BuildSlotGrid(slotNumber);
            ResizeParent(slotNumber, visibleRows);
            ResizeVerticalLayout() ;
     
        }

        /// <summary>
        /// Resizes the parent rect transform based on the number of slots and visible rows.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="visibleRows"></param>
        private void ResizeParent(int slotNumber, int visibleRows) {
            //Set the size of the parent rect transform based on the number of slots
            if(slotNumber <= 4) {
                parentRectTransform.sizeDelta = new Vector2(100f, parentRectHeightStartSize);
                
            }
            else {
                var newHeight = parentRectHeightStartSize + (visibleRows - 1) * SlotVerticalSpacing;
                parentRectTransform.sizeDelta = new Vector2(100f, newHeight);
            }
        }

        /// <summary>
        /// Builds the grid of slots in the inventory.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns></returns>
        private int BuildSlotGrid(int slotNumber) {
            // Set the number of visible rows based on the number of slots and the number of slots per row
            var visibleRows = Mathf.CeilToInt((float)slotNumber / slotsPerRow);
            
            //Build the 2D array of slots
            spawnedSlots = new SlotComponent[visibleRows, slotsPerRow];
            for (var index = 0; index < slotNumber; index++) {
                var row = index / slotsPerRow;
                var col = index % slotsPerRow;

                var slotComp = transform.GetChild(index).GetComponent<SlotComponent>();
                spawnedSlots[row, col] = slotComp;
            }

            return visibleRows;
        }

        /// <summary>
        /// Draws the grid of slots in the inventory.  
        /// </summary>
        /// <param name="slotNumber"></param>
        private void DrawGrid(int slotNumber) {
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

        /// <summary>
        /// Resizes the vertical layout group rect transform based on height of children.
        /// </summary>
        private void ResizeVerticalLayout() {
            // Height calculation
            var height = verticalLayoutGroupRectTransform.Cast<RectTransform>()
                .Where(child => child.gameObject.activeSelf)
                .Sum(child => child.sizeDelta.y + SlotVerticalSpacing) + LayoutPadding;

            // Set the size of the vertical layout group rect transform based on the height of its children
            verticalLayoutGroupRectTransform.sizeDelta = new Vector2(verticalLayoutGroupRectTransform.sizeDelta.x, height);
            
            //Force the layout group to rebuild its layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroupRectTransform);
        }
        
        /// <summary>
        /// Spawns an item in a given empty slot.
        /// </summary>
        [Button]
        private void SpawnItem() {
            // Item instantiation
            var item = Instantiate(itemPrefab, parentRectTransform, true);
            
            // Calculate the spawn position based on the slot position
            var slotPos = spawnedSlots[(int)spawn.x, (int)spawn.y].transform.position;
            var itemSpawnPos = new Vector3(
                slotPos.x - CellPadding,
                slotPos.y + CellPadding,
                slotPos.z
            );
            
            // Set the item position
            item.transform.position = itemSpawnPos;
            
            // Set the source slot position
            spawnedSlots[(int)spawn.x, (int)spawn.y].sourceSlot = true;
            
            //todo: check if placement is valid
            
            // Get the item component
            var itemComponent = item.GetComponent<ItemComponent>();
            
            // Spawn an item in the first empty slot
            for (var i = 0; i < itemComponent.slotWidth; i++) {
                for (var j = 0; j < itemComponent.slotHeight; j++) {
                    if (spawnedSlots[i, j].itemComponent == null) {
                        spawnedSlots[i, j].AddItem(itemComponent);
                    }
                }
            }
        }
   
        // Update the slots when the script is enabled or disabled
        private void OnEnable() => UpdateSlots();
        private void OnDisable() => UpdateSlots();
    }
    
    
}
