using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Items {
    /// <summary>
    /// A class representing a ranged weapon.
    /// </summary>
    public class FPSRangedWeapon : FPSItem {
        private static readonly int Reload = Animator.StringToHash("Reload");
        private static readonly int Aim = Animator.StringToHash("Aim");
        

        [Header("Weapon Stats")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _range = 100f;
        [SerializeField] private float _fireRate = 0.05f;
        [SerializeField] private float _recoilRate = 0.01f;
        [SerializeField] private GameObject _bulletPrefab;      // The bullet prefab
        [SerializeField] private Transform _muzzleTransform;    // The muzzle transform
        [SerializeField] private Transform _bulletSpawnPoint;   // The bullet spawn point
        [SerializeField]private bool firemodes = false;         // Flag to check if the weapon has fire modes
        [SerializeField]private bool _FireModeAuto = false;     // Flag to check if the weapon is automatic
        private bool _isAiming;                                 // Flag to check if the weapon is aiming
        [SerializeField] private string AmmoCalibre = "9MM";    // The bullet calibre
        [SerializeField] private string AmmoType = "FMJ";       // The bullet type HP, FMJ, AP.
        [SerializeField] private int ammo;                      // The current ammo count
        [SerializeField] private int ammoReserve;               // The reserve ammo count
        [SerializeField] private int ammoMax = 30;              // The maximum ammo count
        [SerializeField] private int lowAmmoThreshold = 10;     // The threshold for low ammo
        private float _nextFireTime;                            // The time until the next fire

        /// <summary>
        /// Handles the input for the ranged weapon.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns>Returns if we should clear out movement bools for animator</returns>
        public override bool HandleInput(PlayerInput input) {
            var returnValue = HandleFire(input);
            if (HandleInspection(input)) returnValue = true;
            if (HandleAim(input)) returnValue =  true;
            if (HandleReload(input)) returnValue =  true;
            
            return returnValue;
        }

        /// <summary>
        /// Handles the reload input for the weapon.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns></returns>
        private bool HandleReload(PlayerInput input) {
            // Check if the reload button is pressed and the current animator state is not "Reload"
            if (input.reloadState == InputState.Pressed 
                && Animator.GetCurrentAnimatorStateInfo(0).shortNameHash.ToString() != Reload.ToString()) {
                
                // If there is no ammo in the weapon, return false
                if(ammoReserve <= 0) {
                    return false;
                }
                
                // Set the animator trigger to "Reload"
                Animator.SetTrigger(Reload);
                
                // Set the ammo count to the maximum ammo count
                ammo = ammoMax; 
                
                // Subtract the maximum ammo count from the reserve ammo count
                ammoReserve -= ammoMax;
                
                // Invoke the ammo changed event
                AmmoChanged();
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the aim input for the weapon.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns></returns>
        private bool HandleAim(PlayerInput input) {
            // Check if the aim button is pressed
            if (input.aimState == InputState.Held && !_isAiming) {
                // Set the animator to aim and set the aim flag to true
                _isAiming = true; 
                Animator.SetBool(Aim, true);
                return true;
            }
            // Check if the aim button is released
            if (input.aimState != InputState.Held && _isAiming) {
                // Set the animator to not aim and set the aim flag to false
                _isAiming = false;
                Animator.SetBool(Aim, false);
            }

            return false;
        }

        /// <summary>
        /// Handles the inspection input for the weapon.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns></returns>
        private bool HandleInspection(PlayerInput input) {
            // If the item can be inspected
            if (inspectable) {
                // If the inspect button is pressed
                if (input.inspectState == InputState.Pressed) {
                    // Set the animator trigger to "Inspect"
                    Animator.SetTrigger(Inspect);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the fire input for the weapon.
        /// </summary>
        /// <param name="input">The player input</param>
        /// <returns></returns>
        private bool HandleFire(PlayerInput input) {
            // If the next fire time is greater than 0, return false
            if (_nextFireTime > 0) {
                _nextFireTime -= Time.deltaTime;
            }
            // Flag to check if the weapon should fire
            bool fireweapon;
        
            // If the weapon is automatic, check if the fire button is held
            if (_FireModeAuto) 
                fireweapon = input.fireState == InputState.Held;
            // If the weapon is not automatic, check if the fire button is pressed
            else 
                fireweapon = input.fireState == InputState.Pressed;
        
            // If the fire condition is met
            if (fireweapon) {
                // If the ammo count is 0, play the empty ammo sound
                if(ammo <= 0) {
                    //AudioManager.PlaySound(EmptyAmmo);
                    return false;
                }

                if (_FireModeAuto) {
                    if(_nextFireTime > 0) 
                        return false;
                    else 
                        _nextFireTime = _fireRate;
                }
                
                
                // Set the animator trigger to "Fire"
                Animator.SetTrigger(Fire);
                
                // Subtract the ammo count
                ammo--;
                
                // Trigger the camera shake effect
                cameraEffects.TriggerShake(_recoilRate);
                
                // Invoke the ammo changed event
                AmmoChanged();
                
                return true;
            }
            
            // If the weapon has fire modes
            if(firemodes) {
                // If the fire mode toggle button is pressed
                if (input.toggleFireModeState == InputState.Pressed) {
                    // Toggle the fire mode
                    _FireModeAuto = !_FireModeAuto;
                    
                    // Invoke the ammo changed event
                    AmmoChanged();
                    
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Invokes the ammo changed event.
        /// </summary>
        private void AmmoChanged() {
            GlobalEvents.RaiseAmmoChanged(ammo
                , ammoReserve
                , AmmoCalibre
                , AmmoType
                , _FireModeAuto ? "AUTO" : "SINGLE"
                , lowAmmoThreshold);

        }
    }
    
    public class ItemScriptableObject : ScriptableObject {
        public string itemName;
        public Item itemType;
    }
    
    public class Weapon : ItemScriptableObject { 
        public float damage;
        public float range;
        public float firerate;
        public float recoilRate;
        public bool firemodes;
        public string ammoCalibre;
    }
    
    public class ItemComponet : MonoBehaviour {
        ItemScriptableObject item;
    }
    
    
}
