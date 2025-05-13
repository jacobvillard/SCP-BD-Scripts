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
        [SerializeField] private Vector2Int spawn = new Vector2Int(0, 0); // The spawn position for the item prefab            
        
         private SlotComponent[,] spawnedSlots;                     // The spawned slots in the inventory
         private const float SlotVerticalSpacing = 130f;            
         private const float LayoutPadding = 400f;
         private const float CellPadding = 0f;
         
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
            spawnedSlots = new SlotComponent[slotsPerRow, visibleRows];
            for (var index = 0; index < slotNumber; index++) {
                var row = index / slotsPerRow;
                var col = index % slotsPerRow;

                var slotComp = transform.GetChild(index).GetComponent<SlotComponent>();
                spawnedSlots[col, row] = slotComp;
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

            // Get the item component
            var itemComponent = itemPrefab.GetComponent<ItemComponent>();
            
            // Check if the item can be placed in the slot
            if (!IsPlacementValid(spawn.y, spawn.x, itemComponent.slotWidth, itemComponent.slotHeight)) {
                return;
            }
            
            // Item instantiation
            var item = Instantiate(itemPrefab, parentRectTransform, true);
            
            // Determine offset based on item size
            var offsetX = itemComponent.slotWidth > 1 ? -((itemComponent.slotWidth - 1) * 128 / 2f) : 0f;
            var offsetY = itemComponent.slotHeight > 1 ? -((itemComponent.slotHeight - 1) * 128 / 2f) : 0f;
            
            // Calculate the spawn position based on the slot position
            var slotPos = spawnedSlots[spawn.y, spawn.x].transform.position;
            var itemSpawnPos = new Vector3(
                slotPos.x - offsetX,
                slotPos.y + offsetY,
                slotPos.z
            );
            
            // Set the item position
            item.transform.position = itemSpawnPos;
            
            // Set the source slot position
            spawnedSlots[spawn.y, spawn.x].sourceSlot = true;
            spawnedSlots[spawn.y, spawn.x].itemPrefabRef = item;
            
            // Spawn an item in the first empty slots
            for (var i = 0; i < itemComponent.slotWidth; i++) {
                for (var j = 0; j < itemComponent.slotHeight; j++) {
                    if (spawnedSlots[i, j].itemComponent == null) {
                        spawnedSlots[i, j].AddItem(itemComponent);
                        spawnedSlots[i, j].sourceSlotPosition = new Vector2Int(spawn.y, spawn.x);
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if the placement of an item is valid in the inventory.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private bool IsPlacementValid(int startX, int startY, int width, int height) {
            Debug.Log($"Checking placement for item at ({startX}, {startY}) with size ({width}, {height})");
            Debug.Log($"Grid size: {spawnedSlots.GetLength(0)} x {spawnedSlots.GetLength(1)}");
            
            // Check bounds
            if (startX + width > spawnedSlots.GetLength(0) || startY + height > spawnedSlots.GetLength(1))
                return false;

            // Check for empty slots
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (spawnedSlots[startX + x, startY + y].itemComponent != null)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        public void RemoveItem(Vector2Int slotPosition) {
            // Check if the slot is empty
            if (spawnedSlots[slotPosition.x, slotPosition.y].itemComponent == null) {
                Debug.Log("Slot is empty");
                return;
            }
            
            // Remove the item from the slot
            var itemComponent = spawnedSlots[slotPosition.x, slotPosition.y].itemComponent;
            Destroy(spawnedSlots[slotPosition.x, slotPosition.y].itemPrefabRef);
            
            for (var i = 0; i < itemComponent.slotWidth; i++) {
                for (var j = 0; j < itemComponent.slotHeight; j++) {
                    if (spawnedSlots[i, j].itemComponent != null) {
                        spawnedSlots[i, j].sourceSlot = false;
                        spawnedSlots[i, j].itemComponent = null;
                    }
                }
            }
            
        }
   
        // Update the slots when the script is enabled or disabled
        private void OnEnable() => UpdateSlots();
        private void OnDisable() => UpdateSlots();
    }
    
    
}
