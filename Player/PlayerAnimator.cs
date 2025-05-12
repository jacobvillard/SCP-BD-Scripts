using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using Items;

// This namespace is for player-related classes
namespace Player {
    /// <summary>
    /// This class is responsible for handling the player's animations.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour {
        private static readonly int AnimationTimer = Animator.StringToHash("AnimationTimer");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Run = Animator.StringToHash("Run");

        [Header("Animator Settings")]
        [SerializeField] private Animator animator;                         // The animator component 
        [SerializeField] private List<FPSItem> items = new List<FPSItem>(); // List of items
        [SerializeField] private float equipDelay = 0.5f;                   // The delay of equipping an item
        [SerializeField] private Item strEquip = Item.None;                 // The item to equip at the start
        
        /// <summary>
        /// This is a reference to the camera shake interface. It is used to shake the camera when the player fires a weapon.
        /// </summary>
        public ICameraRecoil CameraRecoil;
        
        private FPSItem _currentItem;       // The current item equipped
        private float _nextEquipTime = 0f;  // The time until the next item can be equipped
        private float _nextAnimTime;        // The time until the next animation can be played
                
        // todo: remove this
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start() {
            EquipItem(strEquip);
        }

        
        /// <summary>
        /// Equips an item by name. This method searches through the list of items and sets the animator to the one that matches the item name.
        /// </summary>
        /// <param name="itemType">The item to equip</param>
        private void EquipItem(Item itemType) {
            // Reset the next equip time
            _nextEquipTime = 0f; 
            
            // Filter the items list to find the item that matches the item type
            foreach (var item in items) {
                if (item.ItemType == itemType) {
                    
                    // Set the current item to the one that matches the item type
                    _currentItem = item;
                    
                    // Set the equip delay to the item's equip time
                    equipDelay = item.EquipTime;
                    
                    // Activate the game object
                    item.gameObject.SetActive(true);
                    
                    // Set the animator to the item's animator
                    animator = item.Animator;
                    
                    // Initialize the item with the camera recoil interface
                    item.Initialize(CameraRecoil);
                }
            }
        }
        
        /// <summary>
        /// Dequips an item by name. This method searches through the list of items and sets the animator to null for the item that matches the item name.
        /// </summary>
        /// <param name="itemName"></param>
        private void DequipItem(string itemName) {
            foreach (var item in items) {
                if (item.ItemName == itemName) {
                    item.gameObject.SetActive(false);
                    animator = null;
                    _currentItem = null;
                }
            }
        }

        /// <summary>
        /// Updates the player's animation based on input and movement.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="movement"></param>
        public void UpdateAnimation(PlayerInput input, PlayerMovement movement) {
            //During the equip delay, we don't want to transition to another animation
            if (_nextEquipTime < equipDelay) {
                _nextEquipTime += Time.deltaTime;
            }
            
            // Check if the currently equipped item implements the IAnimHandleInput interface
            if (_currentItem is IAnimHandleInput animItem) {
                // If the item handler returns true, clear all movement states in the animator
                if (animItem.HandleInput(input))
                    ClearAllMovementStates();
            }
            
            
            // Next animation time is set to the animator's AnimationTimer parameter
            _nextAnimTime = animator.GetFloat(AnimationTimer);
            
            // If the next animation time is greater than 0, we are in a state that should not be interrupted
            if(_nextAnimTime > 0) {
                // Decrease the next animation time by the time since the last frame
                _nextAnimTime -= Time.deltaTime;
                
                // Set the animator's AnimationTimer parameter to the next animation time
                animator.SetFloat(AnimationTimer, _nextAnimTime);
                
                // Return to avoid updating the movement states
                return;
            }
            
            //Handles various movement states for animations
            switch (movement.CurrentState) {
                case PlayerState.Idle:
                    animator.SetBool(Idle, true);
                    animator.SetBool(Walk, false);
                    animator.SetBool(Run, false);
                    break;
                case PlayerState.Walking:
                    animator.SetBool(Walk, true);
                    animator.SetBool(Idle, false);
                    animator.SetBool(Run, false);
                    break;
                case PlayerState.Sprinting:
                    animator.SetBool(Run, true);
                    animator.SetBool(Idle, false);
                    animator.SetBool(Walk, false);
                    break;
                case PlayerState.Jumping:
                    if(input.isSprinting) {
                        animator.SetBool(Run, true);
                        animator.SetBool(Idle, false);
                        animator.SetBool(Walk, false);
                    }
                    else {
                        animator.SetBool(Idle, true);
                        animator.SetBool(Walk, false);
                        animator.SetBool(Run, false);
                    }
                    break;
                case PlayerState.Crouching:
                    animator.SetBool(Idle, true);
                    animator.SetBool(Walk, false);
                    animator.SetBool(Run, false);
                    break;
                case PlayerState.Sliding:
                    animator.SetBool(Idle, true);
                    animator.SetBool(Walk, false);
                    animator.SetBool(Run, false);
                    break;
            }
        }
        
        /// <summary>
        /// Clears all movement states in the animator. This is used to reset the animator when the player is not moving.
        /// </summary>
        private void ClearAllMovementStates() {
            animator.SetBool(Idle, false);
            animator.SetBool(Walk, false);
            animator.SetBool(Run, false);
        }

    }
}


